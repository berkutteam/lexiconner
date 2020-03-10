import ServerBaseErrorModel from './ServerBaseErrorModel.js';

class ServerUnknownErrorModel extends ServerBaseErrorModel {
    constructor(response) {
        super(response);
    }
}

export default ServerUnknownErrorModel;