<template>
    <div class="dashboard-wrapper">
        <!-- <row-loader v-bind:visible="!$store.getters.currentCompanyId"></row-loader> -->

        <ul class="nav">
            <li class="nav-item">
                <router-link v-bind:to="{ name: 'study-items-browse', params: {}}" class="nav-link">Browse items</router-link>
            </li>
            <li class="nav-item">
                <router-link v-bind:to="{ name: 'study-items-learn-cards', params: {}}" class="nav-link">Cards</router-link>
            </li>
        </ul>

        <!-- TODO -->
        <div class="mb-2">
            TODO: display stats
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

export default {
    name: 'study-items-dashboard',
    components: {
        // RowLoader,
        // LoadingButton,
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
