<template>
    <div>
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.SENSORS_LOAD]" class="mb-3"></row-loader>
        <div v-if="sharedState.sensors">
            <!-- Department filter -->
            <div class="row mb-3">
                <div class="col-md-3 col-sm-12">
                    <div class="d-flex">
                        <multiselect 
                            v-bind:id="'selectedDepartment'"
                            v-model="privateState.selectedDepartment" 
                            v-bind:options="departmentList" 
                            v-bind:multiple="false" 
                            v-bind:searchable="true" 
                            v-bind:close-on-select="true" 
                            v-bind:clear-on-select="true" 
                            v-bind:preserve-search="true" 
                            v-bind:show-labels="true" 
                            v-bind:allow-empty="false" 
                            v-bind:preselect-first="false"
                            label="name" 
                            track-by="id" 
                            placeholder="Select department" 
                            v-bind:loading="isDepartmentsLoading"
                            v-bind:disabled="isDepartmentsLoading"
                            v-on:input="onDepartmentChange"
                        >
                        </multiselect>
                    </div>
                </div>
            </div>
            <div class="row mb-5">
                <div class="col">
                    <pagination-wrapper
                        v-bind:paginationResult="sharedState.sensors"
                        v-bind:loadItemsF="loadSensors"
                    >
                        <!-- Card view -->
                        <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                            <div 
                                v-for="(item) in sharedState.sensors.data"
                                v-bind:key="item.id"
                                class="card bg-light w-md-auto w-100 mr-md-2 mr-0 mb-2" 
                            >
                                <!-- <div class="card-header"></div> -->
                                <div class="card-body">
                                    <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                        <h6 class="card-title mb-0">
                                            <router-link v-bind:to="{name: 'sensor-telemetry', params: {sensorId: item.id}}" class="">{{ item.name }}</router-link>
                                        </h6>
                                        <div class="">
                                            <span v-if="item.isOnline" class="badge badge-success align-self-start ml-1">Online</span>
                                            <span v-if="!item.isOnline" class="badge badge-danger align-self-start ml-1">Offline</span>
                                            <router-link 
                                                v-bind:to="{ name: 'sensor-manage', params: {companyId: $store.getters.currentCompanyId, sensorId: item.id}}" 
                                                class="badge badge-secondary align-self-start ml-1"
                                            >
                                                <i class="fas fa-cog"></i>
                                            </router-link>
                                        </div>
                                    </div>
                                
                                    <div class="card-text small">
                                        <div>Serial: {{ item.serial }}</div>
                                        <div>Type: {{ item.type }}</div>
                                        <div>Last IP: {{ item.lastIP }}</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </pagination-wrapper>
                </div>
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
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';

export default {
    name: 'dashboard-sensors',
    components: {
        RowLoader,
        PaginationWrapper,
    },
    props: {
        // route props:
        companyId: String,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                selectedDepartment: null,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isDepartmentsLoading: state => state.loading[storeTypes.COMPANY_DEPARTMENTS_LOAD],
            departmentList: function(state) {
                let list = state.companyDepartments[this.companyId] || [];
                return [
                    {
                        id: null,
                        name: 'All departments'
                    },
                    ...list,
                ]
        },
        })
    },
    created: async function() {
        // load departments
        if(this.companyId) {
            this.$store.dispatch(storeTypes.COMPANY_DEPARTMENTS_LOAD, {
                companyId: this.companyId,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        // load sensors
        this.loadSensors();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadSensors: function({offset = 0, limit = 100} = {}) {
            if(this.companyId) {
                this.$store.dispatch(storeTypes.SENSORS_LOAD, {
                    companyId: this.companyId,
                    departmentId: ((this.privateState.selectedDepartment || {}).id || null),
                    offset: offset,
                    limit: limit,
                }).then().catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },
        onDepartmentChange: function(values) {
            this.loadSensors();
        },
    },
}
</script>

<style lang="scss">
</style>
