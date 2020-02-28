const fs = require('fs');
const path = require('path');

// create temporary .env.[mode].local file with correct VUE_APP_ASPNETCORE_ENVIRONMENT and other env variables
function createConfigEnvFile(vueCliMode, targetPath, {environment}) {
    let fileName = `.env.${vueCliMode}.local`;
    let content = `VUE_APP_ASPNETCORE_ENVIRONMENT=${environment}`;

    return new Promise((resolve, reject) => {
        fs.writeFile(path.join(targetPath, fileName), content, 'utf8', (err) => {
            if(err) {
                return reject(err);
            }
            return resolve(err);
        });
    });
}

module.exports = {
    createConfigEnvFile,
};
