<template>
  <div class="container">
    <h4>
      <span v-if="privateState.modalMode === 'create'">Create word set</span>
      <span v-if="privateState.modalMode === 'edit'">Edit word set</span>
    </h4>
    <hr />
    <form v-on:submit.prevent="createEditWordSet()">
      <div class="form-group">
        <label for="">Name</label>
        <input
          v-model="privateState.wordSetModel.name"
          type="text"
          class="form-control"
          placeholder="Name"
        />
      </div>
      <div class="form-group">
        <label for="">Words language</label>
        <language-code-select
          v-model="privateState.wordSetModel.wordsLanguageCode"
        />
        <small class="form-text text-muted"
          >Specify language for words in the set.</small
        >
      </div>
      <div class="form-group">
        <label for="">Meanings language</label>
        <language-code-select
          v-model="privateState.wordSetModel.meaningsLanguageCode"
        />
        <small class="form-text text-muted"
          >Specify language for words' meanings.</small
        >
      </div>

      <!-- Images -->
      <div class="form-group">
        <label for="">Images</label>
        <button
          type="button"
          class="btn btn-sm custom-btn-normal ml-2"
          v-on:click="onFindWordSetImagesClick()"
          v-bind:disabled="!canSearchWordSetImages()"
        >
          <i class="far fa-images"></i>
        </button>
        <div v-if="privateState.wordSetModel.images" class="">
          <image-strip
            v-bind:images="privateState.wordSetModel.images"
            v-bind:imageHeightPx="100"
          >
          </image-strip>
        </div>
      </div>

      <h6><strong>Words</strong></h6>
      <hr />
      <div>
        <div
          v-for="(word, wordIndex) in privateState.wordSetModel.words"
          v-bind:key="wordIndex"
          class="form-row mb-4"
        >
          <div class="col">
            <input
              v-model="privateState.wordSetModel.words[wordIndex].word"
              type="text"
              class="form-control form-control-sm"
              placeholder="Word"
            />
          </div>
          <div class="col">
            <input
              v-model="privateState.wordSetModel.words[wordIndex].meaning"
              type="text"
              class="form-control form-control-sm"
              placeholder="Meaning"
            />
          </div>

          <!-- Examples -->
          <div class="col">
            <div class="d-flex">
              <div class="flex-grow-1">
                <div
                  v-for="(example, exampleIndex) in word.examples"
                  v-bind:key="exampleIndex"
                  v-bind:class="{ 'mt-1': exampleIndex !== 0 }"
                >
                  <textarea
                    v-model="
                      privateState.wordSetModel.words[wordIndex].examples[
                        exampleIndex
                      ]
                    "
                    type="text"
                    class="form-control form-control-sm"
                    placeholder="Example"
                  />
                </div>
              </div>
              <div class="d-flex flex-column justify-content-end ml-1">
                <button
                  type="button"
                  class="btn btn-sm custom-btn-normal mb-1"
                  v-on:click="onRemoveWordExampleClick(wordIndex)"
                >
                  <i class="fas fa-minus"></i>
                </button>
                <button
                  type="button"
                  class="btn btn-sm custom-btn-normal"
                  v-on:click="onAddWordExampleClick(wordIndex)"
                >
                  <i class="fas fa-plus"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Images -->
          <div class="col">
            <div class="d-flex">
              <div class="flex-grow-1">
                <div v-if="word.images" class="">
                  <image-strip
                    v-bind:images="word.images"
                    v-bind:imageHeightPx="60"
                  >
                  </image-strip>
                </div>
              </div>
              <div class="ml-1">
                <button
                  type="button"
                  class="btn btn-sm custom-btn-normal"
                  v-on:click="onFindWordImagesClick(wordIndex)"
                  v-bind:disabled="!canSearchWordImages(word)"
                >
                  <i class="far fa-images"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- Other controls -->
          <div class="col flex-grow-0">
            <button
              type="button"
              class="btn btn-sm custom-btn-normal ml-1"
              v-on:click="onRemoveWordClick(wordIndex)"
            >
              <i class="fas fa-times"></i>
            </button>
          </div>
        </div>

        <button
          type="button"
          class="btn btn-sm custom-btn-normal"
          v-on:click="onAddWordClick()"
        >
          <i class="fas fa-plus"></i>
        </button>
      </div>
      <hr />

      <div class="form-check mb-4">
        <input
          type="checkbox"
          class="form-check-input"
          id="isPublishedCheck"
          v-model="privateState.wordSetModel.isPublished"
        />
        <label class="form-check-label" for="isPublishedCheck">
          <i
            v-if="!privateState.wordSetModel.isPublished"
            class="fas fa-lock"
          ></i>
          <i
            v-if="privateState.wordSetModel.isPublished"
            class="fas fa-lock-open"
          ></i>
          Publish
        </label>
      </div>
      <loading-button
        type="submit"
        v-bind:loading="
          sharedState.loading[privateState.storeTypes.WORD_SET_CREATE] ||
          sharedState.loading[privateState.storeTypes.WORD_SET_UPDATE]
        "
        class="btn custom-btn-normal btn-block"
        >Save</loading-button
      >
    </form>

    <!-- Find images modal -->
    <find-images-modal ref="findImagesModal"> </find-images-modal>
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
import PaginationWrapper from "@/components/PaginationWrapper";
import LanguageCodeSelect from "@/components/LanguageCodeSelect";
import LearningLanguageNotSelectedAlert from "@/components/LearningLanguageNotSelectedAlert";
import FindImagesModal from "@/components/FindImagesModal";
import ImageGrid from "@/components/ImageGrid";
import ImageStrip from "@/components/images/ImageStrip";

