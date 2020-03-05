'use strict';

import Vue from 'vue';
import Vuex from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import api from './utils/api';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        // one loading object for all resources
        loading: {
            // <storeType>: bool
        },

        // app config from server
        config: {
            clientAuth: null,
            urls: null,
        },

        // contains enums elements that is loaded from server
        // E.g. {'measurementUnit': ['none','celsius','fahrenheit'],}
        enums: null,

        termsOfUse: null,
        countries: null,

        // list of language DTO
        languages: null,
        
        timeZones: null,

        auth: {
            isAuthenticated: false,
            isRegistrationCompleted: false,
            user: null,
            registrationInfo: null,
            preRegistrationUser: null,

            myPermissions: null,

            scopedPermissions: {
                permissions: [],
                roles: [],
            },
            userScopedPermissions: {
                // <companyId>: {
                //    <userId>: object
                //}
            },
        },
        userAccount: null,
        userInfo: null,

        // currently viewed company
        company: null,
        companyDepartments: {
            // <companyId>: array<object>
        },
        // currently viewed department
        companyDepartment: {
            // <companyId>: {
            // <departmentId>: object
            // }
        },
        companyUsers: {
            // <companyId>: paginationResult (store only current page)
        },
        departmentUsers: {
            // <departmentId>: paginationResult (store only current page)
        },
        // currently viewed user
        companyUser: {
            // <companyId>: {
            // <userId>: object
            // }
        },
        companyUserInvitation: {
            // <invitationId>: object
        },
        myCompanyInvitations: null,

        // paginationResult (store only current page)
        // {items: [], pagination: {}}
        studyItemsPaginationResult: null,

        nav: {
            isVisible: true,
        },
        errorPage: {
            title: null,
            text: null,
        }
    },
    getters: {
        /**
         * Checks if there are any resources are loading
         */
        isAnyLoading(state, getters) {
            let isAnyLoading = Object.keys(state.loading).reduce((accum, key, i) => {
                return accum || state.loading[key];
            }, false);
            // console.log('isAnyLoading', isAnyLoading)
            return isAnyLoading;
        },

        // NB: getters that return methods will run each time you call them, and the result is not cached.
        userId(state, getters) {
            if (!state.auth.user) {
                return null;
            }
            return state.auth.user.profile.sub;
        },
        tokens(state, getters) {
            if (!state.auth.user) {
                return {};
            }
            let { access_token, id_token, refresh_token } = state.auth.user;
            return { access_token, id_token, refresh_token };
        },
    },
    mutations: {
        [storeTypes.LOADING_SET](state, payload) {
            let { target, loading } = payload;
            state.loading = {
                ...loading,
                [target]: loading,
            };
        },

      
        //#region Config

        [storeTypes.CONFIG_SET](state, payload) {
            let { config } = payload;
            state.config = config;
        },

        //#endregion


        //#region Reference info

        // [storeTypes.ENUMS_SET](state, payload) {
        //     let { data } = payload;
        //     state.enums = data;
        // },

        [storeTypes.LANGUAGES_SET](state, payload) {
            let { data } = payload;
            state.languages = data;
        },


        //#endregion


        //#region Auth

        [storeTypes.AUTH_USER_SET](state, payload) {
            let { user } = payload;
            state.auth.isAuthenticated = !!user;
            state.auth.user = { ...user };
        },
        [storeTypes.AUTH_USER_RESET](state) {
            state.auth.isAuthenticated = false;
            state.auth.user = null;
        },
        [storeTypes.AUTH_REGISTRATION_INFO_SET](state, payload) {
            let { registrationInfo } = payload;
            state.auth.registrationInfo = registrationInfo;
        },
        [storeTypes.AUTH_PREREGISTRATION_USER_SET](state, payload) {
            let { data } = payload;
            state.auth.preRegistrationUser = data;
        },
        [storeTypes.AUTH_MY_PERMISSIONS_SET](state, payload) {
            let { myPermissions } = payload;
            state.auth.myPermissions = myPermissions;
        },
        [storeTypes.AUTH_SCOPED_PERMISSIONS_SET](state, payload) {
            let { data } = payload;
            state.auth.scopedPermissions = data;
        },
        [storeTypes.AUTH_USER_SCOPED_PERMISSIONS_SET](state, payload) {
            let { scopeId, userId, data } = payload;
            let userScopedPermissions = { ...(state.auth.userScopedPermissions || {}) };
            state.auth.userScopedPermissions = {
                ...userScopedPermissions,
                [scopeId]: {
                    ...(userScopedPermissions[scopeId] || {}),
                    [userId]: data,
                },
            };
        },

        //#endregion


        //#region User account

        [storeTypes.AUTH_USER_ACCOUNT_SET](state, payload) {
            let { data } = payload;
            state.userAccount = data;
        },

        //#endregion


        //#region Study items

        [storeTypes.STUDY_ITEMS_SET](state, payload) {
            let { data } = payload;
            state.studyItemsPaginationResult = data;
        },
        [storeTypes.STUDY_ITEMS_CREATE_SET](state, payload) {
            let { data } = payload;
            if(state.studyItemsPaginationResult !== null) {
                state.studyItemsPaginationResult.items.unshift(data);
            }
        },

        //#endregion


       //#region Nav

        [storeTypes.NAV_VISIBILITY_SET](state, payload) {
            let { isVisible } = payload;
            state.nav.isVisible = isVisible;
        },

        //#endregion


        //#region Error page

        [storeTypes.ERROR_PAGE_DATA_SET](state, payload) {
            let { data } = payload;
            state.errorPage = { ...state.errorPage, ...data };
        },
        [storeTypes.ERROR_PAGE_DATA_RESET](state) {
            state.errorPage = {};
        },

        //#endregion
    },
    actions: {
        //#region Config

        [storeTypes.CONFIG_LOAD](context) {
            let { commit, dispatch, getters, state } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CONFIG_LOAD,
                loading: true,
            });
            return api.configApi().config().then(({data, ok}) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CONFIG_LOAD,
                    loading: false,
                });
                commit(storeTypes.CONFIG_SET, {
                    config: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CONFIG_LOAD,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion


        //#region Reference info

        // [storeTypes.ENUMS_LOAD](context) {
        //     let { commit, dispatch, getters } = context;
        //     commit(storeTypes.LOADING_SET, {
        //         target: storeTypes.ENUMS_LOAD,
        //         loading: true,
        //     });
        //     return api.accountManager().getEnums().then(({ data, ok }) => {
        //         commit(storeTypes.LOADING_SET, {
        //             target: storeTypes.ENUMS_LOAD,
        //             loading: false,
        //         });
        //         commit(storeTypes.ENUMS_SET, {
        //             data: data
        //         });
        //         return data;
        //     }).catch(err => {
        //         commit(storeTypes.LOADING_SET, {
        //             target: storeTypes.ENUMS_LOAD,
        //             loading: false,
        //         });
        //         throw err;
        //     });
        // },

        [storeTypes.LANGUAGES_LOAD](context) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.LANGUAGES_LOAD,
                loading: true,
            });
            return api.webApi().getLanguages().then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.LANGUAGES_LOAD,
                    loading: false,
                });
                commit(storeTypes.LANGUAGES_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.LANGUAGES_LOAD,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion


        //#region Auth

        //#endregion

        //#region UserInfo

        [storeTypes.USER_INFO_LOAD](context, { }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_INFO_LOAD,
                loading: true,
            });
            return api.accountManager().getUserInfo().then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_LOAD,
                    loading: false,
                });
                commit(storeTypes.USER_INFO_SET, {
                    userInfo: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_LOAD,
                    loading: false,
                });
                throw err;
            });
        },

        [storeTypes.USER_INFO_UPDATE](context, { data }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_INFO_UPDATE,
                loading: true,
            });
            return api.accountManager().updateUserInfo(data).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_UPDATE,
                    loading: false,
                });
                commit(storeTypes.USER_INFO_SET, {
                    userInfo: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_UPDATE,
                    loading: false,
                });
                throw err;
            });
        },

        [storeTypes.USER_INFO_NOTIFICATIONS_UPDATE](context, { data }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_INFO_NOTIFICATIONS_UPDATE,
                loading: true,
            });
            return api.accountManager().updateUserInfoNotifications(data).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_NOTIFICATIONS_UPDATE,
                    loading: false,
                });
                commit(storeTypes.USER_INFO_SET, {
                    userInfo: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_NOTIFICATIONS_UPDATE,
                    loading: false,
                });
                throw err;
            });
        },


        [storeTypes.USER_INFO_TIMEZONE_SET](context, { timeZoneId }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_INFO_TIMEZONE_SET,
                loading: true,
            });
            return api.accountManager().setUserInfoTimeZone({ timeZoneId }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_TIMEZONE_SET,
                    loading: false,
                });
                commit(storeTypes.USER_INFO_SET, {
                    userInfo: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_TIMEZONE_SET,
                    loading: false,
                });
                throw err;
            });
        },

        [storeTypes.USER_INFO_CURRENT_COMPANY_SET](context, { companyId }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_INFO_CURRENT_COMPANY_SET,
                loading: true,
            });
            return api.accountManager().setCurrentUserCompany({ companyId }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_CURRENT_COMPANY_SET,
                    loading: false,
                });
                commit(storeTypes.USER_INFO_SET, {
                    userInfo: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_INFO_CURRENT_COMPANY_SET,
                    loading: false,
                });
                throw err;
            });
        },


        //#endregion


        //#region Study items

        [storeTypes.STUDY_ITEMS_LOAD](context, params) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEMS_LOAD,
                loading: true,
            });
            return api.webApi().getStudyItems(params).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEMS_LOAD,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEMS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEMS_LOAD,
                    loading: false,
                });
                throw err;
            });
        },

        [storeTypes.STUDY_ITEM_CREATE](context, {data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_CREATE,
                loading: true,
            });
            return api.webApi().createStudyItem({data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_CREATE,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEMS_CREATE_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_CREATE,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion
    }
});
