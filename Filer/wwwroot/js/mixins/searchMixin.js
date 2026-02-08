var searchMixin = {
    data() {
        return {
            hasSearch: false,
            isOverResultLimit: false,
            isUseVariantSearch: false,
            isUseSearchAsync: false,
            searchResultLimit: 0,
            searchText: '',
            searchTextTW: '',
            isDataStreaming: false
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
            }
        },
        bindSearchPaths(data) {
            for (var item of data.datas) {
                this.bindSearchPath(item, data);
            }
        },
        bindSearchPath(item, data) {
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
        bindMarks(data, encodeData) {
            for (var item of data.datas) {
                this.bindMark(item, data, encodeData);
            }
        },
        bindMark(item, data, encodeData) {
            var html = '';
            if (data.hasSearch) {
                if (data.isUseVariantSearch)
                    html = this.markWithTW(item.name, item.nameTW, encodeData.searchTextTW);
                else
                    html = this.mark(item.name, encodeData.searchText);
            }
            item.nameWithMark = html;
        },
        loadDataResults(page) {
            var url = this.routeLinkWithSearch('api/search', this.searchText);
            if (page === 'folder') {
                url = this.routeLinkWithSearch('api/search', this.workNum, this.dirPath, this.searchText);
            }

            this.collator = new Intl.Collator(undefined, { numeric: true });

            function getDirNameTitle(item) {
                return item.dirName || item.workDir || '';
            }
            function fetchStream(url, callback) {
                return new Promise((resolve, reject) => {
                    fetch(url).then((response) => {
                        if (!response.ok)
                            throw new Error(`HTTP error! status: ${response.status}`);

                        var decoder = new TextDecoder();
                        var reader = response.body.getReader();

                        var buffer = '';
                        function readChunk() {
                            reader.read().then(({ value, done }) => {
                                if (done) {
                                    if (buffer.trim()) {
                                        callback(buffer);
                                    }
                                    resolve();
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
                            }).catch(reject);
                        }
                        readChunk();
                    }).catch(reject);
                });
            }

            progress.start();
            this.isDataStreaming = true;
            var count = 0;
            fetchStream(url, (line) => {
                if (count >= this.searchResultLimit) {
                    this.isOverResultLimit = true;
                    throw 'STOP_STREAM';
                }
                count++;
                var item = JSON.parse(line);
                this.bindData(item);
                this.bindLink(item, initialData);
                this.bindSearchPath(item, initialData);
                this.bindMark(item, initialData, initialEncodeData);
                //this.datas.push(item);
                item.dirNameTitle = getDirNameTitle(item);
                item.lastWriteTimeUtcAt = new Date(item.lastWriteTimeUtc).getTime();
                var index = this.binarySearch(item, this.datas);
                this.datas.splice(index, 0, item);
            }).catch((err) => {
                // pass
            }).finally(() => {
                progress.done();
                this.isDataStreaming = false;
            });
        },
        compareText(a, b) {
            //return a.localeCompare(b);
            //return a.localeCompare(b, undefined, { numeric: true });
            return this.collator.compare(a || '', b || '');
        },
        compareValue(a, b) {
            if (a === b)
                return 0;
            return a > b ? 1 : -1;
        },
        compare(a, b) {
            if (a.fileType !== b.fileType) {
                return a.fileType - b.fileType;
            }
            
            var result = 0;
            var orderBy = this.orderBy;
            if (orderBy == "name") {
                result = this.compareText(a.name, b.name);
                if (result === 0)
                    result = this.compareValue(a.index, b.index);
            }
            else if (orderBy == "date") {
                //result = this.compareValue(a.lastWriteTimeUtc, b.lastWriteTimeUtc);
                result = this.compareValue(a.lastWriteTimeUtcAt, b.lastWriteTimeUtcAt);
                if (result === 0)
                    result = this.compareText(a.name, b.name);
                if (result === 0)
                    result = this.compareValue(a.index, b.index);
            }
            else if (orderBy == "size") {
                result = this.compareValue(a.fileLength, b.fileLength);
                if (result === 0)
                    result = this.compareText(a.name, b.name);
                if (result === 0)
                    result = this.compareValue(a.index, b.index);
            }
            else {
                //result = this.compareText(
                //    this.getDirNameTitle(a.dirName, a.workDir),
                //    this.getDirNameTitle(b.dirName, b.workDir));
                result = this.compareText(a.dirNameTitle, b.dirNameTitle);
                if (result === 0)
                    result = this.compareText(a.name, b.name);
                if (result === 0)
                    result = this.compareValue(a.index, b.index);
            }
            return this.isOrderByDesc ? result * -1 : result;
        },
        binarySearch(item, datas) {
            var low = 0;
            var high = datas.length;
            while (low < high) {
                var mid = (low + high) >>> 1;
                if (this.compare(item, datas[mid]) < 0) {
                    high = mid;
                } else {
                    low = mid + 1;
                }
            }
            return low;
        },
        reorder() {
            progress.start();
            this.datas.sort((a, b) => {
                return this.compare(a, b);
            });
            progress.done();
        }
    },
    computed: {
        linkTarget() {
            return this.hasSearch ? '_blank' : null;
        },
        linkRel() {
            return this.hasSearch ? 'noopener noreferrer' : null;
        },
        isUseDataStreaming() {
            return this.hasSearch && this.isUseSearchAsync;
        }
    }
};