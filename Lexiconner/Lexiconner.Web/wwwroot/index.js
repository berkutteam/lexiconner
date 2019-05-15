

document.addEventListener("DOMContentLoaded", function (event) {
    httpGet('/config', function (config) {
        start(config);
    });
});

function start(config) {

    var authConfig = {
        authority: config.auth.authority,
        client_id: config.auth.clientId,
        redirect_uri: config.auth.redirectUri,
        response_type: config.auth.responseType,
        scope: config.auth.scope,
        post_logout_redirect_uri: config.auth.postLogoutRedirectUri,
    };
    var userManager = new Oidc.UserManager(authConfig);

    // check user logged in
    userManager.getUser().then(function (user) {
        if (user) {
            console.log("User logged in", user, user.profile);

            // run app
            runApp(user);

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
        userManager.signoutRedirect();
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

                linkEls.forEach(function (item) {
                    item.classList.remove('active');
                });
                pageEls.forEach(function (item) {
                    item.classList.remove('active');
                });

                // show clicked page
                var targetLinkEl = desiredEl;
                var targetPageEls = document.querySelectorAll(`[data-menu-page='${targetLinkEl.dataset.menuLink}']`);
                targetLinkEl.classList.add('active');
                targetPageEls.forEach(function (item) {
                    item.classList.add('active');
                });
            });
        }

        initAppMenu();


        var counter = -1;
        var offset = 0;
        var limit = 5;
        var pages = 0;
        var cardData = {};
        var pictures = [];

        for (var i = 0; i < 69; i++) { // add picture in array for random example picture
            pictures.push(i + ".jpg");
        }

        function getTestData(countOfElmenets) {
            var arrData = [];
            for (var i = 0; i < countOfElmenets; i++) {
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

        function getData(offset = 0, limit = 2, callBack = null) {
            httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

        function showNextCard(direction = 1) {
            counter = counter + direction * 1;

            if (!cardData.items || (counter >= cardData.items.length || counter < 0)) {
                if (cardData.items && direction === -1) {
                    offset = offset - limit;
                    offset = offset < 0 ? ((pages - 1) * limit) : offset;
                }
                if (cardData.items && direction === 1) {
                    offset = offset + limit;
                    offset = offset > cardData.totalCount ? 0 : offset;
                }
                getData(offset, limit, function (response) {
                    cardData = response.data;
                    counter = direction === 1 ? 0 : cardData.items.length - 1;

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
            cardExampleTextEl.innerText = card.exampleText;

            if (!card.examplePicture) {

                cardExampleImageEl.src = "1600x900\\" + pictures[Math.floor(Math.random() * pictures.length)];
            } else {
                cardExampleImageEl.src = card.exampleImageUrl;
            }
        }

        var leftButtonEl = document.getElementById("cardButtonLeft");
        var rightButtonEl = document.getElementById("cardButtonRight");

        leftButtonEl.addEventListener("click", function (e) {
            showNextCard(-1);
        });

        rightButtonEl.addEventListener("click", function (e) {
            showNextCard(1);
        });

        showNextCard();

        function showDataItems(datas) {

            var elRows = 0;
            var elCols = 0;
            //console.log(4, data);
            var mainBlockAllDataItems = document.getElementById('allDataItemsContainer');
            var dataItemsBlockRowDirection = document.createElement('div');
            var itemBlock = null;

            dataItemsBlockRowDirection.classList.add('data-items-block-row-direction');
            dataItemsButtonBlock.style.display = " flex";

            datas.forEach(function (item) {
                if (elCols < 5 && elRows < 8) {
                    itemBlock = document.createElement('div');
                    itemBlock.classList.add('data-items-block');
                    itemBlock.innerText = item.title;
                    dataItemsBlockRowDirection.appendChild(itemBlock);
                    elCols++;
                } else if( elRows < 8) {
                    mainBlockAllDataItems.appendChild(dataItemsBlockRowDirection);
                    dataItemsBlockRowDirection = document.createElement('div');
                    dataItemsBlockRowDirection.classList.add('data-items-block-row-direction');
                    elCols = 0;
                    elRows++;
                }
               
            });

           
        }

        var offset1 = 0;
        var counter1 = 0;

        function showNextPageDataItems(direction = 1) {
            var datas = getTestData(100);
            showDataItems(datas)

        }

        var dataItemsButtonLeftEl = document.getElementById('dataItemsButtonLeft');
        var dataItemsButtonRightEl = document.getElementById('dataItemsButtonRight');

        dataItemsButtonRightEl.addEventListener('click', function (e) {
            showNextPageDataItems(1);
        });

        dataItemsButtonLeftEl.addEventListener('click', function (e) {
            showNextPageDataItems(-1);
        });

        showNextPageDataItems();

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
