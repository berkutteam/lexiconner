<template>
    <div class="my-permissions-wrapper">
       <!-- Word create/edit -->
        <modal 
            name="word-create-edit" 
            height="auto"
            width="450px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">
                        <span v-if="privateState.modalMode === 'create'">Create item</span>
                        <span v-if="privateState.modalMode === 'edit'">Edit item</span>
                    </div>
                    <div v-on:click="$modal.hide('word-create-edit')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                    <form v-on:submit.prevent="createEditWord()">
                        <div class="form-group">
                            <label for="wordModel__title">Word</label>
                            <input v-model="privateState.wordModel.word" type="text" class="form-control" id="wordModel__title" placeholder="Word" />
                        </div>
                        <div class="form-group">
                            <label for="wordModel__description">Meaning</label>
                            <textarea v-model="privateState.wordModel.meaning" type="text" class="form-control" id="wordModel__description" placeholder="Meaning" />
                        </div>
                        <div class="form-group">
                            <label for="wordModel__exampleText">Example text</label>
                            <textarea 
                                v-for="(exampleText, exampleTextIndex) in privateState.wordModel.examples"
                                v-bind:key="`word-exampleText-${exampleTextIndex}`"
                                v-model="privateState.wordModel.examples[exampleTextIndex]" 
                                v-bind:placeholder="`Example text ${exampleTextIndex + 1}`"
                                type="text" 
                                class="form-control mb-1" 
                                id="wordModel__exampleText" 
                            />
                            <div class="btn-group mr-1" role="group">
                                <button 
                                    v-on:click="onAddWordExampleText" 
                                    type="button" 
                                    class="btn btn-sm custom-btn-normal mr-0"
                                >
                                    <i class="fas fa-plus"></i>
                                </button>
                                <button 
                                    v-on:click="onRemoveWordExampleText" 
                                    type="button" 
                                    class="btn btn-sm custom-btn-normal"
                                >
                                    <i class="fas fa-minus"></i>
                                </button>
                            </div>

                             <button 
                                v-on:click="onSearchWordExamplesClick"
                                v-bind:disabled="!canLoadWordExamples"
                                type="button" 
                                class="btn btn-sm custom-btn-normal mr-0"
                            >
                                <i class="fas fa-search-plus"></i>
                            </button>
                        </div>
                        <div class="form-group form-check">
                            <!-- <input v-model="privateState.wordModel.isFavourite" class="form-check-input" type="checkbox" id="wordModel__isFavourite"> -->
                            <!-- <label class="form-check-label" for="wordModel__isFavourite">
                                Favourite
                            </label> -->
                        </div>
                        <div class="form-group">
                            <span v-on:click="onFavoriteChange(!privateState.wordModel.isFavourite)" class="cursor-pointer">
                                <i v-if="privateState.wordModel.isFavourite" class="fas fa-star text-warning "></i>
                                <i v-else class="far fa-star text-warning"></i>
                                <span class="ml-2">Favorite</span>
                            </span>
                        </div>
                        <div class="form-group">
                            <label for="">Word language</label>
                            <language-code-select
                                v-model="privateState.wordModel.wordLanguageCode"
                            />
                        </div>
                        <div class="form-group">
                            <label for="">Meaning language</label>
                            <language-code-select
                                v-model="privateState.wordModel.meaningLanguageCode"
                            />
                        </div>
                        <div class="form-group">
                            <label for="">Tags</label>
                            <tags-multiselect 
                                v-model="privateState.wordModel.tags" 
                            ></tags-multiselect>
                        </div>
                        <div class="form-group">
                            <label for="">Collections (aka folders)</label>
                            <custom-collections-multiselect
                                v-model="privateState.wordModel.customCollectionIds" 
                            ></custom-collections-multiselect>
                        </div>
                        <loading-button 
                            type="submit"
                            v-bind:loading="sharedState.loading[privateState.storeTypes.WORD_CREATE] || sharedState.loading[privateState.storeTypes.WORD_UPDATE]"
                            class="btn custom-btn-normal btn-block"
                        >Save</loading-button>
                    </form>
                </div>
            </div>
        </modal>

        <!-- Word examples modal -->
        <modal 
            name="word-examples" 
            height="auto"
            width="400px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">
                        <span>Word examples</span>
                    </div>
                    <div v-on:click="$modal.hide('word-examples')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORD_EXAMPLES_LOAD]"></row-loader>

                    <form v-on:submit.prevent="addWordExamples()" class="mt-2">
                            <div
                                v-for="(exampleOption) in wordExamplesOptions"
                                v-bind:key="exampleOption.randomId"
                                class="w-100"
                            >
                                <div class="form-check mb-2">
                                    <input 
                                        v-bind:id="`example-${exampleOption.randomId}`" 
                                        v-on:change="(e) => toggleSuggestedWordExample(exampleOption.randomId)"
                                        v-bind:checked="privateState.selectedWordExampleIds.includes(exampleOption.randomId)"
                                        class="form-check-input cursor-pointer" 
                                        type="checkbox" 
                                        value="something" 
                                    >
                                    <label for="`example-${exampleOption.randomId}`" class="form-check-label">
                                        {{exampleOption.example}}
                                    </label>
                                </div>
                            </div>

                            <loading-button 
                                type="submit"
                                v-bind:loading="false"
                                class="btn custom-btn-normal btn-block mt-3"
                            >Save</loading-button>
                    </form>
                </div>
            </div>
        </modal>
    </div>
