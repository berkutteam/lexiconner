'use strict';

import Vue from 'vue';
import Oidc from 'oidc-client';
import {UserInfoService} from 'oidc-client/src/UserInfoService.js';
import store from '@/store';
import { storeTypes } from '@/constants/index';
import router from '@/router';

// cheatsheet for Oidc
// https://github.com/IdentityModel/oidc-client-js/wiki
// import {
//     Version,
//     Log,
//     OidcClient,
//     OidcClientSettings,
//     WebStorageStateStore,
//     InMemoryWebStorage,
//     UserManager, // extends OidcClient
//     AccessTokenEvents,
//     MetadataService,
//     CordovaPopupNavigator,
//     CordovaIFrameNavigator,
//     CheckSessionIFrame,
//     TokenRevocationClient,
//     SessionMonitor,
//     Global,
//     User
// } from 'oidc-client';

// let {
       // the claims represented by a combination of the id_token and the user info endpoint. contains all claims as keys
       // NB: custom bool claims are encoded as strings: true, false
//     profile, 

//     id_token,
//     access_token,
//     refresh_token,
//     expires_at, // int - seconds
//     scope,
//     session_state,
//     state,
//     token_type,
//     expired, // func
//     expires_in, // func - returns seconds
//     scopes, // func - returns array<string>
// } = user;
// let {
//     amr, //array<string>
//     auth_time, // int - seconds
//     idp,
//     sid,
//     sub, // user id

//     // optional (custom)
//     company_id,
//     email,
//     given_name,
//     name,
//     permissions,
//     preferred_username,
// } = profile;

class AuthService {
    constructor() {
        this.config = null;
    }

    init(authConfig) {
        let self = this;
        this.config = authConfig;
        this.userManager = new Oidc.UserManager({
            authority: this.config.authority,
            client_id: this.config.clientId,
            redirect_uri: this.config.redirectUri,
            response_type: this.config.responseType,
            scope: this.config.scopes.join(' '), // E.g. "scope1 scope2 scope3"
            post_logout_redirect_uri: this.config.postLogoutRedirectUri,

            // access_token renew
            automaticSilentRenew: true,
            silentRequestTimeout: 10000,
            accessTokenExpiringNotificationTime: 3 * 60, // 3 mins in secs

            loadUserInfo: true,
        });


        // create interal Oidc UserInfoService to manually read claims from JWT
        // NB: cam read cliams only from access_token
        this.userInfoService = new UserInfoService(this.userManager.settings); // don't use destruction to save getters
        // console.log("AuthService: userInfoService", this.userInfoService);

        // setup logging
        Oidc.Log.logger = console;
        Oidc.Log.level = Oidc.Log.INFO;

        // called when token was refreshed
        this.userManager.events.addUserLoaded(function () {
            console.log("AuthService: userLoaded");
        });
        // called before logout
        this.userManager.events.addUserUnloaded(function () {
            console.log("AuthService: userUnloaded");
            store.commit(storeTypes.AUTH_USER_RESET);
        });
        this.userManager.events.addAccessTokenExpiring(function () {
            console.log("AuthService: access_token expiring...");
        });
        this.userManager.events.addAccessTokenExpired(function () {
            console.log("AuthService: access_token expired");

            // access_token haven't been refreshed, logout
            self.logout();
        });
        this.userManager.events.addSilentRenewError(function (err) {
            console.error("AuthService: silentRenewError: ", err);
        });
        this.userManager.events.addUserSignedOut((function () {
            console.error("AuthService: userSignedOut");

            Vue.notify({
                group: 'app-important',
                type: 'warn',
                title: 'Warning',
                text: 'Session expired. You have beed logged out.'
            });

            // logout quitely for sure
            // user can have expired refresh_token, but is still valid access_token
            this.logoutWithoutRedirect().then(() => {
                router.push({name: 'home'});
            }).catch(err => {
                console.error(err);
            });
        }).bind(this));

        // setup custom token refreshing (aditional to automatic) to keep user tokens fresh
        // with actual permissions
        this.refreshTokensInterval = this.refreshTokensInterval || 5 * 60 * 1000; // 5min
        this.refreshTokensIntervalFunc = this.refreshTokensIntervalFunc || setInterval((async function() {
            console.log(`AuthService: running refreshTokensInterval (every ${this.refreshTokensInterval} ms)...`);
            await this.refreshTokens({withFullscreenLoader: false});
        }).bind(this), this.refreshTokensInterval);
    }

    /**
     * Fully authenticated. Has all tokens and completed registration process
     */
    async isAuthenticated() {
        let user = await this.userManager.getUser();
        let isRegistrationCompleted = user && user.profile && (user.profile.is_pre_registration === 'false');
        return !!user; // && isRegistrationCompleted ? true : false;
    }

