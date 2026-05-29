# Kế Hoạch Tái Cấu Trúc CSS — AutoHub

## Phân Tích Hiện Trạng

Sau khi đọc toàn bộ code, tôi phát hiện **4 vấn đề nghiêm trọng** trong hệ thống CSS hiện tại:

| Vấn đề | Vị trí | Hậu quả |
|--------|--------|---------|
| **CSS Admin lẫn Client** | `site.css` dòng 203-310 | Override nhau, khó debug |
| **File trùng lặp hoàn toàn** | `site.css` dòng 57-107 **=** `_Layout.cshtml.css` | Build ra 2 lần vào trình duyệt |
| **CSS inline trong HTML** | `_Layout.cshtml` dòng 15, 41, 42, 45, 51 | Không thể override, không tái sử dụng |
| **Class mơ hồ, không scope** | `.tech-panel`, `.filter-bar`... | Dễ conflict khi thêm trang mới |

---

## Phần 1 — Sơ Đồ Cây Thư Mục Đề Xuất

Tôi áp dụng **ITCSS (Inverted Triangle CSS)** được đơn giản hoá — phù hợp nhất với quy mô dự án MVC vừa.

> **Tại sao ITCSS?** ITCSS sắp xếp CSS theo thứ tự từ "ảnh hưởng rộng nhất" xuống "ảnh hưởng hẹp nhất". Specificity tăng dần từ trên xuống → **không bao giờ bị override ngoài ý muốn**.

```
wwwroot/
└── css/
    ├── main.css                  ← Entry point cho Client (import tất cả bên dưới)
    ├── admin.css                 ← Entry point cho Admin (import riêng)
    │
    ├── 1-settings/               ← [ITCSS Layer 1] Biến toàn cục - không sinh ra CSS
    │   ├── _variables.css        ← Custom properties (:root), màu sắc, khoảng cách
    │   └── _typography.css       ← @import font, font-size scale, line-height
    │
    ├── 2-base/                   ← [ITCSS Layer 2] Reset & HTML element defaults
    │   ├── _reset.css            ← box-sizing, margin 0, padding 0
    │   └── _elements.css         ← a, h1-h6, p, img... styling mặc định
    │
    ├── 3-overrides/              ← [ITCSS Layer 3] Bootstrap overrides tập trung 1 chỗ
    │   └── _bootstrap.css        ← .btn-primary, .nav-pills, .border-top...
    │
    ├── 4-layouts/                ← [ITCSS Layer 4] Khung bố cục trang
    │   ├── _navbar.css           ← Navbar client
    │   └── _footer.css           ← Footer client
    │
    ├── 5-components/             ← [ITCSS Layer 5] UI components tái sử dụng
    │   ├── _buttons.css          ← .btn-neon, .btn-ferrari...
    │   ├── _cards.css            ← Card xe, card phụ tùng
    │   ├── _hero.css             ← .hero-banner, .hero-overlay, .hero-content
    │   ├── _forms.css            ← .input-premium, form layouts
    │   └── _progress.css         ← .circuit-progress, .circuit-progress-bar
    │
    ├── 6-admin/                  ← [RIÊNG BIỆT] Toàn bộ CSS khu vực Admin
    │   ├── _admin-layout.css     ← .admin-layout, .admin-sidebar, .admin-content
    │   ├── _admin-menu.css       ← .admin-menu, .admin-menu-item
    │   └── _admin-filters.css    ← .filter-bar, .filter-group, .filter-select
    │
    └── 7-utilities/              ← [ITCSS Layer 7] Single-purpose helpers (nếu cần)
        └── _helpers.css          ← .text-gradient-gold, .label-tech...
```

> **Lưu ý quan trọng:** `_Layout.cshtml.css` (CSS Isolation) **không xoá** mà giữ nguyên — nhưng **làm rỗng** nó, vì toàn bộ nội dung đã được chuyển vào hệ thống `wwwroot/css/`. CSS Isolation chỉ nên dùng cho style **cực kỳ cục bộ** của 1 view cụ thể.

---

## Phần 2 — Chiến Lược Xử Lý CSS Theo Khu Vực

### 2a. Hai Entry Point tách biệt

```
main.css  →  Chỉ nhúng vào _Layout.cshtml       (Client)
admin.css →  Chỉ nhúng vào _AdminLayout.cshtml  (Admin)
```

