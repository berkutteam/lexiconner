'use strict';

import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import ErrorView from './views/ErrorView.vue';
import TermsOfUse from './views/TermsOfUse.vue';
import Login from './views/Login.vue';
import Register from './views/Register.vue';
import Logout from './views/Logout.vue';

import UserDictionaryDashboard from './views/UserDictionary/UserDictionaryDashboard.vue';

import WordsDashboard from './views/Words/WordsDashboard.vue';
import WordsBrowse from './views/Words/WordsBrowse.vue';
import WordsLearnFlashCards from './views/Words/WordsLearnFlashCards.vue';
import WordsLearnWordMeaning from './views/Words/WordsLearnWordMeaning.vue';
import WordsLearnMeaningWord from './views/Words/WordsLearnMeaningWord.vue';
import WordsLearnMatchWords from './views/Words/WordsLearnMatchWords.vue';
import WordsLearnBuildWords from './views/Words/WordsLearnBuildWords.vue';
import WordsLearnListenWords from './views/Words/WordsLearnListenWords.vue';

import WordSetsDashboard from './views/WordSets/WordSetsDashboard.vue';

import UserFilmsBrowse from './views/UserFilms/UserFilmsBrowse.vue';

import Dashboard from './views/Dashboard.vue';
import DashboardHome from './views/Dashboard/Home.vue';
import DashboardGateways from './views/Dashboard/Gateways.vue';
import DashboardSensors from './views/Dashboard/Sensors.vue';
import DashboardDepartments from './views/Dashboard/Departments.vue';
import DashboardUsers from './views/Dashboard/Users.vue';
import MyPermissions from './views/MyPermissions.vue';
import ManageCompanyPermissions from './views/ManageCompanyPermissions.vue';
import ManageCompanyUserPermissions from './views/ManageCompanyUserPermissions.vue';
import UserProfile from './views/UserProfile.vue';
import SensorTelemetry from './views/SensorTelemetry.vue';
import SensorManage from './views/SensorManage.vue';
import CompanyCreate from './views/CompanyCreate.vue';
import CompanyManage from './views/CompanyManage.vue';
import CompanyDepartment from './views/CompanyDepartment.vue';
import CompanyUserProfile from './views/CompanyUserProfile.vue';
import MyCompanyInvitations from './views/MyCompanyInvitations.vue';
import { storeTypes } from '@/constants/index';

import store from '@/store';
import utils from '@/utils';

Vue.use(Router)

/**
 * Maps route params and query params to props
 * @param {*} route 
 */
function mapRouteParamsToProps(route) {
    return {
        ...route.params,
        query: {
            ...route.query,
        },
    };
}

/**
 * Waits before processing a route if there are some store actions loading.
 * The aim - to prevent a user from being redirected from protected route due to yet not loaded auth info
 * when page was refreshed.
 * NB: do not guarantee that all the required data will be loaded until wait time end.
 */
async function waitAppInitialization({ to, from, next }) {
    const waitRetries = 5;
    const waitTimeStepFactor = 0.5;
    let waitTimeMs = 500;

    if (store.getters.isAnyLoading) {
        // wait a little bit and then process the route
        for (let i = 0; i < waitRetries; i++) {
            console.log(`Waiting app initialization before processing to route ${to.name}: ${to.path}. Wait time ms: ${waitTimeMs}`);
            await utils.waitAsync(waitTimeMs);

            if (!store.getters.isAnyLoading) {
                break;
            }

            // continue
            waitTimeMs = waitTimeMs + waitTimeMs * waitTimeStepFactor;
        }
    }

    console.log(`App was initialized. Processing to route ${to.name}: ${to.path}.`);
}

function checkAuthenticated({ to, from, next }) {
    if (store.state.auth.isAuthenticated) {
        next();
    } else {
        console.error(`Access denied to route ${to.name}: ${to.path}. Unauthenticated.`);

        Vue.notify({
            group: 'error',
            type: 'error',
            title: 'Access denied!',
            text: 'Looks like you are not logged in, or session expired. Try to relogin.'
        });

        // next(false); // abort the current navigation
        next({
            name: "home", // back to safety route //
            query: {},
        });
    }
}

function checkPermissions({ to, from, next, permisions, scopeId }) {
    if (store.getters.isUserHasPermissions(permisions, scopeId)) {
        next();
    } else {
        console.error(`Access denied to route ${to.name}: ${to.path}. Unauthorized - insufficient permissions.`);

        Vue.notify({
            group: 'error',
            type: 'error',
            title: 'Access denied!',
            text: 'Insufficient permissions!'
        });

        next(false); // abort the current navigation
    }
}

