<template>
  <div v-if="sharedState.userInfo" class="user-company-control-wrapper">
    <div v-if="sharedState.userInfo.companies">
      <h5>Current company:</h5>
      <div class="row">
        <div class="col col-md-4">
          <div
            class="
              d-flex
              justify-content-start
              align-items-start
              company-select-wrapper
            "
          >
            <multiselect
              v-model="privateState.selectedCompany"
              v-bind:options="sharedState.userInfo.companies"
              v-bind:multiple="false"
              v-bind:searchable="true"
              v-bind:close-on-select="true"
              v-bind:clear-on-select="true"
              v-bind:preserve-search="true"
              v-bind:show-labels="true"
              v-bind:allow-empty="false"
              v-bind:preselect-first="false"
              label="name"
              track-by="id"
              placeholder="Select company"
              v-on:input="onCurrentCompanyChange"
              v-bind:loading="isLoading"
              v-bind:disabled="isLoading"
            >
            </multiselect>
            <button
              v-if="
                $store.getters.isUserHasPermissions(
                  ['CompanyUpdate'],
                  $store.getters.currentCompanyId
                )
              "
              v-on:click="onManageCompanyClick()"
              type="button"
              class="btn btn-outline-secondary ml-2"
            >
              <i class="fas fa-cog"></i>
            </button>
            <button
              v-if="
                $store.getters.isUserHasPermissions(['CompanyCreate'], null)
              "
              v-on:click="onCreateCompanyClick($event)"
              type="button"
              class="btn btn-outline-success ml-2"
            >
              <i class="fas fa-plus"></i>
            </button>
            <loading-button
              v-if="$store.getters.currentCompanyId"
              type="button"
              v-bind:loading="
                sharedState.loading[privateState.storeTypes.COMPANY_LEAVE]
              "
              v-on:click.native="leaveCurrentCompany()"
              class="btn btn-outline-danger ml-2"
            >
              <i class="fas fa-user-minus"></i>
            </loading-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import userConfirmationUtil from "@/utils/userConfirmation";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

export default {
  name: "current-company-control",
  props: {},
  components: {
    // RowLoader,
    LoadingButton,
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        selectedCompany: null,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      isLoading: (state) => state.loading[storeTypes.USER_INFO_LOAD],
    }),
  },
  created: function () {
    this.setSelectedCompany();

    // update model on state change
    this.$store.subscribe((mutation, state) => {
      let { type, payload } = mutation;
      if (type === storeTypes.USER_INFO_SET) {
        this.setSelectedCompany();
      }
    });
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {
    setSelectedCompany: function () {
      if (this.sharedState.userInfo) {
        let current = this.sharedState.userInfo.companies.find(
          (x) => x.isCurrent
        );
        if (current) {
          this.privateState.selectedCompany = { ...current };
        } else {
          this.privateState.selectedCompany = null;
        }
      }
    },
    onCurrentCompanyChange: function (values) {
      let selectedCompanyId = this.privateState.selectedCompany.id;

      this.$appFullscreenLoader({
        isVisible: true,
        title: "Switching company...",
      });

      this.$store
        .dispatch(storeTypes.USER_INFO_CURRENT_COMPANY_SET, {
          companyId: selectedCompanyId,
        })
        .then(() => {
          return authService.refreshTokens({ withFullscreenLoader: false });
        })
        .then(() => {
          this.$appFullscreenLoader({
            isVisible: false,
          });
        })
        .catch((err) => {
          this.$appFullscreenLoader({
            isVisible: false,
          });

          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    onManageCompanyClick: function () {
      this.$router.push({
        name: "company-manage",
        params: {
          companyId: this.$store.getters.currentCompanyId,
        },
      });
    },
    onCreateCompanyClick(e) {
      this.$router.push({ name: "company-create" });
    },
    // leaveCurrentCompany() {
    //     if(window.confirm('Leave company?')) {
    //         this.$store.dispatch(storeTypes.COMPANY_LEAVE, {
    //             companyId: this.$store.getters.currentCompanyId,
    //         }).then(() => {
    //             this.privateState.selectedCompany = null;

    //             // reload info
    //             this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});
    //         }).catch(err => {
    //             console.error(err);
    //             notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
    //         });
    //     }
    // },
    leaveCurrentCompany: function () {
      userConfirmationUtil
        .handleUserConfirmationFlow({
          actionF: ({ userConfirmation }) => {
            return this.$store.dispatch(storeTypes.COMPANY_LEAVE, {
              companyId: this.$store.getters.currentCompanyId,
              userConfirmation,
            });
          },
        })
        .then(({ isProcessed }) => {
          if (isProcessed) {
            // left

            // reload info
            this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});
          } else {
            // not left
          }
        })
        .catch((err) => {
          console.error(err);
        });
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
