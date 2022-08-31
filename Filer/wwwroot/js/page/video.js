var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        workNum: 0,
        filePath: '',
        fileName: '',
        parentDirPath: '',
        parentDirName: '',
        videoUrl: ''
    },
    created() {
        this.videoUrl = this.routeLink("api/file/video",
            initialData.workNum, initialData.filePath);
        this.initData(initialData);
        this.initPath(this.getPagePath());
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        //fileNavbar({
        //    enableHover: true
        //});
        this.initPlayer();
        this.isLoaded = true;
        var _this = this;
        setTimeout(function () {
            _this.initScrollPos();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        initPlayer() {
            var player = videojs('player', {
                sources: [{ src: this.videoUrl }],
                controls: true,
                autoplay: false,
                preload: 'auto',
                fluid: true,
                controlBar: {
                    pictureInPictureToggle: false
                }
            });
            player.on('ready', function () {
                var volume = storage.volume();
                player.volume(volume);
            });
            player.on('volumechange', function () {
                var volume = player.volume();
                storage.setVolume(volume);
            });
        }
    }
});