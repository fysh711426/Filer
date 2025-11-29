var searchMixin = {
    data() {
        return {
            isSearchOpen: false,
            isSearchFocus: false,
            searchInputText: ''
        };
    },
    methods: {
        initSearch(searchText) {
            this.searchInputText = searchText || '';
            if (this.searchInputText && storage.isSearchOpen())
                this.isSearchOpen = true;
        },
        backSearch() {
            this.isSearchOpen = false;
            storage.setIsSearchOpen(this.isSearchOpen);
        },
        toggleSearch() {
            this.isSearchOpen = !this.isSearchOpen;
            storage.setIsSearchOpen(this.isSearchOpen);
            setTimeout(() => {
                this.$refs.searchInput.focus();
            }, 1);
        },
        onSearchFocus(focus) {
            this.isSearchFocus = focus;
        },
        clearSearch() {
            this.searchInputText = '';
            this.$refs.searchInput.focus();
        },
        search(page) {
            //var search = this.searchInputText.replace(/[<>:"\/\\|?*]/g, "");

            var pattern = /[<>:"\/\\|?*]/g;
            var match = pattern.test(this.searchInputText);
            if (match) {
                function escapeHtml(unsafe) {
                    return unsafe
                        .replace(/&/g, "&amp;")
                        .replace(/</g, "&lt;")
                        .replace(/>/g, "&gt;")
                        .replace(/"/g, "&quot;")
                        .replace(/'/g, "&#039;");
                }
                var msg = `
                    <div class="search-input-error">
                        <div class="search-input-error-inner">
                            <span>${escapeHtml(this.local.searchInputErrorMessage)}</span>
                            <span>\\ \/ : * ? \" < > |</span>
                        </div>
                    </div>
                `;
                toast.show(msg);
                return;
            }

            var search = this.searchInputText;
            if (search) {
                var url = this.routeLinkWithSearch(search);
                if (page === 'folder')
                    url = this.routeLinkWithSearch('folder', this.workNum, this.dirPath, search);
                location.href = url;
            }
        }
    }
};