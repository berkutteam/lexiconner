"use strict";

import Http from "./http.js";

import ServerValidationErrorModel from "../models/ServerValidationErrorModel.js";
import NetworkErrorModel from "../models/NetworkErrorModel";
import ServerErrorModel from "../models/ServerErrorModel";
import ServerUnknownErrorModel from "../models/ServerUnknownErrorModel.js";
import ServerNotFoundErrorModel from "../models/ServerNotFoundErrorModel.js";

import authService from "@/services/authService";

function buildUrl(
  urlTemplate,
  endpoint,
  queryStringParams = {},
  doEncodeURI = true,
  doEncodeURIComponents = true
) {
  if (!endpoint) throw new Error("endpoint can't be empty");
  let url = urlTemplate.replace("<endpoint>", endpoint);
  if (doEncodeURI) {
    url = encodeURI(url);
  }

  // build query string
  if (queryStringParams && typeof queryStringParams === "object") {
    let processedParamCount = 0;
    let queryString = Object.keys(queryStringParams).reduce((res, key, i) => {
      if (
        queryStringParams[key] === null ||
        queryStringParams[key] === undefined
      ) {
        return res;
      }

      if (processedParamCount === 0) res += "?";
      else res += "&";

      if (doEncodeURIComponents) {
        res += `${encodeURIComponent(key)}=${encodeURIComponent(
          queryStringParams[key]
        )}`;
      } else {
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
    authService.getAuthTokensAsync().then(
      (tokens) => {
        let { identityToken, accessToken, refreshToken } = tokens || {};
        axiosConfig.headers = axiosConfig.headers || {};
        axiosConfig.headers["Authorization"] = `${authenticationScheme} ${
          accessToken || null
        }`;
        Http.axios(axiosConfig)
          .then((response) => {
            let { config, data, headers, request, status, statusText } =
              response;
            resolve(response);
          })
          .catch((err) => {
            let { config, isAxiosError, request, response, message, stack } =
              err;
            reject(err); // reject with error response from server (if present)
          });
      },
      (err) => {
        reject(err);
      }
    );
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
    })
      .then((response) => {
        let { config, data, headers, request, status, statusText } = response;
        resolve(response);
      })
      .catch((err) => {
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
  if (response === null || message === "Network Error") {
    throw new NetworkErrorModel(err);
  }

  // log auth errors in headers
  // www-authenticate can contain message or object with errors
  if (response.headers["www-authenticate"]) {
    console.error(
      `www-authenticate error: ${response.headers["www-authenticate"]}`
    );
  }

  // throw if not success status code
  if (response.status === 400) {
    // Validation error
    throw new ServerValidationErrorModel(response);
  } else if (response.status === 404) {
    throw new ServerNotFoundErrorModel(response);
  } else if (response.status === 500) {
    // Server error
    throw new ServerErrorModel(response);
  } else {
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

  identity() {
    let url = `${this.config.identityUrl}/api/v1/browser-extension/<endpoint>`;

    return {
      // default IdentityServe OAuth2 endpoints

      // custom endpoints
      login({ email, password, clientId, extensionVersion }) {
        return axiosRequest({
          url: buildUrl(url, `account/login`, {}),
          method: "post",
          data: { email, password, clientId, extensionVersion },
        })
          .then(handleApiResponse)
          .catch(handleApiErrorResponse);
      },
      refreshTokens({
        identityToken,
        accessToken,
        refreshToken,
        clientId,
        extensionVersion,
      }) {
        return axiosRequest({
          url: buildUrl(url, `account/refresh-tokens`, {}),
          method: "post",
          data: {
            identityToken,
            accessToken,
            refreshToken,
            clientId,
            extensionVersion,
          },
        })
          .then(handleApiResponse)
          .catch(handleApiErrorResponse);
      },
    };
  }

  webApi() {
    let url = `${this.config.apiUrl}/api/v2/browser-extension/<endpoint>`;

    return {
      // profile
      getProfile() {
        return axiosAuthRequest({
          url: buildUrl(url, `profile/me`, {}),
          method: "get",
        })
          .then(handleApiResponse)
          .catch(handleApiErrorResponse);
      },
      selectProfileLearningLanguage({ languageCode }) {
        return axiosAuthRequest({
          url: buildUrl(url, `profile/learning-languages/${languageCode}`, {}),
          method: "post",
          data: {},
        })
          .then(handleApiResponse)
          .catch(handleApiErrorResponse);
      },

      getLanguages() {
        return axiosRequest({
          url: buildUrl(url, `languages`, {}),
          method: "get",
        })
          .then(handleApiResponse)
          .catch(handleApiErrorResponse);
      },
    };
  }
}

export default new API();
