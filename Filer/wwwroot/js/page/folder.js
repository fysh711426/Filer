var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        type: {
            folder: 0,
            video: 1,
            audio: 2,
            image: 3,
            text: 4,
            other: 5
        },
        viewModes: [
            'view',
            'thumbnail',
            'download'
        ],
        viewMode: '',
        viewModeIndex: 0,
        orderBys: [
            'name',
            'date',
            'size'
        ],
        orderBy: '',
        orderByIndex: 0,
        isOrderByDesc: false,
        imageScales: [
            '100',
            '50',
            '25'
        ],
        imageScale: '',
        imageScaleIndex: 0,
        host: '',
        scheme: '',
        isAndroid: '',
        workNum: 0,
        dirPath: '',
        dirName: '',
        parentDirPath: '',
        parentDirName: '',
        datas: [],
        isUseDeepLink: false,
        deepLinkPackage: '',
        previewSelected: null
    },
    created() {
        this.isUseDeepLink = storage.isUseDeepLink();
        this.deepLinkPackage = storage.deepLinkPackage();
        this.viewMode = storage.viewMode() || 'view';
        for (var i = 0; i < this.viewModes.length; i++) {
            if (this.viewModes[i] === this.viewMode) {
                this.viewModeIndex = i;
            }
        }
        this.orderBy = storage.orderBy() || 'name';
        for (var i = 0; i < this.orderBys.length; i++) {
            if (this.orderBys[i] === this.orderBy) {
                this.orderByIndex = i;
            }
        }
        this.isOrderByDesc = storage.isOrderByDesc();
        this.imageScale = storage.imageScale() || '100';
        for (var i = 0; i < this.imageScales.length; i++) {
            if (this.imageScales[i] === this.imageScale) {
                this.imageScaleIndex = i;
            }
        }
        this.bindData(initialData);
        this.bindLink(initialData);
        this.initData(initialData);
        this.initPath(this.getPagePath());
        window.addEventListener('pageshow', this.restorePage);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        gotop('.gotop');
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            fileNavbar({
                enableHover: true,
                enableImageOver: true
            });
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        bindData(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                item.isPreviewOver = false;
                item.showPreview = false;
                item.loaded = false;
            }
        },
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                if (item.fileType === this.type.folder) {
                    //item.link = this.routeLink('folder', data.workNum, item.path) +
                    //    '?orderBy=' + this.orderBy;
                    item.link = this.routeLink('folder', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.text) {
                    item.link = this.routeLink('text', data.workNum, item.path);
                    item.history = this.routeLink('api/history', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.image) {
                    item.link = this.routeLink('image', data.workNum, item.path);
                    item.history = this.routeLink('api/history', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.audio) {
                    if (data.isAndroid && this.isUseDeepLink) {
                        var _package = '';
                        if (this.deepLinkPackage !== '') {
                            _package = 'package=' + this.deepLinkPackage + ';';
                        }

                        var routeLink = this.routeLink(
                            'api/file/audio', data.workNum, item.path);

                        var link = 'intent://' + data.host + '/' + routeLink +
                            '#Intent;' + _package +
                            'action=android.intent.action.VIEW;' +
                            'category=android.intent.category.DEFAULT;' +
                            'category=android.intent.category.BROWSABLE;' +
                            'scheme=' + data.scheme + ';type=video/mp3;end';

                        item.link = link;
                    }
                    else {
                        item.link = this.routeLink('audio', data.workNum, item.path);
                    }
                    item.history = this.routeLink('api/history', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.video) {
                    if (data.isAndroid && this.isUseDeepLink) {
                        var _package = '';
                        if (this.deepLinkPackage !== '') {
                            _package = 'package=' + this.deepLinkPackage + ';';
                        }

                        var routeLink = this.routeLink(
                            'api/file/video', data.workNum, item.path);

                        var link = 'intent://' + data.host + '/' + routeLink +
                            '#Intent;' + _package + 
                            'action=android.intent.action.VIEW;' +
                            'category=android.intent.category.DEFAULT;' +
                            'category=android.intent.category.BROWSABLE;' +
                            'scheme=' + data.scheme + ';type=video/mp4;end';

                        item.link = link;
                    }
                    else {
                        item.link = this.routeLink('video', data.workNum, item.path);
                    }
                    item.thumbnail = this.routeLink('api/thumbnail/video', data.workNum, item.path);
                    item.preview = this.routeLink('api/thumbnail/video/preview', data.workNum, item.path);
                    item.history = this.routeLink('api/history', data.workNum, item.path);
                    continue;
                }
            }
        },
        onVideoItemClick(item) {
            if (this.isAndroid && this.isUseDeepLink) {
                this.tempSelectedPath = this.getItemPath(item);
                this.prevPath = this.tempSelectedPath;
                this.selectedPath = this.prevPath;
                if (item.history && !item.hasHistory)
                    this.history(item.history).then(function (response) {
                        if (response.ok)
                            item.hasHistory = true;
                    });
                location.href = item.link;
                return;
            }
            this.onItemClick(item);
        },
        onViewModeChange() {
            this.viewModeIndex = (this.viewModeIndex + 1) % this.viewModes.length;
            this.viewMode = this.viewModes[this.viewModeIndex];
            storage.setViewMode(this.viewMode);
            var _this = this;
            setTimeout(function () {
                _this.rebindImageOver();
            }, 1);
        },
        onOrderByChange() {
            this.orderByIndex = (this.orderByIndex + 1) % this.orderBys.length;
            this.orderBy = this.orderBys[this.orderByIndex];
            storage.setOrderBy(this.orderBy);
            var orderByWithDesc = this.orderBy;
            if (this.isOrderByDesc)
                orderByWithDesc += 'Desc';
            cookie.setOrderBy(orderByWithDesc);
            //location.href = location.origin + location.pathname + '?orderBy=' + this.orderBy;
            //location.reload();
            this.reload();
        },
        onIsOrderByDescChange() {
            this.isOrderByDesc = !this.isOrderByDesc;
            storage.setIsOrderByDesc(this.isOrderByDesc);
            var orderByWithDesc = this.orderBy;
            if (this.isOrderByDesc)
                orderByWithDesc += 'Desc';
            cookie.setOrderBy(orderByWithDesc);
            //location.reload();
            this.reload();
        },
        onImageScaleChange() {
            this.imageScaleIndex = (this.imageScaleIndex + 1) % this.imageScales.length;
            this.imageScale = this.imageScales[this.imageScaleIndex];
            storage.setImageScale(this.imageScale);
        },
        loadImage: function (url) {
            return new Promise(function (resolve) {
                var img = new Image();
                img.onload = function () {
                    if (this.complete === true) {
                        resolve(true);
                        img = null;
                    }
                }
                img.onerror = function () {
                    resolve(false);
                    img = null;
                }
                img.src = url;
            });
        },
        onPreviewClick(item) {
            var _this = this;
            if (this.previewSelected) {
                if (this.previewSelected !== item) {
                    this.previewSelected.showPreview = false;
                    this.previewSelected.isPreviewOver = false;
                    this.previewSelected = null;
                }
            }
            this.previewSelected = item;
            item.isPreviewOver = !item.isPreviewOver;
            if (item.isPreviewOver) {
                var task = this.loadImage(item.preview);
                setTimeout(async function () {
                    var loaded = await task;
                    if (loaded) {
                        if (_this.previewSelected === item) {
                            item.showPreview = true;
                        }
                    }
                }, 800);
            }
            else {
                item.showPreview = false;
            }
            if (!item.isPreviewOver) {
                this.previewSelected = null;
            }
        },
        onImageLoaded(item) {
            item.loaded = true;
        },
        reloadPage(url) {
            return fetch(url, {
                method: 'GET',
                credentials: 'include'
            });
        },
        reload() {
            var _this = this;
            progress.start();
            this.reloadPage(location.href).then(function (response) {
                if (response.ok) {
                    response.text().then(function (html) {
                        var regex = /<script>\s+var initialData = ([\s\S]*?);\s+<\/script>/;
                        var match = html.match(regex);
                        if (match) {
                            var initialData = JSON.parse(match[1]);
                            _this.bindData(initialData);

                            //----- fix image loaded -----
                            var loadedDict = {};
                            for (var i = 0; i < _this.datas.length; i++)
                                loadedDict[_this.datas[i].path] =
                                    _this.datas[i].loaded;
                            for (var i = 0; i < initialData.datas.length; i++)
                                initialData.datas[i].loaded =
                                    loadedDict[initialData.datas[i].path] || false;
                            //----- fix image loaded -----

                            _this.bindLink(initialData);
                            _this.initData(initialData);
                        }
                    });
                }
                progress.done();
            });
        },
        rebindImageOver() {
            // todo: Modify fileNavbar to support event rebinding.
            // Temp solution
            var enableImageOver = true;
            if (enableImageOver) {
                var navbar = document.querySelector('.file-navbar');
                var images = document.querySelectorAll('.file-image-block');
                function onToggle() {
                    if (navbar.className.indexOf('over') === -1)
                        navbar.className = navbar.className + ' over';
                    else
                        navbar.className = navbar.className.replace(' over', '');
                }
                for (var i = 0; i < images.length; i++) {
                    var item = images[i];
                    item.className = item.className + ' over';
                    item.addEventListener('click', onToggle);
                }
            }
        }
    },
    computed: {
        isAndroidUseDeepLink: function () {
            return this.isAndroid && this.isUseDeepLink;
        }
    }
});

function onPreviewClick(e) {
    e.stopPropagation();
}