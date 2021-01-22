<template>
    <multiselect 
        v-model="privateState.selectedCustomCollections" 
        v-bind:placeholder="placeholder" 
        v-bind:label="'name'"
        v-bind:track-by="'id'" 
        v-bind:options="options" 
        v-bind:multiple="true" 
        v-bind:searchable="true" 
        v-bind:taggable="false" 
        v-on:input="onInput"
    >
    <!-- Custom option template -->
        <template slot="option" slot-scope="props">
            <div class="option__desc">
                <span class="option__title">{{ props.option.levelPad }}{{ props.option.name }}</span>
            </div>
        </template>
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
    name: 'custom-collections-multiselect',
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
            default: 'Search for a collection',
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
                //options: [],
                selectedCustomCollections: [],
            },
        };
    },
    computed: {
        // local computed go here
        options: function() {
            if(this.customCollectionsResult !== null) {
                return this.customCollectionsList.map(x => {
                    const levelPad = Array.from({length: x.level + 1}).join('··');
                    return {
                        id: x.id,
                        name: x.name,
                        levelPad: levelPad,
                    };
                });
            }
            return [];
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            customCollectionsTree: state => state.customCollectionsResult,
            customCollectionsResult: state => state.customCollectionsResult.asTree,
            customCollectionsList: state => state.customCollectionsResult.asList,
        }),
    },
    watch: {
        value: function(newValue, oldValue) {
            const isHasNewOptions = this.value.join('_') !== this.privateState.selectedCustomCollections.map(x => x.id).join('_');
            console.log(3, isHasNewOptions, newValue, oldValue)
            if(isHasNewOptions) {
                this.setSelectedOptions(newValue);
            }
        }
    },
    created: async function() {
        let self = this;

        // load custom collections
        if(this.customCollectionsResult === null) {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTIONS_LOAD, {})
            .then((customCollectionsResult) => {
            })
            .catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
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
            let options = value;
            this.$emit('input', options.map(x => x.id));
        },
        setSelectedOptions: function(customCollectionIdsToSelect) {
            // make initialy passed collections as selected
            if(customCollectionIdsToSelect && Array.isArray(customCollectionIdsToSelect)) {
                let selectedOptions = customCollectionIdsToSelect.map(customCollectionId => {
                    const option = this.options.find(x => x.id === customCollectionId);
                    return {
                        id: option.id, 
                        name: option.name,
                        levelPad: option.levelPad,
                    }
                });
                // this.privateState.options = [...options];
                this.privateState.selectedCustomCollections = [...selectedOptions];
            }
        }
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
