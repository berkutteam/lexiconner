import ServerBaseErrorModel from './ServerBaseErrorModel.js';

class ServerErrorModel extends ServerBaseErrorModel {
    constructor(response) {
        super(response);
    }
}

export default ServerErrorModel;