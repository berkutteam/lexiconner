<template>
  <div class="">
    <div class="row">
      <div class="col-12">
        <!-- <div>
                    <custom-collections
                        v-bind:onSelectedCollectionChange="onSelectedCollectionChange"
                    >
                    </custom-collections>
                </div> -->

        <div v-if="words" class="words-wrapper">
          <!-- Filters -->
          <words-filters v-bind:onChange="loadWords"> </words-filters>

          <row-loader
            v-bind:visible="
              sharedState.loading[privateState.storeTypes.WORDS_LOAD]
            "
          ></row-loader>

          <div>
            <pagination-wrapper
              v-bind:paginationResult="sharedState.wordsPaginationResult"
              v-bind:loadItemsF="loadWords"
              v-bind:showGoToButtons="true"
            >
              <!-- Card view -->
              <div
                v-if="privateState.currentView === 'cards'"
                class="items-card-list"
              >
                <div
                  v-for="item in words"
                  v-bind:key="`card-${item.id}`"
                  class="card bg-light item-card"
                >
                  <div class="item-card-overlay-controls">
                    <button
                      v-on:click="onFindNextWordImagesClick(item.id)"
                      class="btn btn-sm custom-btn-normal"
                    >
                      <i class="far fa-image"></i>
                    </button>
                  </div>
                  <!-- <div class="card-header"></div> -->
                  <img
                    v-if="
                      item.images && item.images.length !== 0 && item.images[0]
                    "
                    class="card-img-top item-card-image"
                    v-bind:src="item.images[0].url"
                    v-bind:alt="item.word"
                  />
                  <img
                    v-else
                    class="card-img-top item-card-image"
                    src="/img/empty-image.png"
                  />
                  <div class="card-body">
                    <div
                      class="
                        d-flex
                        w-100
                        justify-content-between
                        align-items-center
                        mb-1
                      "
                    >
                      <h6 class="card-title mb-0">
                        <span>{{ item.word }}</span>
                      </h6>
                    </div>

                    <div class="card-text small mb-1">
                      <div>{{ item.meaning }}</div>
                    </div>
                    <div class="card-text text-secondary">
                      <div
                        v-for="(exampleText, index2) in item.examples"
                        v-bind:key="`card-${item.id}-exampleText-${index2}`"
                        class="item-card-example-text mb-1"
                      >
                        <i class="fas fa-circle example-text-dot-icon"></i>
                        <span>{{ exampleText }}</span>
                      </div>
                    </div>
                    <div class="card-text">
                      <span class="badge badge-info mr-1">{{
                        item.sourceLanguageCode
                      }}</span>
                      <span
                        v-for="tag in item.tags"
                        v-bind:key="tag"
                        class="badge badge-secondary mr-1"
                        >{{ tag }}</span
                      >
                    </div>

                    <!-- Progress -->
                    <div class="mt-3" style="width: 100%">
                      <progress-bar
                        size="small"
                        bar-color="#67c23a"
                        v-bind:max="100"
                        v-bind:val="item.trainingInfo.totalProgress * 100"
                        text=""
                      ></progress-bar>
                    </div>
                  </div>

                  <!-- Controls -->
                  <div class="card-bottom-controls">
                    <span
                      v-on:click="onWordFavoriteClick(item)"
                      class="card-bottom-control-item"
                    >
                      <i
                        v-if="item.isFavourite"
                        class="fas fa-star text-warning"
                      ></i>
                      <i v-else class="far fa-star text-warning"></i>
                    </span>
                    <span
                      v-if="!item.trainingInfo.isTrained"
                      v-on:click="onMarkWordAsTrained(item.id)"
                      class="card-bottom-control-item"
                    >
                      <i class="fas fa-check"></i>
                    </span>
                    <span
                      v-if="item.trainingInfo.isTrained"
                      v-on:click="onMarkWordAsNotTrained(item.id)"
                      class="card-bottom-control-item"
                    >
                      <i class="fas fa-redo"></i>
                    </span>
                    <span
                      v-on:click="onUpdateWord(item.id)"
                      class="card-bottom-control-item"
                    >
                      <i class="fas fa-pencil-alt"></i>
                    </span>
                    <span
                      v-on:click="onDeleteWord(item.id)"
                      class="card-bottom-control-item"
                    >
                      <i class="fas fa-trash"></i>
                    </span>
                  </div>
                </div>
              </div>
            </pagination-wrapper>
          </div>
        </div>

        <!-- Word create/edit modal -->
        <word-create-update-modal ref="wordCreateUpdateModal">
        </word-create-update-modal>

        <!-- Word images modal -->
        <word-images-modal ref="wordImagesModal"> </word-images-modal>
      </div>
    </div>
  </div>
