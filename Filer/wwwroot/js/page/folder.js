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
        viewMode: '',
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
        this.bindVideoLink(initialData);
        this.initData(initialData);
        window.path = '/' + this.workNum + '/' + this.dirPath;
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
        bindVideoLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
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

                        item.videoLink = link;
                    }
                    else {
                        item.videoLink = this.routeLink('video', data.workNum, item.path);
                    }
                }
            }
        },
        onViewModeChange() {
            this.viewMode = this.viewMode === 'view' ? 'download' : 'view';
            storage.setViewMode(this.viewMode);
        }
    }
});

function onLink(e) {
    e.stopPropagation();
}