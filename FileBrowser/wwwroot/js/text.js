(function () {
    var showTitle = false;
    var title = document.querySelector('.title');
    var textBoard = document.querySelector('.text-board');
    textBoard.onclick = function () {
        if (showTitle) {
            title.className = title.className.replace(' over', '');
            showTitle = false;
        }
        else {
            title.className = title.className + ' over';
            showTitle = true;
        }
    }
})();
