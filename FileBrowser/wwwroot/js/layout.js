(function () {
    // control
    var isOpen = storage.isOpen();
    var control = document.getElementsByClassName('control')[0];
    if (control) {
        var check = function() {
            if (isOpen)
                control.style.display = 'block';
            else 
                control.style.display = 'none';
        }
        check();
        window.addEventListener('resize', check);

        var title = document.querySelector('.title');
        if (title) {
            if (!window.isWorkDir) {
                title.onclick = function () {
                    if (!isOpen) {
                        control.style.display = 'block';
                        isOpen = true;
                    }
                    else {
                        control.style.display = 'none';
                        isOpen = false;
                    }
                    storage.setIsOpen(isOpen);
                };
            }
        }

        var checkControlOrientation = function () {
            var mode = Math.abs(window.orientation) === 90 ?
                'landscape' : 'portrait';
            var buttons = control.querySelectorAll('.button');
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].className = buttons[i].className.replace(' landscape', '');
                buttons[i].className = buttons[i].className.replace(' portrait', '');
                if (mode === 'landscape')
                    buttons[i].className = buttons[i].className + ' landscape';
                else
                    buttons[i].className = buttons[i].className + ' portrait';
            }
        }
        checkControlOrientation();
        window.addEventListener("orientationchange", checkControlOrientation);
    }

    // title
    title = document.querySelector('.title');
    if (title) {
        var checkFreeze = function () {
            var mode = Math.abs(window.orientation) === 90 ?
                'landscape' : 'portrait';
            title.className = title.className.replace(' freeze', '');
            if (mode === 'portrait')
                title.className = title.className + ' freeze';
        }
        checkFreeze();
        window.addEventListener("orientationchange", checkFreeze);
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
