
import DomUtil from '../DomUtil';
let domUtil = new DomUtil();

import globalScopes from './global-scopes.js';

import HttpUtil from '../HttpUtil.js';
let helper = new HttpUtil();


class Cards {

    constructor(user, config) {
        this.user = user;
        this.config = config;
    }

    pageHandler() {

        var limit = 5;
        var counter = globalScopes.getWordOrder().isFromWordList ?
            (globalScopes.getWordOrder().length - Math.floor(globalScopes.getWordOrder().length / limit) * limit) : 0;
        var offsetCards = globalScopes.getWordOrder().isFromWordList ?
            (Math.floor(globalScopes.getWordOrder().length / limit) * limit) : 0;
        var pages = 0;
        var config = this.config;
        var user = this.user;
        var cardData = {};


        function getData(offset = 0, limit = 2, callBack = null) {
            helper.httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

        function showNextCard(direction = 0) {// !! if order from 'Word-List' then the counter is not reset !!

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

                    if (globalScopes.getWordOrder().isFromWordList) {
                        counter = globalScopes.getWordOrder().length - Math.floor(globalScopes.getWordOrder().length / limit) * limit;
                        globalScopes.getWordOrder().isFromWordList = false;
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

        domUtil.addBubleEventListener(leftButtonEl, ".card-button-icon", "click", globalScopes.getEventListenerState().cardLeftButton, function (e) {
            showNextCard(-1);
        });

        var rightButtonEl = document.querySelector(".card-button-right");

        domUtil.addBubleEventListener(rightButtonEl, ".card-button-icon", "click", globalScopes.getEventListenerState().cardRightButton, function (e) {
            showNextCard(1);
        });


        var leftButtonElMobileVersion = document.getElementById("cardButtonLeftMobileVersion");

        domUtil.addBubleEventListener(leftButtonElMobileVersion, ".card-button-icon", "click", globalScopes.getEventListenerState().cardLeftButtonMobileVersion, function (e) {
            showNextCard(-1);
        });

        var rightButtonElMobileVersion = document.getElementById("cardButtonRightMobileVersion");

        domUtil.addBubleEventListener(rightButtonElMobileVersion, ".card-button-icon", "click", globalScopes.getEventListenerState().cardRightButtonMobileVersion, function (e) {
            showNextCard(1);
        });
        showNextCard(0);
    }
}

export default Cards;
