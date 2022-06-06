var isOpen = sessionStorage.getItem('isOpen');
isOpen = isOpen !== null ? isOpen === "true" : true;

var btnBoard = document.getElementsByClassName('btn-board')[0];
isOpen = !isOpen;
onclick();

function onclick() {
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
document.body.onclick = onclick;

function onBack(e) {
    history.back();
    e.stopPropagation();
}

function onLink(e) {
    e.stopPropagation();
}