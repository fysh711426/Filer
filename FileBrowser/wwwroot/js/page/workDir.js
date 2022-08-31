var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        datas: []
    },
    created() {
        this.initData(initialData);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        gotop('.gotop');
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    }
});