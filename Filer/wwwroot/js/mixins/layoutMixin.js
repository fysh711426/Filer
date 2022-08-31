﻿var layoutMixin = {
    data() {
        return {
            path: '',
            prevPath: '',
            selectedPath: '',
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
        getPagePath() {
            var path = '/' + this.workNum;
            if (this.dirPath) {
                return path + '/' + this.dirPath;
            }
            if (this.filePath) {
                return path + '/' + this.filePath;
            }
            return path;
        },
        getItemPath(item) {
            return '/' + this.workNum + '/' + item.path;
        },
        initPath(path) {
            this.path = path;
            this.prevPath = storage.prevPath();
            storage.setPrevPath(path);
            this.selectedPath = this.prevPath;
        },
        initScrollPos() {
            var scrollPos = storage.scrollPos();
            if (scrollPos.length > 0) {
                var last = scrollPos.pop();
                var path = this.path || '';
                if (last.path !== path) {
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
                document.body.scrollTop || 0;
            var path = this.path || '';
            scrollPos.push({
                pos: pos, path: path
            });
            storage.setScrollPos(scrollPos);
        },
        onItemSelected(item) {
            this.selectedPath = this.getItemPath(item);
        },
        onItemClick(item) {
            var _this = this;
            _this.onScrollPos();
            setTimeout(function () {
                _this.onItemSelected(item);
            }, 1);
        }
    }
};