<template>
    <div class="dashboard-wrapper">
        <!-- <row-loader v-bind:visible="!$store.getters.currentCompanyId"></row-loader> -->
        <div>
            <current-company-control></current-company-control>
        </div>
        <div v-if="$store.getters.currentCompanyId">
            <ul class="nav nav-tabs app-nav-tabs" id="myTab" role="tablist">
                <li class="nav-item">
                    <router-link v-bind:to="{ name: 'dashboard-home', params: {}}" v-bind:class="{'active': $router.currentRoute.name === 'dashboard-home'}" class="nav-link">Dashboard</router-link>
                </li>
                <li class="nav-item">
                    <router-link v-bind:to="{ name: 'dashboard-gateways', params: {companyId: $store.getters.currentCompanyId}}" v-bind:class="{'active': $router.currentRoute.name === 'dashboard-gateways'}" class="nav-link">Gateways</router-link>
                </li>
                <li class="nav-item">
                    <router-link v-bind:to="{ name: 'dashboard-sensors', params: {companyId: $store.getters.currentCompanyId}}" v-bind:class="{'active': $router.currentRoute.name === 'dashboard-sensors'}" class="nav-link">Sensors</router-link>
                </li>
                <li class="nav-item">
                    <router-link v-bind:to="{ name: 'dashboard-departments', params: {companyId: $store.getters.currentCompanyId}}" v-bind:class="{'active': $router.currentRoute.name === 'dashboard-departments'}" class="nav-link">Departments</router-link>
                </li>
                <li
                    v-if="$store.getters.isUserHasPermissions(['CompanyUserRead'], $store.getters.currentCompanyId)"
                    class="nav-item"
                >
                    <router-link v-bind:to="{ name: 'dashboard-users', params: {companyId: $store.getters.currentCompanyId}}" v-bind:class="{'active': $router.currentRoute.name === 'dashboard-users'}" class="nav-link">Users</router-link>
                </li>
            </ul>
            <div v-if="sharedState.userInfo" class="tab-content app-tab-content" id="myTabContent">
                <div class="tab-pane fade show active" id="" role="" aria-labelledby="">
                    <router-view></router-view>
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
import CurrentCompanyControl from '@/components/CurrentCompanyControl';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'dashboard',
    components: {
        CurrentCompanyControl,
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
