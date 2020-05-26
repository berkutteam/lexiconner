<template>
    <div>
        <row-loader v-bind:visible="isLoading"></row-loader>
        <div v-if="companyUsers">
            <div class="mb-3">
                <button 
                    v-if="$store.getters.isUserHasPermissions(['CompanyUserInvite'], $store.getters.currentCompanyId)" 
                    v-on:click="onInviteUserShowClick($event)" type="button" class="btn btn-outline-success">
                    <i class="fas fa-user-plus"></i>
                </button>
            </div>
            <div v-bind:class="{'block-disabled': isLoading}">
                <pagination-wrapper
                    v-bind:paginationResult="companyUsers"
                    v-bind:loadItemsF="loadData"
                >
                    <!-- Card view -->
                    <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                        <div 
                            v-for="(item) in companyUsers.data"
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
                                        <span 
                                            v-if="item.id !== $store.getters.userId && $store.getters.isUserHasPermissions(['CompanyUserManage'], $store.getters.currentCompanyId)" 
                                            v-on:click="deleteUserFromCompany(item.id)"
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
                </pagination-wrapper>

                <modal 
                    name="invite-company-user" 
                    height="auto"
                    width="400px"
                    v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                    v-bind:clickToClose="false"
                >
                    <div class="app-modal">
                        <div class="app-modal-header">
                            <div class="app-modal-title">Invite user to the company</div>
                            <div v-on:click="$modal.hide('invite-company-user')" class="app-modal-close">
                                <i class="fas fa-times"></i>
                            </div>
                        </div>
                        
                        <div class="app-modal-content">
                            <form v-on:submit.prevent="onInviteUser($event)">
                                 <div class="form-group">
                                    <label for="inputEmail">Email address</label>
                                    <input v-model="privateState.invitationModel.email" type="email" class="form-control" id="inputEmail" aria-describedby="emailHelp" placeholder="Enter email" autocomplete="username">
                                    <small id="emailHelp" class="form-text text-muted">Invitation email will be sent to specified address.</small>
                                </div>
                                <div class="form-group">
                                    <label for="">Initial permissions</label>
                                    <multiselect 
                                        v-model="privateState.invitationModel.permissions" 
                                        v-bind:options="permissionList" 
                                        v-bind:multiple="true" 
                                        v-bind:searchable="true" 
                                        v-bind:close-on-select="false" 
                                        v-bind:clear-on-select="false" 
                                        v-bind:preserve-search="true" 
                                        v-bind:show-labels="true" 
                                        v-bind:allow-empty="true" 
                                        v-bind:preselect-first="false"
                                        label="name" 
                                        v-bind:custom-label="permissionLabel"
                                        track-by="permission" 
                                        placeholder="Select permissions" 
                                        v-bind:loading="isPermissionsLoading"
                                        v-bind:disabled="isPermissionsLoading"
                                    >
                                    </multiselect>
                                </div>
                                <div class="form-group">
                                    <label for="">Initial roles</label>
                                    <multiselect 
                                        v-model="privateState.invitationModel.roles" 
                                        v-bind:options="roleList" 
                                        v-bind:multiple="true" 
                                        v-bind:searchable="true" 
                                        v-bind:close-on-select="false" 
                                        v-bind:clear-on-select="false" 
                                        v-bind:preserve-search="true" 
                                        v-bind:show-labels="true" 
                                        v-bind:allow-empty="true" 
                                        v-bind:preselect-first="false"
                                        label="name" 
                                        v-bind:custom-label="roleLabel"
                                        track-by="role" 
                                        placeholder="Select roles" 
                                        v-bind:loading="isPermissionsLoading"
                                        v-bind:disabled="isPermissionsLoading"
                                    >
                                    </multiselect>
                                </div>
                                <loading-button 
                                    type="submit"
                                    v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_USER_INVITE]"
                                    class="btn btn-outline-success btn-block"
                                >Invite</loading-button>
                            </form>
                        </div>
                    </div>
                </modal>
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
import PaginationWrapper from '@/components/PaginationWrapper';

const invitationModelDefault = {
    email: '',
    permissions: [],
    roles: [],
};

export default {
    name: 'dashboard-users',
    components: {
        RowLoader,
        LoadingButton,
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
                invitationModel: {
                    ...invitationModelDefault,
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => state.loading[storeTypes.COMPANY_USERS_LOAD] || state.loading[storeTypes.COMPANY_USER_DELETE],
            companyUsers: function(state) {              
                return state.companyUsers[this.companyId] || null;
            },
            isPermissionsLoading: state => state.loading[storeTypes.AUTH_SCOPED_PERMISSIONS_LOAD],
            permissionList: state => state.auth.scopedPermissions.permissions || [],
            roleList: state => state.auth.scopedPermissions.roles || [],
        }),
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
        permissionLabel(option) {
            return `${option.groupName} - ${option.name}`;
        },
        roleLabel(option) {
            return `${option.name}`;
        },
        loadData: function({offset = 0, limit = 100} = {}) {
            return this.$store.dispatch(storeTypes.COMPANY_USERS_LOAD, {
                companyId: this.companyId,
                offset: offset, 
                limit: limit,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onInviteUserShowClick: function(e) {
            // load available permissions
            if(this.permissionList.length === 0) {
                this.$store.dispatch(storeTypes.AUTH_SCOPED_PERMISSIONS_LOAD, {
                    scopeId: this.$store.getters.currentCompanyId,
                }).then().catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }

            this.$modal.show('invite-company-user');
        },
        onInviteUser: function(e) {
            this.$store.dispatch(storeTypes.COMPANY_USER_INVITE, {
                companyId: this.companyId,
                email: this.privateState.invitationModel.email, 
                permissions: this.privateState.invitationModel.permissions.map(x => x.permission), 
                roles: this.privateState.invitationModel.roles.map(x => x.role), 
            }).then(() => {
                this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Invite has been sent to ${this.privateState.invitationModel.email}!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('invite-company-user');

                // reset
                this.privateState.invitationModel = {
                    ...invitationModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteUserFromCompany: function(userId) {
            if(window.confirm('Delete user from company?')) {
                this.$store.dispatch(storeTypes.COMPANY_USER_DELETE, {
                    companyId: this.companyId,
                    userId: userId, 
                }).then(() => {
                    this.loadData();
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
