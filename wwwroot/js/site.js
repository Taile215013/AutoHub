// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // --- LOCAL SEARCH LOGIC (If any) ---
    const searchInputs = document.querySelectorAll('.search-input-autocomplete');
    
    searchInputs.forEach(searchInput => {
        const suggestionsBox = searchInput.nextElementSibling;
        const category = searchInput.getAttribute('data-category') || 'Cars';
        let timeout = null;

        searchInput.addEventListener('input', function () {
            clearTimeout(timeout);
            const term = this.value.trim();
            
            if (term.length < 1) {
                suggestionsBox.style.display = 'none';
                return;
            }

            timeout = setTimeout(() => {
                fetch(`/api/catalog/suggestions?term=${encodeURIComponent(term)}&category=${category}`)
                    .then(res => res.json())
                    .then(data => {
                        if (data && data.length > 0) {
                            suggestionsBox.innerHTML = '';
                            data.forEach(item => {
                                const div = document.createElement('div');
                                div.className = 'suggestion-item p-2 border-bottom text-dark';
                                div.style.cursor = 'pointer';
                                div.textContent = item;
                                div.addEventListener('click', () => {
                                    searchInput.value = item;
                                    suggestionsBox.style.display = 'none';
                                    searchInput.closest('form').submit();
                                });
                                suggestionsBox.appendChild(div);
                            });
                            suggestionsBox.style.display = 'block';
                        } else {
                            suggestionsBox.style.display = 'none';
                        }
                    });
            }, 300);
        });

        document.addEventListener('click', function (e) {
            if (!searchInput.contains(e.target) && !suggestionsBox.contains(e.target)) {
                suggestionsBox.style.display = 'none';
            }
        });
    });

    // --- HEADER SEARCH LOGIC ---
    const headerForm = document.getElementById('headerSearchForm');
    const headerInput = document.getElementById('headerSearchInput');
    const headerSuggestions = document.getElementById('headerSearchSuggestions');
    let headerTimeout = null;

    if (headerForm && headerInput) {
        headerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            const term = headerInput.value.trim();
            // Since we removed category dropdown, we just search all or default to /cars
            // Actually, if we remove category dropdown, we should submit to a global search, 
            // but for now let's submit to /cars, /moto, or /parts depending on the current path, 
            // OR default to /cars if on homepage.
            let action = "/cars";
            if (window.location.pathname.startsWith("/cars")) action = "/cars";
            if (window.location.pathname.startsWith("/moto")) action = "/moto";
            if (window.location.pathname.startsWith("/parts")) action = "/parts";

            if (term) {
                window.location.href = `${action}?searchTerm=${encodeURIComponent(term)}`;
            } else {
                window.location.href = action;
            }
        });

        headerInput.addEventListener('input', function () {
            clearTimeout(headerTimeout);
            const term = this.value.trim();
            
            if (term.length < 1) {
                headerSuggestions.style.display = 'none';
                return;
            }

            let categoryParams = "All";
            if (window.location.pathname.startsWith("/cars")) categoryParams = "Cars";
            if (window.location.pathname.startsWith("/moto")) categoryParams = "Moto";
            if (window.location.pathname.startsWith("/parts")) categoryParams = "Parts";

            headerTimeout = setTimeout(() => {
                fetch(`/api/catalog/suggestions?term=${encodeURIComponent(term)}&category=${categoryParams}`)
                    .then(res => res.json())
                    .then(data => {
                        if (data && data.length > 0) {
                            headerSuggestions.innerHTML = '';
                            data.forEach(item => {
                                const div = document.createElement('div');
                                div.className = 'suggestion-item p-2 border-bottom text-dark';
                                div.style.cursor = 'pointer';
                                div.textContent = item;
                                div.addEventListener('click', () => {
                                    headerInput.value = item;
                                    headerSuggestions.style.display = 'none';
                                    headerForm.dispatchEvent(new Event('submit'));
                                });
                                headerSuggestions.appendChild(div);
                            });
                            headerSuggestions.style.display = 'block';
                        } else {
                            headerSuggestions.style.display = 'none';
                        }
                    });
            }, 300);
        });

        document.addEventListener('click', function (e) {
            if (headerInput && headerSuggestions && !headerInput.contains(e.target) && !headerSuggestions.contains(e.target)) {
                headerSuggestions.style.display = 'none';
            }
        });
    }

    // --- VIEW TOGGLE LOGIC ---
    const viewButtons = document.querySelectorAll('.view-toggle-btn');
    const productGrid = document.getElementById('product-grid');

    if (viewButtons.length > 0 && productGrid) {
        // Load saved view from localStorage
        const savedView = localStorage.getItem('catalogViewMode') || 'grid-3';
        applyViewMode(savedView);

        viewButtons.forEach(btn => {
            btn.addEventListener('click', function () {
                const view = this.getAttribute('data-view');
                applyViewMode(view);
                localStorage.setItem('catalogViewMode', view);
            });
        });

        function applyViewMode(mode) {
            // Remove active classes
            viewButtons.forEach(b => b.classList.remove('active', 'btn-ferrari'));
            const activeBtn = document.querySelector(`.view-toggle-btn[data-view="${mode}"]`);
            if (activeBtn) {
                activeBtn.classList.add('active', 'btn-ferrari');
            }

            // Remove all row-cols-* classes
            productGrid.className = productGrid.className.replace(/row-cols-\S+/g, '').trim();
            productGrid.classList.remove('list-view');

            if (mode === 'list') {
                productGrid.classList.add('row-cols-1', 'list-view');
            } else if (mode === 'grid-3') {
                productGrid.classList.add('row-cols-1', 'row-cols-md-2', 'row-cols-lg-3');
            } else if (mode === 'grid-4') {
                productGrid.classList.add('row-cols-1', 'row-cols-md-2', 'row-cols-lg-3', 'row-cols-xl-4');
            }
        }
    }
});
