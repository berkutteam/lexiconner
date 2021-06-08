<template>
  <div class="user-profile-wrapper">
    <row-loader
      v-bind:visible="
        sharedState.loading[privateState.storeTypes.AUTH_USER_ACCOUNT_LOAD] ||
        sharedState.loading[privateState.storeTypes.USER_INFO_LOAD]
      "
    ></row-loader>
    <div v-if="sharedState.auth.user && sharedState.auth.user.profile">
      <h5>Main info</h5>
      <div class="row mb-5">
        <div class="col-5">
          <div class="list-group">
            <li class="list-group-item border-gray">
              <div
                class="d-flex w-100 justify-content-start align-items-center"
              >
                <div class="w-100">
                  <form v-on:submit.prevent="updateProfile">
                    <div>
                      <div class="media">
                        <!-- <img src="img/user.png" class="align-self-start mr-3"> -->
                        <div class="media-body">
                          <div>
                            <!-- <div class="form-group">
                              <label for=""><strong>#id</strong></label>
                              <div>{{ sharedState.auth.user.profile.sub }}</div>
                            </div> -->

                            <div class="form-group">
                              <label for=""><strong>Email</strong></label>
                              <input
                                v-model="privateState.profileModel.email"
                                type="text"
                                class="form-control"
                                disabled
                              />
                            </div>

                            <div class="form-group">
                              <label for=""><strong>Name</strong></label>
                              <input
                                v-model="privateState.profileModel.name"
                                type="text"
                                class="form-control"
                              />
                            </div>

                            <div class="form-group">
                              <label for=""
                                ><strong>Native language</strong></label
                              >
                              <language-code-select
                                v-model="
                                  privateState.profileModel.nativeLanguageCode
                                "
                                placeholder="Native language"
                                v-bind:languageLabelGetter="
                                  (option) => `${option.isoLanguageName}`
                                "
                                v-bind:withFlags="true"
                              />
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                    <loading-button
                      type="submit"
                      v-bind:loading="
                        sharedState.loading[
                          privateState.storeTypes.PROFILE_UPDATE
                        ]
                      "
                      class="btn custom-btn-normal"
                      >Save</loading-button
                    >
                  </form>
                </div>
              </div>
            </li>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
"use strict";

import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import languageUtil from "@/utils/language";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";
import TimeInput from "@/components/TimeInput";
import LanguageCodeSelect from "@/components/LanguageCodeSelect";

const profileModelDefault = {
  email: null,
  name: null,
  nativeLanguageCode: null,
};

export default {
  name: "user-profile",
  components: {
    RowLoader,
    LoadingButton,
    LanguageCodeSelect,
  },
  props: {},
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        profileModel: {
          ...profileModelDefault,
        },
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      authUser: (state) => state.auth.user,
      profile: (state) => state.profile,
    }),
  },
  created: async function () {
    // load account
    if (!this.profile) {
      this.$store.dispatch(storeTypes.PROFILE_LOAD);
    }

    // set initial data
    this.privateState.profileModel = {
      ...profileModelDefault,
      ...(this.profile || {}),
    };
    this.setInitialNativeLanguageCode();
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},
  watch: {
    profile: function (newValue, oldValue) {
      if (oldValue === null) {
        this.privateState.profileModel = {
          ...this.privateState.profileModel,
          ...newValue,
        };
        this.setInitialNativeLanguageCode();
      }
    },
  },
  methods: {
    setInitialNativeLanguageCode: function () {
      if (!this.privateState.profileModel.nativeLanguageCode) {
        const langs = languageUtil.detectBrowserLanguages();
        if (langs && langs.length !== 0) {
          this.privateState.profileModel.nativeLanguageCode = langs[0];
        }
      }
    },
    updateProfile: function () {
      this.$store
        .dispatch(storeTypes.PROFILE_UPDATE, {
          data: {
            ...this.privateState.profileModel,
          },
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Profile updated`,
            text: "",
            duration: 5000,
          });

          authService.refreshTokens({ withFullscreenLoader: false });
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
  },
};
</script>

<style lang="scss"></style>
