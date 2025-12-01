var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
    }, created() {
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
    }
});