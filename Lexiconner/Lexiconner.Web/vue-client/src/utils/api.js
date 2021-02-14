'use strict';

import Http from './http.js';

import ServerValidationErrorModel from '../models/ServerValidationErrorModel.js';
import NetworkErrorModel from '../models/NetworkErrorModel';
import ServerErrorModel from '../models/ServerErrorModel';
import ServerUnknownErrorModel from '../models/ServerUnknownErrorModel.js';
import ServerNotFoundErrorModel from '../models/ServerNotFoundErrorModel.js';


import authService from '@/services/authService';

function buildUrl(urlTemplate, endpoint, queryStringParams = {}, doEncodeURI = true, doEncodeURIComponents = true) {
    if (!endpoint)
        throw new Error('endpoint can\'t be empty');
    let url = urlTemplate.replace("<endpoint>", endpoint);
    if (doEncodeURI) {
        url = encodeURI(url);
    }

    // build query string
    if (queryStringParams && typeof queryStringParams === 'object') {
        let processedParamCount = 0;
        let queryString = Object.keys(queryStringParams).reduce((res, key, i) => {
            if (queryStringParams[key] === null || queryStringParams[key] === undefined) {
                return res;
            }

            if (processedParamCount === 0)
                res += '?';
            else
                res += '&';

            if (doEncodeURIComponents) {
                res += `${encodeURIComponent(key)}=${encodeURIComponent(queryStringParams[key])}`;
            }
            else {
                res += `${key}=${queryStringParams[key]}`;
            }

            processedParamCount += 1;
            return res;
        }, "");

        url = `${url}${queryString}`;
    }
    return url;
}

let authenticationScheme = "Bearer";

/**
 * Adds auth header to request before send
 * @param {object} axiosConfig
 * @return {Promise<object>} response
 */
function axiosAuthRequest(axiosConfig) {
    return new Promise((resolve, reject) => {
        authService.getUser().then((user) => {
            let { access_token, id_token, refresh_token } = user;
            axiosConfig.headers = axiosConfig.headers || {};
            axiosConfig.headers["Authorization"] = `${authenticationScheme} ${access_token}`;
            Http.axios(axiosConfig).then(response => {
                let { config, data, headers, request, status, statusText } = response;
                resolve(response);
            }).catch(err => {
                let { config, isAxiosError, request, response, message, stack } = err;
                reject(err); // reject with error response from server (if present)
            });
        }, (err) => {
            reject(err);
        });
    });
}

/**
 * Meddleware before send
 * @param {object} axiosConfig
 * @return {Promise<object>} response
 */
function axiosRequest(axiosConfig) {
    return new Promise((resolve, reject) => {
        Http.axios({
            // headers: {
            //     'Content-Type': 'application/json'
            // },
            ...axiosConfig,
        }).then(response => {
            let { config, data, headers, request, status, statusText } = response;
            resolve(response);
        }).catch(err => {
            let { config, isAxiosError, request, response, message, stack } = err;
            reject(err);
        });
    });
}


/**
 * Handles base response from API.
 * 
 * @param {any} response 
 */
function handleApiResponse(response) {

    if (response.status !== 200 && response.status !== 201) {
        handleApiErrorResponse(response);
    }

    return response.data;
}


/**
 * Handles base error response from API.
 * 
 * @param {any} response 
 */
function handleApiErrorResponse(err) {
    // TODO - add new models if needed
    let { config, isAxiosError, request, response, message, stack } = err;

    // Network erorr or something else
    if(response === null || message === 'Network Error') {
        throw new NetworkErrorModel(err);
    }

    // log auth errors in headers
    // www-authenticate can contain message or object with errors
    if (response.headers['www-authenticate']) {
        console.error(`www-authenticate error: ${response.headers['www-authenticate']}`);
    }

    // throw if not success status code
    if (response.status === 400) { // Validation error
        throw new ServerValidationErrorModel(response);
    }
    else if (response.status === 404) {
        throw new ServerNotFoundErrorModel(response);
    }
    else if (response.status === 500) { // Server error
        throw new ServerErrorModel(response);
    }
    else {
        // if unknown error, just rethrow and do not wrap
        // throw new ServerUnknownErrorModel(response);
        throw err;
    }
}

