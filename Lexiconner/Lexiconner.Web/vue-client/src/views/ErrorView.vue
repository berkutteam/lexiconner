<template>
  <div class="error-page-wrapper">
    <h3>{{ title }}</h3>
    <p>{{ text }}</p>
    <img class="error-image" src="img/error-robot.png" alt="" />
  </div>
</template>

<script>
"use strict";

import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";

export default {
  name: "error",
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
      title: (state) => state.errorPage.title || "Something went wrong!",
      text: (state) => state.errorPage.text || "Try again or contact support.",
    }),
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  destroyed: function () {
    // reset data, to do not accidentaly show old message on next visit
    this.$store.commit(storeTypes.ERROR_PAGE_DATA_RESET);
  },
};
</script>

<style lang="scss"></style>
