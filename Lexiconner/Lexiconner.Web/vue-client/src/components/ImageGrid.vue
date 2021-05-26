<template>
  <div
    v-if="images"
    class="image-grid mb-2"
    v-bind:class="{ [`image-grid--${columnsCount}-columns`]: true }"
  >
    <div class="image-grid-row">
      <div
        v-for="(column, columnIndex) in columnsList"
        v-bind:key="columnIndex"
        class="image-grid-column"
      >
        <div
          v-for="image in getColumnImages(columnIndex)"
          v-bind:key="image.id || image.randomId"
          v-on:click="onImageClick(image)"
          class="image-container image-container--interactable"
          v-bind:class="{
            'image-container--selected': checkImageSelected(image),
          }"
        >
          <img v-bind:src="image.url" class="rounded" alt="" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src

export default {
  name: "image-grid",
  props: {
    // Array<{url:, ...}>
    images: {
      type: Array,
      required: true,
    },
    columnsCount: {
      type: Number,
      required: true,
    },
    onSelectedImagesChange: {
      type: Function,
      required: false,
    },
  },
  components: {},
  data: function () {
    return {
      privateState: {
        selectedImages: {},
      },
    };
  },
  computed: {
    // local computed go here
    columnsList: function () {
      return Array.from({ length: this.columnsCount });
    },

    // store state computed go here
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},
  watch: {},
  methods: {
    // external
    selectImageById: function (id) {
      const image =
        this.images.find((x) => x.id === id || x.randomId === id) || null;
      console.log(`selectImageById.`, id, image);

      if (image) {
        this.onImageClick(image);
      }
    },

    checkImageSelected: function (image) {
      return !!this.privateState.selectedImages[image.id || image.randomId];
    },
    getColumnImages: function (columnIndex) {
      const perColumn = Math.ceil(this.images.length / this.columnsCount);
      const images = this.images.slice(
        columnIndex * perColumn,
        columnIndex * perColumn + perColumn
      );
      return images;
    },
    onImageClick: function (image) {
      if (this.privateState.selectedImages[image.id || image.randomId]) {
        this.privateState.selectedImages = {
          ...this.privateState.selectedImages,
          [image.id || image.randomId]: undefined,
        };
      } else {
        this.privateState.selectedImages = {
          ...this.privateState.selectedImages,
          [image.id || image.randomId]: image,
        };
      }

      if (this.onSelectedImagesChange) {
        this.onSelectedImagesChange(
          Object.values(this.privateState.selectedImages)
        );
      }
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
