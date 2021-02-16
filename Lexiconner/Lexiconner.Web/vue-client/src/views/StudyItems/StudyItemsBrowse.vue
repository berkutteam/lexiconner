<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <div>
                    <custom-collections
                        v-bind:onSelectedCollectionChange="onSelectedCollectionChange"
                    >
                    </custom-collections>
                </div>

                <div v-if="studyItems" class="study-items-wrapper">
                    <h5 class="mb-3">Study items:</h5>

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
                                v-on:click="onCreateStudyItem" 
                                type="button" 
                                class="btn custom-btn-normal"
                            >
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                    </div>

                    <!-- Filters -->
                    <study-items-filters
                        v-bind:onChange="loadStudyItems"
                    >
                    </study-items-filters>
                    
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                    <div>
                        <pagination-wrapper
                            v-bind:paginationResult="sharedState.studyItemsPaginationResult"
                            v-bind:loadItemsF="loadStudyItems"
                            v-bind:showGoToButtons="true"
                        >
                            <!-- List view -->
                            <div v-if="privateState.currentView === 'list'" class="list-group study-items-list">
                                <a 
                                    v-for="(item) in studyItems"
                                    v-bind:key="`list-${item.id}`"
                                    href="javascript:void(0)" 
                                    class="list-group-item list-group-item-action flex-column align-items-start study-item"
                                >
                                    <div class="d-flex w-100 justify-content-between mb-1">
                                        <h6 class="mb-0">{{item.title}}</h6>

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
                                                <span class="badge badge-info mr-1">{{ item.languageCode }}</span>

                                                <!-- Tags -->
                                                <span
                                                    v-for="(tag) in item.tags"
                                                    v-bind:key="tag"
                                                    class="badge badge-secondary mr-1"
                                                >
                                                    {{tag}}
                                                </span>

                                                <!-- Favorite -->
                                                <span v-on:click="onStudyItemFavoriteClick(item)" class="cursor-pointer">
                                                    <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                                    <i v-else class="far fa-star text-warning"></i>
                                                </span>
                                                <span class="ml-2 mr-2">|</span>
                                            </div>

                                            <!-- Buttons -->
                                            <span>
                                                <span v-on:click="onMarkStudyItemAsTrained(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-check"></i>
                                                </span>
                                                <span v-on:click="onMarkStudyItemAsNotTrained(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-redo"></i>
                                                </span>
                                                <span v-on:click="onUpdateStudyItem(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-pencil-alt"></i>
                                                </span>
                                                <span v-on:click="onDeleteStudyItem(item.id)" class="badge badge-secondary cursor-pointer">
                                                    <i class="fas fa-trash"></i>
                                                </span>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="mb-1">
                                        <small>{{ item.description }}</small>
                                    </div>
                                    <div class="text-secondary">
                                        <div
                                            v-for="(exampleText, index2) in item.exampleTexts"
                                            v-bind:key="`card-${item.id}-exampleText-${index2}`"
                                            class="study-item-example-text mb-1"
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
                                    v-for="(item) in studyItems"
                                    v-bind:key="`card-${item.id}`"
                                    class="card bg-light item-card" 
                                >
                                    <!-- <div class="card-header"></div> -->
                                    <img v-if="item.image" class="card-img-top item-card-image" v-bind:src="item.image.url" v-bind:alt="item.title">
                                    <img v-else class="card-img-top item-card-image" src="/img/empty-image.png">
                                    <div class="card-body">
                                        <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                            <h6 class="card-title mb-0">
                                                <span>{{item.title}}</span>
                                            </h6>
                                        </div>
                                    
                                        <div class="card-text small mb-1">
                                            <div>{{ item.description }}</div>
                                        </div>
                                        <div class="card-text text-secondary">
                                            <div
                                                v-for="(exampleText, index2) in item.exampleTexts"
                                                v-bind:key="`card-${item.id}-exampleText-${index2}`"
                                                class="item-card-example-text mb-1"
                                            >
                                                <i class="fas fa-circle example-text-dot-icon"></i>
                                                <span>{{ exampleText }}</span>
                                            </div>
                                        </div>
                                        <div class="card-text">
                                            <span class="badge badge-info mr-1">{{ item.languageCode }}</span>
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
                                        <span v-on:click="onStudyItemFavoriteClick(item)" class="card-bottom-control-item">
                                            <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                            <i v-else class="far fa-star text-warning"></i>
                                        </span>
                                        <span v-on:click="onMarkStudyItemAsTrained(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-check"></i>
                                        </span>
                                        <span v-on:click="onMarkStudyItemAsNotTrained(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-redo"></i>
                                        </span>
                                        <span v-on:click="onUpdateStudyItem(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-pencil-alt"></i>
                                        </span>
                                        <span v-on:click="onDeleteStudyItem(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-trash"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </pagination-wrapper>
                    </div>
                </div>


                <!-- Study item create/edit -->
                <study-item-create-update-modal
                    ref="studyItemCreateUpdateModal"
                >
                </study-item-create-update-modal>
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
import StudyItemsFilters from '@/components/StudyItemsFilters';
import StudyItemCreateUpdateModal from './StudyItemCreateUpdateModal';

