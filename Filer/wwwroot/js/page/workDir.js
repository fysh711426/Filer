var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        datas: []
    },
    created() {
        this.initData(initialData);
        this.initPath('');
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
    }
});