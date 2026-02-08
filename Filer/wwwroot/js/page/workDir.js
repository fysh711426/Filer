Vue.use(ImageLazyLoad(), {
    delay: 300,
    observerOptions: {
        root: null,
        rootMargin: '320px 0px 320px 0px'
    }
});
Vue.use(StayWatch, {
    delay: 1000,
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
        this.bindDatas(initialData);
        this.bindLinks(initialData);
        this.bindSearchPaths(initialData);
        this.bindMarks(initialData, initialEncodeData);
        this.initData(initialData);
        this.initData(initialEncodeData);
        this.initPath('');
        window.addEventListener('pageshow', this.restorePage);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        gotop('.file-navbar-gotop-wrap');
        spinner('.spinner');
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            fileNavbar.enableHover();
            fileNavbar.enableExpand();
            fileNavbar.enableImageOver();
            _this.initScrollPos();
            _this.initSearch('workDir', _this.searchText);
            if (_this.isUseDataStreaming)
                _this.loadDataResults('workDir');
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
    }
});