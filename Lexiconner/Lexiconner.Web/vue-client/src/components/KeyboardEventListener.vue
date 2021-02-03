<template>
  <div></div>
</template>

<script>
  export default {
    name: 'keyboard-event-listener',
    props: {

    },
    created() {
        // listen to global events
      window.addEventListener('keyup', this.handler);
    },
    beforeDestroy() {
      window.removeEventListener('keyup', this.handler);
    },

    methods: {
        handler(e) {
            const component = this;

            // emit KeyboardEvent
            component.$emit('keyup', e);
        }
    }
  }
</script>

<style lang="scss" scoped>
  div {
    display: none;
  }
</style>