var routeMixin = {
    data() {
        return {
            path: '',
            prevPath: '',
            selectedPath: '',
            tempSelectedPath: '',
            // path
            workNum: 0,
            workDir: '',
            dirPath: '',
            dirName: '',
            parentDirPath: '',
            parentDirName: ''
        };
    },
    methods: {
        initPath(path) {
            this.path = path;
            this.prevPath = storage.prevPath();
            this.selectedPath = this.prevPath;
            this.tempSelectedPath = this.selectedPath;
            storage.setPrevPath(this.path);
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
        restorePage() {
            if (this.tempSelectedPath) {
                this.prevPath = this.tempSelectedPath;
                this.selectedPath = this.prevPath;
                storage.setPrevPath(this.path);
            }
        },
        routeLink(...args) {
            var url = '';
            for (var param of args) {
                var splits = param.toString().split('/');
                for (var s of splits) {
                    url += '/' + encodeURIComponent(s);
                }
            }
            if (url.endsWith('/'))
                url = url.slice(0, -1);
            return url;
        },
        routeLinkWithSearch(...args) {
            var url = '';
            if (args.length > 0) {
                var query = '?search=' + encodeURIComponent(args[args.length - 1]);
                var routes = args.slice(0, -1);
                if (routes.length > 0)
                    url += this.routeLink(...routes);
                url += query;
            }
            return url;
        },
        routeLinkWithOrderBy() {
            return this.routeLink.apply(this, arguments) +
                '?orderBy=' + (storage.orderBy() || 'name');
        },
        getWorkDirPath(item) {
            return '/' + item.path;
        },
        getItemPath(item) {
            return '/' + this.workNum + '/' + item.path;
        },
        getPagePath() {
            var path = '/' + this.workNum;
            if (this.filePath) {
                return path + '/' + this.filePath;
            }
            if (this.dirPath) {
                return path + '/' + this.dirPath;
            }
            return path;
        }
    }
};