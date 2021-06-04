<template>
  <multiselect
    v-model="privateState.selectedLanguageOption"
    v-bind:options="languageList"
    v-bind:multiple="false"
    v-bind:searchable="true"
    v-bind:close-on-select="true"
    v-bind:clear-on-select="false"
    v-bind:preserve-search="true"
    v-bind:show-labels="true"
    v-bind:allow-empty="true"
    v-bind:preselect-first="false"
    v-bind:custom-label="languageLabel"
    track-by="iso639_1_Code"
    v-bind:placeholder="placeholder"
    v-bind:loading="isLoading"
    v-bind:disabled="false"
    v-on:input="onInput"
    class="multiselect--languageCodeSelect"
  >
    <!-- Custom selected option (when select closed) -->
    <template slot="singleLabel" slot-scope="props">
      <div class="" style="display: flex; alight-items: center">
        <img
          style="width: 24px; margin-right: 0.5rem"
          class=""
          v-bind:src="`http://purecatamphetamine.github.io/country-flag-icons/3x2/${props.option.countryIsoAlpha2Code.toUpperCase()}.svg`"
        />
        <span class="option__desc">
          <span class="option__title">{{ languageLabel(props.option) }}</span>
        </span>
      </div>
    </template>
    <!-- Custom options -->
    <template v-if="withFlags" slot="option" slot-scope="props">
      <div class="" style="display: flex; alight-items: center">
        <img
          style="width: 24px; margin-right: 0.5rem"
          class=""
          v-bind:src="`http://purecatamphetamine.github.io/country-flag-icons/3x2/${props.option.countryIsoAlpha2Code.toUpperCase()}.svg`"
        />
        <div class="">
          <span class="option__title">{{ languageLabel(props.option) }}</span>
        </div>
      </div>
    </template>
  </multiselect>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import { storeTypes } from "@/constants/index";
import notificationUtil from "@/utils/notification";
import _ from "lodash";

export default {
  name: "language-code-select",
  props: {
    /**
     * Value is languageCode that is passed as v-model="<languageCodeProp>"
     * v-model does this:
     *      v-bind:value="<languageCodeProp>"
     *      v-on:input="<languageCodeProp> = $event"
     */
    value: {
      // type: [String, Object],
      required: true,
      default: null,
    },

    placeholder: {
      type: String,
      required: false,
      default: "Select language",
    },

    // ({iso639_1_Code, isoLanguageName, nativeName}) => {}: string
    languageLabelGetter: {
      type: Function,
      required: false,
      default: null,
    },

    withFlags: {
      type: Boolean,
      required: false,
      default: true,
    },

    // events: change -> languageCode
  },
  components: {},
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        selectedLanguageOption: null,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      isLoading: (state) => state.loading[storeTypes.LANGUAGES_LOAD],
      languageList: function (state) {
        return state.languages || [];
      },
    }),
  },
  watch: {
    // watch lang code changed outside (value prop changed)
    value: function (newValue, oldValue) {
      this.preselectLanguage();
    },
  },
  created: async function () {
    let self = this;

    // load langs
    if (self.sharedState.languageList == null) {
      self.$store
        .dispatch(storeTypes.LANGUAGES_LOAD, {})
        .then(() => {
          self.preselectLanguage();
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    }

    // preselect
    self.preselectLanguage();
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {
    preselectLanguage() {
      if (this.value && this.languageList) {
        this.privateState.selectedLanguageOption =
          this.languageList.find((x) => x.iso639_1_Code === this.value) || null;
      }
    },
    languageLabel(option) {
      if (this.languageLabelGetter) {
        return this.languageLabelGetter(option);
      }
      return `${option.iso639_1_Code} - ${option.isoLanguageName} (${option.nativeName})`;
    },
    onInput: function (value, id) {
      // tell parent that value was changed and it can update its v-model property
      // value is user
      let { isoLanguageName, iso639_1_Code } = value;
      this.$emit("input", iso639_1_Code);

      // emit change event with ISO language code
      this.$emit("change", iso639_1_Code);
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
