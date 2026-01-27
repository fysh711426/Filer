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
        groupMenuOptions: []
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
                { text: this.local.edit, value: 0, icon: 'fa-solid fa-pen fa-fw' },
                { text: this.local.delete, value: 1, icon: 'fa-solid fa-trash fa-fw', className: 'danger', separator: true }
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
        createGroup() {
            var _value = '';
            var open = () => {
                var _prompt = promptModal({
                    //title: this.local.createGroup,
                    content: this.local.createGroup,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel,
                    size: 'modal-sm',
                    btnSize: 'btn-sm',
                    contentSize: 'tight text-base',
                    input: {
                        textarea: false,
                        value: _value,
                        placeholder: this.local.name,
                        required: true
                    }
                });
                _prompt.onClosed = (ele, action, value) => {
                    if (action === 'confirm') {
                        var _group = this.bookmarks.groups.find(it => it.name === value);
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
                    //title: this.local.editGroup,
                    content: this.local.editGroup,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel,
                    size: 'modal-sm',
                    btnSize: 'btn-sm',
                    contentSize: 'tight text-base',
                    input: {
                        textarea: false,
                        value: _value,
                        placeholder: this.local.name,
                        required: true
                    }
                });
                _prompt.onClosed = (ele, action, value) => {
                    if (action === 'confirm') {
                        var _group = this.bookmarks.groups.find(it => it.name === value && it !== group);
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
                btnSize: 'btn-sm',
                contentSize: 'text-base',
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
        onLinkClick() {
        },
        deleteBookmark(group, item) {
            var _confirm = confirmModal({
                //title: this.local.delete,
                content: this.local.deleteBookmarkConfirmMessage,
                confirmText: this.local.delete,
                cancelText: this.local.cancel,
                confirmClass: 'danger',
                size: 'modal-sm',
                btnSize: 'btn-sm',
                contentSize: 'text-base',
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
                        var group = bookmarks.groups.find(it => it.name === _group.name);
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
                                    //if (!tuple) {
                                    //    tuple = {
                                    //        group: group,
                                    //        item: {}
                                    //    };
                                    //    group.items.push(tuple.item);
                                    //    itemMap.set(_item.url, tuple);
                                    //}
                                    if (!tuple || tuple.group !== group) {
                                        if (tuple) {
                                            var index = tuple.group.items.indexOf(tuple.item);
                                            if (index !== -1) {
                                                tuple.group.items.splice(index, 1);
                                            }
                                        }
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
        _import() {
            //this.$refs.file.click();

            var _this = this;
            var _modal = modal(document.querySelector('.modal-template.import'), {
                singleton: false
            });
            _modal.onCreate = function (html) {
                var ViewModel = Vue.extend({
                    template: html,
                    data() {
                        return {
                            fileName: '',
                            hasFile: false,
                            isClearExisting: false,
                            title: _this.local.importModalTitle,
                            content: _this.local.importModalContent,
                            alertText: _this.local.importModalAlertText,
                            checkboxText: _this.local.importModalCheckboxText,
                            clickToChooseFile: _this.local.clickToChooseFile,
                            selected: _this.local.selected,
                            confirmText: _this.local.confirm,
                            cancelText: _this.local.cancel,
                        }
                    },
                    methods: {
                        fileChange() {
                            this.hasFile = this.$refs.file.files.length !== 0;
                            this.fileName = '';
                            if (this.hasFile) {
                                this.fileName = this.$refs.file.files[0].name;
                            }
                        },
                        confirm() {
                            var _update = (_bookmarks) => {
                                if (Array.isArray(_bookmarks.groups)) {
                                    var bookmarks = _this.cloneBookmarks(_this.bookmarks);
                                    _this.updateBookmarks(bookmarks, _bookmarks);
                                    _this.bookmarks = bookmarks;
                                }
                            };
                            var _updateForce = (_bookmarks) => {
                                if (Array.isArray(_bookmarks.groups)) {
                                    var bookmarks = _this.cloneBookmarks({});
                                    _this.updateBookmarks(bookmarks, _bookmarks);
                                    _this.bookmarks = bookmarks;
                                }
                            };

                            //var files = this.$refs.file.files;
                            //if (files.length === 0) {
                            //    return;
                            //}
                            var file = this.$refs.file.files[0];
                            var reader = new FileReader();
                            reader.onload = (event) => {
                                try {
                                    var _bookmarks = JSON.parse(event.target.result);
                                    if (!this.isClearExisting) {
                                        _update(_bookmarks);
                                    } else {
                                        _updateForce(_bookmarks);
                                    }
                                    _this.saveBookmarks();
                                    _modal.close();
                                    //this.$refs.file.value = '';
                                } catch (err) {
                                    var _alert = alertModal({
                                        content: `<error>JSON parse error:</br>${err.message}</error>`,
                                        confirmText: _this.local.confirm
                                    });
                                    _alert.open();
                                }
                            };
                            reader.onerror = () => {
                                var _alert = alertModal({
                                    content: `<error>Failed to read file.</error>`,
                                    confirmText: _this.local.confirm
                                });
                                _alert.open();
                            };
                            reader.readAsText(file, 'utf-8');
                        }
                    }
                });
                var vm = new ViewModel();
                _modal.onRemove = () => {
                    vm.$destroy();
                    vm.$el.remove();
                    vm = null;
                };
                vm.$mount();
                return vm.$el;
            };
            _modal.open();
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
        handleResponse(response) {
            if (!response.ok)
                throw new Error(`HTTP error! status: ${response.status}`);
        },
        handleError(error) {
            var _alert = alertModal({
                content: `<error>${error.message}</error>`,
                confirmText: this.local.confirm
            });
            _alert.open();
        },
        //getBookmarkList() {
        //    progress.start();
        //    fetch('api/bookmark/list', {
        //        method: 'GET'
        //    }).then((response) => {
        //        this.handleResponse(response);
        //        return response.json();
        //    }).then((data) => {
        //        console.log(data);
        //    }).catch((error) => {
        //        this.handleError(error);
        //    }).finally(() => {
        //        progress.done();
        //    });
        //},
        sync() {
            var _updateForce = (_bookmarks) => {
                if (Array.isArray(_bookmarks.groups)) {
                    var bookmarks = this.cloneBookmarks({});
                    this.updateBookmarks(bookmarks, _bookmarks);
                    this.bookmarks = bookmarks;
                }
            };
            var _modal = modal(document.querySelector('.modal-template.confirm'), {
                singleton: false,
                data: {
                    title: this.local.syncModalTitle,
                    content: this.local.syncModalContent,
                    alertText: this.local.syncModalAlertText,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel
                }
            });
            _modal.onClosed = (ele, action) => {
                if (action === 'confirm') {
                    var _loading = loadingModal({
                        content: this.local.syncing,
                        showSpinner: true
                    });
                    _loading.onReady = () => {
                        setTimeout(() => {
                            //progress.start();
                            fetch(`api/bookmark/sync`, {
                                method: 'GET'
                            }).then((response) => {
                                this.handleResponse(response);
                                return response.json();
                            }).then((data) => {
                                if (!data) {
                                    var _alert = alertModal({
                                        content: this.local.backupEmpty,
                                        confirmText: this.local.confirm
                                    });
                                    _alert.open();
                                    return;
                                }
                                _updateForce(data);
                                this.saveBookmarks();
                            }).catch((error) => {
                                this.handleError(error);
                            }).finally(() => {
                                //progress.done();
                                _loading.close();
                            });
                        }, 500);
                    };
                    _loading.open();
                }
            }
            _modal.open();
        },
        backup() {
            var _modal = modal(document.querySelector('.modal-template.confirm'), {
                singleton: false,
                data: {
                    title: this.local.backupModalTitle,
                    content: this.local.backupModalContent,
                    alertText: this.local.backupModalAlertText,
                    confirmText: this.local.confirm,
                    cancelText: this.local.cancel
                }
            });
            _modal.onClosed = (ele, action) => {
                if (action === 'confirm') {
                    var _loading = loadingModal({
                        content: this.local.uploading,
                        showSpinner: true
                    });
                    _loading.onReady = () => {
                        setTimeout(() => {
                            var bookmarks = this.cloneBookmarks(this.bookmarks);
                            //progress.start();
                            fetch('api/bookmark/backup', {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json'
                                },
                                body: JSON.stringify(bookmarks)
                            }).then((response) => {
                                this.handleResponse(response);
                                toast.show(this.local.saveSuccess);
                            }).catch((error) => {
                                this.handleError(error);
                            }).finally(() => {
                                //progress.done();
                                _loading.close();
                            });
                        }, 500);
                    };
                    _loading.open();
                }
            }
            _modal.open();
        }
    }
});