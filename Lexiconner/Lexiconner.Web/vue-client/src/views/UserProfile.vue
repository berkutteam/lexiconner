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
                  <form v-on:submit.prevent="updateUser">
                    <div>
                      <div class="media">
                        <!-- <img src="img/user.png" class="align-self-start mr-3"> -->
                        <div class="media-body">
                          <div>
                            <div class="form-group">
                              <label for=""><strong>#id</strong></label>
                              <div>{{ sharedState.auth.user.profile.sub }}</div>
                            </div>

                            <div class="form-group">
                              <label for=""><strong>Email</strong></label>
                              <div>
                                {{ sharedState.auth.user.profile.email }}
                              </div>
                            </div>

                            <div class="form-group">
                              <label for=""
                                ><strong>Given name (username)</strong></label
                              >
                              <div>
                                {{ sharedState.auth.user.profile.given_name }}
                              </div>
                            </div>

                            <div class="form-group">
                              <label for="userInfoModel__phone"
                                ><strong>Phone</strong></label
                              >
                              <vue-tel-input
                                v-model="
                                  privateState.userAccountModel.phoneNumber
                                "
                                v-bind:mode="'international'"
                                v-bind:disabledFetchingCountry="false"
                                v-bind:disabledFormatting="false"
                                v-bind:enabledCountryCode="true"
                                v-bind:inputClasses="[]"
                                v-bind:enabledFlags="true"
                                v-bind:required="true"
                                v-on:validate="onPhoneNumberValidate"
                              >
                              </vue-tel-input>
                            </div>
                            <div v-if="isPhoneChanged" class="form-group">
                              <div>
                                <label
                                  for="userInfoModel__phoneNumberConfirmationToken"
                                  ><strong>Phone confirmation</strong></label
                                >
                                <loading-button
                                  type="button"
                                  v-bind:loading="
                                    sharedState.loading[
                                      privateState.storeTypes
                                        .AUTH_USER_ACCOUNT_PHONE_CHANGE_SMS_TOKEN_SEND
                                    ]
                                  "
                                  v-on:click.native="
                                    sendSmsPhoneNumberChangeToken
                                  "
                                  class="btn btn-outline-secondary btn-sm ml-2"
                                  >Send confirmation code</loading-button
                                >
                              </div>
                              <input
                                v-model="
                                  privateState.userAccountModel
                                    .phoneNumberConfirmationToken
                                "
                                id="userInfoModel__phoneNumberConfirmationToken"
                                type="text"
                                class="form-control"
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
                          privateState.storeTypes.AUTH_USER_ACCOUNT_UPDATE
                        ]
                      "
                      class="btn btn-outline-success"
                      >Save</loading-button
                    >
                  </form>
                </div>
              </div>
            </li>
          </div>
        </div>
      </div>

      <h5>Additional info</h5>
      <div class="row mb-5">
        <div class="col-5">
          <div class="list-group">
            <li class="list-group-item border-gray">
              <div
                class="d-flex w-100 justify-content-start align-items-center"
              >
                <div class="w-100">
                  <form v-on:submit.prevent="updateUserInfo">
                    <div class="form-group">
                      <label for="userInfoModel__name"
                        ><strong>Name</strong></label
                      >
                      <input
                        v-model="privateState.userInfoModel.name"
                        id="userInfoModel__name"
                        type="text"
                        class="form-control"
                      />
                    </div>

                    <div class="form-group">
                      <label for="userInfoModel__language"
                        ><strong>Language</strong></label
                      >
                      <multiselect
                        v-bind:id="'userInfoModel__language'"
                        v-model="privateState.userInfoModel.language"
                        v-bind:options="languageList"
                        v-bind:multiple="false"
                        v-bind:searchable="true"
                        v-bind:close-on-select="true"
                        v-bind:clear-on-select="true"
                        v-bind:preserve-search="true"
                        v-bind:show-labels="true"
                        v-bind:allow-empty="false"
                        v-bind:preselect-first="false"
                        label="name"
                        v-bind:custom-label="languageLabel"
                        track-by="code"
                        placeholder="Select language"
                        v-on:input="onLanguageChange"
                        v-bind:loading="isLanguagesLoading"
                        v-bind:disabled="isLanguagesLoading"
                      >
                      </multiselect>
                    </div>

                    <div class="form-group">
                      <label for="userInfoModel__country"
                        ><strong>Country</strong></label
                      >
                      <multiselect
                        v-bind:id="'userInfoModel__country'"
                        v-model="privateState.userInfoModel.country"
                        v-bind:options="countryList"
                        v-bind:multiple="false"
                        v-bind:searchable="true"
                        v-bind:close-on-select="true"
                        v-bind:clear-on-select="true"
                        v-bind:preserve-search="true"
                        v-bind:show-labels="true"
                        v-bind:allow-empty="false"
                        v-bind:preselect-first="false"
                        label="name"
                        v-bind:custom-label="countryLabel"
                        track-by="code"
                        placeholder="Select country"
                        v-on:input="onCountryChange"
                        v-bind:loading="isCountriesLoading"
                        v-bind:disabled="isCountriesLoading"
                      >
                      </multiselect>
                    </div>

                    <div class="form-group">
                      <label for="userInfoModel__timeZone"
                        ><strong>Timezone</strong></label
                      >
                      <multiselect
                        v-bind:id="'userInfoModel__timeZone'"
                        v-model="privateState.userInfoModel.timeZone"
                        v-bind:options="timeZoneList"
                        v-bind:multiple="false"
                        v-bind:searchable="true"
                        v-bind:close-on-select="true"
                        v-bind:clear-on-select="true"
                        v-bind:preserve-search="true"
                        v-bind:show-labels="true"
                        v-bind:allow-empty="false"
                        v-bind:preselect-first="false"
                        label="timeZoneId"
                        v-bind:custom-label="timeZoneLabel"
                        track-by="timeZoneId"
                        placeholder="Select timezone"
                        v-on:input="onTimeZoneChange"
                        v-bind:loading="isTimeZonesLoading"
                        v-bind:disabled="isTimeZonesLoading"
                      >
                      </multiselect>
                      <div>
                        <small
                          >UTC is used by default if timezone isn't set.</small
                        >
                      </div>
                    </div>
                    <loading-button
                      type="submit"
                      v-bind:loading="
                        sharedState.loading[
                          privateState.storeTypes.USER_INFO_UPDATE
                        ]
                      "
                      class="btn btn-outline-success"
                      >Save</loading-button
                    >
                  </form>
                </div>
              </div>
            </li>
          </div>
        </div>
      </div>

      <h5>Notifications</h5>
      <div class="row mb-5">
        <div class="col-5">
          <div class="list-group">
            <li class="list-group-item border-gray">
              <div
                class="d-flex w-100 justify-content-start align-items-center"
              >
                <div class="w-100">
                  <form v-on:submit.prevent="updateNotifications">
                    <div class="form-group form-check">
                      <input
                        v-model="privateState.notificationsModel.isAlarmEnabled"
                        id="isAlarmEnabled"
                        type="checkbox"
                        class="form-check-input"
                      />
                      <label class="form-check-label" for="isAlarmEnabled"
                        >Alarm enabled</label
                      >
                    </div>
                    <div class="form-group form-check">
                      <input
                        v-model="
                          privateState.notificationsModel.isAlarmSoundEnabled
                        "
                        id="isAlarmSoundEnabled"
                        type="checkbox"
                        class="form-check-input"
                      />
                      <label class="form-check-label" for="isAlarmSoundEnabled"
                        >Alarm sound</label
                      >
                    </div>
                    <div class="form-group form-check">
                      <input
                        v-model="
                          privateState.notificationsModel.isDoNotDisturbEnabled
                        "
                        id="isDoNotDisturbEnabled"
                        type="checkbox"
                        class="form-check-input"
                      />
                      <label
                        class="form-check-label"
                        for="isDoNotDisturbEnabled"
                        >Do not disturb</label
                      >
                    </div>
                    <div
                      v-if="
                        privateState.notificationsModel.isDoNotDisturbEnabled
                      "
                      class="form-group"
                    >
                      <div class="row">
                        <div class="col">
                          <div class="form-group">
                            <label for="doNotDisturbFrom">From</label>
                            <time-input
                              v-model="
                                privateState.notificationsModel.doNotDisturbFrom
                              "
                            />
                          </div>
                        </div>
                        <div class="col">
                          <div class="form-group">
                            <label for="doNotDisturbTo">To</label>
                            <time-input
                              v-model="
                                privateState.notificationsModel.doNotDisturbTo
                              "
                            />
                          </div>
                        </div>
                      </div>
                    </div>
                    <loading-button
                      type="submit"
                      v-bind:loading="
                        sharedState.loading[
                          privateState.storeTypes.USER_INFO_NOTIFICATIONS_UPDATE
                        ]
                      "
                      class="btn btn-outline-success"
                      >Save</loading-button
                    >
                  </form>
                </div>
              </div>
            </li>
          </div>
        </div>
      </div>

      <h5>Security</h5>
      <div class="row mb-5">
        <div class="col-5">
          <div class="list-group">
            <li class="list-group-item border-gray">
              <div
                class="d-flex w-100 justify-content-start align-items-center"
              >
                <div class="w-100">
                  <div><strong>Change password</strong></div>
                  <hr />
                  <form v-on:submit.prevent="changePassword">
                    <div class="form-group">
                      <label for="inputPassword">Old password</label>
                      <input
                        v-model="privateState.passwordChangeModel.passwordOld"
                        type="password"
                        class="form-control"
                        placeholder="Old password"
                        autocomplete="new-password"
                      />
                    </div>
                    <div class="form-group">
                      <label for="inputPassword">New password</label>
                      <input
                        v-model="privateState.passwordChangeModel.passwordNew"
                        type="password"
                        class="form-control"
                        placeholder="New password"
                        autocomplete="new-password"
                      />
                    </div>
                    <div class="form-group">
                      <label for="inputPasswordConfirm"
                        >Confirm new password</label
                      >
                      <input
                        v-model="
                          privateState.passwordChangeModel.passwordNewConfirm
                        "
                        type="password"
                        class="form-control"
                        placeholder="Confirm new password"
                        autocomplete="new-password"
                      />
                    </div>
                    <loading-button
                      type="submit"
                      v-bind:loading="
                        sharedState.loading[
                          privateState.storeTypes.AUTH_USER_PASSWORD_CHANGE
                        ]
                      "
                      class="btn btn-outline-secondary"
                      >Change password</loading-button
                    >
                  </form>
                </div>
              </div>
            </li>
          </div>
        </div>
      </div>

      <h5>Danger Zone</h5>
      <div class="row mb-5">
        <div class="col-5">
          <div class="list-group">
            <li class="list-group-item border-danger">
              <div
                class="d-flex w-100 justify-content-between align-items-center"
              >
                <div class="text-danger">
                  <div><strong>Delete account</strong></div>
                  <div>
                    <small
                      >You account and all related data will be deleted!</small
                    >
                  </div>
                  <div><small>NB: This action is irreversible!</small></div>
                </div>
                <loading-button
                  type="button"
                  v-bind:loading="
                    sharedState.loading[
                      privateState.storeTypes.AUTH_USER_ACCOUNT_DELETE
                    ]
                  "
                  class="btn-danger"
                  v-on:click.native="deleteAccount()"
                  >Delete</loading-button
                >
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
import userConfirmationUtil from "@/utils/userConfirmation";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";
import TimeInput from "@/components/TimeInput";

