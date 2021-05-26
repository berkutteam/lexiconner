<template>
  <div class="container-fluid app-router-view-wrapper">
    <router-view></router-view>
    <!-- <hello-world /> -->
  </div>
</template>

<script>
"use strict";

import "@/styles/popup.scss";

import HelloWorld from "@/components/HelloWorld.vue";

export default {
  name: "App",
  components: {},
};
</script>

<style></style>
