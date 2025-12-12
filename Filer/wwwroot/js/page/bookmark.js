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
        selected: null
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
                group.showItems = false;
            }
        },
        onBookmarkClick(item) {
            this.selected = item;
        },
        onBookmarkGroupClick(group) {
            group.showItems = !group.showItems;
        },
        addGroup() {

        },
        Import() {

        },
        Export() {

        },
        Upload() {

        }
    }
});