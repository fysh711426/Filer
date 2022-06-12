var storage = null;
(function () {
    var IS_OPEN = 'IS_OPEN';
    var VOLUMN = 'VOLUMN';
    var THEME = 'THEME';
    var TEXT_THEME = 'TEXT_THEME';
    var IS_USE_DEEP_LINK = 'IS_USE_DEEP_LINK';
    var DEEP_LINK_PACKAGE = 'DEEP_LINK_PACKAGE';

    var _storage = {
        getAll: function () {
            return {
                isOpen: sessionStorage.getItem(IS_OPEN),
                volume: localStorage.getItem(VOLUMN),
                theme: localStorage.getItem(THEME),
                textTheme: localStorage.getItem(TEXT_THEME),
                isUseDeepLink: localStorage.getItem(IS_USE_DEEP_LINK),
                deepLinkPackage: localStorage.getItem(DEEP_LINK_PACKAGE)
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
            sessionStorage.setItem(VOLUMN, val);
        },
        theme: function () {
            var theme = localStorage.getItem(THEME);
            return theme !== null ? theme : 'dark';
        },
        setTheme: function (val) {
            sessionStorage.setItem(THEME, val);
        },
        textTheme: function () {
            var textTheme = localStorage.getItem(TEXT_THEME);
            return textTheme !== null ? textTheme : 'dark';
        },
        setTextTheme: function (val) {
            sessionStorage.setItem(TEXT_THEME, val);
        },
        isUseDeepLink: function () {
            var isUseDeepLink = localStorage.getItem(IS_USE_DEEP_LINK);
            return isUseDeepLink !== null ? isUseDeepLink === "true" : true;
        },
        setIsUseDeepLink: function (val) {
            sessionStorage.setItem(IS_USE_DEEP_LINK, val);
        },
        deepLinkPackage: function () {
            var deepLinkPackage = localStorage.getItem(DEEP_LINK_PACKAGE);
            return deepLinkPackage !== null ? deepLinkPackage : '';
        },
        setDeepLinkPackage: function (val) {
            sessionStorage.setItem(DEEP_LINK_PACKAGE, val);
        }
    }
    storage = _storage;
})();