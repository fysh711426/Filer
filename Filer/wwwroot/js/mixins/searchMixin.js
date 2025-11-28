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
            var search = this.searchInputText.replace(/[<>:"\/\\|?*`]/g, "");
            if (search) {
                var url = this.routeLinkWithSearch(search);
                if (page === 'folder')
                    url = this.routeLinkWithSearch('folder', this.workNum, this.dirPath, search);
                location.href = url;
            }
        }
    }
};