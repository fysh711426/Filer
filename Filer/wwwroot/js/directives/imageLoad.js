//(function () {
//    var _init = function (el, src) {
//        _clean(el);

//        var onload = function () {
//            el.setAttribute('data-loaded', '');
//            _clean(el);
//        };
//        el._vue_image_load_onload = onload;

//        el.removeAttribute('data-loaded');

//        var _src = el.getAttribute('src');
//        if (_src === src && el.complete) {
//            onload();
//            return;
//        }
//        el.addEventListener('load', onload);
//        el.src = src;
//    };
//    var _clean = function (el) {
//        if (el._vue_image_load_onload) {
//            el.removeEventListener('load', el._vue_image_load_onload);
//            el._vue_image_load_onload = null;
//        }
//    };
//    Vue.directive('image-load', {
//        inserted(el, binding) {
//            _init(el, binding.value);
//        },
//        componentUpdated(el, binding) {
//            if (binding.value !== binding.oldValue) {
//                _init(el, binding.value);
//            }
//        },
//        unbind(el) {
//            _clean(el);
//        }
//    });
//})();

//(function () {
//    var observerOptions = {
//        root: null,
//        rootMargin: '90px 0px 90px 0px',
//    };
//    var observer = new IntersectionObserver((entries) => {
//        entries.forEach(entry => {
//            var el = entry.target;
//            if (entry.isIntersecting) {
//                observer.unobserve(el);
//                el.onload = () => {
//                    el.setAttribute('data-loaded', '');
//                };
//                el.src = el._vue_image_load_src;
//            }
//        });
//    }, observerOptions);
//    Vue.directive('image-load', {
//        inserted(el, binding) {
//            el._vue_image_load_src = binding.value;
//            observer.observe(el);
//        },
//        componentUpdated(el, binding) {
//            if (binding.value !== binding.oldValue) {
//                el._vue_image_load_src = binding.value;
//                el.removeAttribute('data-loaded');
//                el.onload = null;
//                observer.observe(el);
//            }
//        },
//        unbind(el) {
//            el.onload = null;
//            observer.unobserve(el);
//        }
//    });
//})();

(function () {
    var observerOptions = {
        root: null,
        rootMargin: '90px 0px 90px 0px',
    };
    function onLoaded() {
        var el = this;
        el.setAttribute('data-loaded', '');
        el.removeEventListener('load', onLoaded);
    }
    var observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            var el = entry.target;
            if (entry.isIntersecting) {
                observer.unobserve(el);
                el.removeEventListener('load', onLoaded);
                el.addEventListener('load', onLoaded);
                el.src = el._vue_image_load_src;
            }
        });
    }, observerOptions);
    Vue.directive('image-load', {
        inserted(el, binding) {
            el._vue_image_load_src = binding.value;
            observer.observe(el);
        },
        componentUpdated(el, binding) {
            if (binding.value !== binding.oldValue) {
                el._vue_image_load_src = binding.value;
                el.removeAttribute('data-loaded');
                el.removeEventListener('load', onLoaded);
                observer.observe(el);
            }
        },
        unbind(el) {
            el.removeEventListener('load', onLoaded);
            observer.unobserve(el);
        }
    });
})();