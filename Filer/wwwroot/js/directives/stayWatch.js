//var StayWatch = {
//    install: function (Vue, options) {
//        var _options = options ?? {};
//        var _delay = _options.delay ?? 3000;
//        var _observerOptions = _options.observerOptions ?? {
//            root: null,
//            rootMargin: '-40% 0px -40% 0px'
//        };
//        var observer = new IntersectionObserver((entries) => {
//            entries.forEach(entry => {
//                var el = entry.target;
//                clearTimeout(el._vue_stay_watch_timerId);
//                if (entry.isIntersecting) {
//                    el._vue_stay_watch_timerId = setTimeout(() => {
//                        observer.unobserve(el);
//                        el._vue_stay_watch_binding.value();
//                        el._vue_stay_watch_timerId = null;
//                    }, _delay);
//                }
//            });
//        }, _observerOptions);
//        function observeLoaded(el) {
//            if (el._vue_stay_watch_mObserver)
//                el._vue_stay_watch_mObserver.disconnect();

//            var checkStatus = function () {
//                if (el.hasAttribute('data-loaded')) {
//                    observer.observe(el);
//                } else {
//                    clearTimeout(el._vue_stay_watch_timerId);
//                    observer.unobserve(el);
//                }
//            };

//            checkStatus();
//            var mObserver = new MutationObserver(function (mutations) {
//                mutations.forEach(function (mutation) {
//                    if (mutation.attributeName === 'data-loaded') {
//                        checkStatus();
//                    }
//                });
//            });
//            mObserver.observe(el, {
//                attributes: true,
//                attributeFilter: ['data-loaded']
//            });
//            el._vue_stay_watch_mObserver = mObserver;
//        }
//        Vue.directive('stay-watch', {
//            inserted(el, binding) {
//                el._vue_stay_watch_binding = binding;
//                observeLoaded(el);
//            },
//            unbind(el) {
//                clearTimeout(el._vue_stay_watch_timerId);
//                observer.unobserve(el);
//                if (el._vue_stay_watch_mObserver)
//                    el._vue_stay_watch_mObserver.disconnect();
//            }
//        });
//    }
//};

var StayWatch = {
    install: function (Vue, options) {
        var _options = options ?? {};
        var _delay = _options.delay ?? 2000;
        var _observerOptions = _options.observerOptions ?? {
            root: null,
            rootMargin: '-25% 0px -25% 0px'
        };
        var observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                var el = entry.target;
                clearTimeout(el._vue_stay_watch_timerId);
                if (entry.isIntersecting) {
                    el._vue_stay_watch_timerId = setTimeout(() => {
                        observer.unobserve(el);
                        el._vue_stay_watch_binding.value();
                        el._vue_stay_watch_timerId = null;
                    }, _delay);
                }
            });
        }, _observerOptions);
        function observeLoaded(el) {
            var checkStatus = function () {
                if (el.hasAttribute('data-loaded')) {
                    observer.observe(el);
                } else {
                    clearTimeout(el._vue_stay_watch_timerId);
                    observer.unobserve(el);
                }
            };
            checkStatus();
            var mObserver = new MutationObserver(function (mutations) {
                checkStatus();
            });
            mObserver.observe(el, {
                attributes: true,
                attributeFilter: ['data-loaded']
            });
            el._vue_stay_watch_mObserver = mObserver;
        }
        Vue.directive('stay-watch', {
            inserted(el, binding) {
                el._vue_stay_watch_binding = binding;
                observeLoaded(el);
            },
            unbind(el) {
                clearTimeout(el._vue_stay_watch_timerId);
                observer.unobserve(el);
                if (el._vue_stay_watch_mObserver)
                    el._vue_stay_watch_mObserver.disconnect();
            }
        });
    }
};