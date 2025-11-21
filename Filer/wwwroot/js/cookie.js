var cookie = null;
(function () {
    var ORDER_BY = 'orderBy';

    function getCookie(name) {
        var value = '; ' + document.cookie;
        var parts = value.split('; ' + name + '=');
        if (parts.length === 2) 
            return decodeURIComponent(parts.pop().split(';')[0]);
        return null;
    }

    function setCookie(name, value, days = 360, path = "/") {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + '=' + encodeURIComponent(value) + expires + '; path=' + path;
    }

    var _cookie = {
        orderBy: function() {
            var orderBy = getCookie(ORDER_BY);
            return orderBy !== null ? orderBy : '';
        },
        setOrderBy: function(val) {
            setCookie(ORDER_BY, val);
        }
    }
    cookie = _cookie;
})();