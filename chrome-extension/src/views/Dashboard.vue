<template>
  <div>
    <div v-if="profile" class="alert alert-secondary mb-4" role="alert">
      Welcome back, {{ profile.name }}.
    </div>
    <div>
      <h6>Last added words:</h6>
      <div>TODO x</div>
    </div>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";

export default {
  name: "dashboard",
  components: {},
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      profile: (state) => state.profile,
    }),

    // store getter
    ...mapGetters([]),
  },
  mounted: function () {},
  updated: function () {},
  beforeDestroy: function () {},
  destroyed: function () {},
  methods: {},
};
</script>

<style scoped lang="scss"></style>