export default new Router({
    routes: [
        {
            path: '/',
            name: 'home',
            component: Home,
            props: true,
            meta: { layout: 'default' },
        },
        {
            path: '/error',
            name: 'error',
            component: ErrorView,
            props: true,
            meta: { layout: 'default' },
        },
        {
            path: '/terms-of-use',
            name: 'terms-of-use',
            component: TermsOfUse,
            props: true,
            meta: { layout: 'no-sidebar' },
        },
        {
            path: '/user-dictionary',
            name: 'user-dictionary',
            component: UserDictionaryDashboard,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/user-dictionary/wordset/:userWordSetId/words',
            name: 'user-dictionary-wordset-words',
            component: WordsBrowse,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words-dashboard',
            name: 'words-dashboard',
            component: WordsDashboard,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/browse',
            name: 'words-browse',
            component: WordsBrowse,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/falshcards',
            name: 'words-learn-falshcards',
            component: WordsLearnFlashCards,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/wordmeaning',
            name: 'words-learn-wordmeaning',
            component: WordsLearnWordMeaning,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/meaningword',
            name: 'words-learn-meaningword',
            component: WordsLearnMeaningWord,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/matchwords',
            name: 'words-learn-matchwords',
            component: WordsLearnMatchWords,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/buildword',
            name: 'words-learn-buildwords',
            component: WordsLearnBuildWords,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/words/learn/listenwords',
            name: 'words-learn-listenwords',
            component: WordsLearnListenWords,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/wordsets',
            name: 'wordsets',
            component: WordSetsDashboard,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/user-films/browse',
            name: 'user-films-browse',
            component: UserFilmsBrowse,
            props: true,
            meta: { layout: 'default' },
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },

        {
            path: '/dashboard',
            name: 'dashboard',
            component: Dashboard,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
            children: [
                {
                    path: '/',
                    name: 'dashboard-home',
                    component: DashboardHome,
                    props: true,
                },
                {
                    path: 'companies/:companyId/gateways',
                    name: 'dashboard-gateways',
                    component: DashboardGateways,
                    props: true,
                },
                {
                    path: 'companies/:companyId/sensors',
                    name: 'dashboard-sensors',
                    component: DashboardSensors,
                    props: true,
                },
                {
                    path: 'companies/:companyId/departments',
                    name: 'dashboard-departments',
                    component: DashboardDepartments,
                    props: true,
                },
                {
                    path: 'companies/:companyId/users',
                    name: 'dashboard-users',
                    component: DashboardUsers,
                    props: true,
                    beforeEnter: (to, from, next) => {
                        checkPermissions({ to, from, next, permisions: ['CompanyUserRead'], scopeId: to.params.companyId });
                    },
                },
            ],
        },
        {
            path: '/permissions/my',
            name: 'my-permissions',
            component: MyPermissions,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/permissions/:companyId/manage',
            name: 'manage-company-permissions',
            component: ManageCompanyPermissions,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['ScopedPermissionManage'], scopeId: to.params.companyId });
            },
        },
        {
            path: '/permissions/:companyId/user/:userId/manage',
            name: 'manage-company-user-permissions',
            component: ManageCompanyUserPermissions,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['ScopedPermissionManage'], scopeId: to.params.companyId });
            },
        },
        {
            path: '/companies/invitations',
            name: 'my-company-invitations',
            component: MyCompanyInvitations,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/about',
            name: 'about',
            // route level code-splitting
            // this generates a separate chunk (about.[hash].js) for this route
            // which is lazy-loaded when the route is visited.
            component: () => import(/* webpackChunkName: "about" */ './views/About.vue'),
            props: true,
        },
        {
            path: '/login',
            name: 'login',
            component: Login,
            props: true,
        },
        {
            path: '/register',  //'?invitationId=&preRegistrationUserId='
            name: 'register',
            component: Register,
            props: mapRouteParamsToProps,
        },
        {
            path: '/logout',
            name: 'logout',
            component: Logout,
            props: true,
        },
        {
            path: '/profile',
            name: 'user-profile',
            component: UserProfile,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/sensors/:sensorId/telemetries',
            name: 'sensor-telemetry',
            component: SensorTelemetry,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/companies/:companyId/sensors/:sensorId/manage',
            name: 'sensor-manage',
            component: SensorManage,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
            },
        },
        {
            path: '/companies/create',
            name: 'company-create',
            component: CompanyCreate,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['CompanyCreate'], scopeId: null });
            },
        },
        {
            path: '/companies/:companyId/manage',
            name: 'company-manage',
            component: CompanyManage,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['CompanyUpdate'], scopeId: to.params.companyId });
            },
        },
        {
            path: '/companies/:companyId/departments/:departmentId',
            name: 'company-department',
            component: CompanyDepartment,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['CompanyDepartmentManage'], scopeId: to.params.companyId });
            },
        },
        {
            path: '/companies/:companyId/users/:userId/profile',
            name: 'company-user-profile',
            component: CompanyUserProfile,
            props: true,
            beforeEnter: async (to, from, next) => {
                await waitAppInitialization({ to, from, next });
                checkAuthenticated({ to, from, next });
                checkPermissions({ to, from, next, permisions: ['CompanyUserRead'], scopeId: to.params.companyId });
            },
        },
    ]
});



