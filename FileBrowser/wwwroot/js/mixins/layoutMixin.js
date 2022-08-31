var layoutMixin = {
    data() {
        return {
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
        routeLink() {
            var url = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var param = arguments[i].toString();
                var splits = param.split('/');
                for (var index in splits) {
                    url += '/' + encodeURIComponent(splits[index]);
                }
            }
            return url;
        },
        initScrollPos() {
            var scrollPos = storage.scrollPos();
            if (scrollPos.length > 0) {
                var last = scrollPos.pop();
                var path = window.path || '';
                if (last.path !== window.path) {
                    // child
                    if (path !== '' && path.indexOf(last.path) > -1)
                        return;
                    storage.setScrollPos([]);
                    return;
                }
                storage.setScrollPos(scrollPos);
                document.documentElement.scrollTop = last.pos;
            }
        },
        onScrollPos() {
            var scrollPos = storage.scrollPos();
            var pos = window.pageYOffset ||
                document.documentElement.scrollTop ||
                document.body.scrollTop || 0;;
            var path = window.path || '';
            scrollPos.push({
                pos: pos, path: path
            });
            storage.setScrollPos(scrollPos);
        }
    }
};