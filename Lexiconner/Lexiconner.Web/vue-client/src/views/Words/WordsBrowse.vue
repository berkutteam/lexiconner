<template>
    <div class="">
        <div class="row">
            <div class="col-12">
                <div>
                    <custom-collections
                        v-bind:onSelectedCollectionChange="onSelectedCollectionChange"
                    >
                    </custom-collections>
                </div>

                <div v-if="words" class="words-wrapper">
                    <h5 class="mb-3">Words:</h5>

                    <!-- Toolbar -->
                    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Toolbar with button groups">
                        <div class="btn-group mr-2" role="group" aria-label="View toggle">
                            <button 
                                v-on:click="toggleView" 
                                v-bind:class="{'btn-primary': privateState.currentView === 'list', 'btn-secondary' : privateState.currentView !== 'list'}"
                                type="button" 
                                class="btn"
                            >
                                <i class="fas fa-list"></i>
                            </button>
                            <button 
                                v-on:click="toggleView" 
                                v-bind:class="{'btn-primary': privateState.currentView === 'cards', 'btn-secondary' : privateState.currentView !== 'cards'}"
                                type="button" 
                                class="btn"
                            >
                                <i class="fas fa-th"></i>
                            </button>
                        </div>
                        <div class="btn-group mr-2" role="group" aria-label="Create a new item">
                            <button 
                                v-on:click="onCreateWord" 
                                type="button" 
                                class="btn custom-btn-normal"
                            >
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                    </div>

                    <!-- Filters -->
                    <words-filters
                        v-bind:onChange="loadWords"
                    >
                    </words-filters>
                    
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                    <div>
                        <pagination-wrapper
                            v-bind:paginationResult="sharedState.wordsPaginationResult"
                            v-bind:loadItemsF="loadWords"
                            v-bind:showGoToButtons="true"
                        >
                            <!-- List view -->
                            <div v-if="privateState.currentView === 'list'" class="list-group words-list">
                                <a 
                                    v-for="(item) in words"
                                    v-bind:key="`list-${item.id}`"
                                    href="javascript:void(0)" 
                                    class="list-group-item list-group-item-action flex-column align-items-start word"
                                >
                                    <div class="d-flex w-100 justify-content-between mb-1">
                                        <h6 class="mb-0">{{item.word}}</h6>

                                        <!-- Controls -->
                                        <div class="d-flex justify-content-end flex-grow-1">
                                            <div class="d-flex align-items-center">
                                                <!-- Progress -->
                                                <div class="mr-2" style="width: 60px">
                                                    <progress-bar 
                                                        size="small" 
                                                        bar-color="#67c23a" 
                                                        v-bind:max="100"
                                                        v-bind:val="item.trainingInfo.totalProgress * 100" 
                                                        text=""
                                                    ></progress-bar>
                                                </div>
                                                <span class="badge badge-info mr-1">{{ item.sourceLanguageCode }}</span>

                                                <!-- Tags -->
                                                <span
                                                    v-for="(tag) in item.tags"
                                                    v-bind:key="tag"
                                                    class="badge badge-secondary mr-1"
                                                >
                                                    {{tag}}
                                                </span>

                                                <!-- Favorite -->
                                                <span v-on:click="onWordFavoriteClick(item)" class="cursor-pointer">
                                                    <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                                    <i v-else class="far fa-star text-warning"></i>
                                                </span>
                                                <span class="ml-2 mr-2">|</span>
                                            </div>

                                            <!-- Buttons -->
                                            <span>
                                                <span v-on:click="onMarkWordAsTrained(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-check"></i>
                                                </span>
                                                <span v-on:click="onMarkWordAsNotTrained(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-redo"></i>
                                                </span>
                                                <span v-on:click="onUpdateWord(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-pencil-alt"></i>
                                                </span>
                                                <span v-on:click="onDeleteWord(item.id)" class="badge badge-secondary cursor-pointer">
                                                    <i class="fas fa-trash"></i>
                                                </span>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="mb-1">
                                        <small>{{ item.meaning }}</small>
                                    </div>
                                    <div class="text-secondary">
                                        <div
                                            v-for="(exampleText, index2) in item.examples"
                                            v-bind:key="`card-${item.id}-exampleText-${index2}`"
                                            class="word-example-text mb-1"
                                        >
                                            <i class="fas fa-circle example-text-dot-icon"></i>
                                            <span>{{ exampleText }}</span>
                                        </div>
                                    </div>
                                </a>
                            </div>

                            <!-- Card view -->
                            <div v-if="privateState.currentView === 'cards'" class="items-card-list">
                                <div
                                    v-for="(item) in words"
                                    v-bind:key="`card-${item.id}`"
                                    class="card bg-light item-card" 
                                >
                                    <div class="item-card-overlay-controls">
                                        <button v-on:click="onFindNextWordImagesClick(item.id)" class="btn btn-sm custom-btn-normal">
                                            <i class="far fa-image"></i>
                                        </button>
                                    </div>
                                    <!-- <div class="card-header"></div> -->
                                    <img v-if="item.images && item.images.length !== 0 && item.images[0]" class="card-img-top item-card-image" v-bind:src="item.images[0].url" v-bind:alt="item.word">
                                    <img v-else class="card-img-top item-card-image" src="/img/empty-image.png">
                                    <div class="card-body">
                                        <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                            <h6 class="card-title mb-0">
                                                <span>{{item.word}}</span>
                                            </h6>
                                        </div>
                                    
                                        <div class="card-text small mb-1">
                                            <div>{{ item.meaning }}</div>
                                        </div>
                                        <div class="card-text text-secondary">
                                            <div
                                                v-for="(exampleText, index2) in item.examples"
                                                v-bind:key="`card-${item.id}-exampleText-${index2}`"
                                                class="item-card-example-text mb-1"
                                            >
                                                <i class="fas fa-circle example-text-dot-icon"></i>
                                                <span>{{ exampleText }}</span>
                                            </div>
                                        </div>
                                        <div class="card-text">
                                            <span class="badge badge-info mr-1">{{ item.sourceLanguageCode }}</span>
                                            <span
                                                v-for="(tag) in item.tags"
                                                v-bind:key="tag"
                                                class="badge badge-secondary mr-1"
                                            >{{tag}}</span>
                                        </div>

                                        <!-- Progress -->
                                        <div class="mt-3" style="width: 100%">
                                            <progress-bar 
                                                size="small" 
                                                bar-color="#67c23a" 
                                                v-bind:max="100"
                                                v-bind:val="item.trainingInfo.totalProgress * 100" 
                                                text=""
                                            ></progress-bar>
                                        </div>
                                    </div>

                                    <!-- Controls -->
                                    <div class="card-bottom-controls">
                                        <span v-on:click="onWordFavoriteClick(item)" class="card-bottom-control-item">
                                            <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                            <i v-else class="far fa-star text-warning"></i>
                                        </span>
                                        <span v-on:click="onMarkWordAsTrained(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-check"></i>
                                        </span>
                                        <span v-on:click="onMarkWordAsNotTrained(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-redo"></i>
                                        </span>
                                        <span v-on:click="onUpdateWord(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-pencil-alt"></i>
                                        </span>
                                        <span v-on:click="onDeleteWord(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-trash"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </pagination-wrapper>
                    </div>
                </div>


                <!-- Word create/edit -->
                <word-create-update-modal
                    ref="wordCreateUpdateModal"
                >
                </word-create-update-modal>

                <!-- Word images -->
                <word-images-modal
                    ref="wordImagesModal"
                >
                </word-images-modal>
            </div>
        </div>
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
import CustomCollections from '@/components/CustomCollections';
import WordsFilters from '@/components/WordsFilters';
import WordCreateUpdateModal from './WordCreateUpdateModal';
import WordImagesModal from './WordImagesModal';

