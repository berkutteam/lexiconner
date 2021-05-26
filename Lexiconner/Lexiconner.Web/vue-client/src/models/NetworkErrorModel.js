import _ from "lodash";

class NetworkErrorModel {
  constructor(err = {}) {
    this.config = err.config;
    this.isAxiosError = err.isAxiosError;
    this.request = err.request;
    this.response = null;
    this.message = err.message;
    this.stack = err.stack;
  }
}

export default NetworkErrorModel;
