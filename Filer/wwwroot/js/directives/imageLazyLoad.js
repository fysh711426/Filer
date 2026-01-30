//(function () {
//    var observerOptions = {
//        root: null,
//        rootMargin: '320px 0px 320px 0px',
//        threshold: 0.01
//    };
//    var observer = new IntersectionObserver((entries) => {
//        entries.forEach(entry => {
//            var el = entry.target;
//            _cleanTimer(el);
//            if (entry.isIntersecting) {
//                el._vue_image_lazy_load_timerId = setTimeout(() => {
//                    observer.unobserve(el);
//                    _init(el, el._vue_image_lazy_load_src);
//                }, 500);
//            }
//        });
//    }, observerOptions);
//    var _init = function (el, src) {
//        _clean(el);

//        var onload = function () {
//            el.setAttribute('data-loaded', '');
//            _clean(el);
//        };
//        el._vue_image_lazy_load_onload = onload;

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
//        if (el._vue_image_lazy_load_onload) {
//            el.removeEventListener('load', el._vue_image_lazy_load_onload);
//            el._vue_image_lazy_load_onload = null;
//        }
//    };
//    var _cleanTimer = function (el) {
//        if (el._vue_image_lazy_load_timerId) {
//            clearTimeout(el._vue_image_lazy_load_timerId);
//            el._vue_image_lazy_load_timerId = null;
//        }
//    };
//    Vue.directive('image-lazy-load', {
//        inserted(el, binding) {
//            el._vue_image_lazy_load_src = binding.value;
//            observer.observe(el);
//        },
//        componentUpdated(el, binding) {
//            if (binding.value !== binding.oldValue) {
//                el._vue_image_lazy_load_src = binding.value;
//                if (el.hasAttribute('data-loaded')) {
//                    _init(el, binding.value);
//                }
//            }
//        },
//        unbind(el) {
//            _clean(el);
//            _cleanTimer(el);
//            observer.unobserve(el);
//        }
//    });
//})();

var ImageLazyLoad = function (name) {
    return {
        install: function (Vue, options) {
            var _options = options ?? {};
            var _delay = _options.delay ?? 500;
            var _observerOptions = _options.observerOptions ?? {
                root: null,
                rootMargin: '320px 0px 320px 0px'
            };
            function onLoaded() {
                var el = this;
                el.setAttribute('data-loaded', '');
                el.removeEventListener('load', onLoaded);
            }
            var observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    var el = entry.target;
                    clearTimeout(el._vue_image_lazy_load_timerId);
                    if (entry.isIntersecting) {
                        el._vue_image_lazy_load_timerId = setTimeout(() => {
                            observer.unobserve(el);
                            el.removeEventListener('load', onLoaded);
                            el.addEventListener('load', onLoaded);
                            el.src = el._vue_image_lazy_load_src;
                            el._vue_image_lazy_load_timerId = null;
                        }, _delay);
                    }
                });
            }, _observerOptions);
            Vue.directive(name || 'image-lazy-load', {
                inserted(el, binding) {
                    el._vue_image_lazy_load_src = binding.value;
                    observer.observe(el);
                },
                componentUpdated(el, binding) {
                    if (binding.value !== binding.oldValue) {
                        el._vue_image_lazy_load_src = binding.value;
                        el.removeAttribute('data-loaded');
                        el.removeEventListener('load', onLoaded);
                        clearTimeout(el._vue_image_lazy_load_timerId);
                        observer.observe(el);
                    }
                },
                unbind(el) {
                    el.removeEventListener('load', onLoaded);
                    clearTimeout(el._vue_image_lazy_load_timerId);
                    observer.unobserve(el);
                }
            });
        }
    };
};