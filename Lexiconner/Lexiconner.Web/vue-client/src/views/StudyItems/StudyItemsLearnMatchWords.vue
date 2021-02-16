<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEM_TRAINING_MATCHWORDS_START]" class="mb-2"></row-loader>

                <!-- Listen to keyboard events -->
                <keyboard-event-listener
                    v-on:keyup="handleKeyboardEvent"
                ></keyboard-event-listener>

                <div class="study-items-learn-matchwords-wrapper">
                    <h5 class="mb-3 d-flex">
                        Match words

                        <!-- Tooltip -->
                        <VTooltip class="ml-2">
                            <small>
                                <i class="fas fa-info-circle"></i>
                            </small>

                            <template #popper>
                                <div class="mb-1">Space - reveal aswer/go to the next</div>
                                <!-- <div class="mb-1">Left Arrow - go to the previous</div> -->
                                <!-- <div class="">Right Arrow - go to the next</div> -->
                            </template>
                        </VTooltip>
                    </h5>
                    <div v-if="isAllTrained">
                        <div class="alert alert-secondary" role="alert">
                            Everything is already trained!
                        </div>
                    </div>
                    <div v-if="!isAllTrained && trainingMatchWords" class="card bg-light training-card">
                        <!-- Image -->
                        <div class="card-img-top">
                            
                            <div class="image-collage image-collage--5images">
                                <div 
                                    v-for="(item, index) in trainingMatchWords.items"
                                    v-bind:key="item.studyItem.id"
                                    v-bind:class="{[`image-container-${index}`]: true}"
                                    class="image-container"
                                >
                                    <img v-if="item.studyItem.image" class="training-image" v-bind:src="item.studyItem.image.url" v-bind:alt="item.studyItem.title">
                                    <img v-else class="card-img-top" src="/img/empty-image.png">
                                </div>
                            </div>
                        </div>
                        
                        <div class="card-body">
                            <div 
                                class="row"
                                v-for="(item, index) in trainingMatchWords.items"
                                v-bind:key="item.studyItem.id"
                                v-bind:class="{'mt-2': index !== 0}"
                            >
                                <div class="col-md-6 d-flex align-items-center">
                                    <div class="w-100">
                                        <span class="badge badge-secondary mr-1">{{ index + 1 }}.</span>
                                        <span>{{ item.studyItem.title }}</span>
                                    </div>
                                </div>
                                <div class="col-md-6 d-flex align-items-center">
                                    <!-- Render select for answer for each word -->
                                    <div class="w-100">
                                        <multiselect 
                                            v-model="privateState.studyItemSelectedOptions[index]" 
                                            v-bind:class="{
                                                'multiselect--fixSmallWidthOptions': true,
                                                'multiselect--correctAnswer': privateState.isShowAnswers && checkItemAnswerCorrect(item.studyItem.id) === true,
                                                'multiselect--wrongAnswer': privateState.isShowAnswers && checkItemAnswerCorrect(item.studyItem.id) === false,
                                            }"
                                            v-bind:placeholder="'Select meaning'"
                                            v-bind:selectLabel="''" 
                                            v-bind:label="'value'"
                                            v-bind:track-by="'randomId'" 
                                            v-bind:options="leftOptions" 
                                            v-bind:multiple="false" 
                                            v-bind:searchable="false" 
                                            v-bind:taggable="false" 
                                            v-bind:disabled="privateState.isShowAnswers"
                                            v-on:input="(possibleOption) => onOptionSelect(item.studyItem.id, possibleOption)"
                                        >
                                        </multiselect>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div v-if="!privateState.isTrainingFinished" class="card-bottom-controls">
                            <span v-on:click="onShowAnswersClick()" class="card-bottom-control-item text-danger">
                                <i class="fas fa-question"></i>
                            </span>
                            <span v-on:click="onSubmitAnswerClick()" class="card-bottom-control-item text-success" v-bind:class="{'disabled': !isCanBeSubmited}">
                                <i class="fas fa-check"></i>
                            </span>
                        </div>
                    </div>

                    <!-- Summary -->
                    <div v-if="privateState.isTrainingFinished" class="mt-2">
                        <small>
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
                                    <router-link v-bind:to="{ name: 'study-items-dashboard', params: {}}" class="btn custom-btn-normal btn-sm text-white">
                                        <i class="fas fa-chevron-left mr-1"></i>
                                        <span>Back</span>
                                    </router-link>
                                    <button v-on:click="startTraining" type="button" class="btn custom-btn-normal btn-sm ml-1">
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
    name: 'study-items-learn-matchwords',
    components: {
        RowLoader,
        KeyboardEventListener,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                itemsLimit: 5,
                isShowAnswers: false,
                itemResults: [],
                isTrainingFinished: false,
                studyItemSelectedOptions: [],
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
            return this.trainingMatchWords !== null && this.trainingMatchWords.items.length === 0;
        },
        isCanBeSubmited: function() { 
            // when all selects are choosen
            return this.privateState.itemResults.length === this.trainingMatchWords.items.length;
        },
        totalItemsCount: function() { 
            if(this.trainingMatchWords === null) {
                return this.privateState.itemsLimit;
            }
            return this.trainingMatchWords.items.length;
        },
        leftOptions: function() {
            if(this.trainingMatchWords) {
                // filter out selected options
                return this.trainingMatchWords.possibleOptions.filter(x => {
                    return !this.privateState.studyItemSelectedOptions.some(y => y.randomId === x.randomId);
                });
            }
            return [];
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            trainingMatchWords: state => state.trainingMatchWords,
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
            return this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_MATCHWORDS_START, {
                collectionId: this.$store.getters.currentCustomCollectionId,
                limit: this.privateState.itemsLimit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        startTraining: function() {
            this.privateState.itemResults = [];
            this.privateState.studyItemSelectedOptions = [];
            this.privateState.isShowAnswers = false;
            this.privateState.isTrainingFinished = false;
            this.privateState.summary.correctItemsCount = 0;
            this.privateState.summary.incorrectItemsCount = 0;

            this.loadTraining();
        },
        onOptionSelect: function(studyItemId, possibleOption) {
            this.handleItemResponse({
                itemId: studyItemId, 
                isCorrect: possibleOption.correctForStudyItemId === studyItemId,
                optionId: possibleOption.randomId,
            });
        },
        onShowAnswersClick: function() {
            // mark unanswered an incorrect
            const unansweredItems = this.trainingMatchWords.items.filter(x => {
                return !this.privateState.itemResults.some(y => y.itemId === x.studyItem.id);
            });
            for(const item of unansweredItems) {
                this.handleItemResponse({
                    itemId: item.studyItem.id, 
                    isCorrect: false
                });
            }
         
            this.saveTraining();
            this.privateState.isShowAnswers = true;
        },
        onSubmitAnswerClick: function() {
            this.saveTraining();
            this.privateState.isShowAnswers = true;
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
        checkItemAnswerCorrect: function(itemId) {
            const answer = this.privateState.itemResults.find(y => y.itemId === itemId) || null;
            return answer && answer.isCorrect;
        },
        handleKeyboardEvent: function(e) {
            switch(e.which) {
                // Space
                case 32:
                    this.onShowAnswersClick();
                    break;
                // ArrowLeft
                case 37:
                    break;
                // ArrowRight
                case 39:
                    break;
            }
        },
        saveTraining: function() {
            this.privateState.isTrainingFinished = true;
            return this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_MATCHWORDS_SAVE, {
                data: {
                    trainingType: this.trainingMatchWords.trainingType,
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