const userAccountModelDefault = {
  phoneNumber: null,
  phoneNumberConfirmationToken: null,
};

const userInfoModelDefault = {
  name: null,
  language: null,
  country: null,
  timeZone: null,
};

const notificationsModelDefault = {
  isAlarmEnabled: false,
  isAlarmSoundEnabled: false,
  isDoNotDisturbEnabled: false,
  doNotDisturbFrom: null,
  doNotDisturbTo: null,
};

const passwordChangeModelDefault = {
  passwordOld: "",
  passwordNew: "",
  passwordNewConfirm: "",
};

export default {
  name: "user-profile",
  components: {
    RowLoader,
    LoadingButton,
    TimeInput,
  },
  props: {},
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        userAccountModel: {
          ...userAccountModelDefault,
        },
        userInfoModel: {
          ...userInfoModelDefault,
        },
        notificationsModel: {
          ...notificationsModelDefault,
        },
        passwordChangeModel: {
          ...passwordChangeModelDefault,
        },
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      isLanguagesLoading: (state) => state.loading[storeTypes.LANGUAGES_LOAD],
      isCountriesLoading: (state) => state.loading[storeTypes.COUNTRIES_LOAD],
      isTimeZonesLoading: (state) => state.loading[storeTypes.TIMEZONES_LOAD],
      languageList: (state) => state.languages || [],
      countryList: (state) => state.countries || [],
      timeZoneList: (state) => state.timeZones || [],
    }),

    isPhoneChanged: function () {
      return (
        this.privateState.userAccountModel.phoneNumber !==
        ((this.sharedState.userAccount || {}).phoneNumber || null)
      );
    },
  },
  created: async function () {
    // load account
    if (!this.sharedState.userAccount) {
      this.$store.dispatch(storeTypes.AUTH_USER_ACCOUNT_LOAD, {});
    }

    // load userinfo
    if (!this.sharedState.userInfo) {
      this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});
    }

    if (!this.sharedState.countries) {
      this.$store.dispatch(storeTypes.COUNTRIES_LOAD, {});
    }
    if (!this.sharedState.languages) {
      this.$store.dispatch(storeTypes.LANGUAGES_LOAD, {});
    }
    if (!this.sharedState.timeZones) {
      this.$store.dispatch(storeTypes.TIMEZONES_LOAD, {});
    }

    // set initial data
    this.privateState.userAccountModel = {
      ...userAccountModelDefault,
      ...(this.sharedState.userAccount || {}),
    };
    this.privateState.userInfoModel = {
      ...userInfoModelDefault,
      ...(this.sharedState.userInfo || {}),
    };

    this.privateState.notificationsModel = {
      ...notificationsModelDefault,
      ...((this.sharedState.userInfo || {}).notificationSettings || {}),
    };

    // update model on state change
    this.$store.subscribe((mutation, state) => {
      let { type, payload } = mutation;
      if (type === storeTypes.AUTH_USER_ACCOUNT_SET) {
        this.privateState.userAccountModel = {
          ...this.privateState.userAccountModel,
          ...state.userAccount,
        };
      }
      if (type === storeTypes.USER_INFO_SET) {
        this.privateState.userInfoModel = {
          ...this.privateState.userInfoModel,
          ...state.userInfo,
        };

        this.privateState.notificationsModel = {
          ...this.privateState.notificationsModel,
          ...state.userInfo.notificationSettings,
        };
      }
    });
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {
    onPhoneNumberValidate({ number, isValid, country }) {
      let { e164, input, international, national, rfc3966, significant } =
        number;
      let { areaCodes, dialCode, iso2, name, priority } = country;
    },
    languageLabel(option) {
      return `${option.name} / ${option.nativeName}`;
    },
    countryLabel(option) {
      return `${option.name} (${option.code})`;
    },
    timeZoneLabel(option) {
      return `${option.timeZoneId} (${option.countryName} / ${
        option.countryCode
      } ${option.comment ? `/ ${option.comment}` : ""})`;
    },
    onLanguageChange: function (values) {},
    onCountryChange: function (values) {},
    onTimeZoneChange: function (values) {},
    sendSmsPhoneNumberChangeToken: function () {
      this.$store
        .dispatch(storeTypes.AUTH_USER_ACCOUNT_PHONE_CHANGE_SMS_TOKEN_SEND, {
          phoneNumber: this.privateState.userAccountModel.phoneNumber,
        })
        .then(() => {})
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    updateUser: function () {
      this.$store
        .dispatch(storeTypes.AUTH_USER_ACCOUNT_UPDATE, {
          data: {
            phoneNumber: this.privateState.userAccountModel.phoneNumber,
            phoneNumberConfirmationToken:
              this.privateState.userAccountModel.phoneNumberConfirmationToken,
          },
        })
        .then(() => {
          this.privateState.userAccountModel.phoneNumberConfirmationToken =
            null;

          authService.refreshTokens({ withFullscreenLoader: false });
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    updateUserInfo: function () {
      this.$store
        .dispatch(storeTypes.USER_INFO_UPDATE, {
          data: {
            name: this.privateState.userInfoModel.name,
            language: this.privateState.userInfoModel.language,
            country: this.privateState.userInfoModel.country,
            timeZone: this.privateState.userInfoModel.timeZone,
          },
        })
        .then(() => {
          authService.refreshTokens({ withFullscreenLoader: false });
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    updateNotifications: function () {
      this.$store
        .dispatch(storeTypes.USER_INFO_NOTIFICATIONS_UPDATE, {
          data: {
            ...this.privateState.notificationsModel,
          },
        })
        .then(() => {
          authService.refreshTokens({ withFullscreenLoader: false });
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    changePassword: function () {
      this.$store
        .dispatch(storeTypes.AUTH_USER_PASSWORD_CHANGE, {
          ...this.privateState.passwordChangeModel,
        })
        .then(() => {
          this.$notify({
            group: "app-important",
            type: "success",
            title: `You account password has been changed.`,
            text: "",
          });

          // reset model
          this.privateState.passwordChangeModel = {
            ...passwordChangeModelDefault,
          };

          authService.refreshTokens({ withFullscreenLoader: false });
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },

    deleteAccount: function () {
      userConfirmationUtil
        .handleUserConfirmationFlow({
          actionF: ({ userConfirmation }) => {
            return this.$store.dispatch(storeTypes.AUTH_USER_ACCOUNT_DELETE, {
              userConfirmation,
            });
          },
        })
        .then(({ isProcessed }) => {
          if (isProcessed) {
            // deleted
            authService
              .logoutWithoutRedirect()
              .then(() => {
                this.$router.push({ name: "home" });
              })
              .catch((err) => {
                console.error(err);
                window.alert("Something went wrong. Try to refresh the page.");
              });
          } else {
            // not deleted
          }
        })
        .catch((err) => {
          console.error(err);
        });
    },
  },
};
</script>

<style lang="scss"></style>
