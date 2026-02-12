var bookmarkMixin = {
    data() {
        return {
            fileType: 0,
            bookmark: null
        };
    },
    methods: {
        initBookmark() {
            this.bookmark = null;
            var url = this.getBookmarkUrl();
            var bookmarks = storage.bookmarks();
            var tuple = this.findBookmark(bookmarks, url);
            if (tuple) {
                this.bookmark = tuple.item;
            }
        },
        saveBookmark(groupName) {
            groupName = groupName || 'Other';
            var url = this.getBookmarkUrl();
            var bookmarks = storage.bookmarks();
            var group = this.findGroup(bookmarks, groupName);
            if (!group) {
                group = {
                    name: groupName,
                    items: []
                };
                bookmarks.groups.push(group);
            }
            var tuple = this.findBookmark(bookmarks, url);
            if (tuple) {
                var index = tuple.group.items.indexOf(tuple.item);
                if (index !== -1) {
                    tuple.group.items.splice(index, 1);
                }
            }
            var bookmark = {
                url: url,   // key
                fileType: this.fileType,
                workNum: this.workNum,
                path: this.path,
                name: this.getBookmarkName(),
                link: this.getBookmarkLink(),
                thumb: this.getBookmarkThumb()
            };
            group.items.push(bookmark);
            storage.setBookmarks(bookmarks);
            this.initBookmark();
            toast.show(this.local.bookmarkSaveMessage);
        },
        removeBookmark() {
            var url = this.getBookmarkUrl();
            var bookmarks = storage.bookmarks();
            var tuple = this.findBookmark(bookmarks, url);
            if (tuple) {
                var index = tuple.group.items.indexOf(tuple.item);
                if (index !== -1) {
                    tuple.group.items.splice(index, 1);
                }
            }
            storage.setBookmarks(bookmarks);
            this.initBookmark();
            toast.show(this.local.bookmarkRemoveMessage);
        },
        findBookmark(bookmarks, url) {
            for (var group of bookmarks.groups) {
                for (var item of group.items) {
                    if (item.url === url) {
                        return {
                            group: group,
                            item: item
                        };
                    }
                }
            }
            return null;
        },
        findGroup(bookmarks, name) {
            for (var group of bookmarks.groups) {
                if (group.name === name) {
                    return group;
                }
            }
            return null;
        },
        getBookmarkUrl() {
            return this.getPageShowPath();
        },
        getBookmarkLink() {
            if (this.fileType === this.type.workDir)
                return this.routeLink('folder', this.workNum);
            if (this.fileType === this.type.folder)
                return this.routeLink('folder', this.workNum, this.path);
            if (this.fileType === this.type.text)
                return this.routeLink('text', this.workNum, this.path);
            if (this.fileType === this.type.image)
                return this.routeLink('image', this.workNum, this.path);
            if (this.fileType === this.type.audio)
                return this.routeLink('audio', this.workNum, this.path);
            if (this.fileType === this.type.video)
                return this.routeLink('video', this.workNum, this.path);
        },
        getBookmarkThumb() {
            if (this.fileType === this.type.image)
                return this.routeLink('api/thumbnail/image', this.workNum, this.path);
            if (this.fileType === this.type.video)
                return this.routeLink('api/thumbnail/video', this.workNum, this.path);
            return '';
        },
        getBookmarkName() {
            return this.fileName || this.dirName || this.workDir || '';
        },
        onBookmarkInit(_popover) {
            var _this = this;
            _popover.onCreate = function (html) {
                var _bookmarks = storage.bookmarks();
                if (_bookmarks.groups.length === 0) {
                    _bookmarks.groups.push({
                        name: 'Other',
                        items: []
                    });
                }
                var _selected = null;
                if (_this.bookmark) {
                    var tuple = _this.findBookmark(_bookmarks, _this.bookmark.url);
                    if (tuple) {
                        _selected = tuple.group;
                    }
                }
                var ViewModel = Vue.extend({
                    template: html,
                    data() {
                        return {
                            selected: _selected,
                            groups: _bookmarks.groups
                        }
                    },
                    methods: {
                        onItemClick(group) {
                            if (group === this.selected) {
                                _this.removeBookmark();
                                this.selected = null;
                            }
                            else {
                                _this.saveBookmark(group.name);
                                this.selected = group;
                            }
                            setTimeout(() => {
                                _popover.close();
                            }, 50);
                        }
                    }
                });
                var vm = new ViewModel();
                _popover.onRemove = () => {
                    vm.$destroy();
                    vm.$el.remove();
                    vm = null;
                };
                vm.$mount();
                return vm.$el;
            };
        }
    }
};