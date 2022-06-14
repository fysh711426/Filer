function initCircles(element, val) {
    var circles = element.querySelectorAll('.theme-cicle');
    for (var i = 0; i < circles.length; i++) {
        var checked = circles[i].className.indexOf('checked') > -1;
        if (circles[i].getAttribute('data-val') === val) {
            if (!checked)
                circles[i].className = circles[i].className + ' checked';
        }
        else {
            if (checked)
                circles[i].className = circles[i].className.replace(' checked', '');
        }
    }
    var hidden = element.querySelector('input');
    hidden.value = val;
}

function onTheme(element) {
    var isCheck = element.className.indexOf('checked') > -1;
    var val = element.getAttribute('data-val');
    if (!isCheck) {
        initCircles(element.parentNode, val);
        storage.setTheme(val);
        showSuccess();
        window.onThemeChange(val);
    }
}

function onTextTheme(element) {
    var isCheck = element.className.indexOf('checked') > -1;
    var val = element.getAttribute('data-val');
    if (!isCheck) {
        initCircles(element.parentNode, val);
        storage.setTextTheme(val);
        showSuccess();
    }
}

function initCheck(element, isCheck) {
    var checkbox = element.querySelector('input');
    var mark = element.querySelector('.checkbox-mark');
    if (isCheck) {
        checkbox.setAttribute('checked', '');
        mark.className = mark.className + ' checked';
        mark.innerHTML = '<svg name="Checkmark" width="18" height="18" viewBox="0 0 18 18" xmlns="http://www.w3.org/2000/svg"><g fill="none" fill-rule="evenodd"><polyline stroke-width="2" points="3.5 9.5 7 13 15 5"></polyline></g></svg>';
    }
    else {
        checkbox.removeAttribute('checked');
        mark.className = mark.className.replace(' checked', '');
        mark.innerHTML = '<svg name="Checkmark" width="18" height="18" viewBox="0 0 18 18" xmlns="http://www.w3.org/2000/svg"><g fill="none" fill-rule="evenodd"><polyline stroke="transparent" stroke-width="2" points="3.5 9.5 7 13 15 5"></polyline></g></svg>';
    }
}

function onIsUseDeepLink(element) {
    var mark = element.querySelector('.checkbox-mark');
    var isCheck = mark.className.indexOf('checked') > -1;
    isCheck = !isCheck;
    initCheck(element, isCheck);
    storage.setIsUseDeepLink(isCheck);
    showSuccess();
}

function onDeepLinkPackage(element) {
    var val = element.value;
    var prev = element.getAttribute('data-prev');
    if (prev !== val) {
        storage.setDeepLinkPackage(val);
        element.setAttribute('data-prev', val);
        showSuccess();
    }
}

var showCount = 0;
function showSuccess() {
    var notice = document.querySelector('.notice-warp');
    var placeholder = document.querySelector( '.notice-placeholder');
    notice.innerHTML = '';
    notice.className = notice.className.replace(' show', '');
    placeholder.style.display = 'block';
    showCount++;
    setTimeout(function () {
        notice.innerHTML = '<div class="notice success">已儲存變更。</div>';
        notice.className = notice.className + ' show';
        placeholder.style.display = 'none';
        var temp = showCount;
        setTimeout(function () {
            if (temp === showCount) {
                notice.innerHTML = '';
                notice.className = notice.className.replace(' show', '');
                placeholder.style.display = 'block';
            }
        }, 3000);
    }, 100);
}

(function init() {
    //----- theme -----
    (function () {
        var theme = storage.theme();
        var cicles = document.querySelector('#theme');
        initCircles(cicles, theme);
    })();
    //----- textTheme -----
    (function () {
        var textTheme = storage.textTheme();
        var cicles = document.querySelector('#textTheme');
        initCircles(cicles, textTheme);
    })();
    //----- isUseDeepLink -----
    (function () {
        var isUseDeepLink = storage.isUseDeepLink();
        var checkbox = document.querySelector('#isUseDeepLink');
        initCheck(checkbox, isUseDeepLink);
    })();
    //----- deepLinkPackage -----
    (function () {
        var deepLinkPackage = storage.deepLinkPackage();
        var input = document.querySelector('#deepLinkPackage');
        input.value = deepLinkPackage;
        input.setAttribute('data-prev', deepLinkPackage);
    })();
})();
