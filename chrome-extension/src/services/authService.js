"use strict";

import Vue from "vue";
import store from "@/store";
import jwtDecode from "jwt-decode";
import moment from "moment";
import { EventEmitter } from "events";
import { storeTypes } from "@/constants/index";
import router from "@/router";
import apiUtil from "@/utils/api";

export const authEvents = {
  isAuthenticatedChanged: "isAuthenticatedChanged",
  tokensRefreshFailed: "tokensRefreshFailed",
};

class AuthService extends EventEmitter {
  constructor() {
    super();
    this.logPrefix = "AuthService.";
    this.tokenRefreshTimeout = null;
    this.refreshTokensMaxRetries = 3;
    this.refreshTokensRetries = 0;
    this.config = {
      clientId: null,
    };
  }

  init(config) {
    this.config = {
      ...this.config,
      ...config,
    };

    // setup custom token refreshing
    this.startAuthTokensRefresh();
  }

  /** Stores user auth tokens in chrome.storage */
  storeAuthTokensAsync({ identityToken, accessToken, refreshToken }) {
    console.log(`storeUserAuthTokensAsync.`, {
      identityToken,
      accessToken,
      refreshToken,
    });

    // NB: use chrome.storage.local to avoid syncing between browsers
    return new Promise((resolve, reject) => {
      chrome.storage.local.set(
        {
          userAuthTokens: {
            identityToken,
            accessToken,
            refreshToken,
          },
        },
        () => resolve()
      );
    });
  }

  getAuthTokensAsync() {
    return new Promise((resolve, reject) => {
      chrome.storage.local.get(["userAuthTokens"], (result) => {
        const userAuthTokens =
          !result.userAuthTokens ||
          Object.keys(result.userAuthTokens).length === 0
            ? null
            : result.userAuthTokens;
        resolve(userAuthTokens);
      });
    });
  }

  removeAuthTokensAsync() {
    return new Promise((resolve, reject) => {
      chrome.storage.local.set({ userAuthTokens: null }, (result) => {
        resolve();
      });
    });
  }

  decodeIdentityToken(identityToken) {
    const decoded = jwtDecode(identityToken);
    const {
      // default Idsr4 claims
      nbf, // not valid before (seconds since Unix epoch)
      exp, // expiration (seconds since Unix epoch)
      iss, // issued by (URI)
      aud, // audience
      sub, // subject (user id)
      auth_time, // time when authentication occured
      idp, // Idsr identity provider
      iat, // issued at (seconds since Unix epoch)

      // no custom claims, only the default ones
    } = decoded;
    return decoded;
  }

  decodeAccessToken(accessToken) {
    const decoded = jwtDecode(accessToken);
    const {
      // default Idsr4 claims
      nbf, // not valid before (seconds since Unix epoch)
      exp, // expiration (seconds since Unix epoch)
      iss, // issued by (URI)
      aud, // audience
      sub, // subject (user id)
      auth_time, // time when authentication occured
      idp, // Idsr identity provider
      jti, // JWT token id
      iat, // issued at (seconds since Unix epoch)
      scope, // array of allowed scopes and resources. E.g. openid, profile, offline_access, webapi

      // custom claims
      client_email,
      client_email_verified,
      client_browser_extension_version,
      // ...
    } = decoded;
    return decoded;
  }

  buildUserFromAccessTokenClaims(accessToken) {
    const accessTokenDecoded = this.decodeIdentityToken(accessToken);
    const user = {};
    for (const key in accessTokenDecoded) {
      if (Object.hasOwnProperty.call(accessTokenDecoded, key)) {
        user[key] = accessTokenDecoded[key];
      }
    }
    return user;
  }

  async loginAsync({ email, password, clientId, extensionVersion }) {
    try {
      const data = await store.dispatch(storeTypes.AUTH_LOGIN_REQUEST, {
        data: {
          email,
          password,
          clientId,
          extensionVersion,
        },
      });

      await this.storeAuthTokensAsync({ ...data });

      const isAuthenticated = await this.checkIsAuthenticatedAsync();

      // store user in store
      const user = this.buildUserFromAccessTokenClaims(data.accessToken);
      store.commit(storeTypes.AUTH_USER_SET, {
        user,
      });

      console.log(this.logPrefix, `loginAsync.`, `Login successfull.`);
    } catch (err) {
      console.error(this.logPrefix, err);
      throw err;
    }
  }

  async logoutAsync() {
    // clear tokens
    await this.removeAuthTokensAsync();
    await this.stopAuthTokensRefresh();
  }

