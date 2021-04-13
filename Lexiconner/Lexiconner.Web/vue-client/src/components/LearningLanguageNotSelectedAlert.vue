<template>
    <div v-if="isLearningLanguageCodeSelected === false">
        <div class="alert alert-info text-center" role="alert">
            Select learning language to start.
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';

export default {
    name: 'learning-language-not-selected-alert',
    components: {
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        }),

        // store getter
        ...mapGetters([
            'selectedLearningLanguageCode',
            'isLearningLanguageCodeSelected',
        ]),
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
