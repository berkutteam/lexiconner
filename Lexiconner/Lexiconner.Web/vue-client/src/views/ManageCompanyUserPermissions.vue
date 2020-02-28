<template>
    <div class="manage-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <div class="mb-3">
                    <a v-on:click="$router.go(-1)" href="javascript:;"><i class="fas fa-long-arrow-alt-left"></i> Back</a>
                </div>
                <div>
                    <h5 v-if="userPermissions" class="mb-3">Manage {{userPermissions.name}} permissions</h5>
                    <div class="mb-2">
                        <div><strong>Permissions:</strong></div>
                        <multiselect 
                            v-model="privateState.selectedPermissions" 
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
                            v-on:input="onScopedPermissionsChange"
                            v-bind:loading="isPermissionsLoading"
                            v-bind:disabled="isPermissionsLoading"
                        >
                        </multiselect>
                    </div>
                    <div class="mb-2">
                        <div><strong>Roles:</strong></div>
                        <multiselect 
                            v-model="privateState.selectedRoles" 
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
                            v-on:input="onScopedRolesChange"
                            v-bind:loading="isPermissionsLoading"
                            v-bind:disabled="isPermissionsLoading"
                        >
                        </multiselect>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'manage-user-permissions',
    components: {
        RowLoader,
        LoadingButton,
    },
    props: {
        // route props:
        companyId: String,
        userId: String,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                selectedPermissions: [],
                selectedRoles: [],
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isPermissionsLoading: state => state.loading[storeTypes.AUTH_SCOPED_PERMISSIONS_LOAD] || state.loading[storeTypes.AUTH_USER_SCOPED_PERMISSIONS_LOAD] || state.loading[storeTypes.AUTH_USER_SCOPED_PERMISSIONS_UPDATE],
            permissionList: state => state.auth.scopedPermissions.permissions || [],
            roleList: state => state.auth.scopedPermissions.roles || [],
            userPermissions: function(state) {
                if(state.auth.userScopedPermissions[this.companyId] && state.auth.userScopedPermissions[this.companyId][this.userId]) {
                    return state.auth.userScopedPermissions[this.companyId][this.userId];
                }
                return null;
            },
        }),
    },
    created: async function() {
        await this.loadInitialData();
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
        loadInitialData: async function() {
            // load available permissions
            if(this.permissionList.length === 0) {
                this.$store.dispatch(storeTypes.AUTH_SCOPED_PERMISSIONS_LOAD, {
                    scopeId: this.$store.getters.currentCompanyId,
                }).then().catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }

            // load user permissions
            this.$store.dispatch(storeTypes.AUTH_USER_SCOPED_PERMISSIONS_LOAD, {
                scopeId: this.$store.getters.currentCompanyId,
                userId: this.userId,
            }).then(data => {
                // set initially selected values
                this.privateState.selectedPermissions = data.permissions;
                this.privateState.selectedRoles = data.roles;
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updatePermissions: _.debounce(function({permissions = [], roles = []}) {
            this.$store.dispatch(storeTypes.AUTH_USER_SCOPED_PERMISSIONS_UPDATE, {
                scopeId: this.$store.getters.currentCompanyId,
                userId: this.userId, 
                permissions: permissions.map(x => x.permission),
                roles: roles.map(x => x.role),
            }).then(data => {

            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);

                // reload data
                this.loadInitialData();
            });
        }, 1500),
        onScopedPermissionsChange: function(values) {
            this.updatePermissions({permissions: this.privateState.selectedPermissions, roles: this.privateState.selectedRoles});
        },
        onScopedRolesChange: function(values) {
            this.updatePermissions({permissions: this.privateState.selectedPermissions, roles: this.privateState.selectedRoles});
        },
    },
}
</script>


