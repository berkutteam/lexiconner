

class HttpUtil {

    constructor() {

    }

    httpRequest(url, data, typeOfrequest, authToken = null, callback) {
        var xhr = new XMLHttpRequest();

        xhr.withCredentials = true; // force to show browser's default auth dialog
        xhr.open(typeOfrequest, url, true);

        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onload = function () {
            if (xhr.status === 200) {

                callback(JSON.parse(xhr.responseText));

            } else if (xhr.status === 500) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert('Request failed.  Returned status of ' + xhr.status);
            } else if (xhr.status === 401 || xhr.status === 403) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert("Authentication time is up");
                //logout();
            }
        };

        if (authToken !== null) {
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        }
        xhr.send(JSON.stringify(data));
    }

    deleteRequest(url, authToken) {
        var xhr = new XMLHttpRequest();

        xhr.withCredentials = true; // force to show browser's default auth dialog
        xhr.open('DELETE', url, true);

        xhr.onload = function () {
            if (xhr.status === 200) {

                console.log("Item successfully deleted");

            } else if (xhr.status === 500) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert('Request failed.  Returned status of ' + xhr.status);
            } else if (xhr.status === 401 || xhr.status === 403) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert("Authentication time is up");
                //logout();
            }
        };

        if (authToken !== null) {
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        }
        xhr.send();
    }

    httpGet(url, callBack, authToken = null) {
        var xhr = new XMLHttpRequest();
        xhr.withCredentials = true; // force to show browser's default auth dialog
        xhr.open('GET', url);


        xhr.onload = function () {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);

                callBack(data);
                console.log(1, data);

            } else if (xhr.status === 500) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert('Request failed.  Returned status of ' + xhr.status);
            } else if (xhr.status === 401 || xhr.status === 403) {
                console.error('Request failed.  Returned status of ' + xhr.status);
                alert("Authentication time is up");
                //console.log(222222, xhr.getResponseHeader('WWW-Authenticate')); returns
            }
            else {
                console.error('Request failed.  Returned status of ' + xhr.status);
            }

        };


        if (authToken !== null) {
            xhr.setRequestHeader("Authorization", "Bearer " + authToken);
        }
        xhr.send();
    }

   

}

export default HttpUtil;