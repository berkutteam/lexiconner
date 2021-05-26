<template>
  <div class="mb-3">
    <form v-on:submit.prevent="() => {}" class="form-inline">
      <div class="form-group mr-2">
        <label class="sr-only" for="userFilmsRequestParams__search"
          >Search</label
        >
        <input
          v-bind:value="sharedState.userFilmsRequestParams.search"
          v-on:input="onSearchChange"
          type="text"
          class="form-control"
          id="userFilmsRequestParams__search"
          placeholder="Search"
        />
      </div>

      <!-- Reload -->
      <div class="form-group mr-2">
        <button v-on:click="reload" type="button" class="btn custom-btn-normal">
          <i class="fas fa-sync-alt"></i>
        </button>
      </div>

      <!-- Reset -->
      <div class="form-group">
        <button
          v-on:click="resetRequestParams"
          type="button"
          class="btn custom-btn-normal"
        >
          <i class="fas fa-times"></i>
        </button>
      </div>
    </form>
  </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { ToggleButton } from "vue-js-toggle-button";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

export default {
  name: "words-filters",
  props: {
    onChange: {
      type: Function,
      required: false,
      default: null,
    },
  },
  components: {
    // RowLoader,
    // LoadingButton,
    // ToggleButton,
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
    }),
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  beforeDestroy: function () {},
  destroyed: function () {
    this.resetRequestParams();
  },

  methods: {
    callOnChange: function () {
      if (this.onChange) {
        this.onChange();
      }
    },
    callOnChangeDebounce: _.debounce(function () {
      if (this.onChange) {
        this.onChange();
      }
    }, 500),
    onSearchChange: function (e) {
      this.$store.commit(storeTypes.USER_FILMS_REQUEST_PARAMS_SET, {
        search: e.target.value,
      });
      this.callOnChangeDebounce();
    },
    reload: function () {
      this.callOnChange();
    },
    resetRequestParams: function () {
      this.$store.commit(storeTypes.USER_FILMS_REQUEST_PARAMS_RESET, {});
      this.callOnChange();
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
