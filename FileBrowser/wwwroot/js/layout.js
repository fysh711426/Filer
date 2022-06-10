(function () {
    var isOpen = sessionStorage.getItem('isOpen');
    isOpen = isOpen !== null ? isOpen === "true" : true;

    var btnBoard = document.getElementsByClassName('control')[0];
    if (btnBoard) {
        if (isOpen)
            btnBoard.style.display = 'block';

        document.body.onclick = function () {
            if (!isOpen) {
                btnBoard.style.display = 'block';
                isOpen = true;
            }
            else {
                btnBoard.style.display = 'none';
                isOpen = false;
            }
            sessionStorage.setItem('isOpen', isOpen);
        };
    }
})();

function onBack(e) {
    history.back();
    e.stopPropagation();
}

function onLink(e) {
    e.stopPropagation();
}
