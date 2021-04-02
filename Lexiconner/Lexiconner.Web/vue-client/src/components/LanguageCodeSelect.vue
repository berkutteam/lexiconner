<template>
    <multiselect 
        v-model="privateState.selectedLanguageOption" 
        v-bind:options="languageList" 
        v-bind:multiple="false" 
        v-bind:searchable="true" 
        v-bind:close-on-select="true" 
        v-bind:clear-on-select="false" 
        v-bind:preserve-search="true" 
        v-bind:show-labels="true" 
        v-bind:allow-empty="true" 
        v-bind:preselect-first="false"
        v-bind:custom-label="languageLabel"
        track-by="iso639_1_Code" 
        v-bind:placeholder="placeholder"
        v-bind:loading="isLoading"
        v-bind:disabled="false"
        v-on:input="onInput"
    >
    </multiselect>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import _ from 'lodash';

export default {
    name: 'language-code-select',
    props: {
        /**
         * Value is languageCode that is passed as v-model="<languageCodeProp>"
         * v-model does this:
         *      v-bind:value="<languageCodeProp>"
         *      v-on:input="<languageCodeProp> = $event"
         */
        value: {
            // type: [String, Object],
            required: true,
            default: null,
        },

        placeholder: {
            type: String,
            required: false,
            default: 'Select language',
        },
        
        // events: change
    },
    components: {
        // RowLoader,
        // LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                selectedLanguageOption: null,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => state.loading[storeTypes.LANGUAGES_LOAD],
            languageList: function(state) {
                return state.languages || [];
            },
        }),
    },
    created: async function() {
        let self = this;

        // load langs
        if(self.sharedState.languageList == null) {
            self.$store.dispatch(storeTypes.LANGUAGES_LOAD, {})
            .then(() => {
                self.preselectLanguage();
            })
            .catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        // preselect
        self.preselectLanguage();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        preselectLanguage() {
            if(this.value && this.languageList) {
                this.privateState.selectedLanguageOption = this.languageList.find(x => x.iso639_1_Code === this.value) || null;
            }
        },
        languageLabel(option) {
            return `${option.iso639_1_Code} - ${option.isoLanguageName} (${option.nativeName})`;
        },
        onInput: function(value, id) {
            // tell parent that value was changed and it can update its v-model property
            // value is user
            let {isoLanguageName, iso639_1_Code} = value;
            this.$emit('input', iso639_1_Code);

            // emit change event with ISO language code
            this.$emit('change', iso639_1_Code);
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
