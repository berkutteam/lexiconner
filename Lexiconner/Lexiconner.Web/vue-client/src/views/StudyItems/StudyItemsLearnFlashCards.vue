<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START]"></row-loader>

                <div class="study-items-learn-flash-cards-wrapper">
                    <h5 class="mb-3">Flash cards</h5>
                    <div v-if="isAllTrained">
                        <div class="alert alert-secondary" role="alert">
                            Everyting is already trained!
                        </div>
                    </div>
                    <div v-if="!isAllTrained && currentItem" class="card bg-light training-card">
                        <img v-if="currentItem.image" class="card-img-top training-image" v-bind:src="currentItem.image.url" v-bind:alt="currentItem.title">
                        <img v-else class="card-img-top" src="/img/empty-image.png">
                        <div class="card-body">
                            <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                <h6 class="card-title mb-0">
                                    <span>{{currentItem.title}}</span>
                                </h6>
                            </div>
                        
                            <div v-if="privateState.isShowCurrentItemDetails" class="card-text small mb-1">
                                <div>{{ currentItem.description }}</div>
                            </div>
                            <div v-if="privateState.isShowCurrentItemDetails" class="card-text small text-secondary mb-1">
                                <div
                                    v-for="(exampleText, index2) in currentItem.exampleTexts"
                                    v-bind:key="`card-${currentItem.id}-exampleText-${index2}`"
                                >
                                    <small><em>{{ exampleText }}</em></small>
                                </div>
                            </div>
                        </div>
                        <div class="card-bottom-controls">
                            <span v-on:click="onPrevClick()" class="card-bottom-control-item">
                                <i class="fas fa-chevron-left"></i>
                            </span>
                            <span v-on:click="onShowClick()" class="card-bottom-control-item text-danger">
                                <i class="fas fa-question"></i>
                            </span>
                            <span v-on:click="onNextClick()" class="card-bottom-control-item text-success">
                                <i class="fas fa-chevron-right"></i>
                            </span>
                        </div>
                    </div>

                    <!-- Summary -->
                    <div class="mt-2">
                        <small>
                            <div class="text-center">
                                <span>{{privateState.summary.trainedItemsCount}} / {{privateState.summary.totalItemsCount}}</span>
                            </div>
                            <div>
                                <span>
                                    Correct: <span class="badge badge-success">{{privateState.summary.correctItemsCount}}</span>
                                </span>
                                <span class="ml-1">
                                    Incorrect: <span class="badge badge-danger">{{privateState.summary.incorrectItemsCount}}</span>
                                </span>
                            </div>
                            <div v-if="privateState.isTrainingFinished" class="mt-2">
                                <div>
                                    <router-link v-bind:to="{ name: 'study-items-dashboard', params: {}}" class="btn btn-secondary btn-sm text-white">
                                        <i class="fas fa-chevron-left mr-1"></i>
                                        <span>Back</span>
                                    </router-link>
                                    <button v-on:click="startTraining" type="button" class="btn btn-success btn-sm ml-1">
                                        Train next
                                        <i class="fas fa-play ml-1"></i>
                                    </button>
                                </div>
                            </div>
                        </small>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import datetimeUtil from '@/utils/datetime';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';

export default {
    name: 'study-items-learn-flash-cards',
    components: {
        RowLoader,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                currentItemIndex: 0,
                isShowCurrentItemDetails: false,
                itemResults: [],
                isTrainingFinished: false,
                summary: {
                    totalItemsCount: 0,
                    trainedItemsCount: 0,
                    correctItemsCount: 0,
                    incorrectItemsCount: 0,
                }
            },
        };
    },
    computed: {
        // local computed go here
        isAllTrained: function() { 
            return this.trainingFlashcards !== null && this.trainingFlashcards.items.length === 0;
        },
        currentItem: function() { 
            if(this.trainingFlashcards === null || this.trainingFlashcards.items.length === 0) {
                return null;
            }
            return this.trainingFlashcards.items[this.privateState.currentItemIndex];
        },
        totalItemsCount: function() { 
            if(this.trainingFlashcards === null) {
                return 0;
            }
            return this.trainingFlashcards.items.length;
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            trainingFlashcards: state => state.trainingFlashcards,
        }),
    },
    created: async function() {
        this.startTraining();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadTraining: function({limit = 10} = {}) {
            return this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_START, {
                collectionId: this.$store.getters.currentCustomCollectionId,
                limit: limit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        startTraining: function() {
            this.privateState.currentItemIndex = 0;
            this.privateState.isShowCurrentItemDetails = 0;
            this.privateState.itemResults = [];
            this.privateState.isTrainingFinished = false;

            this.loadTraining();
        },
        goToCard: function(index = 0) {
            if(this.trainingFlashcards === null) {
                return;
            }
            let count = this.trainingFlashcards.items.length;
            
            // last card was shown - save training
            if(index === count) {
                this.saveTraining();
                return;
            }

            index = Math.min(index, count - 1);
            index = Math.max(index, 0);
            this.privateState.currentItemIndex = index;
            this.privateState.isShowCurrentItemDetails = false;
        },
        onPrevClick: function() {
            this.goToCard(this.privateState.currentItemIndex - 1);
        },
        onShowClick: function() {
            this.handleItemResponse({
                itemId: this.currentItem.id, 
                isCorrect: false
            });
            this.privateState.isShowCurrentItemDetails = true;
        },
        onNextClick: function() {
            this.handleItemResponse({
                itemId: this.currentItem.id, 
                isCorrect: true
            });
            this.goToCard(this.privateState.currentItemIndex + 1);
        },
        handleItemResponse: function({itemId, isCorrect}) {
            let isHandled = this.privateState.itemResults.some(x => x.itemId === itemId);
            if(isHandled) {
                return;
            }
            this.privateState.itemResults.push({
                itemId, 
                isCorrect,
            });
            this.privateState.summary = {
                totalItemsCount: this.totalItemsCount,
                trainedItemsCount: this.privateState.summary.trainedItemsCount + 1,
                correctItemsCount: isCorrect ? this.privateState.summary.correctItemsCount + 1 : this.privateState.summary.correctItemsCount,
                incorrectItemsCount: !isCorrect ? this.privateState.summary.incorrectItemsCount + 1 : this.privateState.summary.incorrectItemsCount,
            }
        },
        saveTraining: function() {
            this.privateState.isTrainingFinished = true;
            return this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_FLASHCARDS_SAVE, {
                data: {
                    trainingType: this.trainingFlashcards.trainingType,
                    itemsResults: [
                        ...this.privateState.itemResults,
                    ],
                }, 
            }).then(() => {
                
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
