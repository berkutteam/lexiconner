<template>
    <div>
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.GATEWAYS_LOAD]" class="mb-3"></row-loader>
        <div v-if="sharedState.gatewaysData">
            <!-- Card view -->
            <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                <div 
                    v-for="(item) in sharedState.gatewaysData.data"
                    v-bind:key="item.id"
                    class="card bg-light w-md-auto w-100 mr-md-2 mr-0 mb-2" 
                >
                    <!-- <div class="card-header"></div> -->
                    <div class="card-body">
                        <div class="d-flex w-100 justify-content-between align-items-center">
                            <h6 class="card-title">
                                {{ item.name }}
                            </h6>
                            <span v-if="item.isOnline" class="badge badge-success align-self-start ml-1">Online</span>
                            <span v-if="!item.isOnline" class="badge badge-danger align-self-start ml-1">Offline</span>
                        </div>
                       
                        <div class="card-text small">
                            <div>Serial: {{ item.serial }}</div>
                            <div>Last IP: {{ item.lastIP }}</div>
                            <div>Sensors: {{ item.sensors ? item.sensors.length : 0 }}</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
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
    name: 'dashboard-gateways',
    components: {
        RowLoader,
    },
    props: {
        // route props:
        companyId: String,
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
        if(this.companyId) {
            this.$store.dispatch(storeTypes.GATEWAYS_LOAD, {
                companyId: this.companyId,
            }).then().catch(err => {
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
