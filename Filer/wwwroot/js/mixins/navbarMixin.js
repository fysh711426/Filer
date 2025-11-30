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
        }
    }
};