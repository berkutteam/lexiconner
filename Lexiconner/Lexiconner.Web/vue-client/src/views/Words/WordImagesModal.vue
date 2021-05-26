<template>
  <div class="">
    <!-- Word create/edit -->
    <modal
      name="word-images"
      height="auto"
      width="450px"
      v-bind:classes="[
        'v--modal',
        'v--modal-box',
        'v--modal-box--overflow-visible',
        'v--modal-box--sm-fullwidth',
      ]"
      v-bind:clickToClose="false"
      v-bind:scrollable="true"
      v-on:closed="onModalClosed()"
    >
      <div class="app-modal">
        <div class="app-modal-header">
          <div class="app-modal-title">
            <span>Word images</span>
          </div>
          <div v-on:click="$modal.hide('word-images')" class="app-modal-close">
            <i class="fas fa-times"></i>
          </div>
        </div>

        <div class="app-modal-content">
          <row-loader
            v-bind:visible="
              sharedState.loading[privateState.storeTypes.WORD_IMAGES_FIND]
            "
            class="mb-2"
          ></row-loader>

          <form v-on:submit.prevent="onUpdateWordImagesClick()">
            <div v-if="images" class="form-group">
              <image-grid
                ref="imageGrid1"
                v-bind:images="images"
                v-bind:columnsCount="3"
                v-bind:onSelectedImagesChange="onSelectedImagesChange"
              >
              </image-grid>
            </div>

            <div class="form-group mt-2">
              <label for="wordModel__description">Add image by URL</label>
              <div class="d-flex align-items-center">
                <input
                  v-model="privateState.imageUrl"
                  type="text"
                  class="form-control form-control-sm"
                  placeholder="URL"
                />
                <button
                  v-on:click="onAddImageByUrlClick()"
                  v-bind:disabled="!privateState.imageUrl"
                  type="button"
                  class="btn btn-sm custom-btn-normal ml-1"
                >
                  <i class="fas fa-plus"></i>
                </button>
              </div>
              <!-- TODO: use validation error style -->
              <div v-if="privateState.imageUrlErrorMessage" class="mt-1">
                {{ privateState.imageUrlErrorMessage }}
              </div>
            </div>

            <loading-button
              type="submit"
              v-bind:loading="
                sharedState.loading[privateState.storeTypes.WORD_IMAGES_UPDATE]
              "
              v-bind:disabled="!isCanBeSaved"
              class="btn custom-btn-normal btn-block"
              >Save</loading-button
            >
          </form>
        </div>
      </div>
    </modal>
  </div>
</template>

<script>
"use strict";

// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import _ from "lodash";
import { Guid } from "guid-ts";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notificationUtil from "@/utils/notification";
import datetimeUtil from "@/utils/datetime";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";
import PaginationWrapper from "@/components/PaginationWrapper";
import LanguageCodeSelect from "@/components/LanguageCodeSelect";
import TagsMultiselect from "@/components/TagsMultiselect";
import CustomCollectionsMultiselect from "@/components/CustomCollectionsMultiselect";
import CustomCollections from "@/components/CustomCollections";
import WordsFilters from "@/components/WordsFilters";
import ImageGrid from "@/components/ImageGrid";

import ProgressBar from "vue-simple-progress";

const defaultPrivateState = {
  storeTypes: storeTypes,
  wordId: null,
  selectedImages: [],
  imageUrl: null,
  imageUrlErrorMessage: null,
  imagesAddedByUrl: [],
};

export default {
  name: "word-images-modal",
  props: {},
  components: {
    RowLoader,
    LoadingButton,
    ImageGrid,
  },
  data: function () {
    return {
      privateState: {
        ...defaultPrivateState,
      },
    };
  },
  computed: {
    isCanBeSaved: function () {
      return this.privateState.selectedImages.length !== 0;
    },
    images: function () {
      return [
        ...((this.wordImagesPaginationResult || {}).items || []),
        ...this.privateState.imagesAddedByUrl,
      ];
    },

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
      wordImagesPaginationResult: (state) => state.wordImagesPaginationResult,
    }),
  },
  watch: {},
  created: function () {},
  mounted: function () {},
  updated: function () {},
  beforeDestroy: function () {},
  destroyed: function () {},

  methods: {
    show: function ({ wordId }) {
      // reset
      this.privateState.wordId = wordId;

      if (this.wordImagesPaginationResult === null) {
        this.findNextWordImages({ wordId });
      }

      this.$modal.show("word-images");
    },
    hide: function () {
      this.$modal.hide("word-images");
      this.reset();
    },
    reset: function () {
      this.privateState = _.cloneDeep(defaultPrivateState);
      this.$store.commit(storeTypes.WORD_IMAGES_FIND_SET, {
        wordImagesPaginationResult: null,
      });
    },
    onModalClosed: function () {
      this.reset();
    },
    onSelectedImagesChange: function (images) {
      this.privateState.selectedImages = images;
    },
    onAddImageByUrlClick: function () {
      if (!this.privateState.imageUrl) {
        return;
      }

      // validate URL (exception wilbe thrown)
      try {
        new URL(this.privateState.imageUrl);
      } catch (err) {
        this.privateState.imageUrlErrorMessage = "Invalid image URL";
        return;
      }

      const addedImage = {
        randomId: Guid.newGuid().toString(),
        url: this.privateState.imageUrl,
        isAddedByUrl: true,
        // skip other fields (wil be set on the server)
      };
      this.privateState.imagesAddedByUrl = [
        ...this.privateState.imagesAddedByUrl,
        addedImage,
      ];
      this.privateState.imageUrl = null;
      this.privateState.imageUrlErrorMessage = null;

      // make selected
      setTimeout(() => {
        this.$refs.imageGrid1.selectImageById(addedImage.randomId);
      }, 300);
    },
    onUpdateWordImagesClick: function () {
      this.updateWordImages({
        wordId: this.privateState.wordId,
        images: this.privateState.selectedImages,
      });
    },
    findNextWordImages: function ({ wordId }) {
      this.$store
        .dispatch(storeTypes.WORD_IMAGES_FIND, {
          wordId,
        })
        .then(() => {})
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    updateWordImages: function ({ wordId, images }) {
      this.$store
        .dispatch(storeTypes.WORD_IMAGES_UPDATE, {
          wordId,
          data: {
            images,
          },
        })
        .then(() => {
          this.$notify({
            group: "app",
            type: "success",
            title: `Word images has been updated!`,
            text: "",
            duration: 5000,
          });

          this.hide();
        })
        .catch((err) => {
          console.error(err);
          notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
  },
};
</script>
