'use strict';

import Vue from 'vue';
import Notifications from 'vue-notification';
import Multiselect from 'vue-multiselect';
import VueModal from 'vue-js-modal';
import VueTelInput from 'vue-tel-input';
import { Datetime } from 'vue-datetime';
import Paginate from 'vuejs-paginate';
import VueScrollTo from 'vue-scrollto';
import {
    // Directives
    VTooltip,
    VClosePopper,
    // Components
    Dropdown,
    Tooltip,
    Menu
} from 'v-tooltip'

import FullscreenLoader from '@/plugins/fullscreen-loader';

import DefaultLayout from '@/layouts/Default';
import NoSidebarLayout from '@/layouts/NoSidebar';
import HomeLayout from '@/layouts/Home';

import App from './App.vue';
import router from './router';
import store from './store';
import { storeTypes } from '@/constants/index';
import './registerServiceWorker';
import authService from './services/authService';
import utils from './utils/index';
import api from './utils/api';

Vue.config.productionTip = false;

console.log('BASE_URL: ', process.env.BASE_URL);

document.addEventListener("DOMContentLoaded", function () {
    console.log('DOMContentLoaded');
    console.log('process.env: ', process.env);
    console.log('process.env.VUE_APP_ASPNETCORE_ENVIRONMENT: ', process.env.VUE_APP_ASPNETCORE_ENVIRONMENT);
});

//// register globally
Vue.use(Notifications);
Vue.component('multiselect', Multiselect);
Vue.use(VueModal, { componentName: "modal", dialog: true });
Vue.use(VueTelInput);
Vue.component('datetime', Datetime);
Vue.component('paginate', Paginate);
Vue.use(VueScrollTo, {
    container: "body",
    duration: 500,
    easing: "ease",
    offset: -60,
    force: true,
    cancelable: true,
    onStart: false,
    onDone: false,
    onCancel: false,
    x: false,
    y: true
});

// tooltip
Vue.directive('tooltip', VTooltip)
Vue.directive('close-popper', VClosePopper)
Vue.component('VDropdown', Dropdown)
Vue.component('VTooltip', Tooltip)
Vue.component('VMenu', Menu)

// custom plugins
Vue.use(FullscreenLoader);

// layouts
Vue.component('default-layout', DefaultLayout);
Vue.component('no-sidebar-layout', NoSidebarLayout);
Vue.component('home-layout', HomeLayout);

runApp();

function runApp() {
    store.dispatch(storeTypes.CONFIG_LOAD).then(async (config) => {
        let {
            urls,
            clientAuth,
        } = config;

        // init
        authService.init(clientAuth);
        api.init(urls);

        await authService.getUser().then(user => {
            if (user) {
                if (user.profile.is_pre_registration === 'true') {
                    // user logged in using external provider and must complete registration flow
                    console.log(`User logged in using external provider and must complete registration flow.`);
                    router.push({
                        name: 'register',
                        params: {
                        },
                        query: {
                            preRegistrationUserId: user.profile.sub,
                        },
                    });
                } else {
                    console.log(`User logged in and completed registration.`);
                }
            }

            return user;
        });

        if(await authService.isAuthenticated()) {
            // load profile before rendering the app
            await store.dispatch(storeTypes.PROFILE_LOAD, {});
        }

        if (router.history.current.name === 'error') {
            router.push({
                path: '/',
            });
        }

        renderApp();
    }).catch(err => {
        console.error(err);
        store.commit(storeTypes.ERROR_PAGE_DATA_SET, {
            data: {
                title: 'App configuration load error!',
                text: '',
            },
        });
        router.push({
            path: 'error',
        });
        renderApp();
    });
}

function renderApp() {
    // render
    new Vue({
        router,

        // provide the store using the "store" option.
        // this will inject the store instance to all child components.
        store,

        render: h => {
            console.log('Vue render');
            return h(App);
        }
    }).$mount('#app');
}
