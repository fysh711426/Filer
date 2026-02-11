var httpMixin = {
    data() {
        return {
        };
    },
    methods: {
        handleResponse(response) {
            if (!response.ok)
                throw new Error(`HTTP error! status: ${response.status}`);
        },
        handleError(error) {
            var _alert = alertModal({
                content: `<error>${error.message}</error>`,
                confirmText: this.local.confirm
            });
            _alert.open();
        }
    }
};