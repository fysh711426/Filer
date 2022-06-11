function onCheck(element) {
    var checkbox = element.querySelector('input');
    var mark = element.querySelector('.checkbox-mark');
    var isCheck = mark.className.indexOf('checked') > -1;
    isCheck = !isCheck;
    if (isCheck) {
        checkbox.setAttribute('checked', '');
        mark.className = mark.className + ' checked';
        mark.innerHTML = '<svg name="Checkmark" width="18" height="18" viewBox="0 0 18 18" xmlns="http://www.w3.org/2000/svg"><g fill="none" fill-rule="evenodd"><polyline stroke="#000000" stroke-width="2" points="3.5 9.5 7 13 15 5"></polyline></g></svg>';
    }
    else {
        checkbox.removeAttribute('checked');
        mark.className = mark.className.replace(' checked', '');
        mark.innerHTML = '<svg name="Checkmark" width="18" height="18" viewBox="0 0 18 18" xmlns="http://www.w3.org/2000/svg"><g fill="none" fill-rule="evenodd"><polyline stroke="transparent" stroke-width="2" points="3.5 9.5 7 13 15 5"></polyline></g></svg>';
    }
}