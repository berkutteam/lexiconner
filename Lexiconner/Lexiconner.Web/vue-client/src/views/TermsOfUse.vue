<template>
    <div class="terms-of-use-page-wrapper">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.TERMS_OF_USE_LOAD]"></row-loader>
        <div v-if="sharedState.termsOfUse">
            <div v-html="sharedState.termsOfUse.text">
            </div>
        </div>
    </div>
</template>

<script>
'use strict';

import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import utils from '@/utils/index';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'terms-of-use',
    components: {
        RowLoader,
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
        }),
    },
    created: async function() {
        if(!this.sharedState.termsOfUse) {
            this.$store.dispatch(storeTypes.TERMS_OF_USE_LOAD, {}).then((data) => {
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }
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
