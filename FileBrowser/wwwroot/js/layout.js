(function () {
    // control
    var isOpen = storage.isOpen();
    var btnBoard = document.getElementsByClassName('control')[0];
    if (btnBoard) {
        var check = function() {
            if (isOpen)
                btnBoard.style.display = 'block';
            else 
                btnBoard.style.display = 'none';
        }
        check();
        window.addEventListener('resize', check);

        var title = document.querySelector('.title');
        if (title) {
            title.onclick = function () {
                if (!isOpen) {
                    btnBoard.style.display = 'block';
                    isOpen = true;
                }
                else {
                    btnBoard.style.display = 'none';
                    isOpen = false;
                }
                storage.setIsOpen(isOpen);
            };
        }
    }

    //title
    title = document.querySelector('.title');
    if (title) {
        window.addEventListener("orientationchange", function () {
            var mode = Math.abs(window.orientation) === 90 ?
                'landscape' : 'portrait';
            title.className = title.className.replace(' freeze', '');
            if (mode === 'portrait')
                title.className = title.className + ' freeze';
        });
    }
    
    // image title
    var showTitle = false;
    var imageBoards = document.querySelectorAll('.image-board');
    for (var i = 0; i < imageBoards.length; i++) {
        imageBoards[i].onclick = function () {
            if (showTitle) {
                title.className = title.className.replace(' over', '');
                showTitle = false;
            }
            else {
                title.className = title.className + ' over';
                showTitle = true;
            }
        }
    }
})();

function onLink(e) {
    e.stopPropagation();
}
