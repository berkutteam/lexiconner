
function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "https://localhost:5006/api/v2/identity";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        };
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("logout").addEventListener("click", logout, false);

var config = {
    authority: "https://localhost:5004",
    client_id: "webtestspa",
    redirect_uri: "https://localhost:5022/callback.html",
    response_type: "code",
    scope: "openid profile webapi",
    post_logout_redirect_uri: "https://localhost:5022/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        console.log(user);
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});


