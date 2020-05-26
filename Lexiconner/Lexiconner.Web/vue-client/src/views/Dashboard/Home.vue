<template>
    <div>
        <img src="img/logo.png" alt="" />
    </div>
</template>

<script>
'use strict';

import '@/styles/index.scss';

import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'dashboard-home',
    components: {
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        })
    },
    created: async function() {
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },
}
</script>

<style lang="scss">
</style>