import ProgressBar from 'vue-simple-progress'

export default {
    name: 'words-browse',
    components: {
        RowLoader,
        PaginationWrapper,
        CustomCollections,
        WordsFilters,
        WordCreateUpdateModal,
        WordImagesModal,
        ProgressBar,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                currentView: localStorage.getItem(`wordsBrowse_currentView`) || 'list', // ['list', 'cards']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            words: state => state.wordsPaginationResult ? state.wordsPaginationResult.items : null,
        }),
    },
    created: function() {
        this.loadWords({});

        this.unwatch = this.$store.watch(
            (state, getters) => getters.currentCustomCollectionId,
            (newValue, oldValue) => {
                this.loadWords({});
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
        loadWords: function({offset = 0, limit = 50} = {}) {
            return this.$store.dispatch(storeTypes.STUDY_ITEMS_LOAD, {
                offset: offset, 
                limit: limit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onSelectedCollectionChange: function(nextCollectionId) {
        },
        toggleView: function() {
            this.privateState.currentView = this.privateState.currentView === 'list' ? 'cards' : 'list';
            localStorage.setItem(`wordsBrowse_currentView`, this.privateState.currentView);
        },
        onCreateWord: function() {
            this.$refs.wordCreateUpdateModal.show({wordId: null});
        },
        onUpdateWord: function(wordId) {
            this.$refs.wordCreateUpdateModal.show({wordId});
        },
        onAddWordExampleText: function() {
            this.privateState.wordModel.examples.push("");
        },
        onRemoveWordExampleText: function() {
            this.privateState.wordModel.examples.pop();
        },
        onDeleteWord: function(wordId) {
            if(confirm('Are you sure?')) {
                this.deleteWord(wordId);
            }
        },
        onWordFavoriteClick: function(word) {
            if(word.isFavourite) {
                this.deleteWordFromFavourites(word.id);
            } else {
                this.addWordToFavourites(word.id);
            }
        },
        onMarkWordAsTrained: function(wordId) {
            this.markWordAsTrained(wordId);
        },
        onMarkWordAsNotTrained: function(wordId) {
            this.markWordAsNotTrained(wordId);
        },
        onFindNextWordImagesClick: function(wordId) {
            // console.log(1, this.words.filter(item => item.images && item.images.length !== 0 && !item.images[0]))
            this.$refs.wordImagesModal.show({ wordId });
        },
        deleteWord: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_DELETE, {
                wordId: wordId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item has been deleted!`,
                    text: '',
                    duration: 5000,
                });

                const itemCountThresholdBeforeReload = 3;
                if(this.words.length <= itemCountThresholdBeforeReload) {
                    // reload
                    this.loadWords();
                }
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        addWordToFavourites: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_ADD_TO_FAVOURITES, {
                wordId: wordId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteWordFromFavourites: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_DELETE_FROM_FAVOURITES, {
                wordId: wordId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markWordAsTrained: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_TRAINING_MARK_AS_TRAINED, {
                wordId: wordId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item marked as trained.`,
                    text: '',
                    duration: 5000,
                });

                // reload
                this.loadWords();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markWordAsNotTrained: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_TRAINING_MARK_AS_NOT_TRAINED, {
                wordId: wordId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item marked as not trained.`,
                    text: '',
                    duration: 5000,
                });

                // reload
                this.loadWords();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
