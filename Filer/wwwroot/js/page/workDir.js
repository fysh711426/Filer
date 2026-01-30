Vue.use(ImageLazyLoad(), {
    delay: 500,
    observerOptions: {
        root: null,
        rootMargin: '320px 0px 320px 0px'
    }
});
Vue.use(StayWatch, {
    delay: 2000,
    observerOptions: {
        root: null,
        rootMargin: '-25% 0px -25% 0px'
    }
});
var vm = new Vue({
    el: '#app',
    mixins: [
        layoutMixin,
        routeMixin,
        folderMixin,
        searchMixin,
        navbarMixin
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
        this.initPath('');
        //this.initSearch(this.searchText);
        window.addEventListener('pageshow', this.restorePage);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        gotop('.file-navbar-gotop-wrap');
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            fileNavbar.enableHover();
            fileNavbar.enableExpand();
            fileNavbar.enableImageOver();
            _this.initScrollPos();
            _this.initSearch('workDir', _this.searchText);
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
    }
});