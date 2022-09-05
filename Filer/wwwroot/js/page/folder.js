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
        deepLinkPackage: ''
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
                item.id = i + 1;
                item.isPreviewOver = false;
                item.isPreviewLoading = false;
                item.isPreviewLoaded = false;
                //item.isPreviewOnClick = false;
            }
        },
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                if (item.fileType === this.type.folder) {
                    item.link = this.routeLink('folder', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.text) {
                    item.link = this.routeLink('text', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.audio) {
                    item.link = this.routeLink('audio', data.workNum, item.path);
                    continue;
                }
                if (item.fileType === this.type.image) {
                    item.link = this.routeLink('image', data.workNum, item.path);
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
                    continue;
                }
            }
        },
        onVideoItemClick(item) {
            if (this.isAndroid && this.isUseDeepLink) {
                this.tempSelectedPath = this.getItemPath(item);
                this.prevPath = this.tempSelectedPath;
                this.selectedPath = this.prevPath;
                location.href = item.link;
                return;
            }
            this.onItemClick(item);
        },
        onViewModeChange() {
            this.viewModeIndex = (this.viewModeIndex + 1) % this.viewModes.length;
            this.viewMode = this.viewModes[this.viewModeIndex];
            storage.setViewMode(this.viewMode);
        },
        //onPreviewMounted(el) {
        //    tooltip('#' + el.id, {
        //        template: '.video-tooltip-template',
        //        placement: 'right'
        //    });
        //},
        onPrevClick(item) {
            //item.isPreviewOver = !item.isPreviewOver;
        },
        onPrevOver(item) {
            item.isPreviewOver = true;
            item.isPreviewLoading = true;
        },
        onPrevLeave(item) {
            item.isPreviewOver = false;
        },
        onPreviewLoaded(item) {
            item.isPreviewLoaded = true;
        }
    }
});

function onPreviewClick(e) {
    e.stopPropagation();
}