**`wwwroot/css/main.css`** — file tổng hợp Client:
```css
/* Layer 1 - Settings */
@import '1-settings/_variables.css';
@import '1-settings/_typography.css';

/* Layer 2 - Base */
@import '2-base/_reset.css';
@import '2-base/_elements.css';

/* Layer 3 - Overrides */
@import '3-overrides/_bootstrap.css';

/* Layer 4 - Layouts */
@import '4-layouts/_navbar.css';
@import '4-layouts/_footer.css';

/* Layer 5 - Components */
@import '5-components/_buttons.css';
@import '5-components/_cards.css';
@import '5-components/_hero.css';
@import '5-components/_forms.css';
@import '5-components/_progress.css';

/* Layer 7 - Utilities */
@import '7-utilities/_helpers.css';
```

**`wwwroot/css/admin.css`** — file tổng hợp Admin (kế thừa base, thêm admin-specific):
```css
/* Kế thừa phần nền tảng từ client (variables, reset, bootstrap override) */
@import '1-settings/_variables.css';
@import '1-settings/_typography.css';
@import '2-base/_reset.css';
@import '2-base/_elements.css';
@import '3-overrides/_bootstrap.css';

/* Shared components cần thiết cho Admin */
@import '5-components/_buttons.css';
@import '5-components/_forms.css';
@import '5-components/_progress.css';

/* Chỉ Admin mới load được */
@import '6-admin/_admin-layout.css';
@import '6-admin/_admin-menu.css';
@import '6-admin/_admin-filters.css';

@import '7-utilities/_helpers.css';
```

> **Tại sao tách 2 entry point?** Admin **không bao giờ** load `.hero-banner`, `.hero-content`... và Client **không bao giờ** load `.admin-sidebar`, `.admin-menu`... → giảm payload, tránh conflict specificity hoàn toàn.

### 2b. CSS Isolation (.cshtml.css) — Khi nào nên dùng?

| Nên dùng ✅ | Không nên dùng ❌ |
|---|---|
| Style **chỉ có ý nghĩa trong 1 view** (vd: hiệu ứng đặc biệt cho trang Login) | Style dùng chung 2 view trở lên |
| Style mang **tính thí nghiệm** chưa muốn đưa vào hệ thống | Class utilities (.text-gradient-gold) |
| Style overwrite **rất cục bộ** (1 element) | Layout, component tái sử dụng |

**Ví dụ cụ thể** — File `Views/Home/Index.cshtml.css`:
```css
/* Style CHỈ của trang Homepage, không đi đâu khác */
.homepage-hero-cta {
  animation: float 3s ease-in-out infinite;
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-8px); }
}
```

---

## Phần 3 — Quy Chuẩn BEM Áp Dụng Cho AutoHub

**BEM = Block__Element--Modifier**

### 3a. Cấu trúc đặt tên

```
.block              → Thành phần độc lập, có thể đứng riêng
.block__element     → Con của block, không tồn tại nếu không có block
.block--modifier    → Biến thể của block hoặc element
```

### 3b. Ví dụ refactor từ code thực tế

**TRƯỚC (code hiện tại — không BEM):**
```css
.admin-sidebar { ... }
.admin-sidebar.collapsed { ... }
.admin-sidebar.collapsed .sidebar-text { ... }
.admin-menu { ... }
.admin-menu-item a { ... }
.admin-menu-item a:hover { ... }
.admin-menu-item.active a { ... }
```

**SAU (BEM chuẩn):**
```css
/* Block */
.sidebar { ... }

/* Modifier — biến thể thu nhỏ */
.sidebar--collapsed { ... }

/* Element — con của sidebar */
.sidebar__text { ... }
.sidebar--collapsed .sidebar__text { display: none; }

/* Block */
.nav-menu { ... }

/* Element */
.nav-menu__item { ... }
.nav-menu__link { ... }

/* Modifier — trạng thái active */
.nav-menu__item--active .nav-menu__link { ... }
.nav-menu__link:hover { ... }
```

### 3c. Quy tắc đặt tên cho dự án AutoHub

| Prefix | Dùng cho | Ví dụ |
|--------|----------|-------|
| `sidebar__` | Elements trong sidebar | `.sidebar__brand`, `.sidebar__text` |
| `nav-menu__` | Items trong menu điều hướng | `.nav-menu__item`, `.nav-menu__link` |
| `hero__` | Elements trong hero banner | `.hero__title`, `.hero__cta` |
| `card__` | Elements trong card xe/phụ tùng | `.card__image`, `.card__price` |
| `filter__` | Elements trong bộ lọc | `.filter__group`, `.filter__input` |
| `--active` | Trạng thái đang chọn | `.nav-menu__item--active` |
| `--collapsed` | Trạng thái thu gọn | `.sidebar--collapsed` |
| `--dark` | Biến thể màu tối | `.card--dark` |

### 3d. Quy tắc "không được làm"