  /**
   * Checks user fully authenticated
   */
  async checkIsAuthenticatedAsync() {
    let isAuthenticated = true;
    let user = null;

    // has tokens
    const tokens = await this.getAuthTokensAsync();
    if (!tokens) {
      isAuthenticated = false;
      user = null;
    } else {
      user = this.buildUserFromAccessTokenClaims(tokens.accessToken);

      // validate tokens are not expired
      const identityTokenDecoded = this.decodeIdentityToken(
        tokens.identityToken
      );
      const accessTokenDecoded = this.decodeIdentityToken(tokens.accessToken);

      if (
        moment.utc().isSameOrAfter(moment.unix(identityTokenDecoded.exp).utc())
      ) {
        console.error(
          this.logPrefix,
          `checkIsAuthenticatedAsync.`,
          `identityToken is expired.`
        );
        isAuthenticated = false;
        user = null;
      }
      if (
        moment.utc().isSameOrAfter(moment.unix(accessTokenDecoded.exp).utc())
      ) {
        console.error(
          this.logPrefix,
          `checkIsAuthenticatedAsync.`,
          `accessToken is expired.`
        );
        isAuthenticated = false;
        user = null;
      }
    }

    this.emit(authEvents.isAuthenticatedChanged, {
      isAuthenticated,
      user,
    });

    return isAuthenticated;
  }

  startAuthTokensRefresh() {
    if (this.tokenRefreshTimeout === null) {
      console.log(this.logPrefix, `startAuthTokensRefresh.`);
      this._refreshTokensAsync();
    }
  }

  stopAuthTokensRefresh() {
    console.log(this.logPrefix, `stopAuthTokensRefresh.`);
    clearTimeout(this.tokenRefreshTimeout);
    this.refreshTokensRetries = 0;
  }

  /** Recursive function that does tokens refresh */
  async _refreshTokensAsync() {
    clearTimeout(this.tokenRefreshTimeout);

    // if(this.tokenRefreshTimeout !== null) {
    //     console.log(this.logPrefix, `Tokens refresh already started. Skipping.`);
    //     return;
    // }

    const { identityToken, accessToken, refreshToken } =
      await this.getAuthTokensAsync();

    if (!identityToken || !accessToken || !refreshToken) {
      throw new Error(`Can't start token refresh. Tokens aren't set!`);
    }

    const identityTokenDecoded = this.decodeIdentityToken(identityToken);
    const accessTokenDecoded = this.decodeIdentityToken(accessToken);

    if (identityTokenDecoded.exp !== accessTokenDecoded.exp) {
      console.error(
        this.logPrefix,
        `identityToken expiration differs from accessToken expiration.`
      );
    }

    const expMoment = moment.unix(accessTokenDecoded.exp).utc();

    // choose such a timeout so we do token refresh before its expiration for sure
    // but if that 'before time' is already past then do refresh exactly at expiration time
    // and if token already expired then refresh it immediately
    // e.g. 5 minutes before
    let timeoutMs;
    if (moment.utc().isSameOrAfter(expMoment)) {
      console.log(this.logPrefix, `accessToken already expired!`);
      timeoutMs = 0;
    } else {
      const toExpirationMs = expMoment.diff(moment.utc(), "milliseconds");
      const beforeExpMoment = expMoment.clone().subtract(5, "minutes");
      if (moment.utc().isSameOrAfter(beforeExpMoment)) {
        timeoutMs = toExpirationMs;
      } else {
        timeoutMs = beforeExpMoment.diff(moment.utc(), "milliseconds");
      }
    }

    // check retries limit reached
    if (this.refreshTokensRetries >= this.refreshTokensMaxRetries) {
      console.error(
        this.logPrefix,
        `Tokens refresh retries limit (${this.refreshTokensRetries} / ${this.refreshTokensMaxRetries}) is reached. Do log out.`
      );
      this.emit(authEvents.tokensRefreshFailed);
      return;
    }

    console.log(
      this.logPrefix,
      `Sheduled the next tokens refresh at ${moment
        .utc()
        .add(timeoutMs, "milliseconds")
        .format()} (in ${timeoutMs} ms).`
    );
    this.tokenRefreshTimeout = setTimeout(async () => {
      this.refreshTokensRetries += 1;
      console.log(
        this.logPrefix,
        `Refreshing tokens, try ${this.refreshTokensRetries} / ${this.refreshTokensMaxRetries}...`
      );

      // refresh token
      try {
        let data = await apiUtil.identity().refreshToken({
          identityToken: identityToken,
          accessToken: accessToken,
          refreshToken: refreshToken,
          clientId: this.config.clientId,
        });
        await this.storeAuthTokensAsync({
          identityToken: data.data.identityToken,
          accessToken: data.data.accessToken,
          refreshToken: data.data.refreshToken,
        });
        this.refreshTokensRetries = 0;
        console.log(this.logPrefix, `Tokens refresh done.`);
      } catch (err) {
        console.error(this.logPrefix, `Tokens refresh error:`, err);
      }

      // schedule next refresh (recursive run)
      this._refreshTokens();
    }, timeoutMs);
  }
}

export default new AuthService();
