/*
* Additional config for vue-cli.
* Run after .env.[mode].(local)? file loaded
* https://cli.vuejs.org/config/#vue-config-js
*/

process.env.VUE_APP_VERSION = require('./package.json').version;

console.log('\n\nreading vue.config.js...');
console.log('process.env.NODE_ENV:', process.env.NODE_ENV);
console.log('process.env.VUE_APP_ASPNETCORE_ENVIRONMENT:', process.env.VUE_APP_ASPNETCORE_ENVIRONMENT);
console.log('\n\n');

if(!process.env.VUE_APP_ASPNETCORE_ENVIRONMENT) {
    throw new Error(`'process.env.VUE_APP_ASPNETCORE_ENVIRONMENT' env variable must be set!`);
}

let defaultConfig = {
    pages: {
        index: {
            // entry for the page
            entry: './src/main.js',
            // the source template
            template: './public/index.html',
            // output as dist/index.html
            filename: 'index.html',
            // chunks to include on this page, by default includes
            // extracted common chunks and vendor chunks.
            // chunks: ['chunk-vendors', 'chunk-common', 'index']
        },
        callback: {
            // entry for the page
            entry: './src/callback.js',
            // the source template
            template: './public/callback.html',
            // output as dist/callback.html
            filename: 'callback.html',
        }
    }
};

let envConfigs = {
    'DevelopmentLocalhost': {
        // BASE_URL
        // '/' in the end will be added by vue-cli
        publicPath: '', 
    },
    'DevelopmentHeroku': {
        // BASE_URL
        // '/' in the end will be added by vue-cli
        publicPath: '', 
    },
    'ProductionHeroku': {
        // BASE_URL
        // '/' in the end will be added by vue-cli
        publicPath: '', 
    },
};

let envConfig = envConfigs[process.env.VUE_APP_ASPNETCORE_ENVIRONMENT];
if(!envConfig) {
    throw new Error(`Can't find config for '${process.env.VUE_APP_ASPNETCORE_ENVIRONMENT}'`);
}

let config = {...defaultConfig, ...envConfig};

module.exports = config;