</template>

<script>
'use strict';

// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import datetimeUtil from '@/utils/datetime';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import TagsMultiselect from '@/components/TagsMultiselect';
import CustomCollectionsMultiselect from '@/components/CustomCollectionsMultiselect';
import CustomCollections from '@/components/CustomCollections';
import WordsFilters from '@/components/WordsFilters';

import ProgressBar from 'vue-simple-progress'

const wordModelDefault = {
    title: null,
    description: null,
    examples: [""],
    isFavourite: false,
    wordLanguageCode: "en",
    meaningLanguageCode: "ru",
    tags: [],
    customCollectionIds: [],
};

export default {
    name: 'word-create-update-modal',
    props: {
    },
    components: {
        RowLoader,
        LoadingButton,
        LanguageCodeSelect,
        TagsMultiselect,
        CustomCollectionsMultiselect,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                wordModel: _.cloneDeep(wordModelDefault),
                modalMode: 'create', // ['create', 'edit']
                selectedWordExampleIds: [], // string[]
            },
        };
    },
    computed: {
        // local computed go here
        canLoadWordExamples: function() {
            return this.privateState.wordModel.wordLanguageCode && this.privateState.wordModel.word;
        },
        // exampels options excluding already selected examples
        wordExamplesOptions: function() {
            return !this.wordExamples ? [] : this.wordExamples.examples.filter(x => {
                return !this.privateState.wordModel.examples.some(y => y === x);
            });
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            word: state => state.word,
            wordExamples: state => state.wordExamples,
        }),
    },
    watch:  {
    },
    created: function() {
        // watch when edited tem loaded
        this.unwatch = this.$store.watch(
            (state, getters) => state.word,
            (newValue, oldValue) => {
                this.privateState.wordModel = { ...newValue };
            }
        );
    },
    mounted: function() {
    },
    updated: function() {
    },
    beforeDestroy: function() {
        this.unwatch();
    },
    destroyed: function() {
    },

    methods: {
        show: function({ wordId, customCollectionIds = [] }) {
            // reset
            this.privateState.wordModel = _.cloneDeep(wordModelDefault);

            if(wordId !== null) {
                this.privateState.modalMode = 'edit';

                this.loadWord({ wordId });
            } else {
                this.privateState.modalMode = 'create';
                this.privateState.wordModel = {
                    ..._.cloneDeep(wordModelDefault),
                    customCollectionIds: [
                        ...customCollectionIds,
                    ]
                };
            }

            this.$modal.show('word-create-edit');
        },
        hide: function() {
            this.$modal.hide('word-create-edit');

            // reset
            this.privateState.wordModel = _.cloneDeep(wordModelDefault);
        },
        loadWord: function({wordId} = {}) {
            return this.$store.dispatch(storeTypes.WORD_LOAD, {
                wordId, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        loadWordExamples: function({ languageCode, word } = {}) {
            return this.$store.dispatch(storeTypes.WORD_EXAMPLES_LOAD, {
                languageCode,
                word, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onFavoriteChange: function(value) {
            this.privateState.wordModel.isFavourite = value;
        },
        onAddWordExampleText: function() {
            this.privateState.wordModel.examples.push("");
        },
        onRemoveWordExampleText: function() {
            if(this.privateState.wordModel.examples.length > 1) {
                this.privateState.wordModel.examples.pop();
            }
        },
        onSearchWordExamplesClick: function() {
            if(!this.canLoadWordExamples) {
                return;
            }

            // reset
            this.privateState.selectedWordExampleIds = [];

            this.loadWordExamples({
                languageCode: this.privateState.wordModel.wordLanguageCode, 
                word: this.privateState.wordModel.word,
            });
            this.$modal.show('word-examples');
        },
        toggleSuggestedWordExample: function(exampleRandomId) {
            const isAlreadySelected = this.privateState.selectedWordExampleIds.includes(exampleRandomId);
            if(isAlreadySelected) {
                this.privateState.selectedWordExampleIds = this.privateState.selectedWordExampleIds.filter(x => x !== exampleRandomId);
            } else {
                this.privateState.selectedWordExampleIds.push(exampleRandomId);
            }
        },
        addWordExamples: function() {
            var selectedExamples = this.wordExamplesOptions.filter(x => this.privateState.selectedWordExampleIds.includes(x.randomId));

            this.privateState.wordModel.examples = [
                ...this.privateState.wordModel.examples.filter(x => !!x),
                ...selectedExamples.map(x => x.example),
            ];
            this.$modal.hide('word-examples');
        },
        createEditWord: function() {
            if(this.privateState.modalMode === 'create') {
                this.createWord();
            } else if(this.privateState.modalMode === 'edit') {
                this.updateWord();
            }
        },
        createWord: function() {
            this.$store.dispatch(storeTypes.WORD_CREATE, {
                data: {
                    ...this.privateState.wordModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item '${this.privateState.wordModel.word}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.wordModel = _.cloneDeep(wordModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateWord: function() {
            this.$store.dispatch(storeTypes.WORD_UPDATE, {
                wordId: this.privateState.wordModel.id,
                data: {
                    ...this.privateState.wordModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item '${this.privateState.wordModel.word}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.wordModel = _.cloneDeep(wordModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
