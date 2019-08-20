
// polyfills must be included before all other code
import "@babel/polyfill";

import _ from 'lodash';
import moment from 'moment';

// Oidc
import {
    Version,
    Log,
    OidcClient,
    OidcClientSettings,
    WebStorageStateStore,
    InMemoryWebStorage,
    UserManager,
    AccessTokenEvents,
    MetadataService,
    CordovaPopupNavigator,
    CordovaIFrameNavigator,
    CheckSessionIFrame,
    TokenRevocationClient,
    SessionMonitor,
    Global,
    User
} from 'oidc-client';
import Oidc from 'oidc-client';

import HttpUtil from './modules/HttpUtil.js';
let helper = new HttpUtil();

import DomUtil from './modules/DomUtil.js';
let domUtil = new DomUtil();

import globalScopes from './modules/pages/global-scopes.js';


import Dashboard from './modules/pages/dashboard.js';
import Cards from './modules/pages/cards.js';
import WordList from './modules/pages/word-list.js';

import UserUtil from './modules/UserUtil.js';
let userUtil = {};

document.addEventListener("DOMContentLoaded", function (event) {
    helper.httpGet('/config', function (config) {
        start(config);
    });
});


function start(config) {

    var pageHandlers = {};
    userUtil = new UserUtil(config);

    userUtil.getUser(function (user) {

        if (user) {
            userUtil.checkIdentityApi(user);
            initPageHandlers(user, config);
            runApp(user, config);

        } else {
            console.log("User not logged in");
            userUtil.login();
        }
    });


    function initPageHandlers(user, config) {
        pageHandlers['dashboard'] = new Dashboard();
        pageHandlers['cards'] = new Cards(user, config);
        pageHandlers['word-list'] = new WordList(user, config);
    }

    function runApp(user, config) {


        /**
         * Inits app menu, enables menu links
         */
        function initAppMenu() {

            domUtil.addBubleEventListener('body', '[data-route-link]', 'click', globalScopes.getEventListenerState().menuLinks, function (e, actualEl, desiredEl) {
                e.preventDefault();
                e.stopPropagation();

                var targetLinkEl = desiredEl;
                goToRoute(targetLinkEl.dataset.routeLink);
            });
        }

        domUtil.addBubleEventListener('body', '.js-logout-button', 'click', globalScopes.getEventListenerState().logoutButton, function (e, desiredEl) {
            e.stopPropagation();
            userUtil.logout();
        });

        initAppMenu();

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
                var pageHandler = pageHandlers[route];
                if (!pageHandler) {
                    console.error(`Can't find handler for page: ${route}`);
                } else {
                    pageHandler.pageHandler(item);
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

        function checkingServerResponse(user) {

            function getData(offset = 0, limit = 2, callBack = null) {
                helper.httpGet(config.urls.api + '/api/v2/studyitems' + '?' + 'offset=' + offset + '&' + 'limit=' + limit, function (data) {
                    console.log(2, data);
                    if (callBack !== null) {
                        callBack(data);
                    }
                }, user.access_token);
            }

            getData(0, 1, function (response) {
                if (response.data.items.length === 0) {
                    console.log('respone', false);
                    var descriptionIssue = document.querySelector('.js-empty-data');
                    descriptionIssue.classList.replace('hidden', 'active');

                    window.location.hash = "#no-response";
                }
            });
        }//checks server connection

        checkingServerResponse(user);
    }


}
