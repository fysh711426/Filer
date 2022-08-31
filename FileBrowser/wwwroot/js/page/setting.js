var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        isUseDeepLink: false,
        deepLinkPackage: ''
    },
    created() {
        this.isUseDeepLink = storage.isUseDeepLink();
        this.deepLinkPackage = storage.deepLinkPackage();
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
        onIsUseDeepLink() {
            this.isUseDeepLink = !this.isUseDeepLink;
            storage.setIsUseDeepLink(this.isUseDeepLink);
            toast.show('儲存成功');
        },
        onDeepLinkPackage() {
            storage.setDeepLinkPackage(this.deepLinkPackage);
            toast.show('儲存成功');
        }
    }
});