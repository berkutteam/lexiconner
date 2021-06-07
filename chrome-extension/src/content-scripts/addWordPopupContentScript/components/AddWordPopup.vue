<template>
  <div
    ref="modalOverlay"
    v-if="privateState.isOpen"
    v-on:click="(e) => onOverlayClick(e)"
    class="modal-overlay"
  >
    <div
      ref="modalWrapper"
      class="modal-wrapper"
      v-bind:style="privateState.popupCoords"
    >
      <div class="modal-container">
        <!-- <div class="modal-header">
          <h5>Add word</h5>
        </div> -->

        <form
          v-if="!privateState.showCreatedWordSummary"
          v-on:submit.prevent="createWord()"
        >
          <div class="modal-body">
            <div>
              <label>Word</label>
              <input v-model="privateState.wordModel.word" />
            </div>
            <br />
            <div>
              <div>
                <div><strong>Possible meanings</strong></div>
                <div v-if="wordMeaningsLoading">Loading...</div>
                <div
                  v-if="!wordMeaningsLoading && wordMeanings"
                  class="word-meaning-list"
                >
                  <div
                    v-for="(meaning, meaningIndex) in wordMeanings.meanings"
                    v-bind:key="`word-meaning-${meaningIndex}`"
                    v-on:click="
                      privateState.wordModel.meaning = meaning.meaning
                    "
                    class="meaning-option"
                    v-bind:class="{
                      selected:
                        privateState.wordModel.meaning === meaning.meaning,
                    }"
                  >
                    {{ meaning.meaning }}
                  </div>
                </div>
              </div>
              <br />
              <div>
                <div><strong>Custom meaning</strong></div>
                <textarea
                  v-model="privateState.wordModel.meaning"
                  type="text"
                />
              </div>
            </div>
            <br />
            <div>
              <label>Examples</label>
              <textarea
                v-for="(exampleText, exampleIndex) in privateState.wordModel
                  .examples"
                v-bind:key="`word-example-${exampleIndex}`"
                v-model="privateState.wordModel.examples[exampleIndex]"
                v-bind:placeholder="`Example ${exampleIndex + 1}`"
                type="text"
                class=""
              />
            </div>
            <br />
            <div style="display: flex">
              <div style="flex: 1; margin-right: 0.5rem">
                <label>Word language</label>
                <language-code-select
                  v-model="privateState.wordModel.wordLanguageCode"
                  placeholder="Word language"
                  v-bind:languageLabelGetter="
                    (option) => `${option.isoLanguageName}`
                  "
                  v-bind:withFlags="true"
                  v-on:change="
                    (languageCode) =>
                      (privateState.wordModel.wordLanguageCode = languageCode)
                  "
                />
              </div>
              <div style="flex: 1">
                <label>Meaning language</label>
                <language-code-select
                  v-model="privateState.wordModel.meaningLanguageCode"
                  placeholder="Meaning language"
                  v-bind:languageLabelGetter="
                    (option) => `${option.isoLanguageName}`
                  "
                  v-bind:withFlags="true"
                  v-on:change="onMeaningLanguageCodeChange"
                />
              </div>
            </div>
            <br />
          </div>

          <div class="modal-footer">
            <button
              type="submit"
              v-bind:disabled="
                sharedState.loading[privateState.storeTypes.WORD_CREATE]
              "
              class=""
            >
              Save
            </button>
            <button class="" v-on:click="onCloseClick()">Cancel</button>
          </div>
        </form>

        <div v-if="privateState.showCreatedWordSummary">
          <div>Word successfully added to your dictionary</div>
          <div v-if="createdWord">
            {{ createdWord.word }} - {{ createdWord.meaning }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import languageUtil from "@/utils/language";
import wordUtil from "@/utils/word";
import notificationUtil from "@/utils/notification";
import LanguageCodeSelect from "@/components/LanguageCodeSelect";

const wordModelDefault = {
  word: "",
  meaning: "",
  examples: [""],
  wordLanguageCode: "",
  meaningLanguageCode: "",
};

export default {
  name: "add-word-popup",
  components: {
    LanguageCodeSelect,
  },
  props: {},
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        isOpen: false,
        popupCoords: {
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
        },
        wordModel: {
          ...wordModelDefault,
        },
        showCreatedWordSummary: false,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      wordMeaningsLoading: (state) =>
        state.loading[storeTypes.WORD_MEANINGS_LOAD],
      wordMeanings: (state) => state.wordMeanings,
      createdWord: (state) => state.createdWord,
    }),

    // store getter
    ...mapGetters([]),
  },
  updated: function () {},
  watch: {
    // reload meanings when word is edited
    "privateState.wordModel.word": function (newVale, oldValue) {
      if (!newVale) {
        return;
      }
      this.loadWordMeaningsDebounce({
        word: newVale,
        wordLanguageCode: this.privateState.wordModel.wordLanguageCode,
        meaningLanguageCode: this.privateState.wordModel.meaningLanguageCode,
      });
    },
  },
  methods: {
    open: function (selectionCoords, wordObj) {
      console.log(`AddWordPopup.open.`, selectionCoords, wordObj);
      const { word, wordLanguageCode, examples } = wordObj;

      if (selectionCoords) {
        // compute coords to place popup near the selection
        let { top, right, bootom, left, width, height, x, y } = selectionCoords;
        const popupWidthPx = 360;
        const popupHeighthPx = 360;
        const minPxFromWindowBorders = 5;

        // place bottom center first
        let topPx = top + height + 10;
        let leftPx = left; // - parseInt(popupWidthPx / 2);
        let bottomPx = "unset";
        let rightPx = "unset";

        if (topPx + popupHeighthPx >= window.innerHeight) {
          // place top
          topPx = top - 10 - popupHeighthPx;
          bottomPx = "unset";

          if (topPx < minPxFromWindowBorders) {
            topPx = minPxFromWindowBorders;
            bottomPx = "unset";
          }
        }

        if (leftPx + popupWidthPx >= window.innerWidth) {
          // place right
          leftPx = "unset";
          rightPx = minPxFromWindowBorders;
        }
        if (leftPx < minPxFromWindowBorders) {
          // place left
          leftPx = minPxFromWindowBorders;
          rightPx = "unset";
        }

        this.privateState.popupCoords = {
          top: topPx === "unset" ? "unset" : topPx + "px",
          left: leftPx === "unset" ? "unset" : leftPx + "px",
          bottom: bottomPx === "unset" ? "unset" : bottomPx + "px",
          right: rightPx === "unset" ? "unset" : rightPx + "px",
          transform: null,
        };

        console.log("popupCoords:", this.privateState.popupCoords);
      }

      const detectedLanguages = languageUtil.detectBrowserLanguages();
      const meaningLanguageCode = detectedLanguages
        ? detectedLanguages.find((x) => x !== wordLanguageCode)
        : "";

      this.privateState.wordModel = {
        ...wordModelDefault,
        word,
        examples,
        wordLanguageCode,
        meaningLanguageCode,
      };

      // reset word meanings
      if (this.wordMeanings && this.wordMeanings.word !== word) {
        this.$store.commit(storeTypes.WORD_MEANINGS_SET, {
          wordMeanings: null,
        });
      }

      this.loadWordMeanings({ word, wordLanguageCode, meaningLanguageCode });

      this.privateState.isOpen = true;
    },
    close: function () {
      this.privateState.isOpen = false;
    },
    reset: function () {
      this.privateState.wordModel = {
        ...wordModelDefault,
      };
      this.privateState.showCreatedWordSummary = false;
    },
    onCloseClick: function () {
      this.close();
      this.reset();
      this.$emit("close");
    },
    onOverlayClick: function (e) {
      const isOverlayClicked = this.$refs.modalOverlay === e.target;
      if (isOverlayClicked) {
        this.close();
      }
    },
    loadWordMeanings: function ({
      word,
      wordLanguageCode,
      meaningLanguageCode,
    }) {
      return this.$store
        .dispatch(storeTypes.WORD_MEANINGS_LOAD, {
          word,
          wordLanguageCode,
          meaningLanguageCode,
        })
        .then()
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    loadWordMeaningsDebounce: _.debounce(function (params) {
      this.loadWordMeanings({ ...params });
    }, 500),
    onMeaningLanguageCodeChange: function (languageCode) {
      this.privateState.wordModel.meaningLanguageCode = languageCode;

      this.loadWordMeaningsDebounce({
        word: this.privateState.wordModel.word,
        wordLanguageCode: this.privateState.wordModel.wordLanguageCode,
        meaningLanguageCode: this.privateState.wordModel.meaningLanguageCode,
      });
    },
    createWord: function () {
      return this.$store
        .dispatch(storeTypes.WORD_CREATE, {
          data: {
            ...this.privateState.wordModel,
          },
        })
        .then(() => {
          this.privateState.showCreatedWordSummary = true;

          setTimeout(() => {
            this.close();
            this.reset();
          }, 2000);
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
  },
};
</script>

<style scoped lang="scss">
.modal-overlay {
  box-sizing: border-box;
  position: fixed;
  z-index: 9998;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  // background-color: rgba(0, 0, 0, 0.5);
  background: transparent;
  display: table;
  transition: opacity 0.3s ease;
}

.modal-wrapper {
  box-sizing: border-box;
  display: block;
  position: fixed;
  z-index: 9998;
  top: 50%;
  left: 50%;

  // set base font
  font-size: 14px;
  font-weight: normal;
  font-style: normal;
  font-family: sans-serif;
}

.modal-container {
  box-sizing: border-box;
  min-width: 360px;
  max-width: 360px;
  width: 360px;
  min-height: 360px;
  max-height: 360px;
  height: 360px;
  margin: 0px auto;
  padding: 1rem;
  background-color: #fff;
  border-radius: 2px;
  box-shadow: 0px 1px 10px 2px rgb(19 18 18 / 50%);
  transition: all 0.3s ease;
  font-family: Helvetica, Arial, sans-serif;
  overflow-y: auto;
}

.modal-header {
  margin-bottom: 20px;
}

.modal-body {
  margin: 0;
}

.modal-footer {
  display: flex;
  margin-top: 20px;
}

.word-meaning-list {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  align-items: flex-start;
  justify-content: flex-start;
  max-height: 100px;
  overflow-y: auto;

  .meaning-option {
    margin-bottom: 0.2rem;
    padding: 0.5rem;
    background-color: #ececec;
    width: auto;
    box-sizing: border-box;
    cursor: pointer;
    margin-top: 0.2rem;

    &:hover {
      background-color: darken(#ececec, 10%);
    }

    &:not(:last-child) {
      margin-right: 0.2rem;
    }

    &.selected {
      background-color: darken(#ececec, 10%);
    }
  }
}
</style>
