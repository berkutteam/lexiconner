<template>
  <div class="container-fluid app-router-view-wrapper">
    <router-view></router-view>
    <!-- <hello-world /> -->

    <!-- #region notifications. each group - separate puprose. -->

    <!-- should be notable -->
    <notifications group="error" position="top center" v-bind:duration="7000" />

    <!-- less important. just information -->
    <notifications group="app" position="top right" />

    <!-- important information -->
    <notifications
      group="app-important"
      position="top center"
      v-bind:duration="5000"
    />

    <!-- #endregion -->
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
