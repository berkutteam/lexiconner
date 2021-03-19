<template>
    <div class="">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORD_TRAINING_WORDMEANING_START]" class="mb-2"></row-loader>

                <!-- Listen to keyboard events -->
                <keyboard-event-listener
                    v-on:keyup="handleKeyboardEvent"
                ></keyboard-event-listener>

                <div class="words-learn-wordmeaning-wrapper">
                    <div v-bind:id="`trainingTopAnchor`"></div>

                    <h5 class="mb-3 d-flex">
                        Word-Meaning

                        <!-- Tooltip -->
                        <VTooltip class="ml-2">
                            <small>
                                <i class="fas fa-info-circle"></i>
                            </small>

                            <template #popper>
                                <div class="mb-1">Space - reveal aswer/go to the next</div>
                                <!-- <div class="mb-1">Left Arrow - go to the previous</div> -->
                                <div class="">Right Arrow - go to the next</div>
                            </template>
                        </VTooltip>
                    </h5>
                    <div v-if="isAllTrained">
                        <div class="alert alert-secondary" role="alert">
                            Everything is already trained!
                        </div>
                    </div>
                    <div v-if="!isAllTrained && currentItem" class="card bg-light training-card">
                        <!-- Image -->
                        <img v-if="currentItem.word.images && currentItem.word.images.length !== 0" class="card-img-top training-image" v-bind:src="currentItem.word.images[0].url" v-bind:alt="currentItem.word.word">
                        <img v-else class="card-img-top training-image" src="/img/empty-image.png">

                        <div class="card-body">
                            <div class="d-flex w-100 justify-content-between align-items-center mb-2">
                                <h6 class="card-title training-title mb-0">
                                    <span>{{currentItem.word.word}}</span>
                                </h6>
                            </div>

                            <!-- Options -->
                            <div class="d-flex flex-column justify-content-center align-items-start">
                                <div
                                    v-for="(possibleOption, index) in currentItem.possibleOptions"
                                    v-bind:key="possibleOption.randomId"
                                    class="w-100"
                                >
                                    <button 
                                        v-on:click="onPossibleMeaningClick(possibleOption)"
                                        type="button" 
                                        class="btn btn-block mb-1 text-left"
                                        v-bind:class="{
                                            'btn-outline-secondary': !privateState.isCurrentItemAnswered || (privateState.isCurrentItemAnswered && !possibleOption.isCorrect && currentItemAnswerOptionIdOrNotSet !== possibleOption.randomId),
                                            'btn-success': privateState.isCurrentItemAnswered && possibleOption.isCorrect,
                                            'btn-danger': privateState.isCurrentItemAnswered && !possibleOption.isCorrect && currentItemAnswerOptionIdOrNotSet === possibleOption.randomId,
                                        }"
                                        v-bind:disabled="privateState.isCurrentItemAnswered"
                                    >
                                        <span class="training-option-text">
                                            {{ index + 1 }}. {{possibleOption.value}}
                                        </span>
                                    </button>
                                </div>
                            </div>
                        
                            <!-- Details (when answered) -->
                            <div v-if="privateState.isShowCurrentItemDetails">
                                <hr />
                                <div v-if="privateState.isShowCurrentItemDetails" class="card-text mb-1 training-description">
                                    <div>{{ currentItem.word.meaning }}</div>
                                </div>
                                <div v-if="privateState.isShowCurrentItemDetails" class="card-text text-secondary training-example-text mb-1">
                                    <div
                                        v-for="(exampleText, index2) in currentItem.word.examples"
                                        v-bind:key="`card-${currentItem.word.id}-exampleText-${index2}`"
                                        class="mb-1"
                                    >
                                        <i class="fas fa-circle example-text-dot-icon"></i>
                                        <span>{{ exampleText }}</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-bottom-controls">
                            <!-- Dropdown with aditional actions -->
                            <div class="card-bottom-control-item dropdown">
                                <span class="contained-button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                    <i class="fas fa-ellipsis-v"></i>
                                </span>
                                <div class="dropdown-menu dropdown-menu-left" aria-labelledby="dropdownMenuButton">
                                    <button v-on:click="onWordMarkAsLearnedClick(currentItem.word.id)" class="dropdown-item" type="button">
                                        <i class="fas fa-check-double text-success mr-2"></i>
                                        <span>Mark as learned</span>
                                    </button>
                                </div>
                            </div>
                            <!-- <span v-on:click="onPrevClick()" class="card-bottom-control-item" v-bind:class="{'disabled': privateState.currentItemIndex === 0}">
                                <i class="fas fa-chevron-left"></i>
                            </span> -->
                            <span v-on:click="onShowClick()" class="card-bottom-control-item text-danger">
                                <i class="fas fa-question"></i>
                            </span>
                            <span v-on:click="onNextClick()" class="card-bottom-control-item" v-bind:class="{'disabled': !privateState.isCurrentItemAnswered}">
                                <i class="fas fa-chevron-right"></i>
                            </span>
                        </div>
                    </div>

                    <!-- Summary -->
                    <div class="mt-2">
                        <small>
                            <div class="text-center">
                                <span>{{privateState.currentItemIndex + 1}} / {{totalItemsCount}}</span>
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
                                    <router-link v-bind:to="{ name: 'words-dashboard', params: {}}" class="btn custom-btn-normal btn-sm text-white">
                                        <i class="fas fa-chevron-left mr-1"></i>
                                        <span>Back</span>
                                    </router-link>
                                    <button v-on:click="startTraining" type="button" class="btn custom-btn-normalbtn-sm ml-1">
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
import KeyboardEventListener from '@/components/KeyboardEventListener';

