var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
    },
    mounted() {
        document.querySelector('.layout').style.opacity = 1;
    },
    methods: {
    }
});