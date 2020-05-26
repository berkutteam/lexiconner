<template>
  <div class="login-wrapper">
    <!-- <p>Redirecting to login page...</p> -->
    <i class="fas fa-circle-notch fa-spin loader"></i>
  </div>
</template>

<script>
'use strict';

import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';

export default {
    name: 'login',
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

