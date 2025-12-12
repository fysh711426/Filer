var routeMixin = {
    data() {
        return {
            pagePath: '',
            prevPath: '',
            selectedPath: '',
            tempSelectedPath: '',
            workNum: 0,
            workDir: '',
            path: '',
            dirPath: '',
            dirName: '',
            parentDirPath: '',
            parentDirName: ''
        };
    },
    methods: {
        initPath(pagePath) {
            this.pagePath = pagePath;
            this.prevPath = storage.prevPath();
            this.selectedPath = this.prevPath;
            this.tempSelectedPath = this.selectedPath;
            storage.setPrevPath(this.pagePath);
        },
        initScrollPos() {
            var scrollPos = storage.scrollPos();
            if (scrollPos.length > 0) {
                var last = scrollPos.pop();
                var pagePath = this.pagePath || '';
                if (last.pagePath !== pagePath) {
                    // child
                    if (pagePath !== '' && pagePath.indexOf(last.pagePath) > -1)
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
            var pagePath = this.pagePath || '';
            scrollPos.push({
                pos: pos, pagePath: pagePath
            });
            storage.setScrollPos(scrollPos);
        },
        restorePage() {
            if (this.tempSelectedPath) {
                this.prevPath = this.tempSelectedPath;
                this.selectedPath = this.prevPath;
                storage.setPrevPath(this.pagePath);
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
            if (url.startsWith('/'))
                url = url.slice(1);
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
            //return '/' + item.workNum;
            return '' + item.workNum;
        },
        getItemPath(item) {
            //return '/' + item.workNum + '/' + item.path;
            return item.workNum + '/' + item.path;
        },
        getPagePath() {
            //var path = '/' + this.workNum;
            var path = '' + this.workNum;
            if (this.filePath)
                return path + '/' + this.filePath;
            if (this.dirPath)
                return path + '/' + this.dirPath;
            return path;
        },
        getPageShowPath() {
            if (!this.path)
                return this.workDir;
            return this.workDir + '/' + this.path;
        }
    }
};