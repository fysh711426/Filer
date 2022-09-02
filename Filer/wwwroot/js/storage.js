var storage = null;
(function () {
    var IS_OPEN = 'IS_OPEN';
    var VOLUMN = 'VOLUMN';
    var THEME = 'THEME';
    var TEXT_THEME = 'TEXT_THEME';
    var IS_USE_DEEP_LINK = 'IS_USE_DEEP_LINK';
    var DEEP_LINK_PACKAGE = 'DEEP_LINK_PACKAGE';
    var SCROLL_POS_ARRAY = 'SCROLL_POS_ARRAY';
    var VIEW_MODE = 'VIEW_MODE';
    var PREV_PATH = 'PREV_PATH';

    var _storage = {
        getAll: function () {
            return {
                isOpen: _storage.isOpen(),
                volume: _storage.volume(),
                theme: _storage.theme(),
                textTheme: _storage.textTheme(),
                isUseDeepLink: _storage.isUseDeepLink(),
                deepLinkPackage: _storage.deepLinkPackage()
            }
        },
        isOpen: function () {
            var isOpen = sessionStorage.getItem(IS_OPEN);
            return isOpen !== null ? isOpen === "true" : true;
        },
        setIsOpen: function (val) {
            sessionStorage.setItem(IS_OPEN, val);
        },
        volume: function () {
            var volume = localStorage.getItem(VOLUMN);
            return volume !== null ? parseFloat(volume) : 0.0;
        },
        setVolume: function (val) {
            localStorage.setItem(VOLUMN, val);
        },
        theme: function () {
            var theme = localStorage.getItem(THEME);
            return theme !== null ? theme : '';
        },
        setTheme: function (val) {
            localStorage.setItem(THEME, val);
        },
        textTheme: function () {
            var textTheme = localStorage.getItem(TEXT_THEME);
            return textTheme !== null ? textTheme : '';
        },
        setTextTheme: function (val) {
            localStorage.setItem(TEXT_THEME, val);
        },
        isUseDeepLink: function () {
            var isUseDeepLink = localStorage.getItem(IS_USE_DEEP_LINK);
            return isUseDeepLink !== null ? isUseDeepLink === "true" : true;
        },
        setIsUseDeepLink: function (val) {
            localStorage.setItem(IS_USE_DEEP_LINK, val);
        },
        deepLinkPackage: function () {
            var deepLinkPackage = localStorage.getItem(DEEP_LINK_PACKAGE);
            return deepLinkPackage !== null ? deepLinkPackage : '';
        },
        setDeepLinkPackage: function (val) {
            localStorage.setItem(DEEP_LINK_PACKAGE, val);
        },
        scrollPos: function () {
            var json = sessionStorage.getItem(SCROLL_POS_ARRAY);
            var scrollPos = JSON.parse(json);
            return scrollPos !== null ? scrollPos : [];
        },
        setScrollPos: function (val) {
            var json = JSON.stringify(val);
            sessionStorage.setItem(SCROLL_POS_ARRAY, json);
        },
        viewMode: function () {
            var viewMode = localStorage.getItem(VIEW_MODE);
            return viewMode !== null ? viewMode : '';
        },
        setViewMode: function (val) {
            localStorage.setItem(VIEW_MODE, val);
        },
        prevPath: function () {
            var prevPath = sessionStorage.getItem(PREV_PATH);
            return prevPath !== null ? prevPath : '';
        },
        setPrevPath: function (val) {
            sessionStorage.setItem(PREV_PATH, val);
        }
    }
    storage = _storage;
})();