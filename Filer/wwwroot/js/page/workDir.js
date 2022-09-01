var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        datas: []
    },
    created() {
        this.initData(initialData);
        this.bindLink(initialData);
        this.initPath('');
        window.addEventListener('pageshow', this.restorePage);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                item.link = this.routeLink('folder', i + 1);
            }
        }
    }
});