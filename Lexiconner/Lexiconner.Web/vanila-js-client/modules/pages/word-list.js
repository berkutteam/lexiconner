
import DomUtil from '../DomUtil';
let domUtil = new DomUtil();

import globalScopes from './global-scopes.js';

import HttpUtil from '../HttpUtil.js';
let helper = new HttpUtil();


class WordList {

    constructor(user, config) {
        this.user = user;
        this.config = config;
    }

    pageHandler(pageEl) {

        var user = this.user;
        var config = this.config;
        

        var itemListContainerEl = pageEl.querySelector('.js-item-list-container');
        var itemListEl = pageEl.querySelector('.js-item-list');
        var listItemTemplateEl = itemListContainerEl.querySelector('.js-list-item-template');

        function getData(offset = 0, limit = 2, callBack = null) {
            helper.httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                console.log(2, data);
                if (callBack !== null) {
                    callBack(data);
                }
            }, user.access_token);
        }

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


        domUtil.addBubleEventListener(firstButtonEl, ".button-icon", "click", globalScopes.getEventListenerState().itemListFirstButton, function (e) {
            page = 0;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(prevButtonEl, ".button-icon", "click", globalScopes.getEventListenerState().itemListPrevButton, function (e) {
            page = page <= 0 ? 0 : page - 1;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(nextButtonEl, ".button-icon", "click", globalScopes.getEventListenerState().itemListNextButton, function (e) {
            page = (page < calcPagesCount() - 1) ? page + 1 : (calcPagesCount() - 1);
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(lastButtonEl, ".button-icon", "click", globalScopes.getEventListenerState().itemListLastButton, function (e) {
            page = calcPagesCount() - 1;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(itemListContainerEl, '.js-item-text', 'click', globalScopes.getEventListenerState().itemListButtonFromListToCard, function (e, desiredEl) {
            e.stopPropagation();

            globalScopes.getWordOrder().length = (page * limit) + Number(desiredEl.getAttribute('position-in-list'));
            globalScopes.getWordOrder().isFromWordList = true;

            window.location.hash = '#cards';

        });

        domUtil.addBubleEventListener(itemListContainerEl, '.list-item-icon-delete', 'click', globalScopes.getEventListenerState().itemListDeleteButton, function (e, actualEl, desiredEl) {
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

        domUtil.addBubleEventListener(itemListContainerEl, '.list-item-icon-put', 'click', globalScopes.getEventListenerState().itemListPutButton, function (e, actualEl, desiredEl) {
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

            domUtil.addBubleEventListener(formPutButtonEl, '.js-put-button', 'click', globalScopes.getEventListenerState().formPutButton, function (e) {

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


        domUtil.addBubleEventListener(itemListContainerEl, '.item-list-pager__add-button', 'click', globalScopes.getEventListenerState().itemListAddButton, function (e, actualEl, desiredEl) {
            e.stopPropagation();

            var formAddButtonEl = document.querySelector('.js-add-button');

            formButtonEl.forEach(function (item) {
                item.classList.remove('active');
                item.classList.add('hidden');
            });

            formAddButtonEl.classList.replace('hidden', 'active');
            modalWindowForm.showModal();

            domUtil.addBubleEventListener(formAddButtonEl, ".js-add-button", "click", globalScopes.getEventListenerState().formAddButton, function (e, desiredEl) {

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
}

export default WordList;