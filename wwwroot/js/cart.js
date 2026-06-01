// ──────────────────────────────────────────────────────────────
// CART CORE
// ──────────────────────────────────────────────────────────────
const CART_KEY = 'autohub_cart';
const isLoggedIn = window.isLoggedIn === true;

// ── LOCAL STORAGE FUNCTIONS ──
function getLocalCart() {
    try { return JSON.parse(localStorage.getItem(CART_KEY)) || []; }
    catch { return []; }
}
function saveLocalCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
}
function clearLocalCart() {
    localStorage.removeItem(CART_KEY);
}

// ── GET CART ──
async function getCart() {
    if (!isLoggedIn) return getLocalCart();
    
    try {
        const res = await fetch('/Cart/GetCart');
        if (res.ok) {
            const data = await res.json();
            return Array.isArray(data) ? data : [];
        }
    } catch (e) { console.error('Error fetching cart:', e); }
    return [];
}

// ── SYNC CART ──
async function syncCartIfLoggedIn() {
    if (!isLoggedIn) return;
    const localCart = getLocalCart();
    if (localCart.length > 0) {
        try {
            await fetch('/Cart/SyncCart', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(localCart.map(i => ({ name: i.name, price: i.price, type: i.type, qty: i.qty })))
            });
            clearLocalCart();
        } catch (e) { console.error('Error syncing cart:', e); }
    }
}

// ── ACTIONS ──
async function addToCart(name, price, type) {
    if (!isLoggedIn) {
        const cart = getLocalCart();
        const existing = cart.find(i => i.name === name);
        if (existing) existing.qty = (existing.qty || 1) + 1;
        else cart.push({ name, price: parseFloat(price) || 0, type: type || 'vehicle', qty: 1 });
        saveLocalCart(cart);
        updateCartBadge();
        showCartToast(name);
        return;
    }

    // Đã đăng nhập
    try {
        await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, price: parseFloat(price) || 0, type: type || 'vehicle', qty: 1 })
        });
        updateCartBadge();
        showCartToast(name);
    } catch (e) { console.error('Error adding to DB cart:', e); }
}

async function removeFromCart(indexOrName) {
    if (!isLoggedIn) {
        const cart = getLocalCart();
        cart.splice(indexOrName, 1);
        saveLocalCart(cart);
        await renderCartItems();
        updateCartBadge();
        return;
    }

    try {
        await fetch('/Cart/RemoveItem', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: indexOrName })
        });
        await renderCartItems();
        updateCartBadge();
    } catch (e) { console.error('Error removing from DB cart:', e); }
}

async function clearCart() {
    if (!isLoggedIn) {
        saveLocalCart([]);
        await renderCartItems();
        updateCartBadge();
        return;
    }

    try {
        await fetch('/Cart/ClearCart', { method: 'POST' });
        await renderCartItems();
        updateCartBadge();
    } catch (e) { console.error('Error clearing DB cart:', e); }
}

async function changeQty(indexOrName, delta) {
    if (!isLoggedIn) {
        const cart = getLocalCart();
        cart[indexOrName].qty = Math.max(1, (cart[indexOrName].qty || 1) + delta);
        saveLocalCart(cart);
        await renderCartItems();
        updateCartBadge();
        return;
    }

    // Lấy số lượng mới
    const cart = await getCart();
    const item = cart.find(i => i.name === indexOrName);
    if (!item) return;
    const newQty = Math.max(1, (item.qty || 1) + delta);

    try {
        await fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: indexOrName, qty: newQty })
        });
        await renderCartItems();
        updateCartBadge();
    } catch (e) { console.error('Error updating DB cart qty:', e); }
}


// ──────────────────────────────────────────────────────────────
// BADGE & DRAWER & RENDER
// ──────────────────────────────────────────────────────────────
async function updateCartBadge() {
    const cart = await getCart();
    const total = cart.reduce((sum, i) => sum + (i.qty || 1), 0);
    const badge = document.getElementById('cartBadge');
    if (!badge) return;
    badge.textContent = total;
    badge.style.display = total > 0 ? 'flex' : 'none';
}

async function openCart() {
    const overlay = document.getElementById('cartOverlay');
    const drawer = document.getElementById('cartDrawer');
    overlay.style.display = 'block';
    drawer.style.display = 'block';
    setTimeout(() => { drawer.style.transform = 'translateX(0)'; }, 10);
    await renderCartItems();
}

function closeCart() {
    const drawer = document.getElementById('cartDrawer');
    drawer.style.transform = 'translateX(100%)';
    setTimeout(() => {
        drawer.style.display = 'none';
        if (document.getElementById('cartOverlay')) {
            document.getElementById('cartOverlay').style.display = 'none';
        }
    }, 300);
}

