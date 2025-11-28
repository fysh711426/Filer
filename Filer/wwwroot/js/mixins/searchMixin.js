var searchMixin = {
    data() {
        return {
            isSearchOpen: false,
            isSearchFocus: false,
            searchInputText: ''
        };
    },
    methods: {
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
            if (this.searchInputText) {
                var url = this.routeLinkWithSearch(this.searchInputText);
                if (page === 'folder')
                    url = this.routeLinkWithSearch('folder', this.workNum, this.dirPath, this.searchInputText);
                location.href = url;
            }
        }
    }
};