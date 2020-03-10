import _ from 'lodash';

class ServerBaseErrorModel {
    constructor(response = {}) {
        _.assignIn(this, response); // Inject all props in an object
    }
}

export default ServerBaseErrorModel;