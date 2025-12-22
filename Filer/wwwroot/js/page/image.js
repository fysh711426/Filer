var vm = new Vue({
    el: '#app',
    mixins: [
        layoutMixin,
        routeMixin,
        bookmarkMixin
    ],
    data: {
        datas: [],
        imagePath: '',
        imageName: '',
        imageIndex: 0,
        isImageControlOver: true
    },
    created() {
        this.bindLink(initialData);
        this.initData(initialData);
        this.initPath(this.getPagePath());
        this.imagePath = this.filePath;
        this.imageName = this.fileName;
        for (var i = 0; i < this.datas.length; i++) {
            if (this.datas[i].path === this.imagePath) {
                this.imageIndex = i;
            }
        }
        var item = this.datas[this.imageIndex];
        this.lazyLoadImage(this.imageIndex, this.imagePath, this.imageName, item);
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        this.isLoaded = true;
        setTimeout(function () {
            fileNavbar.enableHover();
            fileNavbar.enableImageOver();
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                item.link = '';
            }
        },
        loadImage(url, callback) {
            var img = new Image();
            img.onload = function () {
                callback();
                img = null;
            }
            img.src = url;
        },
        lazyLoadImage(imageIndex, imagePath, imageName, item) {
            var link = this.routeLink('api/file/image', this.workNum, imagePath);
            //progress.start();
            var _this = this;
            this.loadImage(link, function () {
                _this.imageIndex = imageIndex;
                _this.imagePath = imagePath;
                _this.imageName = imageName;
                _this.datas[imageIndex].link = link;
                //progress.done();
            });
            var history = this.routeLink('api/history', this.workNum, imagePath);
            if (history && !item.hasHistory)
                this.history(history).then(function (response) {
                    if (response.ok)
                        item.hasHistory = true;
                });
        },
        onPrev() {
            var imageIndex = (this.imageIndex - 1 + this.datas.length) % this.datas.length;
            var imagePath = this.datas[imageIndex].path;
            var imageName = this.datas[imageIndex].name;
            var item = this.datas[imageIndex];
            this.lazyLoadImage(imageIndex, imagePath, imageName, item);

            //if (this.imageIndex > 0) {
            //    this.imageIndex--;
            //    this.imagePath = this.datas[this.imageIndex].path;
            //}
        },
        onNext() {
            var imageIndex = (this.imageIndex + 1) % this.datas.length;
            var imagePath = this.datas[imageIndex].path;
            var imageName = this.datas[imageIndex].name;
            var item = this.datas[imageIndex];
            this.lazyLoadImage(imageIndex, imagePath, imageName, item);

            //if (this.imageIndex < this.datas.length - 1) {
            //    this.imageIndex++;
            //    this.imagePath = this.datas[this.imageIndex].path;
            //}
        },
        onImageToggle() {
            this.isImageControlOver = !this.isImageControlOver;
        }
    },
    computed: {
        showPrev: function () {
            return this.isImageControlOver;
            // return this.isImageControlOver && this.imageIndex > 0;
        },
        showNext: function () {
            return this.isImageControlOver;
            // return this.isImageControlOver && this.imageIndex < this.datas.length - 1;
        }
    }
});