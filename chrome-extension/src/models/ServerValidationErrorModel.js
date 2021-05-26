import ServerBaseErrorModel from './ServerBaseErrorModel.js';

class ServerValidationErrorModel extends ServerBaseErrorModel {
    constructor(response) {
        super(response);
    }
}

export default ServerValidationErrorModel;