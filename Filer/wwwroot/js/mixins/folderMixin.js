var folderMixin = {
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
            host: '',
            scheme: '',
            isAndroid: '',
            datas: [],
            isUseDeepLink: false,
            deepLinkPackage: '',
            previewSelected: null
        };
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
                if (item.fileType === this.type.workDir) {
                    //item.link = this.routeLink('folder', item.workNum) +
                    //    '?orderBy=' + this.orderBy;
                    item.link = this.routeLink('folder', item.workNum);
                    continue;
                }
                if (item.fileType === this.type.folder) {
                    //item.link = this.routeLink('folder', data.workNum, item.path) +
                    //    '?orderBy=' + this.orderBy;
                    item.link = this.routeLink('folder', item.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.text) {
                    item.link = this.routeLink('text', item.workNum, item.path);
                    item.history = this.routeLink('api/history', item.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.image) {
                    item.link = this.routeLink('image', item.workNum, item.path);
                    item.history = this.routeLink('api/history', item.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.audio) {
                    if (data.isAndroid && this.isUseDeepLink) {
                        var _package = '';
                        if (this.deepLinkPackage !== '') {
                            _package = 'package=' + this.deepLinkPackage + ';';
                        }

                        var routeLink = this.routeLink(
                            'api/file/audio', item.workNum, item.path);

                        var link = 'intent://' + data.host + '/' + routeLink +
                            '#Intent;' + _package +
                            'action=android.intent.action.VIEW;' +
                            'category=android.intent.category.DEFAULT;' +
                            'category=android.intent.category.BROWSABLE;' +
                            'scheme=' + data.scheme + ';type=video/mp3;end';

                        item.link = link;
                    }
                    else {
                        item.link = this.routeLink('audio', item.workNum, item.path);
                    }
                    item.history = this.routeLink('api/history', item.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.video) {
                    if (data.isAndroid && this.isUseDeepLink) {
                        var _package = '';
                        if (this.deepLinkPackage !== '') {
                            _package = 'package=' + this.deepLinkPackage + ';';
                        }

                        var routeLink = this.routeLink(
                            'api/file/video', item.workNum, item.path);

                        var link = 'intent://' + data.host + '/' + routeLink +
                            '#Intent;' + _package +
                            'action=android.intent.action.VIEW;' +
                            'category=android.intent.category.DEFAULT;' +
                            'category=android.intent.category.BROWSABLE;' +
                            'scheme=' + data.scheme + ';type=video/mp4;end';

                        item.link = link;
                    }
                    else {
                        item.link = this.routeLink('video', item.workNum, item.path);
                    }
                    item.thumbnail = this.routeLink('api/thumbnail/video', item.workNum, item.path);
                    item.preview = this.routeLink('api/thumbnail/video/preview', item.workNum, item.path);
                    item.history = this.routeLink('api/history', item.workNum, item.path);
                    continue;
                }
            }
        },
        history: function (url) {
            return fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        },
        onItemClick(item) {
            this.onScrollPos();
            this.tempSelectedPath = this.getItemPath(item);
            if (item.history && !item.hasHistory)
                this.history(item.history).then(function (response) {
                    if (response.ok)
                        item.hasHistory = true;
                });
            //location.href = item.link;
        },
        onVideoItemClick(item) {
            if (this.isAndroid && this.isUseDeepLink) {
                this.tempSelectedPath = this.getItemPath(item);
                this.prevPath = this.tempSelectedPath;
                this.selectedPath = this.prevPath;
                if (item.history && !item.hasHistory) {
                    var _this = this;
                    var _href = location.href;
                    this.history(item.history).then(function (response) {
                        if (response.ok) {
                            item.hasHistory = true;
                            // refresh folder history
                            _this.reloadPage(_href);
                        }
                    });
                }
                location.href = item.link;
                return;
            }
            this.onItemClick(item);
        },
        onWorkDirClick(item) {
            this.onScrollPos();
            this.tempSelectedPath = this.getWorkDirPath(item);
            //location.href = item.link;
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
        }
    },
    computed: {
        isAndroidUseDeepLink: function () {
            return this.isAndroid && this.isUseDeepLink;
        },
        dirNameTitle: function () {
            return this.dirName || this.workDir || '';
        }
    }
};