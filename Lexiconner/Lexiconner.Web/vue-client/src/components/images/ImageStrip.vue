<template>
  <div v-if="images" class="image-strip">
    <div
      v-for="image in images"
      v-bind:key="image.id || image.randomId"
      class="image-container"
    >
      <img
        v-bind:src="image.url"
        v-bind:style="{ maxHeight: `${imageHeightPx}px` }"
        class=""
        alt=""
      />
    </div>
  </div>
</template>

<script>
// @ is an alias to /src

export default {
  name: "image-strip",
  props: {
    // Array<{url:, ...}>
    images: {
      type: Array,
      required: true,
    },
    imageHeightPx: {
      type: Number,
      required: false,
      default: 100,
    },
  },
  components: {},
  data: function () {
    return {
      privateState: {},
    };
  },
  computed: {
    // local computed go here
    // store state computed go here
  },
  created: async function () {},
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},
  watch: {},
  methods: {},
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss" src="./ImageStrip.scss"></style>
