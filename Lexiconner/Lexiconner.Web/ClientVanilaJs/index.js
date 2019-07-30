import _ from 'lodash';
import moment from 'moment';

// example of using class defined in separate file (module)
import ExampleUtil from './utils/exampleUtil.js';

ExampleUtil.testStatic('test1');
let util = new ExampleUtil();
util.test('test2');

document.addEventListener("DOMContentLoaded", function (event) {
    httpGet('/config', function (config) {
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

    userManager.events.addUserLoaded(function(){
        console.log("userLoaded");
    });
    userManager.events.addUserUnloaded(function(){
        console.log("userUnloaded");
    });
    userManager.events.addAccessTokenExpiring(function(){
        console.log("access_token expiring...");
    });
    userManager.events.addAccessTokenExpired(function(){
        console.log("access_token expired");
    });
    userManager.events.addSilentRenewError(function(err){
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

        /**
         * Listens to all events inside source element
         * @param {any} sourceElSelector
         * @param {any} targetElSelector
         * @param {any} eventName
         * @param {any} eventHandler
         */
        function addBubleEventListener(sourceElSelector, targetElSelector, eventName, eventHandler) {
            var sourceEl = (typeof sourceElSelector === "object") ? sourceElSelector : document.querySelector(sourceElSelector);

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
            addBubleEventListener('body', '[data-route-link]', 'click', function (e, actualEl, desiredEl) {
                e.preventDefault();
                e.stopPropagation();

                var targetLinkEl = desiredEl;
                goToRoute(targetLinkEl.dataset.routeLink);
            });
        }

        function getData(offset = 0, limit = 2, callBack = null) {
            httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

        // handle logout
        // var logoutButtonEls = document.querySelectorAll('.js-logout-button');
        addBubleEventListener('body', '.js-logout-button', 'click', function (e, desiredEl) {
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
            var counter = 0;
            var offsetCards = 0;

            var pages = 0;
            var cardData = {};
            var pictures = [];



            for (var i = 0; i < 69; i++) { // add picture in array for random example picture
                pictures.push(i + ".jpg");
            }

            function getTestData(countOfElmenets, testOffset = 0) {
                var arrData = [];
                for (var i = 0 + testOffset; i < countOfElmenets + testOffset; i++) {
                    if (i % 3 === 0) {
                        arrData.push({
                            title: "title asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad f" + i,
                            description: " itle asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad  " + i,
                            example: "itle asdaaaa aaaa aaaaa aaaaa aaaaaaaaaa aaaaa asd asd asd asd asdasd asfnasdf sad  " + i
                        });
                    } else {
                        arrData.push({
                            title: "title " + i,
                            description: " description " + i,
                            example: "example " + i
                        });

                    }
                }
                return arrData;
            }

            function showNextCard(direction = 1) {
                counter = counter + direction * 1;

                if (window.wordOrder.isFromWordList) {

                    counter = window.wordOrder.length - Math.floor(window.wordOrder.length / limit) * limit;
                    offsetCards = Math.floor(window.wordOrder.length / limit) * limit;
                    window.wordOrder.isFromWordList = false;

                    getData(offsetCards, limit, function (response) {

                        cardData = response.data;

                        pages = Math.ceil(cardData.totalCount / limit);
                        showDataOnCard(cardData.items[counter]);

                    });
                } else {

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

                            if ((direction === 0) || (direction === 1)) {
                                counter = 0;

                            } else {
                                counter = cardData.items.length - 1;
                            }

                            pages = Math.ceil(cardData.totalCount / limit);
                            showDataOnCard(cardData.items[counter]);

                        });


                    } else {
                        showDataOnCard(cardData.items[counter]);
                    }
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

                    cardExampleImageEl.src = "1600x900\\" + pictures[Math.floor(Math.random() * pictures.length)];
                } else {
                    cardExampleImageEl.src = card.image.url;
                }
            }

            var leftButtonEl = document.querySelector(".card-button-left");
            var rightButtonEl = document.querySelector(".card-button-right");

            var leftButtonElMobileVersion = document.getElementById("cardButtonLeftMobileVersion");
            var rightButtonElMobileVersion = document.getElementById("cardButtonRightMobileVersion");

            leftButtonEl.addEventListener("click", function (e) {

                showNextCard(-1);
            });

            rightButtonEl.addEventListener("click", function (e) {
                showNextCard(1);
            });

            leftButtonElMobileVersion.addEventListener("click", function (e) {
                showNextCard(-1);
            });

            rightButtonElMobileVersion.addEventListener("click", function (e) {
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
                var textEl = clone.querySelector('.js-item-text');
                textEl.innerText = item.title;
                textEl.setAttribute('position-in-list', i);
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
                    showPageData(response.data.items);
                });
            }

            function setNumberPage(page) {
                currentButtonEl.innerText = page;
            }

            var firstButtonEl = itemListContainerEl.querySelector('.js-first-page-button');
            var prevButtonEl = itemListContainerEl.querySelector('.js-prev-page-button');
            var currentButtonEl = itemListContainerEl.querySelector('.js-current-page-button');
            var nextButtonEl = itemListContainerEl.querySelector('.js-next-page-button');
            var lastButtonEl = itemListContainerEl.querySelector('.js-last-page-button');

            firstButtonEl.addEventListener('click', function (e) {
                page = 0;
                setNumberPage(page)
                showPage(page, limit);
            });
            prevButtonEl.addEventListener('click', function (e) {
                page = page <= 0 ? 0 : page - 1;
                setNumberPage(page)
                showPage(page, limit);
            });
            nextButtonEl.addEventListener('click', function (e) {
                page = (page < calcPagesCount() - 1) ? page + 1 : (calcPagesCount() - 1);
                setNumberPage(page)
                showPage(page, limit);
            });
            lastButtonEl.addEventListener('click', function (e) {
                page = calcPagesCount() - 1;
                setNumberPage(page)
                showPage(page, limit);
            });

            showPage(page, limit);
            setNumberPage(0);

            addBubleEventListener(itemListContainerEl, '[position-in-list]', 'click', function (e, desiredEl) {
                e.stopPropagation();
                window.wordOrder.length = (page * limit) + Number(desiredEl.getAttribute('position-in-list'));
                window.wordOrder.isFromWordList = true;
                goToRoute('#cards');
            });
        }

        window.pageHandlers['add-word'] = function () {

            var sendButtonEl = document.querySelector('.js-send-word-button');
            var formDataObj = {};
            var addWordFormEl = document.forms.addWordForm;
            var url = `${config.urls.api}/api/v2/StudyItems`;


            sendButtonEl.addEventListener('click', function (e) {
                formDataObj.userId = user.id_token;
                formDataObj.title = addWordFormEl.elements.title.value;
                formDataObj.description = addWordFormEl.elements.description.value;
                formDataObj.exampleText = addWordFormEl.elements.exampleText.value;
                formDataObj.tags = [];
                formDataObj.tags.push(addWordFormEl.elements.tags.value);
                //console.log('FORM',formDataObj);
                //console.log("token", user.id_token);
                serverRequest(url, formDataObj, user.access_token, function (request) {
                    console.log("request", request);
                });
            });

            function serverRequest(url, data, authToken = null, callback = null) {
                var xhr = new XMLHttpRequest();

                xhr.withCredentials = true; // force to show browser's default auth dialog
                xhr.open('POST', url, true);

                xhr.setRequestHeader('Content-Type', 'application/json');
                xhr.onload = function () {
                    if (xhr.status === 200) {

                        callback(xhr.responseText);

                    } else if (xhr.status === 500) {
                        console.error('Request failed.  Returned status of ' + xhr.status);
                        alert('Request failed.  Returned status of ' + xhr.status);
                    } else if (xhr.status === 401 || xhr.status === 403) {
                        console.error('Request failed.  Returned status of ' + xhr.status);
                        alert("Authentication time is up");
                        //logout();
                    }
                };

                if (authToken !== null) {
                    xhr.setRequestHeader("Authorization", "Bearer " + authToken);
                }
                console.log("sss", JSON.stringify(data));
                xhr.send(JSON.stringify(data));
            }
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



function httpGet(url, callBack, authToken = null) {
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true; // force to show browser's default auth dialog
    xhr.open('GET', url);


    xhr.onload = function () {
        if (xhr.status === 200) {
            var data = JSON.parse(xhr.responseText);

            callBack(data);
            console.log(1, data);

        } else if (xhr.status === 500) {
            console.error('Request failed.  Returned status of ' + xhr.status);
            alert('Request failed.  Returned status of ' + xhr.status);
        } else if (xhr.status === 401 || xhr.status === 403) {
            console.error('Request failed.  Returned status of ' + xhr.status);
            alert("Authentication time is up");
            //console.log(222222, xhr.getResponseHeader('WWW-Authenticate')); returns
        }
        else {
            console.error('Request failed.  Returned status of ' + xhr.status);
        }

    };


    if (authToken !== null) {
        xhr.setRequestHeader("Authorization", "Bearer " + authToken);
    }
    xhr.send();
}
