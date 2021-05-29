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

<!-- Add Multiselect CSS. Can be added as a static asset or inside a component. -->
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>

<style></style>
