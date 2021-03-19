<template>
    <div class="">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.AUTH_MY_PERMISSIONS_LOAD]"></row-loader>

                <div v-if="myPermissions">
                    <div v-if="$store.getters.isUserHasPermissions(['ScopedPermissionManage'], this.$store.getters.currentCompanyId)" class="mb-3">
                        <router-link v-bind:to="{ name: 'manage-company-permissions', params: { companyId: this.$store.getters.currentCompanyId }}" class="">Manage company permissions</router-link>
                    </div>
                    <div>
                        <h5 class="mb-3">My permissions</h5>
                        <div>
                            <!-- Global permissions and roles -->
                            <div v-if="myPermissions.global" class="mb-3">
                                <div>
                                    <user-scoped-permissions v-bind:userId="$store.getters.userId" v-bind:userPermissions="myPermissions.global"></user-scoped-permissions>
                                </div>
                            </div>
                            <!-- Scoped permissions and roles -->
                            <div v-if="myPermissions.scoped" class="mb-1">
                                <div 
                                    v-for="(pscope) in myPermissions.scoped" 
                                    v-bind:key="pscope.scopeId"
                                    class=""
                                >
                                    <div>
                                        <user-scoped-permissions v-bind:userId="$store.getters.userId" v-bind:userPermissions="pscope"></user-scoped-permissions>
                                    </div>
                                </div>
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
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import UserScopedPermissions from '@/components/UserScopedPermissions';

export default {
    name: 'my-permissions',
    components: {
        RowLoader,
        UserScopedPermissions,
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
            myPermissions: state => state.auth.myPermissions,
        }),
    },
    created: async function() {
        // user permissions are loaded during startup
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
