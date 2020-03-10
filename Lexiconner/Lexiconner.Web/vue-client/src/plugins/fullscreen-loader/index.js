import Vue from 'vue';
import FullscreenLoader from './FullscreenLoader';
import { events } from './events';

/*
* Fullscreen loader blocks the whole screen and idsplays loader with specified title and text
* Similar solution: https://github.com/realdah/vue-blockui
*/
export default {
    install(Vue, options) {
        // register component globally
        Vue.component('app-fullscreen-loader', FullscreenLoader);

        const appFullscreenLoader = (params) => {
            events.$emit('update-params', params);
        };

        Vue.appFullscreenLoader = appFullscreenLoader;
        Vue.prototype['$appFullscreenLoader'] = appFullscreenLoader;
    },
};
