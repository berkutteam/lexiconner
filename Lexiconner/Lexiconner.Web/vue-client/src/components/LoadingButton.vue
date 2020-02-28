<template>
     <button 
        v-bind:class="{'app-loading-button--loading': loading}"
        v-bind:type="computedType" 
        class="btn app-loading-button"
     >
        <span class="button-flex-wrapper">
            <span class="button-content">
                <slot></slot>
            </span>
            <span v-if="loading" class="loading-icon-wrapper">
                <i class="fas fa-circle-notch fa-spin fa-spin--1s loading-icon"></i>
            </span>
        </span>
    </button>
</template>

<script>
// @ is an alias to /src

export default {
    name: 'loading-button',
    props: {
        type: String,
        loading: Boolean,
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
        computedType: function() {
            return this.type || 'button';
        },

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
