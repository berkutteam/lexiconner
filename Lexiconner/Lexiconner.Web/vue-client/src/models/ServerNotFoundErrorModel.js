import ServerBaseErrorModel from './ServerBaseErrorModel.js';

class ServerNotFoundErrorModel extends ServerBaseErrorModel {
    constructor(response) {
        super(response);
    }
}

export default ServerNotFoundErrorModel;