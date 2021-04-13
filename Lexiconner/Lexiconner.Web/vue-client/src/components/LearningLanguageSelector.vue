<template>
    <div class="d-flex align-items-center">
        <language-code-select
            v-model="selectedLearningLanguageCode"
            placeholder="Learning language"
            v-bind:languageLabelGetter="(option) => `${option.isoLanguageName}`"
            v-bind:withFlags="true"
            v-on:change="onLanguageChange"
        />
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';

export default {
    name: 'learning-language-select',
    props: {
    },
    components: {
        // RowLoader,
        // LoadingButton,
        LanguageCodeSelect,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                languageCode: null,
            },
        };
    },
    computed: {
        selectedLearningLanguageCode: {
            get() {
                return this.$store.getters.selectedLearningLanguageCode;
            },
            set(value) {
                // ignore (set by v-model)
            }
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => state.loading[storeTypes.LANGUAGES_LOAD],
            profile: state => state.profile,
        }),
    },
    created: async function() {
        if(!this.profile) {
            this.$store.dispatch(storeTypes.PROFILE_LOAD, {});
        }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        onLanguageChange(languageCode) {
            this.$store.dispatch(storeTypes.PROFILE_SELECT_LEARNING_LANGUAGE, {
                languageCode: languageCode,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });

            // reload related data
            this.$store.dispatch(storeTypes.WORDS_LOAD, {});
        }
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
