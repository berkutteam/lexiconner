<template>
    <div class="manage-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.COMPANY_USERS_LOAD]"></row-loader>
                <div v-if="companyUsers">
                    <div class="mb-3">
                        <router-link v-bind:to="{ name: 'my-permissions'}">My permissions</router-link>
                    </div>
                    <div>
                        <h5 class="mb-3">Manage users' permissions</h5>
                        <div v-if="companyUsers">
                            <div class="list-group">
                                <router-link 
                                    v-for="(user) in companyUsers.data"
                                    v-bind:key="user.id"
                                    v-bind:to="{ name: 'manage-company-user-permissions', params: { companyId: $store.getters.currentCompanyId, userId: user.id }}" 
                                    class="list-group-item list-group-item-action"
                                >
                                    <p class="mb-1">
                                        {{user.name}}
                                        <small v-if="user.id === $store.getters.userId" class="badge badge-info">(You)</small>
                                    </p>
                                    <p class="mb-0"><small>{{user.id}}</small></p>
                                    <p class="mb-0"><small>{{user.email}}</small></p>
                                </router-link>
                            </div>
                        </div>
                    </div>
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
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'manage-permissions',
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
            companyUsers: state => {
                if(state.userInfo && state.userInfo.currentCompany) {
                    return state.companyUsers[state.userInfo.currentCompany.id] || null;
                }
                return null;
            },
        }),
    },
    created: async function() {
        if(!this.companyUsers) {
            this.$store.dispatch(storeTypes.COMPANY_USERS_LOAD, {
                companyId: this.$store.getters.currentCompanyId,
                offset: 0, 
                limit: 500,
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

    methods: {
    },
}
</script>
