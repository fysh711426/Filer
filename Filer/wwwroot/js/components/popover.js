Vue.component('popover', {
    template: `
        <span ref="wrapper" style="display: contents;">
            <slot></slot>
        </span>
    `,
    props: [
        'template', 'placement', 'preventDefault', 'stopPropagation'
    ],
    data() {
        return {
            popover: null
        };
    },
    mounted() {
        this.$nextTick(() => {
            var button = this.$refs.wrapper.firstElementChild;
            if (button) {
                this._init(button);
            }
        });
    },
    methods: {
        _init(button) {
            var _popover = popover(button, {
                template: this.template,
                placement: this.placement,
                preventDefault: this.preventDefault,
                stopPropagation: this.stopPropagation
            });
            this.popover = _popover;
            this.$emit('init', this.popover);
        }
    }
});