    // /**
    //  * Has all tokens, but not completed registration process
    //  */
    // async isNotFullyAuthenticated() {
    //     let user = await this.userManager.getUser();
    //     let isRegistrationCompleted = user && user.profile && (user.profile.is_pre_registration === 'true');
    //     return !!user && isRegistrationCompleted ? true : false;
    // }

    getUser() {
        return this.userManager.getUser().then(async (user) => {
            // console.log("AuthService: getUser: ", user);
            if(user) {
                console.log("AuthService: isAuthenticated: ", true);
                // console.log("AuthService: user.access_token claims: ", await this.userInfoService.getClaims(user.access_token));

                store.commit(storeTypes.AUTH_USER_SET, {
                    user,
                });
            } else {
                console.log("AuthService: isAuthenticated: ", false);
                
                store.commit(storeTypes.AUTH_USER_RESET);
                // store.commit(storeTypes.USER_INFO_SET, {
                //     userInfo: null,
                // });
            }
            return user;
        }).catch(err => {
            let {message, stack} = err;
            console.error("AuthService: getUser error: ", err);
            
            Vue.notify({
                group: 'error',
                type: 'error',
                title: 'Error',
                text: 'Something went wrong. Can\'t read user info!'
            });
        });
    }

    login() {
        console.log('AuthService: Login requested.');
        this.userManager.signinRedirect().then(() => {
            console.log('AuthService: Redirecting to login...');
        }).catch((err) => {
            let {message, stack} = err;
            console.log('AuthService: Login error - ', err);

            store.commit(storeTypes.ERROR_PAGE_DATA_SET ,{
                data: {
                    title: 'Error',
                    text: 'Something went wrong during login!',
                },
            });

            router.push({name: 'error'});
        });
    }

    logout() {
        console.log('AuthService: Logout requested.');
        this.userManager.signoutRedirect().then(() => {
            console.log('AuthService: Redirecting to logout...');
        }).catch((err) => {
            let {message, stack} = err;
            console.log('AuthService: Logout error - ', err);

            store.commit(storeTypes.ERROR_PAGE_DATA_SET ,{
                data: {
                    title: 'Error',
                    text: 'Something went wrong during logout!',
                },
            });

            router.push({name: 'error'});
        });
    }

    async logoutWithoutRedirect() {
        console.log('AuthService: Logout without redirect requested.');

        // Returns promise to remove from any storage the currently authenticated user.
        await this.userManager.removeUser();
        console.log('AuthService: user removed');

        // Removes stale state entries in storage for incomplete authorize requests.
        await this.userManager.clearStaleState();
        console.log('AuthService: stale state cleared');

        // reload user (will be reset)
        await this.getUser();
    }

    /**
     * Allows to refresh token manually at any time (if user is logged in)
     * Makes 2 requests:
     * - <identity-url>/connect/token to get tokens
     * - <identity-url>/connect/userinfo to user claims
     */
    async refreshTokens({withFullscreenLoader = false} = {}) {
        let self = this;
        if(withFullscreenLoader) {
            Vue.appFullscreenLoader({
                isVisible: true,
                title: 'Updating your profile info...',
            });
        }

        let isAuthenticated = await this.isAuthenticated();
        if(isAuthenticated) {
            try {
                let user = await this.userManager.signinSilent();
                
                console.log("AuthService:.refreshTokens: Silent token renewal successful. ", user);

                // manually update profile with the new claims
                // (profile isn't updated after token refresh)
                let nextClaims = await self.userInfoService.getClaims(user.access_token)

                console.log("AuthService:.refreshTokens: user.profile", user.profile);
                console.log("AuthService:.refreshTokens: nextClaims", nextClaims);
                user.profile = {
                    ...user.profile,
                    ...nextClaims,
                    'test': 'test',
                };
                self.userManager.storeUser(user);

                // reload user
                await self.getUser();
                // self.userManager.events._raiseUserLoaded();

                if(withFullscreenLoader) {
                    Vue.appFullscreenLoader({
                        isVisible: false,
                    });
                }
            } catch(err) {
                console.error("AuthService:.refreshTokens: Error from signinSilent:", err);
                this.userManager.events._raiseSilentRenewError(err);

                if(withFullscreenLoader) {
                    Vue.appFullscreenLoader({
                        isVisible: false,
                    });
                }

                throw err;
            }
        }
        else {
            let msg = "AuthService:.refreshTokens: Can't refresh token. User isn't authenticated.";
            console.error(msg);

            if(withFullscreenLoader) {
                Vue.appFullscreenLoader({
                    isVisible: false,
                });
            }

            throw new Error(msg);
        }
    }
}

export default new AuthService();
