
class UserUtil {

    constructor(config) {
        this.config = config;

        this.authConfig = {
            authority: this.config.auth.authority,
            client_id: this.config.auth.clientId,
            redirect_uri: this.config.auth.redirectUri,
            response_type: this.config.auth.responseType,
            scope: this.config.auth.scope,
            post_logout_redirect_uri: this.config.auth.postLogoutRedirectUri,

            // access_token renew
            automaticSilentRenew: true,
            silentRequestTimeout: 10000,
            accessTokenExpiringNotificationTime: 3 * 60, // 3 mins in secs
        };

        this.userManager = new Oidc.UserManager(this.authConfig);// for tests only

        // setup logging

        Oidc.Log.logger = console;
        Oidc.Log.level = Oidc.Log.INFO;

        this.userManager.events.addUserLoaded(function () {
            console.log("userLoaded");
        });
        this.userManager.events.addUserUnloaded(function () {
            console.log("userUnloaded");
        });
        this.userManager.events.addAccessTokenExpiring(function () {
            console.log("access_token expiring...");
        });
        this.userManager.events.addAccessTokenExpired(function () {
            console.log("access_token expired");
        });
        this.userManager.events.addSilentRenewError(function (err) {
            console.error("silentRenewError: ", err);
        });


    }

    // https://github.com/IdentityModel/oidc-client-js/wiki


    // check user logged in

    getUser(callback) {
        this.userManager.getUser().then(function (user) {

            callback(user);

        });
    }



    login() {
        this.userManager.signinRedirect();
    }

    logout() {
        console.log('Logout requested.')
        this.userManager.signoutRedirect();
    }

    handleAccessTokenExpiration(user) {
        let {
            profile,
            id_token,
            access_token,
            refresh_token,
            expires_at, // int - seconds
            scope,
            session_state,
            state,
            token_type,
            expired, // func
            expires_in, // func - returns seconds
            scopes, // func - returns array<string>
        } = user;
        let {
            amr, //array<string>
            auth_time, // int - seconds
            idp,
            sid,
            sub, // user id
        } = profile;

        // refresh access_token manually before it get expired using refresh_token (if flow allows)
        // ...
    }

    checkIdentityApi(user) {
        var url = `${this.config.urls.api}/api/v2/identity`;

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            console.log('Test Api: ', xhr.status, JSON.parse(xhr.responseText));
        };
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    }

}

export default UserUtil;