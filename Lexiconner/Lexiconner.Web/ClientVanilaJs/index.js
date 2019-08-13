import _ from 'lodash';
import moment from 'moment';

import HttpUtil from './modules/HttpUtil.js';
let helper = new HttpUtil();

import DomUtil from './modules/DomUtil.js';
let domUtil = new DomUtil();

// example of using class defined in separate file (module)
import ExampleUtil from './utils/exampleUtil.js';

ExampleUtil.testStatic('test1');
let util = new ExampleUtil();
util.test('test2');

document.addEventListener("DOMContentLoaded", function (event) {
    helper.httpGet('/config', function (config) {
        start(config);
    });
});


function start(config) {

    // https://github.com/IdentityModel/oidc-client-js/wiki
    var authConfig = {
        authority: config.auth.authority,
        client_id: config.auth.clientId,
        redirect_uri: config.auth.redirectUri,
        response_type: config.auth.responseType,
        scope: config.auth.scope,
        post_logout_redirect_uri: config.auth.postLogoutRedirectUri,

        // access_token renew
        automaticSilentRenew: true,
        silentRequestTimeout: 10000,
        accessTokenExpiringNotificationTime: 3 * 60, // 3 mins in secs
    };

    var userManager = new Oidc.UserManager(authConfig);
    window.userManager = userManager; // for tests only

    // setup logging
    Oidc.Log.logger = console;
    Oidc.Log.level = Oidc.Log.INFO;

    userManager.events.addUserLoaded(function () {
        console.log("userLoaded");
    });
    userManager.events.addUserUnloaded(function () {
        console.log("userUnloaded");
    });
    userManager.events.addAccessTokenExpiring(function () {
        console.log("access_token expiring...");
    });
    userManager.events.addAccessTokenExpired(function () {
        console.log("access_token expired");
    });
    userManager.events.addSilentRenewError(function (err) {
        console.error("silentRenewError: ", err);
    });

    // check user logged in
    userManager.getUser().then(function (user) {
        if (user) {
            console.log("User logged in", user, user.profile);
            //user.access_token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkY0MzgyNUFFMjQ4NDNFNjMzMjYwQjlBOTU1RDExNDQ4NkZGRURCRUMiLCJ0eXAiOiJKV1QiLCJ4NXQiOiI5RGdscmlTRVBtTXlZTG1wVmRFVVNHXy0yLXcifQ.eyJuYmYiOjE1NjQwNTE5MjQsImV4cCI6MTU2NDA1NTUyNCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwNCIsImF1ZCI6WyJodHRwczovL2xvY2FsaG9zdDo1MDA0L3Jlc291cmNlcyIsIndlYmFwaSJdLCJjbGllbnRfaWQiOiJ3ZWJzcGEiLCJzdWIiOiI1ZDM5ODk5YjA5ZDAyNjI1NzRkNWYwOWYiLCJhdXRoX3RpbWUiOjE1NjQwNTE5MjIsImlkcCI6ImxvY2FsIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsIndlYmFwaSIsIm9mZmxpbmVfYWNjZXNzIl0sImFtciI6WyJwd2QiXX0.n9vNo2wkZEEtNS2hHHEroDYiUG0OPoXFkF86poJEJXNhQrndAjQdVMc4FeFehUDP7GOj7pSCAmcllvRiRsUjAQ-IeV5DEjtgFsLxZon8svbb5UPJ-efjULcHT-U2u5a-eWqRQXck1gZ2W9fIzCcaBYzptV_K9gjlhuFLlUVs-L2PQe0gULHu0fKYmZjtdO-bI8hBYo8ZSvvwrRVMVgKp798bmlIX5z12mnh_knLCWCUe-tCn4qe0X0oHgHb3KGRneTR2JpocCQWSvdYYkPm-rR-XK3m8EETq_kxXCdTd1nRcV2pKa0sSynpcLW2MOQiOZ61wuFcsHolGV6K9zWnBFQ";

            // run app
            runApp(user);
            // handleAccessTokenExpiration(user);
            checkIdentityApi(user);
        }
        else {
            console.log("User not logged in");

            // redirect to login
            login();
        }
    });

    function login() {

        userManager.signinRedirect();
    }

    function logout() {
        console.log('Logout requested.')
        userManager.signoutRedirect();
    }

    function handleAccessTokenExpiration(user) {
        let {
            profile,
            id_token,
            access_token,
            refresh_token,
            expires_at, // int - seconds
            scope,
            session_state,
            state,
            token_type,
            expired, // func
            expires_in, // func - returns seconds
            scopes, // func - returns array<string>
        } = user;
        let {
            amr, //array<string>
            auth_time, // int - seconds
            idp,
            sid,
            sub, // user id
        } = profile;

        // refresh access_token manually before it get expired using refresh_token (if flow allows)
        // ...
    }

    function checkIdentityApi(user) {
        var url = `${config.urls.api}/api/v2/identity`;

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            console.log('Test Api: ', xhr.status, JSON.parse(xhr.responseText));
        };
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    }

    function runApp(user) {

        window.checkEventListener = {
            menuLinks: { state: false },
            logoutButton: { state: false },
            cardLeftButton: { state: false },
            cardRightButton: { state: false },
            cardLeftButtonMobileVersion: { state: false },
            cardRightButtonMobileVersion: { state: false },
            itemListFirstButton: { state: false },
            itemListPrevButton: { state: false },
            itemListNextButton: { state: false },
            itemListLastButton: { state: false },
            itemListButtonFromListToCard: { state: false },
            itemListPutButton: { state: false },
            itemListDeleteButton: { state: false },
            itemListAddButton: { state: false },
            formPutButton: { state: false },
            formAddButton: { state: false }
        }; // used for addBubleEventListener() for add one event listener


        /**
         * Listens to all events inside source element
         * @param {any} sourceElSelector
         * @param {any} targetElSelector
         * @param {any} eventName
         * @param {any} eventHandler
         */


        /**
         * Inits app menu, enables menu links
         */
        function initAppMenu() {

            domUtil.addBubleEventListener('body', '[data-route-link]', 'click', window.checkEventListener.menuLinks, function (e, actualEl, desiredEl) {
                e.preventDefault();
                e.stopPropagation();

                var targetLinkEl = desiredEl;
                goToRoute(targetLinkEl.dataset.routeLink);
            });
        }

        function getData(offset = 0, limit = 2, callBack = null) {
            helper.httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

        // handle logout
        // var logoutButtonEls = document.querySelectorAll('.js-logout-button');
        domUtil.addBubleEventListener('body', '.js-logout-button', 'click', window.checkEventListener.logoutButton, function (e, desiredEl) {
            e.stopPropagation();
            logout();
        });

        initAppMenu();

        window.wordOrder = {
            length: 0,
            isFromWordList: false
        }; // used in pageHandlers['word-list'] for eventListener



        window.pageHandlers = {};

        window.pageHandlers['no-response'] = function () {

        }

        window.pageHandlers['dashboard'] = function () {

        }

        window.pageHandlers['cards'] = function () {


            var limit = 5;
            var counter = window.wordOrder.isFromWordList ? (window.wordOrder.length - Math.floor(window.wordOrder.length / limit) * limit) : 0;
            var offsetCards = window.wordOrder.isFromWordList ? (Math.floor(window.wordOrder.length / limit) * limit) : 0;
            var pages = 0;
            var cardData = {};

            function showNextCard(direction = 0) {// !!!!!!!!!!!!!!!!!!!!!!

                counter = counter + direction * 1;

                if (!cardData.items || (counter >= cardData.items.length || counter < 0)) {
                    if (cardData.items && direction === -1) {
                        offsetCards = offsetCards - limit;
                        offsetCards = offsetCards < 0 ? ((pages - 1) * limit) : offsetCards;
                    }
                    if (cardData.items && direction === 1) {
                        offsetCards = offsetCards + limit;
                        offsetCards = offsetCards > cardData.totalCount ? 0 : offsetCards;
                    }
                    getData(offsetCards, limit, function (response) {
                        cardData = response.data;

                        if (window.wordOrder.isFromWordList) {
                            counter = window.wordOrder.length - Math.floor(window.wordOrder.length / limit) * limit;
                            window.wordOrder.isFromWordList = false;
                        } else {

                            if (direction === -1) {
                                counter = cardData.items.length - 1;
                            } else {
                                counter = 0;
                            }

                        }

                        pages = Math.ceil(cardData.totalCount / limit);

                        showDataOnCard(cardData.items[counter]);

                    });

                } else {
                    showDataOnCard(cardData.items[counter]);
                }

            }

            function showDataOnCard(card) {
                var cardEl = document.getElementById("mainBlockCard");
                var cardTitleEl = cardEl.querySelector("#cardTitle");
                var cardDescEl = cardEl.querySelector("#cardDesc");
                var cardExampleTextEl = cardEl.querySelector("#cardExampleText");
                var cardExampleImageEl = cardEl.querySelector("#cardExampleImage");


                cardTitleEl.innerText = card.title;
                cardDescEl.innerText = card.description;
                if (!card.exampleText) {
                    cardExampleTextEl.classList.add('hidden');
                } else {
                    cardExampleTextEl.innerText = card.exampleText;
                    cardExampleTextEl.classList.remove('hidden');
                }

                if (!card.image) {

                    cardExampleImageEl.src = "images\\default.png";
                } else {
                    cardExampleImageEl.src = card.image.url;
                }
            }


            var leftButtonEl = document.querySelector(".card-button-left");

            domUtil.addBubleEventListener(leftButtonEl, ".card-button-icon", "click", window.checkEventListener.cardLeftButton, function (e) {
                showNextCard(-1);
            });



            var rightButtonEl = document.querySelector(".card-button-right");

            domUtil.addBubleEventListener(rightButtonEl, ".card-button-icon", "click", window.checkEventListener.cardRightButton, function (e) {
                showNextCard(1);
            });


            var leftButtonElMobileVersion = document.getElementById("cardButtonLeftMobileVersion");

            domUtil.addBubleEventListener(leftButtonElMobileVersion, ".card-button-icon", "click", window.checkEventListener.cardLeftButtonMobileVersion, function (e) {
                showNextCard(-1);
            });

            var rightButtonElMobileVersion = document.getElementById("cardButtonRightMobileVersion");

            domUtil.addBubleEventListener(rightButtonElMobileVersion, ".card-button-icon", "click", window.checkEventListener.cardRightButtonMobileVersion, function (e) {
                showNextCard(1);
            });
            showNextCard(0);
        }
        // Show all data

        window.pageHandlers['word-list'] = function (pageEl) {

            var itemListContainerEl = pageEl.querySelector('.js-item-list-container');
            var itemListEl = pageEl.querySelector('.js-item-list');
            var listItemTemplateEl = itemListContainerEl.querySelector('.js-list-item-template');

            function addDataItemsBlock(item, i) {
                var clone = listItemTemplateEl.cloneNode(true);
                var textContainer = clone.querySelector('.js-item-text');
                var textEl = clone.querySelector('.list-item-text-span');

                // begin edit picture elements (delete,put)
                var containerEditPictureEl = clone.querySelector('.container-edit-icon');
                var deletePictureEl = containerEditPictureEl.querySelector('.list-item-icon-delete');
                var putPictureEl = containerEditPictureEl.querySelector('.list-item-icon-put');
                // end edit picture elements (delete,put)

                textEl.innerText = item.title;

                textContainer.setAttribute('position-in-list', i);
                textEl.setAttribute('position-in-list', i);

                deletePictureEl.setAttribute('position-in-list', i);
                putPictureEl.setAttribute('position-in-list', i);

                clone.setAttribute('position-in-list', i);
                clone.classList.remove('list-item--hidden');
                itemListEl.appendChild(clone);
            }

            function showPageData(items) {
                // remove all childs
                while (itemListEl.firstChild) {
                    itemListEl.removeChild(itemListEl.firstChild);
                }

                items.forEach(function (item, i) {
                    addDataItemsBlock(item, i);
                });
            }

            var totalCount = 0;
            var page = 0;
            var limit = 40;
            var pageData = {};
            var offsetWordList = page * limit;
            var calcPagesCount = function () {
                return Math.ceil(totalCount / limit);
            }

            function getPageData(page = 0, limit = 40, callBack) {
                var pages = Math.ceil(totalCount / limit);
                page = page < 0 ? 0 : page;
                page = page > pages ? pages : page;
                offsetWordList = page * limit;

                getData(offsetWordList, limit, callBack);
            }

            function showPage(page, limit) {
                getPageData(page, limit, function (response) {
                    totalCount = response.data.totalCount;
                    pageData = response.data;

                    showPageData(response.data.items);
                });
            }

            function setNumberPage(page) {
                currentButtonEl.innerText = page + 1;
            }

            var firstButtonEl = itemListContainerEl.querySelector('.js-first-page-button');
            var prevButtonEl = itemListContainerEl.querySelector('.js-prev-page-button');
            var currentButtonEl = itemListContainerEl.querySelector('.js-current-page-button');
            var nextButtonEl = itemListContainerEl.querySelector('.js-next-page-button');
            var lastButtonEl = itemListContainerEl.querySelector('.js-last-page-button');


            domUtil.addBubleEventListener(firstButtonEl, ".button-icon", "click", window.checkEventListener.itemListFirstButton, function (e) {
                page = 0;
                setNumberPage(page)
                showPage(page, limit);
            });

            domUtil.addBubleEventListener(prevButtonEl, ".button-icon", "click", window.checkEventListener.itemListPrevButton, function (e) {
                page = page <= 0 ? 0 : page - 1;
                setNumberPage(page)
                showPage(page, limit);
            });

            domUtil.addBubleEventListener(nextButtonEl, ".button-icon", "click", window.checkEventListener.itemListNextButton, function (e) {
                page = (page < calcPagesCount() - 1) ? page + 1 : (calcPagesCount() - 1);
                setNumberPage(page)
                showPage(page, limit);
            });

            domUtil.addBubleEventListener(lastButtonEl, ".button-icon", "click", window.checkEventListener.itemListLastButton, function (e) {
                page = calcPagesCount() - 1;
                setNumberPage(page)
                showPage(page, limit);
            });

            domUtil.addBubleEventListener(itemListContainerEl, '.js-item-text', 'click', window.checkEventListener.itemListButtonFromListToCard, function (e, desiredEl) {
                e.stopPropagation();

                window.wordOrder.length = (page * limit) + Number(desiredEl.getAttribute('position-in-list'));
                window.wordOrder.isFromWordList = true;
                goToRoute('#cards');
            });

            domUtil.addBubleEventListener(itemListContainerEl, '.list-item-icon-delete', 'click', window.checkEventListener.itemListDeleteButton, function (e, actualEl, desiredEl) {
                e.stopPropagation();

                var numberOfItem = Number(desiredEl.getAttribute('position-in-list'));

                if (confirm(`Do you want to delete ${pageData.items[numberOfItem].title}`)) {

                    var idItemUrl = `${config.urls.api}/api/v2/StudyItems/${pageData.items[numberOfItem].id}`;
                    deleteData(idItemUrl, user.access_token);

                    alert('Deleted');
                    showPage(page, limit);
                }
            });

            var idItemPutUrl = '';// used for put
            var postUrl = `${config.urls.api}/api/v2/StudyItems`;// used for add item
            var formDataSend = {};
            var modalWindowForm = document.querySelector('.form-dialog');
            var formButtonEl = document.querySelectorAll('.form-control-button');

            document.querySelector('.modal-window-close-button').onclick = function () {
                modalWindowForm.close();
            };

            domUtil.addBubleEventListener(itemListContainerEl, '.list-item-icon-put', 'click', window.checkEventListener.itemListPutButton, function (e, actualEl, desiredEl) {
                e.stopPropagation();

                var numberOfItem1 = Number(desiredEl.getAttribute('position-in-list'));
                var formPutButtonEl = document.querySelector('.js-put-button');

                formButtonEl.forEach(function (item) {
                    item.classList.remove('active');
                    item.classList.add('hidden');
                });

                formPutButtonEl.classList.replace('hidden', 'active');
                modalWindowForm.showModal();

                defaultFullfieldForm(pageData.items[numberOfItem1]);

                domUtil.addBubleEventListener(formPutButtonEl, '.js-put-button', 'click', window.checkEventListener.formPutButton, function (e) {

                    idItemPutUrl = `${config.urls.api}/api/v2/StudyItems/${pageData.items[numberOfItem1].id}`;

                    if (makeRequestBody(formDataSend, pageData.items[numberOfItem1])) {

                        updateData(idItemPutUrl, formDataSend, user.access_token);
                        modalWindowForm.close();

                        var listItem = itemListEl.querySelectorAll('.js-list-item-template');
                        var listItemTextSpan = listItem[numberOfItem1].querySelector('.list-item-text-span');

                        listItemTextSpan.innerText = formDataSend.title;

                    }
                });

            });// event listener on PUT button


            domUtil.addBubleEventListener(itemListContainerEl, '.item-list-pager__add-button', 'click', window.checkEventListener.itemListAddButton, function (e, actualEl, desiredEl) {
                e.stopPropagation();

                var formAddButtonEl = document.querySelector('.js-add-button');

                formButtonEl.forEach(function (item) {
                    item.classList.remove('active');
                    item.classList.add('hidden');
                });

                formAddButtonEl.classList.replace('hidden', 'active');
                modalWindowForm.showModal();

                domUtil.addBubleEventListener(formAddButtonEl, ".js-add-button", "click", window.checkEventListener.formAddButton, function (e, desiredEl) {

                    if (makeRequestBody(formDataSend)) {
                        sendData(postUrl, formDataSend, user.access_token);
                        modalWindowForm.close();
                        location.reload()
                    }

                });
            });// event listener on ADD button

            var formResetButton = document.querySelector('.reset-button');

            formResetButton.onclick = function (e) {

                var WordFormEl = document.forms.WordForm;

                if (WordFormEl.elements.title.classList.contains('input-field-empty-js')) {
                    WordFormEl.elements.title.classList.remove('input-field-empty-js');
                }

                if (WordFormEl.elements.description.classList.contains('input-field-empty-js')) {
                    WordFormEl.elements.description.classList.remove('input-field-empty-js');
                }

                if (WordFormEl.elements.exampleText.classList.contains('input-field-empty-js')) {
                    WordFormEl.elements.exampleText.classList.remove('input-field-empty-js');
                }

                if (WordFormEl.elements.tags.classList.contains('input-field-empty-js')) {
                    WordFormEl.elements.tags.classList.remove('input-field-empty-js');
                }

            };// removes all input fields with red border when click on "reset"

            function defaultFullfieldForm(data) {
                var WordFormEl = document.forms.WordForm;

                WordFormEl.elements.title.value = data.title;

                WordFormEl.elements.description.value = data.description;

                WordFormEl.elements.exampleText.value = data.exampleText;

                //WordFormEl.elements.tags.value = data.tags;
            }

            function makeRequestBody(formDataSend) {

                var WordFormEl = document.forms.WordForm;

                formDataSend.userId = user.id_token;

                formDataSend.title = WordFormEl.elements.title.value ? WordFormEl.elements.title.value :
                    WordFormEl.elements.title.classList.add('input-field-empty-js');

                formDataSend.description = WordFormEl.elements.description.value ? WordFormEl.elements.description.value :
                    WordFormEl.elements.description.classList.add('input-field-empty-js');

                formDataSend.exampleText = WordFormEl.elements.exampleText.value ? WordFormEl.elements.exampleText.value :
                    WordFormEl.elements.exampleText.classList.add('input-field-empty-js');

                formDataSend.tags = [];
                WordFormEl.elements.tags.value ? formDataSend.tags.push(WordFormEl.elements.tags.value) :
                    WordFormEl.elements.tags.classList.add('input-field-empty-js')


                WordFormEl.elements.title.onfocus = function () {
                    if (WordFormEl.elements.title.classList.contains('input-field-empty-js')) {
                        WordFormEl.elements.title.classList.remove('input-field-empty-js');
                    }
                }

                WordFormEl.elements.description.onfocus = function () {
                    if (WordFormEl.elements.description.classList.contains('input-field-empty-js')) {
                        WordFormEl.elements.description.classList.remove('input-field-empty-js');
                    }
                }

                WordFormEl.elements.exampleText.onfocus = function () {
                    if (WordFormEl.elements.exampleText.classList.contains('input-field-empty-js')) {
                        WordFormEl.elements.exampleText.classList.remove('input-field-empty-js');
                    }
                }

                WordFormEl.elements.tags.onfocus = function () {
                    if (WordFormEl.elements.tags.classList.contains('input-field-empty-js')) {
                        WordFormEl.elements.tags.classList.remove('input-field-empty-js');
                    }
                }

                if (formDataSend.title && formDataSend.description && formDataSend.exampleText && (formDataSend.tags.length !== 0)) {
                    return true;
                }
            }// makes request body and checks empty field(s)


            function sendData(url, data, authToken) {
                helper.httpRequest(url, data, 'POST', authToken, function (request) {
                    alert("Item was add");
                });
            }

            function updateData(url, data, authToken) {
                helper.httpRequest(url, data, 'PUT', authToken, function (request) {
                    alert(`Item was update`);

                });
            }

            function deleteData(url, authToken) {

                console.log("Delete to", url);
                helper.deleteRequest(url, authToken);

            }

            showPage(page, limit);
            setNumberPage(0);
        }

        window.pageHandlers['something'] = function () {

        }
        //// base routing

        function goToRoute(route) {
            if (route !== window.location.hash) {
                window.location.hash = route;
            }
        }

        function processRoute(route) {
            // drop '#' from begining if exists
            route = route.replace(/^#/gi, '');

            // hide all menu links and all menu pages
            var linkEls = document.querySelectorAll('[data-route-link]');
            var pageEls = document.querySelectorAll('[data-route]');

            linkEls.forEach(function (item) {
                item.classList.remove('active');
            });
            pageEls.forEach(function (item) {
                item.classList.remove('active');
            });

            // show clicked page
            var targetLinkEls = document.querySelectorAll("[data-route-link='" + route + "']");
            var targetPageEls = document.querySelectorAll("[data-route='" + route + "']");

            targetLinkEls.forEach(function (item) {
                item.classList.add('active');
            });

            targetPageEls.forEach(function (item) {
                item.classList.add('active');
                var pageHandler = window.pageHandlers[route];
                if (!pageHandler) {
                    console.error(`Can't find handler for page: ${route}`);
                } else {
                    pageHandler(item);
                }
            });
        }

        // listen hash changes
        window.addEventListener('hashchange', function (e) {
            var oldURL = e.oldURL;
            var newUrl = e.newUrl;
            var newHash = window.location.hash;
            processRoute(newHash);
        }, false);

        // run route that is already in hash
        if (!!window.location.hash) {
            processRoute(window.location.hash);
        }
        ////

        function checkingServerResponse() {
            getData(0, 1, function (response) {
                if (response.data.items.length === 0) {
                    console.log('respone', false);
                    var descriptionIssue = document.querySelector('.js-empty-data');
                    descriptionIssue.classList.replace('hidden', 'active');

                    goToRoute("#no-response");
                }
            });
        }//checks server connection

        checkingServerResponse();

    }
}
