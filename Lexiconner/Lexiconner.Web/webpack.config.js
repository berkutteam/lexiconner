const path = require('path');

module.exports = {
    mode: 'development', // development, production, none
    entry: './ClientVanilaJs/index.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve(__dirname, 'wwwroot/dist')
    },
    devtool: 'eval-source-map',
    // watch: true
};