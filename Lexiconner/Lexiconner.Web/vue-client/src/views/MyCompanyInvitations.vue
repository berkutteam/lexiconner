<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.MY_COMPANY_INVITATIONS_LOAD]"></row-loader>

                <div v-if="myCompanyInvitations">
                    <h5>My invitations:</h5>
                    <ul v-bind:class="{'block-disabled': sharedState.loading[privateState.storeTypes.MY_COMPANY_INVITATIONS_LOAD]}" class="list-group">
                        <li 
                            v-for="(item) in myCompanyInvitations" 
                            v-bind:key="item.id"
                            class="list-group-item"
                        >
                            <div class="d-flex w-100 justify-content-between">
                                <div>
                                    <h5 class="mb-1">{{item.companyName}}</h5>
                                    
                                    <div class="mb-1">
                                        <div><small><strong>Permissions ({{item.permissions.length}}):</strong></small></div>
                                        <div
                                            v-for="(permissionDisplay) in item.permissions"
                                            v-bind:key="`permission-${permissionDisplay.permission}`" 
                                            class="mb-0"
                                        >
                                            <small>
                                                {{ permissionDisplay.name }} / {{ permissionDisplay.description }}
                                            </small>
                                        </div>
                                    </div>

                                   <div>
                                        <div><small><strong>Roles ({{item.roles.length}}):</strong></small></div>
                                        <div
                                            v-for="(roleDisplay) in item.roles"
                                            v-bind:key="`role-${roleDisplay.role}`" 
                                            class="mb-0"
                                        >
                                            <small>
                                                {{ roleDisplay.name }}
                                            </small>
                                        </div>
                                   </div>
                                </div>
                                <div>
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_USER_INVITATION_ACCEPT]"
                                        v-on:click.native="acceptInvitation(item.id)"
                                        class="btn btn-outline-success mr-2"
                                    >Accept</loading-button>
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_USER_INVITATION_REJECT]"
                                        v-on:click.native="rejectInvitation(item.id)"
                                        class="btn btn-outline-danger"
                                    >Reject</loading-button>
                                </div>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'my-company-invitations',
    components: {
        RowLoader,
        LoadingButton,
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
            myCompanyInvitations: state => state.myCompanyInvitations,
        }),
    },
    created: async function() {
       this.loadInitialData();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadInitialData: function() {
            this.$store.dispatch(storeTypes.MY_COMPANY_INVITATIONS_LOAD, {}).then(() => {
            }).catch((err) => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        acceptInvitation: function(invitationId) {
            this.$store.dispatch(storeTypes.COMPANY_USER_INVITATION_ACCEPT, {
                invitationId: invitationId,
            }).then(() => {
                this.loadInitialData();

                // reload info
                this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});
            }).catch((err) => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        rejectInvitation: function(invitationId) {
            this.$store.dispatch(storeTypes.COMPANY_USER_INVITATION_REJECT, {
                invitationId: invitationId,
            }).then(() => {
                this.loadInitialData();
            }).catch((err) => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