</template>

<script>
"use strict";

// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import datetimeUtil from "@/utils/datetime";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";
import PaginationWrapper from "@/components/PaginationWrapper";
import CustomCollections from "@/components/CustomCollections";
import WordsFilters from "@/components/WordsFilters";
import WordCreateUpdateModal from "./WordCreateUpdateModal";
import WordImagesModal from "./WordImagesModal";

import ProgressBar from "vue-simple-progress";

export default {
  name: "words-browse",
  components: {
    RowLoader,
    PaginationWrapper,
    // CustomCollections,
    WordsFilters,
    WordCreateUpdateModal,
    WordImagesModal,
    ProgressBar,
  },
  props: {
    // route props
    userWordSetId: {
      required: false,
      default: null,
    },
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        currentView: "cards", // ['list', 'cards']
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      words: (state) =>
        state.wordsPaginationResult ? state.wordsPaginationResult.items : null,
    }),
  },
  created: function () {
    this.loadWords({});

    this.unwatch = this.$store.watch(
      (state, getters) => getters.currentCustomCollectionId,
      (newValue, oldValue) => {
        this.loadWords({});
      }
    );
    this.unwatch2 = this.$store.watch(
      (state, getters) => getters.selectedLearningLanguageCode,
      (newValue, oldValue) => {
        this.loadWords({});
      }
    );
  },
  mounted: function () {},
  updated: function () {},
  beforeDestroy: function () {
    this.unwatch();
    this.unwatch2();
  },
  destroyed: function () {},

  methods: {
    loadWords: function ({ offset = 0, limit = 50 } = {}) {
      return this.$store
        .dispatch(storeTypes.WORDS_LOAD, {
          userWordSetId: this.userWordSetId,
          offset: offset,
          limit: limit,
        })
        .then()
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    onSelectedCollectionChange: function (nextCollectionId) {},
    onCreateWord: function () {
      this.$refs.wordCreateUpdateModal.show({ wordId: null });
    },
    onUpdateWord: function (wordId) {
      this.$refs.wordCreateUpdateModal.show({ wordId });
    },
    onAddWordExampleText: function () {
      this.privateState.wordModel.examples.push("");
    },
    onRemoveWordExampleText: function () {
      this.privateState.wordModel.examples.pop();
    },
    onDeleteWord: function (wordId) {
      if (confirm("Are you sure?")) {
        this.deleteWord(wordId);
      }
    },
    onWordFavoriteClick: function (word) {
      if (word.isFavourite) {
        this.deleteWordFromFavourites(word.id);
      } else {
        this.addWordToFavourites(word.id);
      }
    },
    onMarkWordAsTrained: function (wordId) {
      this.markWordAsTrained(wordId);
    },
    onMarkWordAsNotTrained: function (wordId) {
      this.markWordAsNotTrained(wordId);
    },
    onFindNextWordImagesClick: function (wordId) {
      // console.log(1, this.words.filter(item => item.images && item.images.length !== 0 && !item.images[0]))
      this.$refs.wordImagesModal.show({ wordId });
    },
    deleteWord: function (wordId) {
      this.$store
        .dispatch(storeTypes.WORD_DELETE, {
          wordId: wordId,
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Item has been deleted!`,
            text: "",
            duration: 5000,
          });

          const itemCountThresholdBeforeReload = 3;
          if (this.words.length <= itemCountThresholdBeforeReload) {
            // reload
            this.loadWords();
          }
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    addWordToFavourites: function (wordId) {
      this.$store
        .dispatch(storeTypes.WORD_ADD_TO_FAVOURITES, {
          wordId: wordId,
        })
        .then(() => {})
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    deleteWordFromFavourites: function (wordId) {
      this.$store
        .dispatch(storeTypes.WORD_DELETE_FROM_FAVOURITES, {
          wordId: wordId,
        })
        .then(() => {})
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    markWordAsTrained: function (wordId) {
      this.$store
        .dispatch(storeTypes.WORD_TRAINING_MARK_AS_TRAINED, {
          wordId: wordId,
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Item marked as trained.`,
            text: "",
            duration: 5000,
          });

          // reload
          this.loadWords();
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    markWordAsNotTrained: function (wordId) {
      this.$store
        .dispatch(storeTypes.WORD_TRAINING_MARK_AS_NOT_TRAINED, {
          wordId: wordId,
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Item marked as not trained.`,
            text: "",
            duration: 5000,
          });

          // reload
          this.loadWords();
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
  },
};
</script>
