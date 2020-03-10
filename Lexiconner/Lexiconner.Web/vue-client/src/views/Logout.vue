<template>
  <div class="logout-wrapper">
    <!-- <p>Redirecting to logout page...</p> -->
    <i class="fas fa-circle-notch fa-spin loader"></i>
  </div>
</template>

<script>
'use strict';

import authService from '@/services/authService';

export default {
    name: 'logout',
    created: async function() {
        console.log('Logout.vue created');

        authService.logout();
    },
    mounted: function() {
        console.log('Logout.vue mounted');
    },
    updated: function() {
        console.log('Logout.vue updated');
    },
    destroyed: function() {
        console.log('Logout.vue destroyed');
    },
}
</script>

<style lang="scss">
</style>
