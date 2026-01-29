Vue.component('dropdown-menu', {
    template: `
        <span ref="wrapper" style="display: contents;">
            <slot></slot>
        </span>
    `,
    props: [
        'template', 'placement', 'preventDefault', 'stopPropagation', 'enableSelect', 'options'
    ],
    data() {
        return {
            dropdownMenu: null
        };
    },
    mounted() {
        this.$nextTick(() => {
            var button = this.$refs.wrapper.firstElementChild;
            if (button) {
                this.init(button);
            }
        });
    },
    methods: {
        init(button) {
            var _dropdownMenu = dropdownMenu(button, {
                template: this.template,
                placement: this.placement,
                preventDefault: this.preventDefault,
                stopPropagation: this.stopPropagation,
                enableSelect: this.enableSelect,
                options: this.options
            });
            _dropdownMenu.onChange = (ele, val) => {
                this.$emit('change', ele, val);
            };
            this.dropdownMenu = _dropdownMenu;
        }
    }
});