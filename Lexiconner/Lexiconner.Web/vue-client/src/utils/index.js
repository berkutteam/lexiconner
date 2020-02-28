
class Utils {
    constructor() {

    }

    waitAsync(timeout = 100) {
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                resolve();
            }, timeout);
        });
    }
}

export default new Utils();
