<template>
    <div class="home-wrapper">
        
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';

export default {
  name: 'home',
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
        ...mapState({
            sharedState: state => state
        })
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
