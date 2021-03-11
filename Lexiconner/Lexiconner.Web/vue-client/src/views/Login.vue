<template>
  <div class="login-wrapper">
    <main-loader v-bind:visible="true" v-bind:topText="`Redirecting to login page...`" />
  </div>
</template>

<script>
'use strict';

import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import MainLoader from '@/components/loaders/MainLoader';

export default {
    name: 'login',
    components: {
        MainLoader,
    },
    created: async function() {
        console.log('Login.vue created');

        if((await authService.isAuthenticated()) === false) {
            authService.login();
        }
    },
    mounted: function() {
        console.log('Login.vue mounted');
    },
    updated: function() {
        console.log('Login.vue updated');
    },
    destroyed: function() {
        console.log('Login.vue destroyed');
    },
}
</script>

<style lang="scss">
</style>

