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
        dragged: null,
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
        var _this = this;
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
        onDragStart(group, index) {
            this.dragged = {
                group, index
            };
        },
        onDrop(group, index) {
            var dragGroup = this.dragged.group;
            var dragIndex = this.dragged.index;

            if (group === dragGroup && index === dragIndex) {
                this.dragged = null;
                return;
            }

            var dragItem = dragGroup.items[dragIndex];
            dragGroup.items.splice(dragIndex, 1);

            //if (group === dragGroup && index > dragIndex)
            //    index = index - 1;

            group.items.splice(index, 0, dragItem);
            this.dragged = null;
            this.saveBookmarks();
        },
        saveBookmarks() {
            var _bookmarks = {
                groups: []
            };
            for (var group of this.bookmarks.groups) {
                var _group = {
                    name: group.name,
                    items: []
                };
                _bookmarks.groups.push(_group);
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
                                    content: '群組名已存在'
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
                                    content: '群組名已存在'
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
                title: 'Delete',
                content: 'Are you sure you want to delete this group?',
                confirmText: 'Delete',
                confirmClass: 'danger'
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
                title: 'Delete',
                content: 'Are you sure you want to delete this bookmark?',
                confirmText: 'Delete',
                confirmClass: 'danger'
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
        Import() {

        },
        Export() {

        },
        Upload() {

        }
    }
});