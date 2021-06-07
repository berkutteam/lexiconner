<template>
  <div>
    <add-word-popup ref="addWordPopupRef"></add-word-popup>

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
import "@/styles/addWordPopupContentScript.scss";

import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";
import AddWordPopup from "./components/AddWordPopup.vue";

export default {
  name: "App",
  components: {
    AddWordPopup,
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

    // store getter
    ...mapGetters([]),
  },
  methods: {
    showPopup: function (coords, word) {
      console.log(`App.showPopup.`, coords, word);
      this.$refs.addWordPopupRef.open(coords, word);
    },
  },
};
</script>

<!-- Add Multiselect CSS. Can be added as a static asset or inside a component. -->
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>

<style></style>
