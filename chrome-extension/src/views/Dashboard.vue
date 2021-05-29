<template>
  <div>
    <div v-if="profile" class="alert alert-secondary mb-4" role="alert">
      Welcome back, {{ profile.name }}.
    </div>
    <div class="mb-4">
      <learning-language-selector></learning-language-selector>
      <div>
        <small class="text-secondary"
          >Select language for words you want to add from web pages.</small
        >
      </div>
    </div>
    <div class="mb-4">
      <h6>Last added words:</h6>
      <div>TODO x</div>
    </div>
    <div>
      <button
        type="button"
        class="btn btn-primary"
        v-on:click="onLogoutClick()"
      >
        Logout
      </button>
    </div>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import LearningLanguageSelector from "@/components/LearningLanguageSelector";

export default {
  name: "dashboard",
  components: {
    LearningLanguageSelector,
  },
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
  methods: {
    onLogoutClick: function () {
      authService
        .logoutAsync()
        .then(() => {
          console.log(`Redirecting to home after logout...`);
          this.$router.push({
            name: "home",
          });
        })
        .catch((err) => {
          console.error(`Logout error:`, err);
          // notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
  },
};
</script>

<style scoped lang="scss"></style>
