var layoutMixin = {
    data() {
        return {
            type: {
                workDir: 0,
                folder: 1,
                video: 2,
                audio: 3,
                image: 4,
                text: 5,
                other: 6
            },
            theme: '',
            isLoaded: false
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