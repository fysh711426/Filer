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
        dragged: null
    },
    created() {
        var bookmarks = storage.bookmarks();
        this.bindBookmarkData(bookmarks);
        this.bookmarks = bookmarks;
        this.initData(initialData);
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
        bindBookmarkData(bookmarks) {
            for (var i = 0; i < bookmarks.groups.length; i++) {
                var group = bookmarks.groups[i];
                group.expand = true;
            }
        },
        onBookmarkClick(item) {
            this.selected = item;
        },
        onBookmarkGroupClick(group) {
            group.expand = !group.expand;
        },
        onDragStart(group, index) {
            this.dragged = {
                group: group,
                index: index
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
            var bookmarks = storage.bookmarks();
            this.bindBookmarkData(bookmarks);
            this.bookmarks = bookmarks;
            toast.show(this.local.bookmarkSaveMessage);
        },
        createGroup() {

        },
        editGroup(group) {

        },
        deleteGroup(group) {

        },
        deleteBookmark() {

        },
        Import() {

        },
        Export() {

        },
        Upload() {

        }
    }
});