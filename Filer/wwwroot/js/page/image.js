var vm = new Vue({
    el: '#app',
    mixins: [layoutMixin],
    data: {
        workNum: 0,
        filePath: '',
        fileName: '',
        parentDirPath: '',
        parentDirName: '',
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
    },
    mounted() {
        this.theme = document.body.getAttribute('theme');
        onThemeButtonChange(this.theme);
        this.isLoaded = true;
        setTimeout(function () {
            fileNavbar({
                enableHover: true,
                enableImageOver: true
            });
            document.querySelector('.layout').style.opacity = 1;
        }, 1);
    },
    methods: {
        bindLink(data) {
            for (var i = 0; i < data.datas.length; i++) {
                var item = data.datas[i];
                item.link = this.routeLink('api/file/image', data.workNum, item.path);
            }
        },
        onPrev() {
            this.imageIndex = (this.imageIndex - 1 + this.datas.length) % this.datas.length;
            this.imagePath = this.datas[this.imageIndex].path;
            this.imageName = this.datas[this.imageIndex].name;
            //if (this.imageIndex > 0) {
            //    this.imageIndex--;
            //    this.imagePath = this.datas[this.imageIndex].path;
            //}
        },
        onNext() {
            this.imageIndex = (this.imageIndex + 1) % this.datas.length;
            this.imagePath = this.datas[this.imageIndex].path;
            this.imageName = this.datas[this.imageIndex].name;
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