const wordSetModelDefault = {
  name: null,
  wordsLanguageCode: null,
  meaningsLanguageCode: null,
  isPublished: false,
  words: [],
  images: [],
};
const wordSetWordModelDefault = {
  word: null,
  meaning: null,
  examples: [""],
  images: [],
};

export default {
  name: "wordsets-create",
  components: {
    LoadingButton,
    LanguageCodeSelect,
    FindImagesModal,
    ImageStrip,
  },
  props: {
    // route props
    wordSetId: {
      required: false,
      default: null,
    },
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        wordSetModel: {
          ...wordSetModelDefault,
        },
        modalMode: "create", // ['create', 'edit']
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
    }),

    // store getter
    ...mapGetters([]),
  },
  created: function () {
    // reset
    this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);

    if (this.wordSetId) {
      this.privateState.modalMode = "edit";

      this.loadWordSet({ wordSetId: this.wordSetId });
    } else {
      this.privateState.modalMode = "create";
    }

    // watch when edited word loaded
    this.unwatch = this.$store.watch(
      (state, getters) => state.wordSet,
      (newValue, oldValue) => {
        this.privateState.wordSetModel = { ...newValue };
      }
    );
  },
  mounted: function () {},
  updated: function () {},
  beforeDestroy: function () {
    this.unwatch();
  },
  destroyed: function () {},

  methods: {
    canSearchWordSetImages: function () {
      return (
        this.privateState.wordSetModel.wordsLanguageCode &&
        this.privateState.wordSetModel.name
      );
    },
    canSearchWordImages: function (word) {
      return this.privateState.wordSetModel.wordsLanguageCode && word.word;
    },
    onFindWordSetImagesClick: function (wordIndex) {
      this.$refs.findImagesModal.show({
        languageCode: this.privateState.wordSetModel.wordsLanguageCode,
        search: this.privateState.wordSetModel.name,
        onSave: ({ images }) => {
          this.privateState.wordSetModel.images = images;
        },
      });
    },
    onFindWordImagesClick: function (wordIndex) {
      const word = this.privateState.wordSetModel.words[wordIndex];
      if (!word) {
        throw new Error(`Can't find word with index ${wordIndex}`);
      }
      this.$refs.findImagesModal.show({
        languageCode: this.privateState.wordSetModel.wordsLanguageCode,
        search: word.word,
        onSave: ({ images }) => {
          console.log(1, images);
          this.privateState.wordSetModel.words.splice(wordIndex, 1, {
            ...word,
            images: [...images],
          });
        },
      });
    },
    onRemoveWordClick: function (wordIndex) {
      this.privateState.wordSetModel.words.splice(wordIndex, 1);
    },
    onAddWordClick: function () {
      this.privateState.wordSetModel.words.push({
        ...wordSetWordModelDefault,
      });
    },
    onRemoveWordExampleClick: function (wordIndex) {
      if (this.privateState.wordSetModel.words[wordIndex].examples.length > 1) {
        this.privateState.wordSetModel.words[wordIndex].examples.splice(-1, 1);
      }
    },
    onAddWordExampleClick: function (wordIndex) {
      this.privateState.wordSetModel.words[wordIndex].examples.push("");
    },
    createEditWordSet: function () {
      if (this.privateState.modalMode === "create") {
        this.createWordSet();
      } else if (this.privateState.modalMode === "edit") {
        this.updateWordSet();
      }
    },
    createWordSet: function () {
      this.$store
        .dispatch(storeTypes.WORD_SET_CREATE, {
          data: {
            ...this.privateState.wordSetModel,
          },
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Word set '${this.privateState.wordSetModel.name}' has been created!`,
            text: "",
            duration: 5000,
          });

          // reset
          this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    updateWordSet: function () {
      // this.$store.dispatch(storeTypes.WORD_SET_UPDATE, {
      //     wordId: this.privateState.wordSetModel.id,
      //     data: {
      //         ...this.privateState.wordSetModel,
      //     },
      // }).then(() => {
      //      this.$notify({
      //         group: 'app',
      //         type: 'success',
      //         title: `Item '${this.privateState.wordSetModel.name}' has been updated!`,
      //         text: '',
      //         duration: 5000,
      //     });
      //     // reset
      //     this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);
      // }).catch(err => {
      //     console.error(err);
      //     notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
      // });
    },
  },
};
</script>
