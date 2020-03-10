
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

import AppRouter from './modules/AppRouter.js';
let appRouter = {};

import UserUtil from './modules/UserUtil.js';
  export let userUtil = {};

document.addEventListener("DOMContentLoaded", function (event) {
    helper.httpGet('/config', function (config) {
        start(config);
    });
});

function start(config) {

    userUtil = new UserUtil(config);

    userUtil.getUser(function (user) {

        if (user) {
            userUtil.checkIdentityApi(user);
            console.log('User:', user);
            appRouter = new AppRouter(user, config);
            runApp(user, config);

        } else {
            console.log("User not logged in");
            userUtil.login();
        }
    });


    function runApp(user, config) {


        /**
         * Inits app menu, enables menu links
         */
        function initAppMenu() {

            domUtil.addBubleEventListener('body', '[data-route-link]', 'click', globalScopes.getEventListenerState().menuLinks, function (e, actualEl, desiredEl) {
                e.preventDefault();
                e.stopPropagation();

                var targetLinkEl = desiredEl;
                appRouter.goToRoute(targetLinkEl.dataset.routeLink);
            });
        }

        domUtil.addBubleEventListener('body', '.js-logout-button', 'click', globalScopes.getEventListenerState().logoutButton, function (e, desiredEl) {
            e.stopPropagation();
            userUtil.logout();
        });

        initAppMenu();

        
        // listen hash changes
        window.addEventListener('hashchange', function (e) {
            var newHash = window.location.hash;
            appRouter.processRoute(newHash);
        }, false);

        // run route that is already in hash
        if (!!window.location.hash) {
            appRouter.processRoute(window.location.hash);
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

                    appRouter.goToRoute("#no-response");
                }
            });
        }//checks server connection

        checkingServerResponse(user);
    }


}
