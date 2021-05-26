<template>
  <div id="app">
    <div>
      <transition>
        <!-- dynamically render component defined in layout -->
        <component v-bind:is="layout">
          <router-view></router-view>
        </component>
      </transition>
    </div>

    <time-zone-auto-detect />

    <!-- notifications. each group - separate puprose. -->

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

    <app-fullscreen-loader defaultTitle="Loading..." defaultText="" />
  </div>
</template>

<script>
"use strict";

import "@/styles/index.scss";

import { mapState, mapGetters } from "vuex";
import authService from "@/services/authService";
import TimeZoneAutoDetect from "@/components/TimeZoneAutoDetect";

const defaultLayout = "default";

export default {
  name: "app",
  components: {
    TimeZoneAutoDetect,
  },
  data: function () {
    return {
      privateState: {},
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
    }),

    // reads layout from route meta
    layout() {
      return `${this.$route.meta.layout || defaultLayout}-layout`;
    },
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},
};
</script>

<!-- Add Multiselect CSS. Can be added as a static asset or inside a component. -->
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>

<!-- vue-datetimeut -->
<style src="vue-datetime/dist/vue-datetime.css"></style>

<style lang="scss"></style>
