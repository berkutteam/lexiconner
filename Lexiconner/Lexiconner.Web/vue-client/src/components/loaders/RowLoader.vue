<template>
  <div v-if="visible" class="app-row-loader">
    <div v-if="type === 'line'" class="loading-line">
      <div class="loading-line__element"></div>
    </div>

    <div v-if="type === 'circle'">
      <i
        class="fas fa-circle-notch fa-spin fa-spin--1s fa-3x loader-spinner"
      ></i>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src

export default {
  name: "row-loader",
  props: {
    visible: Boolean,
    type: {
      type: String,
      required: false,
      default: "line", // line, circle
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

  methods: {},
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
