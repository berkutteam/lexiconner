

document.addEventListener("DOMContentLoaded", function (event) {
    start();
});

/**
 * Entry point for application
 * */
function start() {

    /**
     * Listens to all events inside source element
     * @param {any} sourceElSelector
     * @param {any} targetElSelector
     * @param {any} eventName
     * @param {any} eventHandler
     */
    function addBubleEventListener(sourceElSelector, targetElSelector, eventName, eventHandler) {
        var sourceEl = document.querySelector(sourceElSelector);
        sourceEl.addEventListener(eventName, function (e) {
            var actualEl = e.target; // element event fired on
            var desiredEl = e.target.closest(targetElSelector); // element we excpect event fired on

            var matches = actualEl.matches(targetElSelector);
            var isChildEl = desiredEl !== null; // if this el is parent

            if (matches || isChildEl) {
                eventHandler(e, actualEl, desiredEl || actualEl);
            }
        });
    }

    /**
     * Inits app menu, enables menu links
     */
    function initAppMenu() {
        addBubleEventListener('body', '[data-menu-link]', 'click', function (e, actualEl, desiredEl) {
            e.preventDefault();
            e.stopPropagation();

            // hide all menu links and all menu pages
            var linkEls = document.querySelectorAll('[data-menu-link]');
            var pageEls = document.querySelectorAll('[data-menu-page]');

            [].forEach.call(linkEls, function (item) {
                item.classList.remove('active');
            });
            [].forEach.call(pageEls, function (item) {
                item.classList.remove('active');
            });

            // show clicked page
            var targetLinkEl = desiredEl;
            var targetPageEls = document.querySelectorAll(`[data-menu-page='${targetLinkEl.dataset.menuLink}']`);
            targetLinkEl.classList.add('active');
            [].forEach.call(targetPageEls, function (item) {
                item.classList.add('active');
            });
        });
    }

    initAppMenu();
}
