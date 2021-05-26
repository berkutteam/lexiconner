import axios from "axios";

export default class Http {
  //make http requests using axios (see config fields here https://github.com/mzabriskie/axios)
  static axios(config) {
    return Http._axiosRequest(config);
  }

  static _axiosRequest(config) {
    if (!config || !config.url) {
      throw new Error("Http request error: config required");
    }

    config.method = config.method || "get";

    if (
      config.url &&
      config.url.indexOf("http") === -1 &&
      config.url.indexOf("https") === -1 &&
      config.url[0] != "/"
    )
      config.url = "/" + config.url;

    return axios(config)
      .then((response) => {
        //console.log("HTTP response received: ",response);
        let { config, data, headers, request, status, statusText } = response;
        return response;
      })
      .catch((err) => {
        console.error("HTTP request error: ", err);
        // if(err && err.response && error.err.status === 401){
        //     window.OidcUserManager.signinRedirect(); //Login Using IdentityServer4
        // }
        // return {};
        throw err;
      });
  }
}
