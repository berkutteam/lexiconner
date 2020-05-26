<template>
    <div>
        <row-loader v-bind:visible="isLoading" class="mb-3"></row-loader>
        <div v-if="departments">
             <div class="mb-3">
                <button 
                    v-if="$store.getters.isUserHasPermissions(['CompanyDepartmentManage'], $store.getters.currentCompanyId)" 
                    v-on:click="onCreateDepartment" type="button" class="btn btn-outline-success">
                    <i class="fas fa-building"></i>
                    <small><i class="fas fa-plus"></i></small>
                </button>
            </div>
            <div v-bind:class="{'block-disabled': isLoading}">
                <div class="row">
                    <div class="col">
                        <!-- Card view -->
                        <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                            <div 
                                v-for="(item) in departments"
                                v-bind:key="item.id"
                                class="card bg-light w-md-auto w-100 mr-md-2 mr-0 mb-2" 
                            >
                                <!-- <div class="card-header"></div> -->
                                <div class="card-body">
                                    <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                        <h6 class="card-title mb-0">
                                            <router-link 
                                                v-if="$store.getters.isUserHasPermissions(['CompanyDepartmentManage'], $store.getters.currentCompanyId)" 
                                                v-bind:to="{ name: 'company-department', params: { companyId: companyId, departmentId: item.id }}" 
                                                class=""
                                            >{{ item.name }}</router-link>
                                            <span
                                                v-if="!$store.getters.isUserHasPermissions(['CompanyDepartmentManage'], $store.getters.currentCompanyId)" 
                                            >{{item.name}}</span>
                                        </h6>
                                        <div class="ml-2">
                                            <span 
                                                v-if="$store.getters.isUserHasPermissions(['CompanyDepartmentManage'], $store.getters.currentCompanyId)" 
                                                v-on:click="deleteDepartment(item.id)"
                                                class="text-secondary cursor-pointer"> 
                                                <i class="fas fa-times"></i>
                                            </span>
                                        </div>
                                    </div>
                                
                                    <div class="card-text small">
                                        <!-- <div>#: {{ item.id }}</div> -->
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <modal 
                name="create-department" 
                height="auto"
                width="450px"
                v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                v-bind:clickToClose="false"
            >
                <div class="app-modal">
                    <div class="app-modal-header">
                        <div class="app-modal-title">Create department</div>
                        <div v-on:click="$modal.hide('create-department')" class="app-modal-close">
                            <i class="fas fa-times"></i>
                        </div>
                    </div>
                    
                    <div class="app-modal-content">
                        <form v-on:submit.prevent="createDepartment()">
                                <div class="form-group">
                                <label for="departmentModel__name">Name</label>
                                <input v-model="privateState.departmentModel.name" type="text" class="form-control" id="departmentModel__name" placeholder="Name" />
                                <small id="" class="form-text text-muted">Department name must be unique across the company.</small>
                            </div>
                            <div class="form-group">
                                <label for="">Manager user</label>
                                <company-user-select
                                    v-model="privateState.departmentModel.managerId"
                                    v-bind:companyId="this.companyId"
                                    v-bind:departmentId="null"
                                />
                            </div>
                            <loading-button 
                                type="submit"
                                v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_CREATE]"
                                class="btn btn-outline-success btn-block"
                            >Create</loading-button>
                        </form>
                    </div>
                </div>
            </modal>
        </div>
    </div>
</template>

<script>
'use strict';

import '@/styles/index.scss';

import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import CompanyUserSelect from '@/components/CompanyUserSelect';
import PaginationWrapper from '@/components/PaginationWrapper';

const departmentModelDefault = {
    name: null,
    managerId: null,
};

export default {
    name: 'dashboard-departments',
    components: {
        RowLoader,
        LoadingButton,
        CompanyUserSelect,
    },
    props: {
        // route props:
        companyId: String,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                departmentModel: {
                    ...departmentModelDefault,
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => {
                return state.loading[storeTypes.COMPANY_DEPARTMENTS_LOAD] || 
                       state.loading[storeTypes.COMPANY_DEPARTMENT_CREATE] ||
                       state.loading[storeTypes.COMPANY_DEPARTMENT_UPDATE] ||
                       state.loading[storeTypes.COMPANY_DEPARTMENT_DELETE];
            },
            departments: state => {
                if(state.userInfo && state.userInfo.currentCompany) {
                    return state.companyDepartments[state.userInfo.currentCompany.id] || null;
                }
                return null;
            },
        })
    },
    created: async function() {
        this.loadData();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },
    methods: {
        loadData: function() {
            if(this.companyId) {
                this.$store.dispatch(storeTypes.COMPANY_DEPARTMENTS_LOAD, {
                    companyId: this.companyId,
                }).then().catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },
        onCreateDepartment: function() {
            this.$modal.show('create-department');
        },
        createDepartment: function() {
            this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_CREATE, {
                companyId: this.companyId,
                data: {
                    name: this.privateState.departmentModel.name,
                    managerId: this.privateState.departmentModel.managerId,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Department '${this.privateState.departmentModel.name}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('create-department');

                // reset
                this.privateState.departmentModel = {
                    ...departmentModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteDepartment: function(departmentId) {
            if(window.confirm('Delete department?')) {
                this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_DELETE, {
                    companyId: this.companyId,
                    departmentId: departmentId,
                }).then(() => {
                    this.$notify({
                        group: 'app',
                        type: 'information',
                        title: `Department has been deleted!`,
                        text: '',
                        duration: 5000,
                    });
                }).catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },
    },
}
</script>

<style lang="scss">
</style>
