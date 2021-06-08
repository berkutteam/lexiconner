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
      <div v-if="lastAddedWords">
        <div v-if="lastAddedWords.length === 0">
          <small>You haven't added any words yet with the extension.</small>
        </div>
        <div v-for="word in lastAddedWords" v-bind:key="`word-${word.id}`">
          {{ word.word }} - {{ word.meaning }}
        </div>
      </div>
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
import notificationUtil from "@/utils/notification";
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
      lastAddedWords: (state) => state.lastAddedWords,
    }),

    // store getter
    ...mapGetters(["selectedLearningLanguageCode"]),
  },
  mounted: function () {
    this.loadLastAddedWords();
  },
  updated: function () {},
  beforeDestroy: function () {},
  destroyed: function () {},
  watch: {
    profile: function (newValue, oldValue) {
      this.loadLastAddedWords();
    },
  },
  methods: {
    loadLastAddedWords: function () {
      if (
        this.lastAddedWords !== null ||
        this.selectedLearningLanguageCode === null
      ) {
        return;
      }

      return this.$store
        .dispatch(storeTypes.WORD_LAST_ADDED_LOAD, {
          wordLanguageCode: this.selectedLearningLanguageCode,
          limit: 5,
        })
        .then()
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
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
