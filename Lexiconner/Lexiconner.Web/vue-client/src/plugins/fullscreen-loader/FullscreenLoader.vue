<template>
    <div v-if="privateState.data.isVisible" class="app-fullscreen-loader">
        <div class="app-loader-strip stripes stripes--animated">
        </div>
        <div class="app-loader-content">
            <div class="content-title">{{ privateState.data.title }}</div>
            <div v-if="privateState.data.text" class="content-text">{{ privateState.data.text }}</div>
        </div>
    </div>
</template>


<script>
'use strict';

import { events } from './events';

let defaultData = null;

export default {
    name: 'app-fullscreen-loader',
    components: {

    },
    props: {
        defaultTitle: {
            type: String,
            default: 'Loading...',
        },
        defaultText: {
            type: String,
            default: '',
        },
    },
    data: function() {
        return {
            privateState: {
                data: {
                    ...defaultData
                },
            },
        };
    },
    mounted: function() {
        defaultData = {
            isVisible: false,
            title: this.defaultTitle,
            text: this.defaultText,
        }
        this.privateState.data = {
            ...defaultData,
        };

        events.$on('update-params', this.updateParams);
    },

    methods: {
        updateParams(params) {
            if (typeof params === 'string') {
                params = { 
                    ...defaultData,
                    title: params,
                };
            }
    
            if (typeof params === 'object') {
                params = { 
                    ...defaultData,
                    ...params,
                };
            }

            this.privateState.data = {
                ...params,
            };
        },
    },
}
</script>
