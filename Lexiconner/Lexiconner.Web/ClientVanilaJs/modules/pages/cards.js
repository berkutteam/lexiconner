
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
        var isShowExampleText = true; //used in study mode


        function getData(offset = 0, limit = 2, callBack = null) {
            helper.httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

        function showNextCard(direction = 0, isShowExampleText) {// !! if order from 'Word-List' then the counter is not reset !!

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

                    showDataOnCard(cardData.items[counter], isShowExampleText);

                });

            } else {
                showDataOnCard(cardData.items[counter], isShowExampleText);
            }

        }

        function showDataOnCard(card, isShowExampleText) {// second parameter used in study mode
            var cardEl = document.getElementById("mainBlockCard");
            var cardTitleEl = cardEl.querySelector("#cardTitle");
            var cardDescEl = cardEl.querySelector("#cardDesc");
            var cardExampleTextEl = cardEl.querySelector("#cardExampleText");
            var cardExampleImageEl = cardEl.querySelector("#cardExampleImage");


            cardTitleEl.innerText = card.title;
            cardDescEl.innerText = card.description;
            if (isShowExampleText) {
                if (!card.exampleText) {
                    cardExampleTextEl.classList.add('hidden');
                } else {
                    cardExampleTextEl.innerText = card.exampleText;
                    cardExampleTextEl.classList.remove('hidden');
                }
            }
            if (!card.image) {

                cardExampleImageEl.src = "images\\default.png";
            } else {
                cardExampleImageEl.src = card.image.url;
            }
        }

        var leftButtonEl = document.querySelector(".card-button-left");

        domUtil.addBubleEventListener(leftButtonEl, ".card-button-icon", "click", globalScopes.getEventListenerState().cardLeftButton, function (e) {
            showNextCard(-1, isShowExampleText);
        });

        var rightButtonEl = document.querySelector(".card-button-right");

        domUtil.addBubleEventListener(rightButtonEl, ".card-button-icon", "click", globalScopes.getEventListenerState().cardRightButton, function (e) {
            showNextCard(1, isShowExampleText);
        });


        var leftButtonElMobileVersion = document.getElementById("cardButtonLeftMobileVersion");

        domUtil.addBubleEventListener(leftButtonElMobileVersion, ".card-button-icon", "click", globalScopes.getEventListenerState().cardLeftButtonMobileVersion, function (e) {
            showNextCard(-1, isShowExampleText);
        });

        var rightButtonElMobileVersion = document.getElementById("cardButtonRightMobileVersion");

        domUtil.addBubleEventListener(rightButtonElMobileVersion, ".card-button-icon", "click", globalScopes.getEventListenerState().cardRightButtonMobileVersion, function (e) {
            showNextCard(1, isShowExampleText);
        });
        showNextCard(0, isShowExampleText);


        domUtil.addBubleEventListener('.card-change-mode-button-container', '.study-mode', 'click', globalScopes.getEventListenerState().cardChangeModeStudyButton, function (e, actualEl, desiredEl) {
            e.stopPropagation();


            var reviewModeButtonEl = document.querySelector('.review-mode');
            var studyModeButtonEl = document.querySelector('.study-mode');

            studyModeButtonEl.classList.remove('active');
            studyModeButtonEl.classList.add('hidden');
            reviewModeButtonEl.classList.remove('hidden');
            reviewModeButtonEl.classList.add('active');

            isShowExampleText = false;
            changeMode();

        });// event listener on STUDY MODE button

        domUtil.addBubleEventListener('.card-change-mode-button-container', '.review-mode', 'click', globalScopes.getEventListenerState().cardChangeModeReviewButton, function (e, actualEl, desiredEl) {
            e.stopPropagation();

            var reviewModeButtonEl = document.querySelector('.review-mode');
            var studyModeButtonEl = document.querySelector('.study-mode');

            reviewModeButtonEl.classList.remove('active');
            reviewModeButtonEl.classList.add('hidden');
            studyModeButtonEl.classList.remove('hidden');
            studyModeButtonEl.classList.add('active');

            isShowExampleText = true;
            setDefaultMode();
        });// event listener on REVIEW MODE button


        var cardEl = document.getElementById("mainBlockCard");
        var cardBlockEl = cardEl.querySelector('.card-block');
        var cardBlockContainerEl = cardEl.querySelector('.card-block-container');
        var studyModeButtonBlockEl = cardEl.querySelector('.study-mode-control-block');
        var cardDescEl = cardBlockContainerEl.querySelector("#cardDesc");
        var cardExampleTextEl = cardBlockContainerEl.querySelector("#cardExampleText");
        var cardExampleImageEl = cardBlockContainerEl.querySelector("#cardExampleImage");
        var cardButtonContainer  = cardEl.querySelector('.card-button-container');

        function changeMode() {

            hideCardItems();

            studyModeButtonBlockEl.classList.remove('hidden');
            studyModeButtonBlockEl.classList.add('active');

            leftButtonEl.classList.remove('active');
            leftButtonEl.classList.add('hidden');

            rightButtonEl.classList.remove('active');
            rightButtonEl.classList.add('hidden');

            cardButtonContainer.classList.remove('active');
            cardButtonContainer.classList.add('hidden');
        }

        function setDefaultMode() {

            showAllCardItems();


            studyModeButtonBlockEl.classList.remove('active');
            studyModeButtonBlockEl.classList.add('hidden');

            leftButtonEl.classList.remove('hidden');
            leftButtonEl.classList.add('active');

            rightButtonEl.classList.remove('hidden');
            rightButtonEl.classList.add('active');

            cardButtonContainer.classList.remove('hidden');
            cardButtonContainer.classList.add('active');
        }

        var studyButtonUnknowEl = document.querySelector("#studyButtonUnknow");
        var studyButtonKnowEl = document.querySelector("#studyButtonKnow");
        var studyButtonNextItemEl = document.querySelector("#studyButtonNextItem");

        domUtil.addBubleEventListener(studyButtonNextItemEl, "#studyButtonNextItem", 'click', globalScopes.getEventListenerState().studyButtonNextItem, function (e, actualEl, desiredEl) {
            e.stopPropagation();
            hideCardItems();
            removeBorder();
            showNextCard(1, isShowExampleText);
        });


        domUtil.addBubleEventListener(studyButtonUnknowEl, "#studyButtonUnknow", 'click', globalScopes.getEventListenerState().studyButtonUnknow, function (e, actualEl, desiredEl) {
            e.stopPropagation();
            showAllCardItems();
            cardBlockEl.classList.add('border-wrong');
        });

        domUtil.addBubleEventListener(studyButtonKnowEl, "#studyButtonKnow", 'click', globalScopes.getEventListenerState().studyButtonKnow, function (e, actualEl, desiredEl) {
            e.stopPropagation();
            showAllCardItems();
            cardBlockEl.classList.add('border-correct');
        });

        function showAllCardItems() {
            cardDescEl.classList.remove('hidden');
            cardDescEl.classList.add('active');

            cardExampleTextEl.classList.remove('hidden');
            cardExampleTextEl.classList.add('active');

            cardExampleImageEl.classList.remove('blur');
        }

        function hideCardItems() {
            cardDescEl.classList.remove('active');
            cardDescEl.classList.add('hidden');

            cardExampleTextEl.classList.remove('active');
            cardExampleTextEl.classList.add('hidden');

            cardExampleImageEl.classList.add('blur');
        }

        function removeBorder() {
            cardBlockEl.classList.remove('border-wrong');
            cardBlockEl.classList.remove('border-correct');
        }
    }
}

export default Cards;
