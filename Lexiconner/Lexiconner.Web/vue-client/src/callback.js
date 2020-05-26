/*
* Identity server signin callback (callback.html)
*/

import Oidc from 'oidc-client';

console.log('Running callback.js...');

let userManager = new Oidc.UserManager({ response_mode: "query" });

userManager.signinRedirectCallback(null).then(function (user) {
    console.log('Callback: user ', user);
    window.location = `${process.env.BASE_URL ? process.env.BASE_URL : '/'}`;
}).catch(function (e) {
    console.error(e);
    // TODO - handle error
    //alert(`Login error. Redirecting back to app...`);
    
    window.location = `${process.env.BASE_URL ? process.env.BASE_URL : '/'}`;
});
