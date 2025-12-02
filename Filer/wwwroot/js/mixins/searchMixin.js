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
                        <div>
                            <span class="search-input-error-icon">
                                <i class="fa-solid fa-circle-exclamation fa-fw fa-lg"></i>
                            </span>
                            <span>${escapeHtml(this.local.searchInputErrorMessage)}</span>
                        </div>
                        <div>\\ \/ : * ? \" < > |</div>
                    </div>
                `;
                toast.show(msg);
                this.$refs.searchInput.blur();
                return;
            }

            var search = this.searchInputText;
            if (search) {
                var url = this.routeLinkWithSearch(search);
                if (page === 'folder')
                    url = this.routeLinkWithSearch('folder', this.workNum, this.dirPath, search);
                location.href = url;
            }
        },
        bindSearchPath(data) {
            for (var item of data.datas) {
                var path = '';
                if (data.hasSearch) {
                    if (!item.dirName)
                        path = item.workDir;
                    else if (!item.parentDirPath)
                        path = item.dirName + ' (' + item.workDir + ')';
                    else
                        path = item.dirName + ' (' + item.workDir + '/' + item.parentDirPath + ')';
                }
                item.searchDirPath = path;
            }
        },
        mark(text, keyword) {
            keyword = keyword.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
            var regex = new RegExp(keyword, "gi");
            return text.replace(regex, '<span class="mark">$&</span>');
        },
        bindMark(data, encodeData) {
            for (var item of data.datas) {
                var html = '';
                if (data.hasSearch) {
                    html = this.mark(item.name, encodeData.searchText);
                }
                item.nameWithMark = html;
            }
        }
    },
    computed: {
        linkTarget() {
            return this.hasSearch ? '_blank' : null;
        },
        linkRel() {
            return this.hasSearch ? 'noopener noreferrer' : null;
        }
    }
};