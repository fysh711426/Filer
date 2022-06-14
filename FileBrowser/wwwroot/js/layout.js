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

    // scrollPos
    var onScrollPos = function () {
        var scrollPos = storage.scrollPos();
        if (scrollPos.length > 0) {
            var last = scrollPos.pop();
            var path = window.path || '';
            if (last.path !== window.path) {
                // child
                if (path !== '' && path.indexOf(last.path) > -1)
                    return;
                storage.setScrollPos([]);
                return;
            }
            storage.setScrollPos(scrollPos);
            document.documentElement.scrollTop = last.pos;
        }
    }
    onScrollPos();
    var main = document.querySelector('.main');
    if (main) {
        main.style.opacity = 1;
    }
})();

function onLink(e) {
    e.stopPropagation();
}

function onFile() {
    var scrollPos = storage.scrollPos();
    var pos = window.pageYOffset ||
        document.documentElement.scrollTop ||
        document.body.scrollTop || 0;;
    var path = window.path || '';
    scrollPos.push({
        pos: pos, path: path
    });
    storage.setScrollPos(scrollPos);
}
