import ServerBaseErrorModel from "./ServerBaseErrorModel.js";

class ServerUnauthorizedError extends ServerBaseErrorModel {
  constructor(response) {
    super(response);
  }
}

export default ServerUnauthorizedError;
