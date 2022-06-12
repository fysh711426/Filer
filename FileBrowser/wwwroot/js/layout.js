(function () {
    var isOpen = storage.isOpen();

    var btnBoard = document.getElementsByClassName('control')[0];
    if (btnBoard) {
        var check = function() {
            if (isOpen && isEnable())
                btnBoard.style.display = 'block';
            else 
                btnBoard.style.display = 'none';
        }
        check();
        
        document.body.onclick = function () {
            if (isEnable()) {
                if (!isOpen) {
                    btnBoard.style.display = 'block';
                    isOpen = true;
                }
                else {
                    btnBoard.style.display = 'none';
                    isOpen = false;
                }
                storage.setIsOpen(isOpen);
            }
        };
        window.addEventListener('resize', check);
    }
})();

function isEnable(e) {
    return window.innerWidth >= 1360;
}

function onBack(e) {
    history.back();
    e.stopPropagation();
}

function onLink(e) {
    e.stopPropagation();
}
