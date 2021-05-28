<template>
  <div>
    <h5>Dashboard</h5>
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
