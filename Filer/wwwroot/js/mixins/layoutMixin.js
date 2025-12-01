var layoutMixin = {
    data() {
        return {
            theme: '',
            isLoaded: false,
        };
    },
    methods: {
        initData(data) {
            for (var i in data) {
                this[i] = data[i];
            }
        },
        toggleTheme() {
            this.theme = this.theme === 'light' ? 'dark' : 'light';
            onThemeButtonChange(this.theme);
            onThemeChange(this.theme);
        },
        onHome() {
            storage.setPrevPath('');
            //location.href = '';
        }
    }
};