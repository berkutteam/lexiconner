<template>
  <div class="register-wrapper">
    <div class="row">
      <div class="col-4">
        <div v-if="globalLoading">
          <row-loader v-bind:visible="globalLoading"></row-loader>
        </div>
        <div v-if="!globalLoading">
          <div v-if="privateState.currentStep === 'invitation-validating'">
            <!-- <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.COMPANY_USER_INVITATION_VALIDATE]"></row-loader> -->
            <div>Validating invitation...</div>
          </div>
          <div v-if="privateState.currentStep === 'invitation-invalid'">
            <div class="alert alert-danger" role="alert">
              Your invitation is invalid or expired.
            </div>
          </div>
          <div v-if="privateState.currentStep === 'phone-enter'">
            <!-- Compalete registration messsage (for external provider users) -->
            <div v-if="!sharedState.auth.isRegistrationCompleted">
              <div class="alert alert-primary" role="alert">
                You should complete registration proccess.
              </div>
            </div>

            <form v-on:submit.prevent="onSubmitPhoneEnterStep">
              <div class="form-group">
                <label for="inputPhone">Phone number</label>
                <vue-tel-input
                  v-model="privateState.model.phoneNumber"
                  v-bind:mode="'international'"
                  v-bind:disabledFetchingCountry="false"
                  v-bind:disabledFormatting="false"
                  v-bind:enabledCountryCode="true"
                  v-bind:inputClasses="[]"
                  v-bind:enabledFlags="true"
                  v-bind:required="true"
                >
                </vue-tel-input>
                <small class="form-text text-muted"
                  >Phone number is used for confirmation.</small
                >
              </div>
              <loading-button
                type="submit"
                v-bind:loading="
                  sharedState.loading[
                    privateState.storeTypes.AUTH_REGISTRATION_SMS_TOKEN_SEND
                  ]
                "
                class="btn btn-outline-primary"
                >Next</loading-button
              >
            </form>
          </div>
          <div v-if="privateState.currentStep === 'phone-validate'">
            <form v-on:submit.prevent="onSubmitPhoneValidateStep">
              <div class="form-group">
                <label for="inputConfiramtionCode">Confirmation code</label>
                <input
                  v-model="privateState.model.phoneNumberConfirmationToken"
                  type="text"
                  class="form-control"
                  id="inputConfiramtionCode"
                  aria-describedby="confiramtionCodeHelp"
                  placeholder="Confirmation code"
                />
                <small id="confiramtionCodeHelp" class="form-text text-muted"
                  >Code was sent to {{ privateState.model.phoneNumber }}.</small
                >
              </div>
              <button
                v-on:click="
                  $event.preventDefault();
                  goToStep('phone-enter');
                "
                type="button"
                class="btn btn-outline-secondary"
              >
                Back
              </button>
              &nbsp;
              <loading-button
                type="submit"
                v-bind:loading="
                  sharedState.loading[
                    privateState.storeTypes.AUTH_REGISTRATION_SMS_TOKEN_VALIDATE
                  ]
                "
                class="btn btn-outline-primary"
                >Next</loading-button
              >
            </form>
          </div>
          <div v-if="privateState.currentStep === 'registration-data-enter'">
            <form v-on:submit.prevent="onSubmitRegistrationDataEnterStep">
              <div class="form-group">
                <label for="inputEmail">Email address</label>
                <input
                  v-model="privateState.model.email"
                  v-bind:disabled="privateState.isEmailInputDisabled"
                  type="email"
                  class="form-control"
                  id="inputEmail"
                  aria-describedby="emailHelp"
                  placeholder="Enter email"
                  autocomplete="username"
                />
                <small id="emailHelp" class="form-text text-muted"
                  >Email is your username.</small
                >
              </div>
              <div class="form-group">
                <label for="inputPassword">Password</label>
                <input
                  v-model="privateState.model.password"
                  type="password"
                  class="form-control"
                  id="inputPassword"
                  aria-describedby="passwordHelp"
                  placeholder="Password"
                  autocomplete="new-password"
                />
                <small
                  v-if="
                    registrationInfo && registrationInfo.passwordSuggestions
                  "
                  id="passwordHelp"
                  class="form-text text-muted password-suggestion-list"
                >
                  <p
                    v-for="item in registrationInfo.passwordSuggestions"
                    v-bind:key="item.id"
                    class="list-item"
                  >
                    {{ item }}
                  </p>
                </small>
              </div>
              <div class="form-group">
                <label for="inputPasswordConfirm">Confirm password</label>
                <input
                  v-model="privateState.model.passwordConfirm"
                  type="password"
                  class="form-control"
                  id="inputPasswordConfirm"
                  aria-describedby="passwordConfirmHelp"
                  placeholder="Confirm password"
                  autocomplete="new-password"
                />
              </div>
              <div class="form-group">
                <label for="inputName">Name</label>
                <input
                  v-model="privateState.model.name"
                  type="text"
                  class="form-control"
                  id="inputName"
                  aria-describedby="nameHelp"
                  placeholder="Enter display name"
                />
                <small id="nameHelp" class="form-text text-muted"
                  >E.g. John Doe.</small
                >
              </div>
              <div class="form-group form-check">
                <input
                  v-model="privateState.model.isAcceptTermsOfUse"
                  type="checkbox"
                  class="form-check-input"
                  id="inputAcceptTermsOfUse"
                />
                <label class="form-check-label" for="inputAcceptTermsOfUse">
                  I accept the
                  <router-link
                    v-bind:to="{ name: 'terms-of-use' }"
                    target="_blank"
                    >Terms Of Use</router-link
                  >
                </label>
              </div>
              <button
                v-on:click="
                  $event.preventDefault();
                  goToStep('phone-validate');
                "
                type="button"
                class="btn btn-outline-secondary"
              >
                Back
              </button>
              &nbsp;
              <loading-button
                type="submit"
                v-bind:loading="
                  sharedState.loading[
                    privateState.storeTypes.AUTH_REGISTER_AS_INTERNAL_USER
                  ]
                "
                class="btn btn-outline-success"
                >Register</loading-button
              >
            </form>
          </div>
          <div v-if="privateState.currentStep === 'registration-completed'">
            <div class="user-registered-wrapper">
              <i class="fas fa-user-check user-registered-icon"></i>
              <h5>You have beed registered!</h5>
              <div>
                <router-link v-bind:to="{ name: 'login' }">Login</router-link>
                now and start working.
              </div>
            </div>
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
import notification from "@/utils/notification";
import utils from "@/utils/index";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