import ProgressBar from 'vue-simple-progress'

export default {
    name: 'study-items-browse',
    components: {
        RowLoader,
        PaginationWrapper,
        CustomCollections,
        StudyItemsFilters,
        StudyItemCreateUpdateModal,
        ProgressBar,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                currentView: localStorage.getItem(`studyItemsBrowse_currentView`) || 'list', // ['list', 'cards']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            studyItems: state => state.studyItemsPaginationResult ? state.studyItemsPaginationResult.items : null,
        }),
    },
    created: function() {
        this.loadStudyItems({});

        this.unwatch = this.$store.watch(
            (state, getters) => getters.currentCustomCollectionId,
            (newValue, oldValue) => {
                this.loadStudyItems({});
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
        loadStudyItems: function({offset = 0, limit = 50} = {}) {
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
            localStorage.setItem(`studyItemsBrowse_currentView`, this.privateState.currentView);
        },
        onCreateStudyItem: function() {
            this.$refs.studyItemCreateUpdateModal.show({studyItemId: null});
        },
        onUpdateStudyItem: function(studyItemId) {
            this.$refs.studyItemCreateUpdateModal.show({studyItemId});
        },
        onAddStudyItemExampleText: function() {
            this.privateState.studyItemModel.exampleTexts.push("");
        },
        onRemoveStudyItemExampleText: function() {
            this.privateState.studyItemModel.exampleTexts.pop();
        },
        onDeleteStudyItem: function(studyItemId) {
            if(confirm('Are you sure?')) {
                this.deleteStudyItem(studyItemId);
            }
        },
        onStudyItemFavoriteClick: function(studyItem) {
            if(studyItem.isFavourite) {
                this.deleteStudyItemFromFavourites(studyItem.id);
            } else {
                this.addStudyItemToFavourites(studyItem.id);
            }
        },
        onMarkStudyItemAsTrained: function(studyItemId) {
            this.markStudyItemAsTrained(studyItemId);
        },
        onMarkStudyItemAsNotTrained: function(studyItemId) {
            this.markStudyItemAsNotTrained(studyItemId);
        },
        deleteStudyItem: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_DELETE, {
                studyItemId: studyItemId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item has been deleted!`,
                    text: '',
                    duration: 5000,
                });

                const itemCountThresholdBeforeReload = 3;
                if(this.studyItems.length <= itemCountThresholdBeforeReload) {
                    // reload
                    this.loadStudyItems();
                }
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        addStudyItemToFavourites: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES, {
                studyItemId: studyItemId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteStudyItemFromFavourites: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES, {
                studyItemId: studyItemId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markStudyItemAsTrained: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_MARK_AS_TRAINED, {
                studyItemId: studyItemId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item marked as trained.`,
                    text: '',
                    duration: 5000,
                });

                // reload
                this.loadStudyItems();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markStudyItemAsNotTrained: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_MARK_AS_NOT_LEARNED, {
                studyItemId: studyItemId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item marked as not trained.`,
                    text: '',
                    duration: 5000,
                });

                // reload
                this.loadStudyItems();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
