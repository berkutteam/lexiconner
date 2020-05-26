<template>
    <multiselect 
        v-model="privateState.selectedTags" 
        tag-placeholder="Add this as new tag" 
        v-bind:placeholder="placeholder" 
        v-bind:label="'name'"
        v-bind:track-by="'id'" 
        v-bind:options="privateState.options" 
        v-bind:multiple="true" 
        v-bind:searchable="true" 
        v-bind:taggable="true" 
        v-on:input="onInput"
        v-on:tag="addTag"
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
    name: 'tags-multiselect',
    props: {
        /**
         * Value is languageCode that is passed as v-model="<languageCodeProp>"
         * v-model does this:
         *      v-bind:value="<languageCodeProp>"
         *      v-on:input="<languageCodeProp> = $event"
         */
        value: {
            // type: Array,
            required: true,
            default: [],
        },
        placeholder: {
            type: String,
            required: false,
            default: 'Search or add a tag',
        },
    },
    components: {
        // RowLoader,
        // LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                options: [],
                selectedTags: [],
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        }),
    },
    created: async function() {
        let self = this;

        // make  initialy passed tags as only available options
        if(this.value && Array.isArray(this.value)) {
            let options = this.value.map(tag => {return {id: tag.substring(0, 2) + '_' + Math.floor((Math.random() * 10000000)), name: tag}});
            this.privateState.options = [...options];
            this.privateState.selectedTags = [...options];
        }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        onInput: function(value, id) {
            // tell parent that value was changed and it can update its v-model property
            // value is user
            let tagOptions = value;
            this.$emit('input', tagOptions.map(x => x.name));
        },
        addTag: function(newTag) {
            const tag = {
                id: newTag.substring(0, 2) + '_' + Math.floor((Math.random() * 10000000)),
                name: newTag,
            }
            this.privateState.options.push(tag);
            this.privateState.selectedTags.push(tag);

            // trigger v-model update
            this.onInput([...this.privateState.selectedTags], null);
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
