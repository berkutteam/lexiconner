<template>
    <div class="user-scoped-permissions-wrapper">
        <div v-if="userPermissions">
            <div>
                <div class="mb-1">
                    <h6>
                        <strong>{{userPermissions.scopeName}}:</strong>

                        <!-- Manage button -->
                        <small v-if="userPermissions.scopeId === 'global' && $store.getters.isUserHasPermissions(['GlobalPermissionManage'], userPermissions.scopeId)" class="mb-3">
                            <!-- TODO -->
                        </small>
                        <small v-if="userPermissions.scopeId !== 'global' && $store.getters.isUserHasPermissions(['ScopedPermissionManage'], userPermissions.scopeId)" class="mb-3">
                            (<router-link v-bind:to="{ name: 'manage-company-user-permissions', params: { companyId: userPermissions.scopeId, userId: userId }}" class="">Manage</router-link>)
                        </small>
                    </h6>

                    <!-- Scope permissions -->
                    <div class="list-group mb-1">
                        <li class="list-group-item list-group-item-primary">
                            <div class="d-flex w-100 justify-content-between">
                                <p class="mb-1"><small><strong>Permissions:</strong></small></p>
                                <small>({{userPermissions.permissions.length}})</small>
                            </div>
                            <p
                                v-for="(permissionDisplay) in userPermissions.permissions"
                                v-bind:key="`scope-${userPermissions.scopeId}-${permissionDisplay.permission}`" 
                                class="mb-0"
                            >
                                <small>
                                    {{ permissionDisplay.name }} / {{ permissionDisplay.description }}
                                </small>
                            </p>
                        </li>
                    </div>

                    <!-- Scope roles -->
                    <div class="list-group">
                        <li 
                            v-for="(role) in userPermissions.roles"
                            v-bind:key="`global-${role.name}`"
                            class="list-group-item list-group-item-info mb-1"
                        >
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">'{{role.name}}' role</h5>
                                <small>({{role.permissions.length}})</small>
                            </div>
                            <p class="mb-0"><small><strong>Permissions:</strong></small></p>
                            <p
                                v-for="(permissionDisplay) in role.permissions"
                                v-bind:key="`global-${permissionDisplay.permission}`" 
                                class="mb-0"
                            >
                                <small>
                                    {{ permissionDisplay.name }} / {{ permissionDisplay.description }}
                                </small>
                            </p>
                        </li>
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

export default {
    name: 'user-scoped-permissions',
    props: {
        userId: String,
        userPermissions: Object,
    },
    components: {
    },
    data: function() {
        return {
            privateState: {
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        }),
    },
    created: async function() {
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

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
</style>
