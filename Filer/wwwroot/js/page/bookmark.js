var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        bookmarks: {},
        //bookmarks: {
        //    groups: [{
        //        name: '',
        //        items: [{
        //            url: '',   // key
        //            fileType: '',
        //            workNum: '',
        //            path: '',
        //            name: '',
        //            link: '',
        //            thumb: ''
        //        }]
        //    }]
        //}
        selected: null,
        groupMenuOptions: [],
        bookmarkMenuOptions: []
    },
    created() {
        var bookmarks = storage.bookmarks();
        this.bindBookmarkData(bookmarks);
        this.bookmarks = bookmarks;
        this.initData(initialData);
        this.initBookmarkMenu();
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        this.isLoaded = true;
        setTimeout(function () {
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        initBookmarkMenu() {
            this.groupMenuOptions = [
                { text: 'Edit', value: 0, icon: 'fa-solid fa-pen fa-fw' },
                { text: 'Delete', value: 1, icon: 'fa-solid fa-trash fa-fw', className: 'danger', separator: true }
            ];
            this.bookmarkMenuOptions = [
                { text: 'Delete', icon: 'fa-solid fa-trash fa-fw', className: 'danger' }
            ];
        },
        bindGroupData(group) {
            group.expand = true;
        },
        bindBookmarkData(bookmarks) {
            for (var group of bookmarks.groups) {
                this.bindGroupData(group);
            }
        },
        onGroupClick(group) {
            group.expand = !group.expand;
        },
        onBookmarkClick(item) {
            if (this.selected === item) {
                this.selected = null;
                return;
            }
            this.selected = item;
        },
        onDragChoose() {
            if (window.navigator && window.navigator.vibrate) {
                window.navigator.vibrate(50);
            }
        },
        onDragChange() {
            this.saveBookmarks();
        },
        cloneBookmarks(bookmarks) {
            var _bookmarks = {
                groups: []
            };
            if (bookmarks.groups) {
                for (var group of bookmarks.groups) {
                    var _group = {
                        name: group.name,
                        items: []
                    };
                    _bookmarks.groups.push(_group);
                    if (group.items) {
                        for (var item of group.items) {
                            var _item = {
                                url: item.url,
                                fileType: item.fileType,
                                workNum: item.workNum,
                                path: item.path,
                                name: item.name,
                                link: item.link,
                                thumb: item.thumb
                            };
                            _group.items.push(_item);
                        }
                    }
                }
            }
            return _bookmarks;
        },
        saveBookmarks() {
            var _bookmarks = this.cloneBookmarks(this.bookmarks);
            storage.setBookmarks(_bookmarks);
            //var bookmarks = storage.bookmarks();
            //this.bindBookmarkData(bookmarks);
            //this.bookmarks = bookmarks;
            //toast.show(this.local.bookmarkSaveMessage);
            toast.show(this.local.saveSuccess);
        },
        onGroupMenuChange(val, group) {
            if (val === 0) {
                this.editGroup(group);
                return;
            }
            this.deleteGroup(group);
        },
        onBookmarkMenuChange(val, group, item) {
            this.deleteBookmark(group, item);
        },
        createGroup() {
            var _value = '';
            var open = () => {
                var _prompt = promptModal({
                    title: this.local.createGroup,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel,
                    input: {
                        textarea: false,
                        value: _value,
                        placeholder: this.local.name,
                        required: true
                    }
                });
                _prompt.onClosed = (ele, action, value) => {
                    if (action === 'confirm') {
                        function _findGroup(bookmarks, name) {
                            for (var _group of bookmarks.groups) {
                                if (_group.name === name) {
                                    return _group;
                                }
                            }
                        }
                        var _group = _findGroup(this.bookmarks, value);
                        if (_group) {
                            setTimeout(() => {
                                var _alert = alertModal({
                                    content: this.local.groupNameExists
                                });
                                _alert.onClosed = (ele, action) => {
                                    setTimeout(() => {
                                        _value = value;
                                        open();
                                    }, 100);
                                }
                                _alert.open();
                            }, 100);
                            return;
                        }
                        var newGroup = {
                            name: value,
                            items: []
                        };
                        this.bindGroupData(newGroup);
                        this.bookmarks.groups.push(newGroup);
                        this.saveBookmarks();
                    }
                }
                _prompt.open();
            }
            open();
        },
        editGroup(group) {
            var _value = group.name;
            var open = () => {
                var _prompt = promptModal({
                    title: this.local.edit,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel,
                    input: {
                        textarea: false,
                        value: _value,
                        placeholder: this.local.name,
                        required: true
                    }
                });
                _prompt.onClosed = (ele, action, value) => {
                    if (action === 'confirm') {
                        function _findGroup(bookmarks, name) {
                            for (var _group of bookmarks.groups) {
                                if (_group !== group && _group.name === name) {
                                    return _group;
                                }
                            }
                        }
                        var _group = _findGroup(this.bookmarks, value);
                        if (_group) {
                            setTimeout(() => {
                                var _alert = alertModal({
                                    content: this.local.groupNameExists
                                });
                                _alert.onClosed = (ele, action) => {
                                    setTimeout(() => {
                                        _value = value;
                                        open();
                                    }, 100);
                                }
                                _alert.open();
                            }, 100);
                            return;
                        }
                        group.name = value;
                        this.saveBookmarks();
                    }
                }
                _prompt.open();
            }
            open();
        },
        deleteGroup(group) {
            var _confirm = confirmModal({
                //title: this.local.delete,
                content: this.local.deleteGroupConfirmMessage,
                confirmText: this.local.delete,
                cancelText: this.local.cancel,
                confirmClass: 'danger',
                size: 'modal-sm',
                btnSize: 'btn-sm'
            });
            _confirm.onClosed = (ele, action) => {
                if (action === 'confirm') {
                    var index = this.bookmarks.groups.indexOf(group);
                    if (index !== -1) {
                        this.bookmarks.groups.splice(index, 1);
                        this.saveBookmarks();
                    }
                }
            }
            _confirm.open();
        },
        deleteBookmark(group, item) {
            var _confirm = confirmModal({
                //title: this.local.delete,
                content: this.local.deleteBookmarkConfirmMessage,
                confirmText: this.local.delete,
                cancelText: this.local.cancel,
                confirmClass: 'danger',
                size: 'modal-sm',
                btnSize: 'btn-sm'
            });
            _confirm.onClosed = (ele, action) => {
                if (action === 'confirm') {
                    var index = group.items.indexOf(item);
                    if (index !== -1) {
                        group.items.splice(index, 1);
                        this.saveBookmarks();
                    }
                }
            }
            _confirm.open();
        },
        updateBookmarks(bookmarks, _bookmarks) {
            function _findGroup(bookmarks, name) {
                for (var _group of bookmarks.groups) {
                    if (_group.name === name) {
                        return _group;
                    }
                }
            }
            
            if (Array.isArray(_bookmarks.groups)) {
                var itemMap = new Map();
                for (var _group of bookmarks.groups) {
                    for (var _item of _group.items) {
                        itemMap.set(_item.url, {
                            group: _group,
                            item: _item
                        });
                    }
                }
                for (var _group of _bookmarks.groups) {
                    if (_group.name) {
                        var group = _findGroup(bookmarks, _group.name);
                        if (!group) {
                            group = {
                                name: _group.name,
                                items: []
                            };
                            bookmarks.groups.push(group);
                        }
                        this.bindGroupData(group);
                        if (Array.isArray(_group.items)) {
                            for (var _item of _group.items) {
                                if (_item.url) {
                                    var tuple = itemMap.get(_item.url);
                                    if (!tuple) {
                                        tuple = {
                                            group: group,
                                            item: {}
                                        };
                                        group.items.push(tuple.item);
                                        itemMap.set(_item.url, tuple);
                                    }
                                    tuple.item.url = _item.url;
                                    tuple.item.fileType = _item.fileType;
                                    tuple.item.workNum = _item.workNum;
                                    tuple.item.path = _item.path;
                                    tuple.item.name = _item.name;
                                    tuple.item.link = _item.link;
                                    tuple.item.thumb = _item.thumb;
                                }
                            }
                        }
                    }
                }
            }
        },
        onFileChange() {
            var files = this.$refs.file.files;
            if (files.length === 0) {
                return;
            }

            var _clearFile = () => {
                this.$refs.file.value = '';
            }
            var _update = (_bookmarks) => {
                if (Array.isArray(_bookmarks.groups)) {
                    var bookmarks = this.cloneBookmarks(this.bookmarks);
                    this.updateBookmarks(bookmarks, _bookmarks);
                    this.bookmarks = bookmarks;
                }
            };
            var _updateForce = (_bookmarks) => {
                if (Array.isArray(_bookmarks.groups)) {
                    var bookmarks = this.cloneBookmarks({});
                    this.updateBookmarks(bookmarks, _bookmarks);
                    this.bookmarks = bookmarks;
                }
            };

            var file = files[0];
            var reader = new FileReader();
            reader.onload = (event) => {
                try {
                    var _bookmarks = JSON.parse(event.target.result);
                    _update(_bookmarks);
                    //_updateForce(_bookmarks);
                    this.saveBookmarks();
                    _clearFile();
                } catch (err) {
                    var _alert = alertModal({
                        content: `<error>JSON 格式錯誤</error>`
                    });
                    _alert.open();
                    _clearFile();
                }
            };
            reader.onerror = () => {
                var _alert = alertModal({
                    content: `<error>讀取檔案失敗</error>`
                });
                _alert.open();
                _clearFile();
            };
            reader.readAsText(file, 'utf-8');
        },
        _import() {
            this.$refs.file.click();
        },
        _export() {
            var _bookmarks = this.cloneBookmarks(this.bookmarks);

            var json = JSON.stringify(_bookmarks, null, 2);
            var blob = new Blob([json], {
                type: "application/json"
            });

            //var date = new Date().toISOString().slice(0, 10);
            //var time = new Date().toISOString().slice(0, 19).replace('T', '_');
            var d = new Date();
            var time =
                d.getFullYear() + '-' +
                String(d.getMonth() + 1).padStart(2, '0') + '-' +
                String(d.getDate()).padStart(2, '0') + '_' +
                String(d.getHours()).padStart(2, '0') + '-' +
                String(d.getMinutes()).padStart(2, '0') + '-' +
                String(d.getSeconds()).padStart(2, '0');

            var url = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = url;
            a.download = `bookmarks_${time}.json`;
            document.body.appendChild(a);
            a.click();

            setTimeout(() => {
                document.body.removeChild(a);
                URL.revokeObjectURL(url);
            }, 1);
        },
        download() {

        },
        upload() {

        }
    }
});