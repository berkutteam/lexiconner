import DomUtil from '../DomUtil';
let domUtil = new DomUtil();

import globalScopes from './global-scopes.js';

import HttpUtil from '../HttpUtil.js';
let helper = new HttpUtil();

class FilmList {
    constructor(user, config) {
        this.user = user;
        this.config = config;
    }

    pageHandler(pageEl) {
        var user = this.user;
        var config = this.config;


        var itemListContainerEl = pageEl.querySelector('.js-film-item-list-container');
        var itemListEl = pageEl.querySelector('.js-film-item-list');
        var listItemTemplateEl = itemListContainerEl.querySelector('.js-film-list-item-template');

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
            var titleEl = clone.querySelector('.film-item-title');
            var releaseEl = clone.querySelector('.film-item-release');

            var watchedRatingContainerEl = clone.querySelector('.film-item-watched-rating-container');
            var watchedAtContainerEl = clone.querySelector('.film-item-watched-container');
            var ratingContainerEl = clone.querySelector('.film-item-rating-container');
            var genreContainerEl = clone.querySelector('.film-item-genre-container');
            var commentContainerEl = clone.querySelector('.film-item-comment');

            var watchedEl = clone.querySelector('.film-item-watched-text');
            var ratingEl = clone.querySelector('.film-item-rating-text');
            var genreEl = clone.querySelector('.film-item-genre-text');
            var commentEl = clone.querySelector('.film-item-comment');



            // begin edit picture elements (delete,put)
            var containerEditPictureEl = clone.querySelector('.film-container-edit-icon');
            var deletePictureEl = containerEditPictureEl.querySelector('.film-list-item-icon-delete');
            var putPictureEl = containerEditPictureEl.querySelector('.film-list-item-icon-put');
            // end edit picture elements (delete,put)

            titleEl.innerText = item.title;
            releaseEl.innerText = item.releasedAt ? item.releasedAt : releaseEl.classList.add('hidden');
            watchedEl.innerText = item.watchedAt ? item.watchedAt : watchedAtContainerEl.classList.add('hidden');
            ratingEl.innerText = item.rating ? item.rating : ratingContainerEl.classList.add('hidden');
            commentEl.innerText = item.comment ? item.comment : commentContainerEl.classList.add('hidden');

            if (!item.genres) {
                genreContainerEl.classList.remove('film-item-genre-container');
                genreContainerEl.classList.add('hidden');
            } else {
                genreEl.innerText = item.genres;
            }

            if (!item.watchedAt && !item.rating) {
                watchedRatingContainerEl.classList.remove('film-item-watched-rating-container');
                watchedRatingContainerEl.classList.add('hidden');
            }


            deletePictureEl.setAttribute('position-in-list', i);
            putPictureEl.setAttribute('position-in-list', i);

            clone.setAttribute('position-in-list', i);
            clone.classList.remove('film-list-item--hidden');
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

        function getTestData(countOfElmenets, testOffset = 0) {
            var arrData = [];
            for (var i = 0 + testOffset; i < countOfElmenets + testOffset; i++) {
                if (i === 0) {
                    arrData.push({
                        title: "title asdaaaa aaaa aaaaa aaaaa aaaa " + i,
                        comment: " asda asd a dasd asfnasdf sad  " + i,
                        rating: 8.5,
                        watchedAt: "12.04.2018",
                        releasedAt: 2017,
                        genres: "genre" + i + ", genre" + (i + 1)
                    });
                } else if (i === 1) {
                    arrData.push({
                        title: "title asdaaaa aaaa aaaaa aaaaa aaaa " + i,
                        comment: " asda asd adasd asfnasdf sad  " + i,
                        rating: 8.5,
                        watchedAt: null,
                        releasedAt: null,
                        genres: "genre" + i + ", genre" + (i + 1)
                    });
                } else if (i === 2) {
                    arrData.push({
                        title: "title  " + i,
                        comment: " asda asd adasd asfnasdf sad  " + i,
                        rating: null,
                        watchedAt: "12.04.2018",
                        releasedAt: 2017,
                        genres: "genre" + i + ", genre" + (i + 1)
                    });
                } else if (i === 3) {
                    arrData.push({
                        title: "title  " + i,
                        comment: " asda asd adasd asfnasdf sad  " + i,
                        rating: 8.5,
                        watchedAt: "12.04.2018",
                        releasedAt: 2017,
                        genres: null
                    });
                } else if (i === 4) {
                    arrData.push({
                        title: "title  " + i,
                        comment: null,
                        rating: 8.5,
                        watchedAt: "12.04.2018",
                        releasedAt: 2017,
                        genres: "genre" + i + ", genre" + (i + 1)
                    });
                } else if (i === 5) {
                    arrData.push({
                        title: "title  " + i,
                        comment: null,
                        rating: 8.5,
                        watchedAt: "12.04.2018",
                        releasedAt: 2017,
                        genres: null
                    });
                } else if (i === 6) {
                    arrData.push({
                        title: "title  " + i,
                        comment: null,
                        rating: null,
                        watchedAt: null,
                        releasedAt: 2017,
                        genres: null
                    });
                } else if (i === 6) {
                    arrData.push({
                        title: "title  " + i,
                        comment: null,
                        rating: null,
                        watchedAt: null,
                        releasedAt: null,
                        genres: null
                    });
                } else {
                    arrData.push({
                        title: "title ",
                        comment: " Comment ",
                        rating: 8.5,
                        watchedAt: "02.04.2010",
                        releasedAt: 2009,
                        genres: "genre" + i
                    });

                }
            }
            //console.log('---Test data----',arrData);

            return arrData;
        }

        function showPage(page, limit) {

            totalCount = 12;
            showPageData(getTestData(totalCount));

            // getPageData(page, limit, function (response) {
            //     totalCount = response.data.totalCount;
            //     pageData = response.data;

            //     showPageData(response.data.items);
            // });
        }

        function setNumberPage(page) {
            currentButtonEl.innerText = page + 1;
        }

        var firstButtonEl = itemListContainerEl.querySelector('.js-film-first-page-button');
        var prevButtonEl = itemListContainerEl.querySelector('.js-film-prev-page-button');
        var currentButtonEl = itemListContainerEl.querySelector('.js-film-current-page-button');
        var nextButtonEl = itemListContainerEl.querySelector('.js-film-next-page-button');
        var lastButtonEl = itemListContainerEl.querySelector('.js-film-last-page-button');


        domUtil.addBubleEventListener(firstButtonEl, ".film-button-icon", "click", globalScopes.getEventListenerState().FilmItemListFirstButton, function (e) {
            page = 0;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(prevButtonEl, ".film-button-icon", "click", globalScopes.getEventListenerState().FilmItemListPrevButton, function (e) {
            page = page <= 0 ? 0 : page - 1;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(nextButtonEl, ".film-button-icon", "click", globalScopes.getEventListenerState().FilmItemListNextButton, function (e) {
            page = (page < calcPagesCount() - 1) ? page + 1 : (calcPagesCount() - 1);
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(lastButtonEl, ".film-button-icon", "click", globalScopes.getEventListenerState().FilmItemListLastButton, function (e) {
            page = calcPagesCount() - 1;
            setNumberPage(page)
            showPage(page, limit);
        });

        domUtil.addBubleEventListener(itemListContainerEl, '.js-film-item-text', 'click', globalScopes.getEventListenerState().FilmItemListButtonFromListToCard, function (e, desiredEl) {
            e.stopPropagation();

            globalScopes.getWordOrder().length = (page * limit) + Number(desiredEl.getAttribute('position-in-list'));
            globalScopes.getWordOrder().isFromWordList = true;

            window.location.hash = '#cards';

        });

        domUtil.addBubleEventListener(itemListContainerEl, '.film-list-item-icon-delete', 'click', globalScopes.getEventListenerState().FilmItemListDeleteButton, function (e, actualEl, desiredEl) {
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

        domUtil.addBubleEventListener(itemListContainerEl, '.film-list-item-icon-put', 'click', globalScopes.getEventListenerState().FilmItemListPutButton, function (e, actualEl, desiredEl) {
            e.stopPropagation();

            var numberOfItem1 = Number(desiredEl.getAttribute('position-in-list'));
            var formPutButtonEl = document.querySelector('.js-film-put-button');

            formButtonEl.forEach(function (item) {
                item.classList.remove('active');
                item.classList.add('hidden');
            });

            formPutButtonEl.classList.replace('hidden', 'active');
            modalWindowForm.showModal();

            defaultFullfieldForm(pageData.items[numberOfItem1]);

            domUtil.addBubleEventListener(formPutButtonEl, '.js-film-put-button', 'click', globalScopes.getEventListenerState().formPutButton, function (e) {

                idItemPutUrl = `${config.urls.api}/api/v2/StudyItems/${pageData.items[numberOfItem1].id}`;

                if (makeRequestBody(formDataSend, pageData.items[numberOfItem1])) {

                    updateData(idItemPutUrl, formDataSend, user.access_token);
                    modalWindowForm.close();

                    var listItem = itemListEl.querySelectorAll('.js-film-list-item-template');
                    var listItemTextSpan = listItem[numberOfItem1].querySelector('.film-list-item-text-span');

                    listItemTextSpan.innerText = formDataSend.title;

                }
            });

        });// event listener on PUT button


        domUtil.addBubleEventListener(itemListContainerEl, '.film-item-list-pager__add-button', 'click', globalScopes.getEventListenerState().FilmItemListAddButton, function (e, actualEl, desiredEl) {
            e.stopPropagation();

            var formAddButtonEl = document.querySelector('.js-film-add-button');

            formButtonEl.forEach(function (item) {
                item.classList.remove('active');
                item.classList.add('hidden');
            });

            formAddButtonEl.classList.replace('hidden', 'active');
            modalWindowForm.showModal();

            domUtil.addBubleEventListener(formAddButtonEl, ".js-film-add-button", "click", globalScopes.getEventListenerState().formAddButton, function (e, desiredEl) {

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

export default FilmList; 