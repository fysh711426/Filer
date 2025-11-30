var vm = new Vue({
    el: '#app',
    mixins: [
        layoutMixin,
        searchMixin,
        navbarMixin
    ],
    data: {
        datas: []
    },
    created() {
        this.initNavbarExpand();
        this.initData(initialData);
        this.bindLink(initialData);
        this.initPath('');
        this.initSearch(this.searchText);
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
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                //item.link = this.routeLink('folder', i + 1) +
                //    '?orderBy=' + this.orderBy;
                item.link = this.routeLink('folder', i + 1);
            }
        }
    }
});