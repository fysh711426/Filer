var searchMixin = {
    data() {
        return {
            hasSearch: false,
            isOverResultLimit: false,
            isUseSearchAsync: false,
            isUseVariantSearch: false,
            searchText: '',
            searchTextTW: ''
        };
    },
    methods: {
        initSearch(page, searchText) {
            var searchInputText = searchText || '';
            var isSearchOpen = false;
            if (searchInputText && storage.isSearchOpen())
                isSearchOpen = true;

            var search = searchBar('.search-box-block');
            search.init(searchInputText, isSearchOpen);
            search.onToggle = function (isOpen) {
                storage.setIsSearchOpen(isOpen);
            }
            search.onSearch = (text) => {
                this.search(page, text);
            }
        },
        search(page, searchText) {
            //var search = this.searchInputText.replace(/[<>:"\/\\|?*]/g, "");

            var pattern = /[<>:"\/\\|?*]/g;
            var match = pattern.test(searchText);
            if (match) {
                function escapeHtml(unsafe) {
                    return unsafe
                        .replace(/&/g, "&amp;")
                        .replace(/</g, "&lt;")
                        .replace(/>/g, "&gt;")
                        .replace(/"/g, "&quot;")
                        .replace(/'/g, "&#039;");
                }
                //var msg = `
                //    <div class="search-input-error">
                //        <div>
                //            <span class="search-input-error-icon">
                //                <i class="fa-solid fa-circle-exclamation fa-fw fa-lg"></i>
                //            </span>
                //            <span>${escapeHtml(this.local.searchInputErrorMessage)}</span>
                //        </div>
                //        <div>\\ \/ : * ? \" < > |</div>
                //    </div>
                //`;
                var msg = escapeHtml(this.local.searchInputErrorMessage);
                toast.show(msg, {
                    template: '.search-input-toast',
                    //delay: 100000
                });
                this.$refs.searchInput.blur();
                return;
            }

            if (searchText) {
                var url = this.routeLinkWithSearch(searchText);
                if (page === 'folder')
                    url = this.routeLinkWithSearch('folder', this.workNum, this.dirPath, searchText);
                location.href = url;

                //var url = this.routeLinkWithSearch('search', searchText);
                //if (page === 'folder')
                //    url = this.routeLinkWithSearch('search', this.workNum, this.dirPath, searchText);
                //location.href = url;
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
        mark(text, search) {
            search = search.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
            var regex = new RegExp(search, "gi");
            return text.replace(regex, '<span class="mark">$&</span>');
        },
        markWithTW(text, textTW, searchTW) {
            if (!text || !textTW || !searchTW)
                return text;

            // 將字串轉成 code point array
            function toCodePoints(str) {
                // Array.from 會正確處理 surrogate pair
                return Array.from(str);
            }

            var texts = toCodePoints(text);
            var textTWs = toCodePoints(textTW);
            var searchTWs = toCodePoints(searchTW);

            var result = [];

            var i = 0;
            while (i <= textTWs.length - searchTWs.length) {
                var match = true;

                for (var j = 0; j < searchTWs.length; j++) {
                    if (textTWs[i + j].toLowerCase() !== searchTWs[j].toLowerCase()) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    var start = i;
                    var end = i + searchTWs.length;

                    result.push({
                        start, end
                    });

                    i += searchTWs.length;
                    continue;
                }
                i++;
            }

            if (result.length == 0)
                return text;

            var html = '';
            var lastIndex = 0;
            for (var r of result) {
                html += texts.slice(lastIndex, r.start).join("");
                html += `<span class="mark">${texts.slice(r.start, r.end).join("")}</span>`;
                lastIndex = r.end;
            }
            html += texts.slice(lastIndex).join("");
            return html;
        },
        bindMark(data, encodeData) {
            for (var item of data.datas) {
                var html = '';
                if (data.hasSearch) {
                    if (data.isUseVariantSearch)
                        html = this.markWithTW(item.name, item.nameTW, encodeData.searchTextTW);
                    else
                        html = this.mark(item.name, encodeData.searchText);
                }
                item.nameWithMark = html;
            }
        },
        loadSearchResults(page) {
            var url = this.routeLinkWithSearch('api/search', this.searchText);
            if (page === 'folder') {
                url = this.routeLinkWithSearch('api/search', this.workNum, this.dirPath, this.searchText);
            }
            function fetchDatas(callback, _error, _finally) {
                fetch(url).then((response) => {
                    var decoder = new TextDecoder();
                    var reader = response.body.getReader();

                    var buffer = '';
                    function readChunk() {
                        reader.read().then(({ value, done }) => {
                            if (done) {
                                if (buffer.trim()) {
                                    callback(buffer);
                                }
                                _finally();
                                return;
                            }
                            var chunk = decoder.decode(value, { stream: true });
                            var lines = (buffer + chunk).split('\n');
                            buffer = lines.pop();

                            for (var line of lines) {
                                if (line.trim()) {
                                    callback(line);
                                }
                            }
                            readChunk();
                        });
                    }
                    readChunk();
                });
            }

            progress.start();
            fetchDatas((line) => {
                var item = JSON.parse(line);
                this.datas.push(item);
            }, () => {
                
            }, () => {
                progress.done();
            });
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