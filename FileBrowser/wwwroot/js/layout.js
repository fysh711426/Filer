(function () {
    var isOpen = sessionStorage.getItem('isOpen');
    isOpen = isOpen !== null ? isOpen === "true" : true;

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
                sessionStorage.setItem('isOpen', isOpen);
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
