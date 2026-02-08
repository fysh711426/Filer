var navbarMixin = {
    data() {
        return {
            viewModes: [
                'view',
                'thumbnail',
                'download'
            ],
            viewMode: '',
            viewModeIndex: 0,
            orderBys: [
                'auto',
                'name',
                'date',
                'size'
            ],
            orderBy: '',
            orderByIndex: 0,
            isOrderByDesc: false,
            imageScales: [
                '100',
                '75',
                '50',
                '25'
            ],
            imageScale: '',
            imageScaleIndex: 0
        };
    },
    methods: {
        initNavbarExpand() {
            this.viewMode = storage.viewMode() || 'view';
            for (var i = 0; i < this.viewModes.length; i++) {
                if (this.viewModes[i] === this.viewMode) {
                    this.viewModeIndex = i;
                }
            }
            this.orderBy = storage.orderBy() || 'auto';
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
        },
        onViewModeChange() {
            this.viewModeIndex = (this.viewModeIndex + 1) % this.viewModes.length;
            this.viewMode = this.viewModes[this.viewModeIndex];
            storage.setViewMode(this.viewMode);
            setTimeout(() => {
                this.rebindImageOver();
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
            if (this.isUseDataStreaming) {
                this.reorder();
                return;
            }
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
            if (this.isUseDataStreaming) {
                this.reorder();
                return;
            }
            //location.reload();
            this.reload();
        },
        onImageScaleChange() {
            this.imageScaleIndex = (this.imageScaleIndex + 1) % this.imageScales.length;
            this.imageScale = this.imageScales[this.imageScaleIndex];
            storage.setImageScale(this.imageScale);
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
                            _this.bindDatas(initialData);

                            //----- fix image loaded -----
                            //var loadedDict = {};
                            //for (var i = 0; i < _this.datas.length; i++)
                            //    loadedDict[_this.datas[i].path] =
                            //        _this.datas[i].loaded;
                            //for (var i = 0; i < initialData.datas.length; i++)
                            //    initialData.datas[i].loaded =
                            //        loadedDict[initialData.datas[i].path] || false;
                            //----- fix image loaded -----

                            _this.bindLinks(initialData);
                            _this.bindSearchPaths(initialData);
                            _this.bindMarks(initialData, initialEncodeData);
                            _this.initData(initialData);
                            _this.initData(initialEncodeData);
                        }
                    });
                }
                progress.done();
            });
        },
        rebindImageOver() {
            fileNavbar.enableImageOver();
        }
    }
};