```css
/* ❌ SAI - Quá generic, dễ conflict */
.active { color: red; }
.btn { ... }
.content { ... }

/* ❌ SAI - Nest quá sâu, specificity cao vô lý */
.admin-sidebar ul li a:hover { ... }

/* ✅ ĐÚNG - Flat BEM, specificity = 1 class */
.sidebar__link:hover { ... }

/* ❌ SAI - !important là mùi code xấu */
.input-premium { color: #fff !important; }

/* ✅ ĐÚNG - Tăng specificity có kiểm soát */
.tech-panel .input-premium { color: #fff; }
```

---

## Phần 4 — Quy Trình Tích Hợp (Integration Workflow)

### 4a. Cập nhật `_Layout.cshtml` (Client)

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - AutoHub</title>

    <!-- 1. Bootstrap base -->
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />

    <!-- 2. Toàn bộ CSS Client (thay thế site.css cũ) -->
    <link rel="stylesheet" href="~/css/main.css" asp-append-version="true" />

    <!-- 3. CSS Isolation tự động (giữ nguyên, .NET Core tự xử lý) -->
    <link rel="stylesheet" href="~/AutoHub.styles.css" asp-append-version="true" />

    <!-- 4. Slot cho View con chèn CSS riêng (nếu cần) -->
    @await RenderSectionAsync("Styles", required: false)
</head>
```

### 4b. Cập nhật `_AdminLayout.cshtml` (Admin)

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - AutoHub Admin</title>

    <!-- 1. Bootstrap base -->
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />

    <!-- 2. Toàn bộ CSS Admin (KHÔNG nhúng main.css, chỉ admin.css) -->
    <link rel="stylesheet" href="~/css/admin.css" asp-append-version="true" />

    <!-- 3. Slot cho View con Admin chèn CSS riêng -->
    @await RenderSectionAsync("Styles", required: false)
</head>
```

> **Lưu ý:** `_AdminLayout.cshtml` hiện tại có bug nhỏ — `<link class="stylesheet"` (thiếu `rel=`). Sẽ fix trong quá trình refactor.

### 4c. `Program.cs` — Không cần thay đổi gì

.NET Core tự động xử lý Static Files Middleware và CSS Isolation. Không cần cấu hình thêm.

### 4d. Xử lý file `_Layout.cshtml.css` (CSS Isolation)

File này hiện tại **trùng hoàn toàn** với `site.css` dòng 57-107. Sau khi refactor:

```css
/* _Layout.cshtml.css — Chỉ giữ lại style CỰC KỲ cục bộ của Layout */
/* Toàn bộ nội dung cũ đã được chuyển vào wwwroot/css/3-overrides/_bootstrap.css */
/* và wwwroot/css/4-layouts/ */

/* File này có thể để trống hoặc xoá đi nếu không có gì đặc thù */
```

---

## Phần 5 — Nội Dung Từng File Sau Khi Tách

### `1-settings/_variables.css`
```css
/* Toàn bộ :root variables từ site.css dòng 7-27 */
:root {
  --bg-creamy: #F5F5F7;
  --text-piano: #111111;
  --text-muted: #666666;
  --accent-ferrari: #FF2828;
  --accent-gold: #D4AF37;
  --accent-mint: #2EE59D;
  --border-light: rgba(17, 17, 17, 0.08);
  --glass-bg: rgba(255, 255, 255, 0.85);
  --glass-blur: 16px;
  --glass-shadow: 0 8px 32px 0 rgba(0, 0, 0, 0.04);
  --bs-primary: #1b6ec2;
  --bs-link-color: #0077cc;
  --bs-body-bg: var(--bg-creamy);
  --bs-body-color: var(--text-piano);
}
```

### `1-settings/_typography.css`
```css
/* @import font + scale */
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;900&display=swap');
```

### `2-base/_reset.css`
```css
*, *::before, *::after { box-sizing: border-box; }

body {
  margin: 0;
  padding: 0;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
```

### `2-base/_elements.css`
```css
body {
  font-family: 'Outfit', sans-serif;
  background-color: var(--bs-body-bg);
  color: var(--bs-body-color);
}

a { color: var(--bs-link-color); }
```

### `3-overrides/_bootstrap.css`
```css
/* Tập trung toàn bộ Bootstrap overrides — dễ tìm, dễ xoá */
a.navbar-brand {
  white-space: normal;
  text-align: center;
  word-break: break-all;
  font-weight: 900;
  letter-spacing: 1.5px;
}

.btn-primary {
  color: #fff;
  background-color: var(--bs-primary);
  border-color: #1861ac;
}

.nav-pills .nav-link.active,
.nav-pills .show > .nav-link {
  color: #fff;
  background-color: var(--bs-primary);
  border-color: #1861ac;
}

.border-top { border-top: 1px solid #e5e5e5; }
.border-bottom { border-bottom: 1px solid #e5e5e5; }
.box-shadow { box-shadow: 0 .25rem .75rem rgba(0, 0, 0, .05); }
button.accept-policy { font-size: 1rem; line-height: inherit; }
```

