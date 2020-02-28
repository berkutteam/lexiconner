<template>
  <div class="user-profile-wrapper company-user-profile-wrapper">
    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_LOAD] || sharedState.loading[privateState.storeTypes.COMPANY_USERS_LOAD]" class="mb-3"></row-loader>
    <div>
        <div class="row mb-5">
            <div class="col-12 col-md-5">
                <h5 class="mt-0">Department</h5>
                <div v-if="department" v-bind:class="{'block-disabled': sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_LOAD]}">
                    <div class="card">
                        <div class="card-body">
                            <form v-on:submit.prevent="updateDepartment">
                                <div>
                                    <div class="form-group">
                                        <label for=""><strong>#id</strong></label>
                                        <div>{{ department.id }}</div>
                                    </div>
                                    <div class="form-group">
                                        <label for="departmentModel__name"><strong>Name</strong></label>
                                        <input v-model="privateState.departmentModel.name" id="departmentModel__name" type="text" class="form-control">
                                    </div>
                                    <div class="form-group">
                                        <label for="departmentModel__managerId"><strong>Manager user</strong></label>
                                        <company-user-select
                                            v-model="privateState.departmentModel.managerId"
                                            v-bind:companyId="this.companyId"
                                            v-bind:departmentId="null"
                                        />
                                    </div>
                                </div>
                                <loading-button 
                                    type="submit"
                                    v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_UPDATE]"
                                    class="btn btn-outline-success"
                                >Save</loading-button>
                            </form>
                    </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row mb-5">
            <div class="col-12">
                <h5 class="mt-0 mb-3">Department users</h5>
                <div v-if="departmentUsers">
                    <div class="mb-3">
                        <button 
                            v-on:click="onAddUserToDepartmentClick($event)" type="button" class="btn btn-outline-success">
                            <i class="fas fa-user-plus"></i>
                        </button>
                    </div>
                    <div v-bind:class="{'block-disabled': sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_USERS_LOAD] || sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_USER_DELETE]}">
                        <!-- Card view -->
                        <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                            <div 
                                v-for="(item) in departmentUsers.data"
                                v-bind:key="item.id"
                                class="card bg-light w-md-auto w-100 mr-md-2 mr-0 mb-2" 
                            >
                                <!-- <div class="card-header"></div> -->
                                <div class="card-body">
                                    <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                        <h6 class="card-title mb-0">
                                            <router-link 
                                                v-bind:to="{ name: 'company-user-profile', params: { companyId: companyId, userId: item.id }}" 
                                                class=""
                                            >
                                                {{ item.name }}
                                            </router-link>
                                        </h6>
                                        <div class="ml-2">
                                            <small v-if="item.id === $store.getters.userId" class="badge badge-info mr-1">(You)</small>
                                            <small v-if="item.id === department.managerId" class="badge badge-primary mr-1">(Manager)</small>
                                            <span 
                                                v-if="item.id !== department.managerId"
                                                v-on:click="removeUserFromDepartment(item.id)"
                                                class="text-secondary cursor-pointer"> 
                                                <i class="fas fa-times"></i>
                                            </span>
                                        </div>
                                    </div>
                                
                                    <div class="card-text small">
                                        <div>Email: {{ item.email }}</div>
                                        <div>Phone Number: {{ item.phoneNumber }}</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <modal 
                        name="add-department-user" 
                        height="auto"
                        width="450px"
                        v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                        v-bind:clickToClose="false"
                    >
                        <div class="app-modal">
                            <div class="app-modal-header">
                                <div class="app-modal-title">Add user to department</div>
                                <div v-on:click="$modal.hide('add-department-user')" class="app-modal-close">
                                    <i class="fas fa-times"></i>
                                </div>
                            </div>
                            
                            <div class="app-modal-content">
                                <form v-on:submit.prevent="addUserToDepartment()">
                                    <div class="form-group">
                                        <label for="">User</label>
                                        <company-user-select
                                            v-model="privateState.addUserToDepartmentModel.userId"
                                            v-bind:companyId="this.companyId"
                                            v-bind:departmentId="null"
                                            v-bind:excludedDepartmentIds="[this.departmentId]"
                                        />
                                    </div>
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_DEPARTMENT_USER_ADD]"
                                        class="btn btn-outline-success btn-block"
                                    >Add</loading-button>
                                </form>
                            </div>
                        </div>
                    </modal>
                </div>
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
import CompanyUserSelect from '@/components/CompanyUserSelect';

const departmentModelDefault = {
    name: null,
    managerId: null,
};

const addUserToDepartmentModelDefault = {
    userId: null,
};

export default {
    name: 'company-department',
    components: {
        RowLoader,
        LoadingButton,
        CompanyUserSelect,
    },
    props: {
        // route props:
        companyId: String,
        departmentId: String,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                departmentModel: {
                    ...departmentModelDefault,
                },
                addUserToDepartmentModel: {
                    ...addUserToDepartmentModelDefault
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            department: function(state) {
                if(state.companyDepartment[this.companyId] && state.companyDepartment[this.companyId][this.departmentId]) {
                    return state.companyDepartment[this.companyId][this.departmentId];
                }
                return null;
            },
            departmentUsers: function(state) {              
                return state.departmentUsers[this.departmentId] || null;
            },
        })
    },
    created: async function() {
        // load department
        this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_LOAD, {
            companyId: this.companyId,
            departmentId: this.departmentId,
        }).then().catch(err => {
            console.error(err);
            notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });

        // load department users
        this.loadDepartmentUsers();

        // set initial data
        this.privateState.departmentModel = {
            ...departmentModelDefault,
            ...(this.department || {}),
        };

        // update model on state change
        this.$store.subscribe((mutation, state) => {
            let {type, payload} = mutation;
            if(type === storeTypes.COMPANY_DEPARTMENT_SET) {
                this.privateState.departmentModel = {
                    ...this.privateState.departmentModel,
                    ...(this.department || {}),
                };
            }
        });
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },
    methods: {
        loadDepartmentUsers: function() {
            this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_USERS_LOAD, {
                companyId: this.companyId,
                departmentId: this.departmentId,
                offset: 0, 
                limit: 500,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateDepartment: function() {
            this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_UPDATE, {
                companyId: this.companyId,
                departmentId: this.departmentId,
                data: {
                    name: this.privateState.departmentModel.name,
                    managerId: this.privateState.departmentModel.managerId,
                },
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onAddUserToDepartmentClick: function() {
            this.$modal.show('add-department-user');
        },
        addUserToDepartment: function() {
            this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_USER_ADD, {
                companyId: this.companyId,
                data: {
                    userId: this.privateState.addUserToDepartmentModel.userId,
                    departmentId: this.departmentId,
                },
            }).then(() => {
                this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `User has been added to the department!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('add-department-user');

                // reset
                this.privateState.addUserToDepartmentModel = {
                    ...addUserToDepartmentModelDefault,
                };

                // reload users
                this.loadDepartmentUsers();
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        removeUserFromDepartment: function(userId) {
            if(window.confirm('Delete user from department?')) {
                this.$store.dispatch(storeTypes.COMPANY_DEPARTMENT_USER_DELETE, {
                    companyId: this.companyId,
                    departmentId: this.departmentId,
                    userId: userId, 
                }).then(() => {
                    // reload users
                    this.loadDepartmentUsers();
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
