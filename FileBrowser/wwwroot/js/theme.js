(function () {
    function setTheme(theme) {
        if (theme !== document.body.className) {
            document.body.className = theme;
        }
    }
    var theme = storage.theme();
    var darkQuery = window.matchMedia("(prefers-color-scheme: dark)");
    if (theme === '')
        theme = darkQuery.matches ? "dark" : "light";
    setTheme(theme);
    window.onThemeChange = function (newTheme) {
        var theme = storage.theme();
        if (newTheme !== theme)
            storage.setTheme(newTheme);
        setTheme(newTheme);
    }
    darkQuery.addListener(function (e) {
        window.onThemeChange(e.matches ? "dark" : "light");
    });
})();
