"use strict";

import Vue from "vue";
import _ from "lodash";
import ServerBaseErrorModel from "@/models/ServerBaseErrorModel";
import NetworkErrorModel from "@/models/NetworkErrorModel";
import ServerUnknownErrorModel from "@/models/ServerUnknownErrorModel.js";
import ServerNotFoundErrorModel from "@/models/ServerNotFoundErrorModel.js";
import ServerUnauthorizedError from "@/models/ServerUnauthorizedError.js";
import ServerForbiddenError from "@/models/ServerForbiddenError.js";

// Server error response
// {
//     config: Object, // axios request config
//     data: any, // data from server
//     header: Object, // request headers
//     request: Object, // looks like axios request instance
//     status: int, // response status
//     statusText: stringify, // response status string representation
// }

// Default error response
// data: {
//     errors: {
// "": string || Array<string>, // generic error
// "DEBUG_StackTrace": string, // StackTrace (only dev)
// "<field-name>": string || Array<string>, // request model field specific erorr
// ...
//     },
//     title: string, // response title (optional)
//     status: int,
//     instance: , // AspNetCore request path. E.g. /api/v1/account/register/sms/send. (optional)
//     type: , // AspNetCore internal response type. (optional)
// }

class Notification {
  constructor() {}

  isNetworkError(err) {
    return err instanceof NetworkErrorModel;
  }

  isServerErrorResponse(err) {
    // for 401, 403 data can be empty string or missing at all
    if (
      err instanceof ServerBaseErrorModel &&
      err.config &&
      _.isObject(err.config) &&
      // err.data &&
      // _.isObject(err.data) &&
      err.headers &&
      _.isObject(err.headers) &&
      err.request &&
      _.isObject(err.request) &&
      _.isInteger(err.status) &&
      _.isString(err.statusText)
    ) {
      return true;
    }
    return false;
  }

  showNetworkError() {
    Vue.notify({
      group: "error",
      type: "error",
      title: "Network error",
      text: "Check your internet connection or try later.",
    });
  }

  showDefaultError() {
    Vue.notify({
      group: "error",
      type: "error",
      title: ":( Opps. Something went wrong...",
      text: "You can try again or contact support.",
    });
  }

  showErrorIfServerErrorResponse(err) {
    if (this.isServerErrorResponse(err)) {
      let { config, data, header, request, status, statusText } = err;

      let ntitle = "Error";
      let ntext = "Something went wrong. Please try again.";

      if (data && _.isObject(data)) {
        let { errors, title } = data;

        ntitle = title || "Error";

        // ignore debugStackTrace that contains debug info
        let keys = Object.keys(errors).filter(
          (key) => key !== "debugStackTrace"
        );
        if (keys.length > 0) {
          let errorList = keys.map((key, i) => {
            if (_.isArray(errors[key])) {
              let subErrorList = errors[key].join("<br/>"); // uses markup
              return `${
                errors[key].length > 1 && i !== 0 ? "<br/>" : ""
              }${subErrorList}`;
            } else if (_.isString(errors[key])) {
              return errors[key];
            }
            return "";
          });
          ntext = errorList.join("<br/>"); // uses markup
        }
      } else if (err instanceof ServerUnauthorizedError) {
        ntitle = "Unauthorized";
        ntext = "You session expired or you are not logged in";
      } else if (err instanceof ServerForbiddenError) {
        ntitle = "Forbidden";
        ntext = "You don't have enough permission to perform the action";
      }

      Vue.notify({
        group: "error",
        type: "error",
        title: ntitle,
        text: ntext,
      });
    }
  }

  showErrorIfServerErrorResponseOrDefaultError(err) {
    if (this.isServerErrorResponse(err)) {
      this.showErrorIfServerErrorResponse(err);
    } else if (this.isNetworkError(err)) {
      this.showNetworkError();
    } else {
      this.showDefaultError();
    }
  }
}

export default new Notification();
