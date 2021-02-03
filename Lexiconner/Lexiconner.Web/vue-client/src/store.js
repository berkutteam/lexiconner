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

        studyItemsRequestParamsDefault: {
            search: null,
            isFavourite: null,
        },
        studyItemsRequestParams: {
            search: null,
            isFavourite: null,
        },

        // paginationResult (store only current page)
        // {items: [], pagination: {}}
        studyItemsPaginationResult: null, // object

        studyItem: null, // object

        trainingStats: null, // object
        trainingFlashcards: null, // object
        trainingWordMeaning: null, // object

        customCollectionsResult: null, // object
        currentCustomCollection: null, // object

        userFilmsRequestParamsDefault: {
            search: null,
        },
        userFilmsRequestParams: {
            search: null,
        },

        // paginationResult (store only current page)
        // {items: [], pagination: {}}
        userFilmsPaginationResult: null, // object

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
        currentCustomCollectionId(state, getters) {
            if (!state.currentCustomCollection) {
                return null;
            }
            return state.currentCustomCollection.id;
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

        [storeTypes.STUDY_ITEMS_REQUEST_PARAMS_SET](state, payload) {
            let params= payload;
            state.studyItemsRequestParams = {
                ...state.studyItemsRequestParams,
                ...params,
            };
        },
        [storeTypes.STUDY_ITEMS_REQUEST_PARAMS_RESET](state, payload) {
            state.studyItemsRequestParams = {
                ...state.studyItemsRequestParamsDefault,
            };
        },
        [storeTypes.STUDY_ITEMS_LOAD_SET](state, payload) {
            let { data } = payload;
            state.studyItemsPaginationResult = data;
        },
        [storeTypes.STUDY_ITEM_LOAD_SET](state, payload) {
            let { studyItem } = payload;
            state.studyItem = studyItem;
        },
        [storeTypes.STUDY_ITEM_CREATE_SET](state, payload) {
            let { data } = payload;
            if(state.studyItemsPaginationResult !== null) {
                state.studyItemsPaginationResult.items.unshift({...data});
            }
        },
        [storeTypes.STUDY_ITEM_UPDATE_SET](state, payload) {
            let { data } = payload;
            if(state.studyItemsPaginationResult !== null) {
                state.studyItemsPaginationResult.items = state.studyItemsPaginationResult.items.map(x => {
                    if(x.id === data.id) {
                        return {...data};
                    }
                    return x;
                });
            }
        },
        [storeTypes.STUDY_ITEM_DELETE_SET](state, payload) {
            let { studyItemId } = payload;
            if(state.studyItemsPaginationResult !== null) {
                state.studyItemsPaginationResult.items = state.studyItemsPaginationResult.items.filter(x => x.id !== studyItemId);
            }
        },

        [storeTypes.STUDY_ITEM_IS_FAVOURITE_SET](state, payload) {
            let { studyItemId, isFavourite } = payload;
            if(state.studyItemsPaginationResult !== null) {
                state.studyItemsPaginationResult.items = state.studyItemsPaginationResult.items.map(x => {
                    if(x.id === studyItemId) {
                        return {...x, isFavourite: isFavourite};
                    }
                    return x;
                });
            }
        },

        //#endregion


        //#region Trainings

        [storeTypes.STUDY_ITEM_TRAINING_STATS_SET](state, payload) {
            let { data } = payload;
            state.trainingStats = data;
        },
        [storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED_SET](state, payload) {
            let { studyItemId } = payload;
        },
        [storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED_SET](state, payload) {
            let { studyItemId } = payload;
        },
        [storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START_SET](state, payload) {
            let { data } = payload;
            state.trainingFlashcards = data;
        },
        [storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START_SET](state, payload) {
            let { data } = payload;
            state.trainingWordMeaning = data;
        },


        //#endregion


        //#region Custom collections

        [storeTypes.CUSTOM_COLLECTIONS_SET](state, payload) {
            let { data } = payload;
            state.customCollectionsResult = data;
        },
        [storeTypes.CUSTOM_COLLECTION_CURRENT_SET](state, payload) {
            let { customCollection } = payload;
            state.currentCustomCollection = {...customCollection};
        },

        //#endregion


        //#region User films

        [storeTypes.USER_FILMS_REQUEST_PARAMS_SET](state, payload) {
            let params= payload;
            state.userFilmsRequestParams = {
                ...state.userFilmsRequestParams,
                ...params,
            };
        },
        [storeTypes.USER_FILMS_REQUEST_PARAMS_RESET](state, payload) {
            state.userFilmsRequestParams = {
                ...state.userFilmsRequestParamsDefault,
            };
        },
        [storeTypes.USER_FILMS_SET](state, payload) {
            let { data } = payload;
            state.userFilmsPaginationResult = data;
        },
        [storeTypes.USER_FILM_CREATE_SET](state, payload) {
            let { data } = payload;
            if(state.userFilmsPaginationResult !== null) {
                state.userFilmsPaginationResult.items.unshift({...data});
            }
        },
        [storeTypes.USER_FILM_UPDATE_SET](state, payload) {
            let { data } = payload;
            if(state.userFilmsPaginationResult !== null) {
                state.userFilmsPaginationResult.items = state.userFilmsPaginationResult.items.map(x => {
                    if(x.id === data.id) {
                        return {...data};
                    }
                    return x;
                });
            }
        },
        [storeTypes.USER_FILM_DELETE_SET](state, payload) {
            let { userFilmId } = payload;
            if(state.userFilmsPaginationResult !== null) {
                state.userFilmsPaginationResult.items = state.userFilmsPaginationResult.items.filter(x => x.id !== userFilmId);
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
            let { commit, dispatch, state, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEMS_LOAD,
                loading: true,
            });
            return api.webApi().getStudyItems({
                collectionId: getters.currentCustomCollectionId,
                ...params,
                ...state.studyItemsRequestParams, // apply params from state
            }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEMS_LOAD,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEMS_LOAD_SET, {
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
        [storeTypes.STUDY_ITEM_LOAD](context, { studyItemId }) {
            let { commit, dispatch, state, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_LOAD,
                loading: true,
            });
            return api.webApi().getStudyItem({
                studyItemId
            }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_LOAD,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_LOAD_SET, {
                    studyItem: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_LOAD,
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
                commit(storeTypes.STUDY_ITEM_CREATE_SET, {
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
        [storeTypes.STUDY_ITEM_UPDATE](context, {studyItemId, data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_UPDATE,
                loading: true,
            });
            return api.webApi().updateStudyItem({studyItemId, data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_UPDATE,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_UPDATE_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_UPDATE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_DELETE](context, {studyItemId}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_DELETE,
                loading: true,
            });
            return api.webApi().deleteStudyItem({studyItemId}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_DELETE,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_DELETE_SET, {
                    studyItemId: studyItemId
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_DELETE,
                    loading: false,
                });
                throw err;
            });
        },

        [storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES](context, {studyItemId}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES,
                loading: true,
            });
            return api.webApi().addStudyItemToFavourites({studyItemId}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_IS_FAVOURITE_SET, {
                    studyItemId: studyItemId,
                    isFavourite: true,
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES](context, {studyItemId}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES,
                loading: true,
            });
            return api.webApi().deleteStudyItemFromFavourites({studyItemId}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_IS_FAVOURITE_SET, {
                    studyItemId: studyItemId,
                    isFavourite: false,
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion


        //#region Trainings

        [storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD](context, params) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD,
                loading: true,
            });
            return api.webApi().getTrainingStatistics().then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_TRAINING_STATS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED](context, { studyItemId }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED,
                loading: true,
            });
            return api.webApi().markStudyItemAsTrained({ studyItemId }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED_SET, {
                    studyItemId
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED](context, { studyItemId }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED,
                loading: true,
            });
            return api.webApi().markStudyItemAsNotTrained({ studyItemId }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED_SET, {
                    studyItemId
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START](context, params) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START,
                loading: true,
            });
            return api.webApi().flashcardsTrainingStart({...params}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_SAVE](context, {data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_SAVE,
                loading: true,
            });
            return api.webApi().flashcardsTrainingSave({data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_SAVE,
                    loading: false,
                });
                // reset
                commit(storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START_SET, {
                    data: null
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_SAVE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START](context, params) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START,
                loading: true,
            });
            return api.webApi().wordmeaningTrainingStart({ ...params }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START,
                    loading: false,
                });
                commit(storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_SAVE](context, { data }) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_SAVE,
                loading: true,
            });
            return api.webApi().wordmeaningTrainingSave({ data }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_SAVE,
                    loading: false,
                });
                // reset
                commit(storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_START_SET, {
                    data: null
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.STUDY_ITEM_TRAINING_WORDMEANING_SAVE,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion
        

        //#region Custom collections

        [storeTypes.CUSTOM_COLLECTIONS_LOAD](context, params) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CUSTOM_COLLECTIONS_LOAD,
                loading: true,
            });
            return api.webApi().getCustomCollections(params).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTIONS_LOAD,
                    loading: false,
                });
                commit(storeTypes.CUSTOM_COLLECTIONS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTIONS_LOAD,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.CUSTOM_COLLECTION_CREATE](context, {data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CUSTOM_COLLECTION_CREATE,
                loading: true,
            });
            return api.webApi().createCustomCollection({data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_CREATE,
                    loading: false,
                });
                commit(storeTypes.CUSTOM_COLLECTIONS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_CREATE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.CUSTOM_COLLECTION_UPDATE](context, {customCollectionId, data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CUSTOM_COLLECTION_UPDATE,
                loading: true,
            });
            return api.webApi().updateCustomCollection({customCollectionId, data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_UPDATE,
                    loading: false,
                });
                commit(storeTypes.CUSTOM_COLLECTIONS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_UPDATE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.CUSTOM_COLLECTION_DELETE](context, {customCollectionId}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CUSTOM_COLLECTION_DELETE,
                loading: true,
            });
            return api.webApi().deleteCustomCollection({customCollectionId}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_DELETE,
                    loading: false,
                });
                commit(storeTypes.CUSTOM_COLLECTIONS_SET, {
                    data: data
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_DELETE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.CUSTOM_COLLECTION_DUPLICATE](context, {customCollectionId, isDeleteItems}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.CUSTOM_COLLECTION_DUPLICATE,
                loading: true,
            });
            return api.webApi().duplicateCustomCollection({customCollectionId, isDeleteItems}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_DUPLICATE,
                    loading: false,
                });
                commit(storeTypes.CUSTOM_COLLECTIONS_SET, {
                    data: data
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.CUSTOM_COLLECTION_DUPLICATE,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion
        

        //#region User films

        [storeTypes.USER_FILMS_LOAD](context, params) {
            let { commit, dispatch, state, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_FILMS_LOAD,
                loading: true,
            });
            return api.webApi().getUserFilms({
                ...params,
                ...state.userFilmsRequestParams, // apply params from state
            }).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILMS_LOAD,
                    loading: false,
                });
                commit(storeTypes.USER_FILMS_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILMS_LOAD,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.USER_FILM_CREATE](context, {data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_FILM_CREATE,
                loading: true,
            });
            return api.webApi().createUserFilm({data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_CREATE,
                    loading: false,
                });
                commit(storeTypes.USER_FILM_CREATE_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_CREATE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.USER_FILM_UPDATE](context, {userFilmId, data}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_FILM_UPDATE,
                loading: true,
            });
            return api.webApi().updateUserFilm({userFilmId, data}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_UPDATE,
                    loading: false,
                });
                commit(storeTypes.USER_FILM_UPDATE_SET, {
                    data: data
                });
                return data;
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_UPDATE,
                    loading: false,
                });
                throw err;
            });
        },
        [storeTypes.USER_FILM_DELETE](context, {userFilmId}) {
            let { commit, dispatch, getters } = context;
            commit(storeTypes.LOADING_SET, {
                target: storeTypes.USER_FILM_DELETE,
                loading: true,
            });
            return api.webApi().deleteUserFilm({userFilmId}).then(({ data, ok }) => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_DELETE,
                    loading: false,
                });
                commit(storeTypes.USER_FILM_DELETE_SET, {
                    userFilmId: userFilmId
                });
                // returns nothing
            }).catch(err => {
                commit(storeTypes.LOADING_SET, {
                    target: storeTypes.USER_FILM_DELETE,
                    loading: false,
                });
                throw err;
            });
        },

        //#endregion
    }
});
