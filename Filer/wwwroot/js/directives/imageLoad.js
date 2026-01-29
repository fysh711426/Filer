(function () {
    var _init = function (el, src) {
        _clean(el);

        var onload = function () {
            el.setAttribute('data-loaded', '');
            _clean(el);
        };
        el._vue_onload_callback = onload;

        el.removeAttribute('data-loaded');

        var _src = el.getAttribute('src');
        if (_src === src && el.complete) {
            onload();
            return;
        }
        el.addEventListener('load', onload);
        el.src = src;
    };
    var _clean = function (el) {
        if (el._vue_onload_callback) {
            el.removeEventListener('load', el._vue_onload_callback);
            el._vue_onload_callback = null;
        }
    };
    Vue.directive('image-load', {
        inserted(el, binding) {
            _init(el, binding.value);
        },
        componentUpdated(el, binding) {
            if (binding.value !== binding.oldValue) {
                _init(el, binding.value);
            }
        },
        unbind(el) {
            _clean(el);
        }
    });
})();