var vm = new Vue({
    el: '#app',
    mixins: [
        layoutMixin,
        routeMixin,
        folderMixin,
        searchMixin,
        navbarMixin,
        bookmarkMixin
    ],
    data: {
    },
    created() {
        this.isUseDeepLink = storage.isUseDeepLink();
        this.deepLinkPackage = storage.deepLinkPackage();
        this.initNavbarExpand();
        this.bindData(initialData);
        this.bindLink(initialData);
        this.bindSearchPath(initialData);
        this.bindMark(initialData, initialEncodeData);
        this.initData(initialData);
        this.initData(initialEncodeData);
        this.initPath(this.getPagePath());
        this.initSearch(this.searchText);
        this.initBookmark();
        window.addEventListener('pageshow', this.restorePage);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        gotop('.gotop');
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            fileNavbar({
                enableHover: true,
                enableImageOver: true
            });
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
    }
});