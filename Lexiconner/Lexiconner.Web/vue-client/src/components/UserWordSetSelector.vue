<template>
  <div class="d-flex align-items-center">
    <multiselect
      v-model="privateState.selectedWordSetOption"
      v-bind:options="wordSetList"
      v-bind:multiple="false"
      v-bind:searchable="true"
      v-bind:close-on-select="true"
      v-bind:clear-on-select="false"
      v-bind:preserve-search="true"
      v-bind:show-labels="true"
      v-bind:allow-empty="true"
      v-bind:preselect-first="false"
      v-bind:custom-label="wordSetLabel"
      track-by="id"
      v-bind:placeholder="'Select word set'"
      v-bind:loading="isLoading"
      v-bind:disabled="false"
      v-on:input="onInput"
      class=""
    >
    </multiselect>
  </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

export default {
  name: "user-word-set-selector",
  props: {
    /**
     * Value is wordSetId that is passed as v-model="<>"
     * v-model does this:
     *      v-bind:value="<>"
     *      v-on:input="<> = $event"
     */
    value: {
      // type: [String],
      required: false,
      default: null,
    },
    useAsInputOnly: {
      required: false,
      default: false,
    },
  },
  components: {
    // RowLoader,
    // LoadingButton,
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        selectedWordSetOption: null,
      },
    };
  },
  computed: {
    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      isLoading: (state) => state.loading[storeTypes.USER_DICTIONARY_LOAD],
      userDictionary: (state) => state.userDictionary,
      wordSetList: function (state) {
        if (state.userDictionary) {
          return [
            ...(this.useAsInputOnly
              ? []
              : [
                  {
                    id: null,
                    name: "All",
                  },
                ]),
            ...state.userDictionary.wordSets,
          ];
        } else {
          return [];
        }
      },
      currentUserDictionaryWordSetId: (state) =>
        state.currentUserDictionaryWordSetId,
    }),
  },
  watch: {
    // watch value changed outside (value prop changed)
    value: function (newValue, oldValue) {
      this.preselectWordSet();
    },
  },
  created: async function () {
    if (!this.userDictionary) {
      this.$store
        .dispatch(storeTypes.USER_DICTIONARY_LOAD, {})
        .then(() => {
          this.preselectWordSet();
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    } else {
      this.preselectWordSet();
    }
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {
    preselectWordSet() {
      if (this.wordSetList) {
        if (this.useAsInputOnly) {
          this.privateState.selectedWordSetOption =
            this.wordSetList.find((x) =>
              this.value ? x.id === this.value : x.isDefault
            ) || null;

          // autoselect default
          if (this.privateState.selectedWordSetOption && !this.value) {
            this.onInput(this.privateState.selectedWordSetOption);
          }
        } else {
          this.privateState.selectedWordSetOption =
            this.wordSetList.find(
              (x) => x.id === this.currentUserDictionaryWordSetId
            ) || null;

          if (this.privateState.selectedWordSetOption) {
            this.onWordSetChange(this.privateState.selectedWordSetOption.id);
          }
        }
      }
    },
    wordSetLabel(option) {
      return `${option.name} ${option.id ? `(${option.wordCount || 0})` : ""}`;
    },
    onInput: function (value, id) {
      // tell parent that value was changed and it can update its v-model property
      // value is user
      let { id: wordSetId } = value;
      this.$emit("input", wordSetId);

      // emit change event
      this.$emit("change", wordSetId);

      if (!this.useAsInputOnly) {
        this.onWordSetChange(wordSetId);
      }
    },
    onWordSetChange(wordSetId) {
      this.$store.commit(storeTypes.USER_DICTIONARY_WORD_SET_CURRENT_SET, {
        currentUserDictionaryWordSetId: wordSetId,
      });
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
