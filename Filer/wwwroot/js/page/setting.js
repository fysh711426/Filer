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
        onIsUseDeepLink() {
            this.isUseDeepLink = !this.isUseDeepLink;
            storage.setIsUseDeepLink(this.isUseDeepLink);
            toast.show(this.local.saveSuccess);
        },
        onDeepLinkPackage() {
            storage.setDeepLinkPackage(this.deepLinkPackage);
            toast.show(this.local.saveSuccess);
        }
    }
});