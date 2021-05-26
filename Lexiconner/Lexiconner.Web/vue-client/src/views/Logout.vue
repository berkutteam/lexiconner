<template>
  <div class="logout-wrapper">
    <main-loader v-bind:visible="true" v-bind:topText="`Logout...`" />
  </div>
</template>

<script>
"use strict";

import authService from "@/services/authService";
import MainLoader from "@/components/loaders/MainLoader";

export default {
  name: "logout",
  components: {
    MainLoader,
  },
  created: async function () {
    console.log("Logout.vue created");
    authService.logout();
  },
  mounted: function () {
    console.log("Logout.vue mounted");
  },
  updated: function () {
    console.log("Logout.vue updated");
  },
  destroyed: function () {
    console.log("Logout.vue destroyed");
  },
};
</script>

<style lang="scss"></style>