class API {
    constructor() {
        this.config = null;
    }

    init(urlsConfig) {
        // contains urls
        this.config = urlsConfig;
    }

    configApi() {
        let url = `${process.env.BASE_URL}api/v1/config`;
        if (process.env.VUE_APP_ASPNETCORE_ENVIRONMENT === 'DevelopmentLocalhost') {
            url = `http://localhost:5007/api/v1/config`; // local server url
        }
        return {
            config: () => {
                return axiosRequest({ url: url, method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
        }
    }

    identity() {
        let url = `${this.config.identityExternalUrl}/api/v1/<endpoint>`;

        return {
            // getRegistrationInfo() {
            //     return axiosRequest({ url: buildUrl(url, `account/register/info`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getPreRegistrationUser({ preRegistrationUserId }) {
            //     return axiosRequest({ url: buildUrl(url, `account/register/preregistration-users/${preRegistrationUserId}`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // sendSmsTokenForRegistration({ phoneNumber, preRegistrationUserId }) {
            //     return axiosRequest({ url: buildUrl(url, `account/register/sms/send`, {}), method: "post", data: { phoneNumber, preRegistrationUserId } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // validateSmsTokenForRegistration({ phoneNumber, token, preRegistrationUserId }) {
            //     return axiosRequest({ url: buildUrl(url, `account/register/sms/validate`, {}), method: "post", data: { phoneNumber, token, preRegistrationUserId } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // register({ user }) {
            //     return axiosRequest({ url: buildUrl(url, `account/register`, {}), method: "post", data: user }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // sendSmsPhoneNumberChangeToken({ phoneNumber }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `account/phone-change/sms/send`, {}), method: "post", data: { phoneNumber } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getUserAccount() {
            //     return axiosAuthRequest({ url: buildUrl(url, `account`, {}), method: "get", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // updateUserAccount(data) {
            //     return axiosAuthRequest({ url: buildUrl(url, `account`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // deleteUserAccount({ userConfirmation }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `account`, {}), method: "delete", data: { userConfirmation: userConfirmation } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // passwordChange({ passwordOld, passwordNew, passwordNewConfirm }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `manage/password/change`, {}), method: "post", data: { passwordOld, passwordNew, passwordNewConfirm } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getMyPermissions({ }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `permissions/my`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getScopedPermissions({ scopeId }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `permissions/scoped/${scopeId}`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getUserScopedPermissions({ scopeId, userId }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `permissions/scoped/${scopeId}/${userId}`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // updateUserScopedPermissions({ scopeId, userId, permissions, roles }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `permissions/scoped/${scopeId}`, {}), method: "put", data: { userId, permissions, roles } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
        };
    }

    webApi() {
        let url = `${this.config.apiExternalUrl}/api/v2/<endpoint>`;

        return {
            // // values (test)
            // testUnauthorized() {
            //     return axiosRequest({ url: buildUrl(url, `values/TestUnauthorized`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },

            // getEnums() {
            //     return axiosRequest({ url: buildUrl(url, `enums`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },

            getLanguages() {
                return axiosRequest({ url: buildUrl(url, `languages`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },

            // getTermsOfUse() {
            //     return axiosRequest({ url: buildUrl(url, `referenceinformation/terms-of-use`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getCountries() {
            //     return axiosRequest({ url: buildUrl(url, `referenceinformation/countries`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getLanguages() {
            //     return axiosRequest({ url: buildUrl(url, `referenceinformation/languages`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // getTimeZones() {
            //     return axiosRequest({ url: buildUrl(url, `referenceinformation/timezones`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },

            // getUserInfo() {
            //     return axiosAuthRequest({ url: buildUrl(url, `userinfo`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // updateUserInfo(data) {
            //     return axiosAuthRequest({ url: buildUrl(url, `userinfo`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // updateUserInfoNotifications(data) {
            //     return axiosAuthRequest({ url: buildUrl(url, `userinfo/notifications`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // setUserInfoTimeZone({ timeZoneId }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `userinfo/timezone`, {}), method: "post", data: { timeZoneId } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },
            // setCurrentUserCompany({ companyId }) {
            //     return axiosAuthRequest({ url: buildUrl(url, `userinfo/current-company`, {}), method: "put", data: { companyId } }).then(handleApiResponse).catch(handleApiErrorResponse);
            // },

            // study items
            getStudyItems({ offset, limit, search, isFavourite, isShuffle, collectionId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems`, { offset, limit, search, isFavourite, isShuffle, collectionId}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            getStudyItem({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/${studyItemId}`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            createStudyItem({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            updateStudyItem({ studyItemId, data }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/${studyItemId}`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            deleteStudyItem({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/${studyItemId}`, {}), method: "delete", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },

            addStudyItemToFavourites({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/${studyItemId}/favourites`, {}), method: "post", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            deleteStudyItemFromFavourites({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/${studyItemId}/favourites`, {}), method: "delete", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },

            // trainings
            getTrainingStatistics() {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/stats`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            markStudyItemAsTrained({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/trained/${studyItemId}`, {}), method: "put" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            markStudyItemAsNotTrained({ studyItemId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/not_trained/${studyItemId}`, {}), method: "put" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            flashcardsTrainingStart({collectionId, limit}) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/flashcards`, {collectionId, limit}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            flashcardsTrainingSave({data}) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/flashcards/save`, {}), method: "post", data: {...data} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            wordmeaningTrainingStart({ collectionId, limit }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/wordmeaning`, { collectionId, limit }), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            wordmeaningTrainingSave({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/wordmeaning/save`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            meaningwordTrainingStart({ collectionId, limit }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/meaningword`, { collectionId, limit }), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            meaningwordTrainingSave({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/meaningword/save`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            matchwordsTrainingStart({ collectionId }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/matchwords`, { collectionId }), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            matchwordsTrainingSave({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `studyitems/trainings/matchwords/save`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },

            // custom collections
            getCustomCollections() {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            createCustomCollection({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            updateCustomCollection({ customCollectionId, data }) {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections/${customCollectionId}`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            markCustomCollectionAsSelected({ customCollectionId }) {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections/${customCollectionId}/select`, {}), method: "put", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            deleteCustomCollection({ customCollectionId, isDeleteItems }) {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections/${customCollectionId}`, {isDeleteItems}), method: "delete", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            duplicateCustomCollection({ customCollectionId }) {
                return axiosAuthRequest({ url: buildUrl(url, `customcollections/${customCollectionId}/duplicate`, {}), method: "post", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },

            // user films
            getUserFilms({ offset, limit, search }) {
                return axiosAuthRequest({ url: buildUrl(url, `userfilms`, {offset, limit, search}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            getUserFilm({ userFilmId }) {
                return axiosAuthRequest({ url: buildUrl(url, `userfilms/${userFilmId}`, {}), method: "get" }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            createUserFilm({ data }) {
                return axiosAuthRequest({ url: buildUrl(url, `userfilms`, {}), method: "post", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            updateUserFilm({ userFilmId, data }) {
                return axiosAuthRequest({ url: buildUrl(url, `userfilms/${userFilmId}`, {}), method: "put", data: { ...data } }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
            deleteUserFilm({ userFilmId }) {
                return axiosAuthRequest({ url: buildUrl(url, `userfilms/${userFilmId}`, {}), method: "delete", data: {} }).then(handleApiResponse).catch(handleApiErrorResponse);
            },
        };
    }
}

export default new API();
