/*
* Identity server signin callback (callback.html)
*/

import Oidc from 'oidc-client';

console.log('Running callback.js...');

let userManager = new Oidc.UserManager({ response_mode: "query" });

userManager.signinRedirectCallback(null).then(function (user) {
    console.log('Callback: user ', user);
    window.location = `${process.env.BASE_URL ? process.env.BASE_URL : '/'}`;
}).catch(function (err) {
    console.error(`Callback error:`, err);
    // TODO - handle error
    //alert(`Login error. Redirecting back to app...`);
    
    const redirectUrl = `${process.env.BASE_URL ? process.env.BASE_URL : '/'}`
    console.log(`Callback:`, `Redirecting to`, redirectUrl);
    window.location = redirectUrl;
});