### `4-layouts/_footer.css`
```css
.footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  white-space: nowrap;
  line-height: 60px;
}
```

### `5-components/_hero.css`
```css
.hero {
  position: relative;
  height: 70vh;
  width: 100%;
  background-size: cover;
  background-position: center;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  border-radius: 0 0 12px 12px;
  overflow: hidden;
  box-shadow: var(--glass-shadow);
}

.hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(0,0,0,0.1) 0%, rgba(0,0,0,0.7) 100%);
  z-index: 1;
}

.hero__content {
  position: relative;
  z-index: 2;
  color: #fff;
  max-width: 800px;
  padding: 2rem;
}

.hero__title {
  font-size: 3.5rem;
  font-weight: 900;
  letter-spacing: -1px;
  text-transform: uppercase;
  margin-bottom: 1rem;
}

.hero__subtitle {
  font-size: 1.25rem;
  font-weight: 300;
  margin-bottom: 2rem;
  opacity: 0.9;
}
```

### `5-components/_buttons.css`
```css
.btn-neon {
  background-color: transparent;
  color: var(--accent-gold);
  border: 1px solid var(--accent-gold);
  padding: 0.75rem 1.5rem;
  border-radius: 6px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
  transition: all 0.3s ease;
  box-shadow: 0 0 10px rgba(212, 175, 55, 0.1);
  cursor: pointer;
  text-decoration: none;
  display: inline-block;
}

.btn-neon:hover {
  background-color: var(--accent-gold);
  color: #000;
  box-shadow: 0 0 20px rgba(212, 175, 55, 0.4);
  transform: translateY(-2px);
}
```

### `6-admin/_admin-layout.css`
```css
/* Scoped hoàn toàn trong admin, không rò ra ngoài */
.admin-layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  width: 260px;
  background-color: var(--text-piano);
  color: #fff;
  padding: 2rem 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
  position: sticky;
  top: 0;
  height: 100vh;
  transition: width 0.3s ease;
  z-index: 100;
}

.sidebar--collapsed { width: 80px; }
.sidebar--collapsed .sidebar__text { display: none; }

.sidebar__content { flex-grow: 1; }

.admin-main {
  flex-grow: 1;
  padding: 2.5rem;
  background-color: var(--bg-creamy);
}
```

### `6-admin/_admin-menu.css`
```css
.nav-menu {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.nav-menu__link {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 1rem;
  color: rgba(255, 255, 255, 0.7);
  text-decoration: none;
  border-radius: 4px;
  transition: all 0.3s ease;
}

.nav-menu__link:hover,
.nav-menu__item--active .nav-menu__link {
  background-color: var(--accent-ferrari);
  color: #fff;
}
```

---

## Câu Hỏi Mở — Cần Bạn Xác Nhận Trước Khi Thực Hiện

> [!IMPORTANT]
> **Câu hỏi 1:** Bạn có muốn tôi **đổi tên class trong các file `.cshtml`** không (ví dụ: `.admin-sidebar` → `.sidebar`, `.admin-menu-item` → `.nav-menu__item`)?
>
> Điều này đòi hỏi chỉnh sửa đồng thời CSS + HTML. Nếu bạn chưa muốn đụng vào HTML hiện tại, tôi có thể **chỉ tách file CSS** mà giữ nguyên tên class.

> [!IMPORTANT]
> **Câu hỏi 2:** Dự án có dùng **Bundling & Minification** (file `bundleconfig.json`) không, hay đang dùng `@import` thuần CSS? Điều này ảnh hưởng đến cách tôi thiết kế entry point.

> [!NOTE]
> **Câu hỏi 3:** Bạn có định dùng **SCSS/Sass** trong tương lai không? Nếu có, tôi sẽ thiết kế thư mục với tiền tố `_` (partial files) sẵn sàng migrate sang Sass.

---

## Tóm Tắt Kết Quả Sau Khi Refactor

| Chỉ số | Trước | Sau |
|--------|-------|-----|
| Số file CSS | 2 (site.css + _Layout.cshtml.css) | ~15 file nhỏ có tổ chức |
| Payload Client | ~8KB (chứa cả admin CSS) | ~5KB (chỉ client) |
| Payload Admin | ~8KB (chứa cả client CSS) | ~4KB (chỉ admin) |
| Nguy cơ conflict | Cao | Gần như 0 |
| Thời gian tìm style | ~30 giây | ~3 giây |
| Khả năng thêm dev mới | Khó | Dễ (mỗi dev phụ trách 1 folder) |
