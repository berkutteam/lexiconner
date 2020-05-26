<template>
  <div class="user-profile-wrapper company-user-profile-wrapper">
    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.COMPANY_USER_LOAD] || sharedState.loading[privateState.storeTypes.AUTH_USER_SCOPED_PERMISSIONS_LOAD]" class="mb-3"></row-loader>
    <div v-if="user">
        <h5 class="mt-0 mb-3">User info:</h5>
        <div class="media d-flex flex-wrap flex-md-nowrap">
            <img src="img/user.png" class="align-self-start mr-3 mb-3">
            <div class="media-body">
                <div>
                    <dl>
                        <dt>#</dt>
                        <dd>{{ user.id }}</dd>
                    
                        <dt>Email</dt>
                        <dd>{{ user.email }}</dd>

                        <dt>Phone number</dt>
                        <dd>{{ user.phoneNumber }}</dd>
                    
                        <dt>Name</dt>
                        <dd>{{ user.name }}</dd>
                    </dl>
                </div>
                <h5 class="mt-0">Permissions:</h5>
                <div>
                    <user-scoped-permissions v-bind:userId="user.id" v-bind:userPermissions="userPermissions"></user-scoped-permissions>
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
import UserScopedPermissions from '@/components/UserScopedPermissions';

export default {
    name: 'company-user-rofile',
    components: {
        RowLoader,
        UserScopedPermissions,
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
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            user: function(state) {
                if(state.companyUser[this.companyId] && state.companyUser[this.companyId][this.userId]) {
                    return state.companyUser[this.companyId][this.userId];
                }
                return null;
            },
            userPermissions: function(state) {
                if(state.auth.userScopedPermissions[this.companyId] && state.auth.userScopedPermissions[this.companyId][this.userId]) {
                    return state.auth.userScopedPermissions[this.companyId][this.userId];
                }
                return null;
            },
        })
    },
    created: async function() {
        // load user
        this.$store.dispatch(storeTypes.COMPANY_USER_LOAD, {
            companyId: this.companyId,
            userId: this.userId,
        }).then().catch(err => {
            console.error(err);
            notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });

        // load user permissions
        this.$store.dispatch(storeTypes.AUTH_USER_SCOPED_PERMISSIONS_LOAD, {
            scopeId: this.companyId,
            userId: this.userId,
        }).then().catch(err => {
            console.error(err);
            notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
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
