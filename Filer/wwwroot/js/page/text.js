var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        themes: [
            'text-dark', 'text-dark-blue',
            'text-light', 'text-gray', 'text-yellow',
            'text-orange', 'text-blue', 'text-green'
        ],
        textTheme: '',
        textThemeIndex: 0,
        workNum: 0,
        filePath: '',
        fileName: '',
        parentDirPath: '',
        parentDirName: ''
    },
    created() {
        this.initData(initialData);
        window.path = '/' + this.workNum + '/' + this.filePath;
    },
    mounted() {
        this.textTheme = document.body.getAttribute('text-theme');
        for (var i = 0; i < this.themes.length; i++) {
            if (this.themes[i] === this.textTheme) {
                this.textThemeIndex = i;
            }
        }
        onTextThemeButtonChange(this.textTheme);
        gotop('.gotop');
        fileNavbar({
            enableHover: true,
            enableTextOver: true
        });
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        toggleTextTheme() {
            this.textThemeIndex = (this.textThemeIndex + 1) % this.themes.length;
            this.textTheme = this.themes[this.textThemeIndex];
            onTextThemeButtonChange(this.textTheme);
            onTextThemeChange(this.textTheme);
        }
    }
});