export default {
  name: "register",
  components: {
    RowLoader,
    LoadingButton,
  },
  props: {
    // route params
    query: {
      invitationId: String,
    },
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        currentStep: "phone-enter",
        model: {
          phoneNumber: "",
          phoneNumberConfirmationToken: "",
          email: "",
          password: "",
          passwordConfirm: "",
          name: "",
          isAcceptTermsOfUse: false,
        },
        isEmailInputDisabled: false,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      registrationInfo: (state) => state.auth.registrationInfo || null,
      invitation: function (state) {
        if (
          this.query.invitationId &&
          state.companyUserInvitation[this.query.invitationId]
        ) {
          return state.companyUserInvitation[this.query.invitationId];
        }
        return null;
      },
      globalLoading: function (state) {
        return (
          state.loading[storeTypes.COMPANY_USER_INVITATION_VALIDATE] ||
          state.loading[storeTypes.COMPANY_USER_INVITATION_LOAD] ||
          state.loading[storeTypes.AUTH_PREREGISTRATION_USER_LOAD] ||
          state.loading[storeTypes.AUTH_REGISTRATION_TOKENS_REFRESH]
        );
      },
    }),
  },
  created: async function () {
    let self = this;

    // check invitation
    if (this.query.invitationId) {
      this.privateState.currentStep = "invitation-validating";
      this.$store
        .dispatch(storeTypes.COMPANY_USER_INVITATION_VALIDATE, {
          invitationId: this.query.invitationId,
        })
        .then((data) => {
          if (data === true) {
            // go to registration
            this.privateState.currentStep = "phone-enter";

            // load invitation info
            this.$store
              .dispatch(storeTypes.COMPANY_USER_INVITATION_LOAD, {
                invitationId: this.query.invitationId,
              })
              .then((data) => {
                // preinput email from invitation
                self.privateState.model.email = data.email;
                self.privateState.isEmailInputDisabled = true;
              })
              .catch((err) => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
              });
          } else {
            // show error
            this.privateState.currentStep = "invitation-invalid";
          }
        })
        .catch((err) => {
          console.error(err);
          notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    }

    // check preregistration info
    if (this.query.preRegistrationUserId) {
      // load and preinput info from external provider
      this.$store
        .dispatch(storeTypes.AUTH_PREREGISTRATION_USER_LOAD, {
          preRegistrationUserId: this.query.preRegistrationUserId,
        })
        .then((data) => {
          // preinput
          if (data) {
            self.privateState.model.phoneNumber =
              (data.claims.find((x) => x.type === "phone_number") || {})
                .value || self.privateState.model.phoneNumber;
            self.privateState.model.email =
              (data.claims.find((x) => x.type === "email") || {}).value ||
              self.privateState.model.email;
            self.privateState.model.name =
              (data.claims.find((x) => x.type === "name") || {}).value ||
              self.privateState.model.name;
          } else {
            // possibly user is alredy registered
            this.handleAfterRegistrationForExternalUser();
          }
        })
        .catch((err) => {
          console.error(err);
          notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    }

    // load registration info
    this.$store
      .dispatch(storeTypes.AUTH_REGISTRATION_INFO_LOAD, {})
      .then()
      .catch((err) => {
        console.error(err);
        notification.showErrorIfServerErrorResponseOrDefaultError(err);
      });
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},
  methods: {
    goToStep(step) {
      this.privateState.currentStep = step;
    },
    onSubmitPhoneEnterStep(e) {
      console.log("onSubmitPhoneEnterStep. model - ", this.privateState.model);
      this.$store
        .dispatch(storeTypes.AUTH_REGISTRATION_SMS_TOKEN_SEND, {
          phoneNumber: this.privateState.model.phoneNumber,
          preRegistrationUserId: this.query.preRegistrationUserId || null,
        })
        .then(() => {
          this.privateState.currentStep = "phone-validate";
        })
        .catch((err) => {
          console.error(err);
          notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    onSubmitPhoneValidateStep(e) {
      console.log(
        "onSubmitPhoneValidateStep. model - ",
        this.privateState.model
      );
      this.$store
        .dispatch(storeTypes.AUTH_REGISTRATION_SMS_TOKEN_VALIDATE, {
          phoneNumber: this.privateState.model.phoneNumber,
          token: this.privateState.model.phoneNumberConfirmationToken,
          preRegistrationUserId: this.query.preRegistrationUserId || null,
        })
        .then((data) => {
          if (data) {
            this.privateState.currentStep = "registration-data-enter";
          } else {
            this.$notify({
              group: "error",
              type: "error",
              title: "Invalid phone confirmation code",
              text: "",
            });
          }
        })
        .catch((err) => {
          console.error(err);
          notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    onSubmitRegistrationDataEnterStep(e) {
      console.log(
        "onSubmitRegistrationDataEnterStep. model - ",
        this.privateState.model
      );
      let user = {
        ...this.privateState.model,
        clientId: this.sharedState.config.clientAuth.clientId,
        invitationId: this.query.invitationId || null,
        preRegistrationUserId: this.query.preRegistrationUserId || null,
      };
      this.$store
        .dispatch(storeTypes.AUTH_REGISTER_AS_INTERNAL_USER, { user })
        .then(async (data) => {
          // this.$notify({
          //     group: 'app',
          //     type: 'success',
          //     title: 'You have beed registered!',
          //     text: 'Logging you in...',
          //     duration: timeout,
          // });

          if (this.query.preRegistrationUserId) {
            // login
            this.handleAfterRegistrationForExternalUser();
          } else {
            // go to completed page
            this.privateState.currentStep = "registration-completed";
          }
        })
        .catch((err) => {
          console.error(err);
          notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    handleAfterRegistrationForExternalUser() {
      if (this.query.preRegistrationUserId) {
        // refresh tokens to grab registered user
        this.$notify({
          group: "app",
          type: "success",
          title: "You have beed registered!",
          text: "Logging you in...",
        });
        this.$store.commit(storeTypes.LOADING_SET, {
          target: storeTypes.AUTH_REGISTRATION_TOKENS_REFRESH,
          loading: true,
        });
        authService
          .refreshTokens({ withFullscreenLoader: true })
          .then(() => {
            this.$router.push({
              name: "dashboard-home",
            });
            this.$store.commit(storeTypes.LOADING_SET, {
              target: storeTypes.AUTH_REGISTRATION_TOKENS_REFRESH,
              loading: false,
            });
          })
          .catch((err) => {
            console.error(err);
            this.$store.commit(storeTypes.LOADING_SET, {
              target: storeTypes.AUTH_REGISTRATION_TOKENS_REFRESH,
              loading: false,
            });
            this.$notify({
              group: "error",
              type: "error",
              title: `Can't login after registration.`,
              text: "Try to refresh the page.",
            });
          });
      }
    },
  },
};
</script>

<style lang="scss"></style>
