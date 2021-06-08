<template>
  <div class="dashboard-wrapper">
    <!-- <row-loader v-bind:visible="!$store.getters.currentCompanyId"></row-loader> -->
  </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

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
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {},
};
</script>
