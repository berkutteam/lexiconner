import ServerBaseErrorModel from "./ServerBaseErrorModel.js";

class ServerForbiddenError extends ServerBaseErrorModel {
  constructor(response) {
    super(response);
  }
}

export default ServerForbiddenError;