export default {
    name: 'words-learn-wordmeaning',
    components: {
        RowLoader,
        KeyboardEventListener,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                itemsLimit: 10,
                currentItemIndex: 0,
                isShowCurrentItemDetails: false,
                itemResults: [],
                isTrainingFinished: false,
                summary: {
                    correctItemsCount: 0,
                    incorrectItemsCount: 0,
                }
            },
        };
    },
    computed: {
        // local computed go here
        isAllTrained: function() { 
            return this.trainingWordMeaning !== null && this.trainingWordMeaning.items.length === 0;
        },
        currentItem: function() { 
            if(this.trainingWordMeaning === null || this.trainingWordMeaning.items.length === 0) {
                return null;
            }
            return this.trainingWordMeaning.items[this.privateState.currentItemIndex];
        },
        totalItemsCount: function() { 
            if(this.trainingWordMeaning === null) {
                return this.privateState.itemsLimit;
            }
            return this.trainingWordMeaning.items.length;
        },
        currentItemAnswerOptionIdOrNotSet: function() {
            if(this.privateState.itemResults.length === this.privateState.currentItemIndex + 1) {
                return this.privateState.itemResults[this.privateState.currentItemIndex].optionId;
            }
            return null;
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            trainingWordMeaning: state => state.trainingWordMeaning,
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
        loadTraining: function() {
            return this.$store.dispatch(storeTypes.WORD_TRAINING_WORDMEANING_START, {
                collectionId: this.$store.getters.currentCustomCollectionId,
                limit: this.privateState.itemsLimit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        startTraining: function() {
            this.privateState.currentItemIndex = 0;
            this.privateState.isCurrentItemAnswered = false;
            this.privateState.isShowCurrentItemDetails = false;
            this.privateState.itemResults = [];
            this.privateState.isTrainingFinished = false;
            this.privateState.summary.correctItemsCount = 0;
            this.privateState.summary.incorrectItemsCount = 0;

            this.loadTraining();
        },
        scrollTop: function() {
            let elSelector = `#trainingTopAnchor`;
            this.$scrollTo(elSelector);
        },
        goToCard: function(index = 0) {
            if(this.trainingWordMeaning === null) {
                return;
            }
            let count = this.trainingWordMeaning.items.length;
            
            // last card was shown - save training
            if(index === count) {
                this.saveTraining();
                return;
            }

            index = Math.min(index, count - 1);
            index = Math.max(index, 0);
            this.privateState.currentItemIndex = index;
            this.privateState.isShowCurrentItemDetails = false;
            this.privateState.isCurrentItemAnswered = false;
        },
        onPossibleMeaningClick: function(possibleOption) {
            this.handleItemResponse({
                itemId: this.currentItem.word.id, 
                isCorrect: possibleOption.isCorrect === true,
                optionId: possibleOption.randomId,
            });
            this.privateState.isShowCurrentItemDetails = true;
            this.privateState.isCurrentItemAnswered = true;
        },
        onPrevClick: function() {
            if(this.privateState.currentItemIndex === 0) {
                return;
            }
            this.goToCard(this.privateState.currentItemIndex - 1);
        },
        onShowClick: function() {
            this.handleItemResponse({
                itemId: this.currentItem.word.id, 
                isCorrect: false
            });
            this.privateState.isShowCurrentItemDetails = true;
            this.privateState.isCurrentItemAnswered = true;
        },
        onNextClick: function() {
            if(!this.privateState.isCurrentItemAnswered) {
                return;
            }
            this.scrollTop();
            this.goToCard(this.privateState.currentItemIndex + 1);
        },
        onWordMarkAsLearnedClick: function(wordId) {
            this.markWordAsLearned(wordId);
        },
        handleItemResponse: function({itemId, isCorrect, optionId}) {
            let isHandled = this.privateState.itemResults.some(x => x.itemId === itemId);
            if(isHandled) {
                return;
            }
            this.privateState.itemResults.push({
                itemId, 
                isCorrect,
                optionId,
            });
            this.privateState.summary = {
                correctItemsCount: isCorrect ? this.privateState.summary.correctItemsCount + 1 : this.privateState.summary.correctItemsCount,
                incorrectItemsCount: !isCorrect ? this.privateState.summary.incorrectItemsCount + 1 : this.privateState.summary.incorrectItemsCount,
            }
        },
        handleKeyboardEvent: function(e) {
            switch(e.which) {
                // Space
                case 32:
                    if(this.privateState.isCurrentItemAnswered) {
                        this.onNextClick();
                    } else {
                        this.onShowClick();
                    }
                    break;
                // ArrowLeft
                case 37:
                    this.onPrevClick();
                    break;
                // ArrowRight
                case 39:
                    this.onNextClick();
                    break;
            }
        },
        saveTraining: function() {
            this.privateState.isTrainingFinished = true;
            return this.$store.dispatch(storeTypes.WORD_TRAINING_WORDMEANING_SAVE, {
                data: {
                    trainingType: this.trainingWordMeaning.trainingType,
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
        markWordAsLearned: function(wordId) {
            this.$store.dispatch(storeTypes.WORD_TRAINING_MARK_AS_TRAINED, {
                wordId: wordId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Word marked as trained.`,
                    text: '',
                    duration: 5000,
                });

                // treat as answered correctly and go to the next
                const possibleOption = this.currentItem.possibleOptions.find(x => x.isCorrect);
                this.handleItemResponse({
                    itemId: wordId, 
                    isCorrect: true,
                    optionId: possibleOption.randomId,
                });
                this.privateState.isCurrentItemAnswered = true;
                this.onNextClick();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
