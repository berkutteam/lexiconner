<template>
    <div v-if="visible" class="app-loader-main">
        <div v-if="topText" class="mb-4">
          {{topText}}
        </div>
        <i class="far fa-clock fa-spin loader-icon"></i>
        <!-- <i class="far fa-hourglass fa-spin loader-icon"></i> -->
        <!-- <i class="fas fa-stopwatch fa-spin loader-icon"></i> -->
        <div v-if="bottomText" class="mt-4">
          {{bottomText}}
        </div>
    </div>
</template>

<script>
// @ is an alias to /src

export default {
    name: 'main-loader',
    props: {
        visible: Boolean,
        topText: String,
        bottomText: String,
    },
    components: {
    },
    data: function() {
        return {
            privateState: {
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        
    },
    created: async function() {
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