async function renderCartItems() {
    const cart = await getCart();
    const container = document.getElementById('cartItems');
    const footer = document.getElementById('cartFooter');

    if (!container) return;

    if (cart.length === 0) {
        container.innerHTML = `
            <div class="text-center py-5" style="color:#555;">
                <div style="font-size:3rem; opacity:0.3;">🛒</div>
                <p class="mt-3" style="font-size:0.9rem;">Giỏ hàng đang trống</p>
                <p style="font-size:0.8rem; color:#444;">Thêm xe hoặc dịch vụ để bắt đầu</p>
            </div>`;
        if (footer) footer.style.display = 'none';
        return;
    }

    let html = '';
    let total = 0;
    cart.forEach((item, i) => {
        const itemTotal = item.price * (item.qty || 1);
        total += itemTotal;
        const typeIcon = item.type === 'service' ? '⚙️' : '🚗';
        
        // Dùng name làm ID nếu đăng nhập, ngược lại dùng index
        const arg = isLoggedIn ? \`'\${item.name.replace(/'/g, "\\'")}'\` : i;

        html += `
            <div style="padding:14px 0; border-bottom:1px solid rgba(255,255,255,0.06); display:flex; gap:12px; align-items:flex-start;">
                <div style="font-size:1.5rem; flex-shrink:0;">${typeIcon}</div>
                <div style="flex:1; min-width:0;">
                    <div style="font-weight:700; color:#fff; font-size:0.9rem; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                        ${item.name}
                    </div>
                    <div style="color:#d4af37; font-weight:800; font-size:0.85rem; margin-top:2px;">
                        ${item.price > 0 ? Number(itemTotal).toLocaleString('vi-VN') + ' đ' : 'Báo giá'}
                    </div>
                    <div style="display:flex; align-items:center; gap:8px; margin-top:6px;">
                        <button onclick="changeQty(${arg}, -1)" style="background:rgba(255,255,255,0.08); border:none; color:#fff; width:24px; height:24px; border-radius:4px; cursor:pointer; font-size:0.9rem; display:flex; align-items:center; justify-content:center;">−</button>
                        <span style="color:#fff; font-weight:700; font-size:0.85rem; min-width:20px; text-align:center;">${item.qty || 1}</span>
                        <button onclick="changeQty(${arg}, 1)" style="background:rgba(255,255,255,0.08); border:none; color:#fff; width:24px; height:24px; border-radius:4px; cursor:pointer; font-size:0.9rem; display:flex; align-items:center; justify-content:center;">+</button>
                    </div>
                </div>
                <button onclick="removeFromCart(${arg})" style="background:transparent; border:none; color:#666; cursor:pointer; font-size:1rem; padding:0; flex-shrink:0; margin-top:2px;" title="Xóa">✕</button>
            </div>`;
    });

    container.innerHTML = html;
    document.getElementById('cartTotal').textContent = Number(total).toLocaleString('vi-VN') + ' đ';
    if (footer) footer.style.display = 'block';
}

async function checkoutCart() {
    const cart = await getCart();
    if (cart.length === 0) { alert('Giỏ hàng đang trống!'); return; }
    const items = cart.map(i => `• ${i.name} (x${i.qty || 1})`).join('\n');
    alert(`✅ Đặt lịch thành công!\n\n${items}\n\nNhân viên sẽ liên hệ xác nhận trong vòng 10 phút.`);
    await clearCart();
    closeCart();
}

// ──────────────────────────────────────────────────────────────
// TOAST
// ──────────────────────────────────────────────────────────────
function showCartToast(name) {
    let toast = document.getElementById('cartToast');
    if (!toast) {
        toast = document.createElement('div');
        toast.id = 'cartToast';
        toast.style.cssText = `position:fixed; bottom:24px; right:24px; background:#1e1e26; border:1px solid var(--accent-mint,#2ee59d);
            color:#2ee59d; padding:12px 20px; border-radius:10px; font-size:0.88rem; font-weight:700; z-index:9999;
            box-shadow:0 4px 20px rgba(0,0,0,0.5); transition:opacity 0.3s; max-width:280px;`;
        document.body.appendChild(toast);
    }
    const shortName = name.length > 30 ? name.substring(0, 30) + '...' : name;
    toast.textContent = '🛒 Đã thêm: ' + shortName;
    toast.style.opacity = '1';
    toast.style.display = 'block';
    clearTimeout(toast._timer);
    toast._timer = setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.style.display = 'none', 300);
    }, 2500);
}

// ── INIT ──
document.addEventListener('DOMContentLoaded', async () => {
    await syncCartIfLoggedIn();
    updateCartBadge();
});
