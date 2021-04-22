<template>
    <div class="">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORD_TRAINING_LISTENWORDS_START]" class="mb-2"></row-loader>

                <!-- Listen to keyboard events -->
                <keyboard-event-listener
                    v-on:keyup="handleKeyboardEvent"
                ></keyboard-event-listener>

                <div class="words-tarining-wrapper words-learn-listenwords-wrapper">
                    <div v-bind:id="`trainingTopAnchor`"></div>

                    <h5 class="mb-3 d-flex">
                        Listen words

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
                            <!-- <div class="d-flex w-100 justify-content-between align-items-center mb-2">
                                <h6 class="card-title training-title mb-0 text-center">
                                    <span>{{currentItem.word.meaning}}</span>
                                </h6>
                            </div> -->

                            <!-- Pronunciation audio -->
                            <div class="trainig-word-pronunciation-audio d-flex flex-row justify-content-center align-items-start mt-2">
                                <i 
                                    v-if="!privateState.isPronunciationAudioPlaying" 
                                    v-on:click="onWordPronunciationAudioClick()" 
                                    class="fas fa-volume-off pronunciation-audio-icon"
                                    v-bind:class="{}"
                                ></i>
                                <i 
                                    v-if="privateState.isPronunciationAudioPlaying" 
                                    class="fas fa-volume-up pronunciation-audio-icon"
                                ></i>

                                <audio v-bind:ref="`pronunciationAudioEl_${currentItem.word.id}`" v-if="getCurrentWordPronunciationAudio()" controls class="hidden">
                                    <source v-if="getCurrentWordPronunciationAudio().audioMp3Url" v-bind:src="getCurrentWordPronunciationAudio().audioMp3Url" type="audio/mpeg">
                                    <source v-if="getCurrentWordPronunciationAudio().audioOggUrl" v-bind:src="getCurrentWordPronunciationAudio().audioOggUrl" type="audio/ogg">
                                    Your browser does not support the audio element.
                                </audio>
                            </div>

                            <!-- Word input -->
                            <div class="d-flex flex-row flex-wrap justify-content-center align-items-start mt-4">
                                <input 
                                    ref="currentWordEnteredInput"
                                    v-model="privateState.currentWordEntered" 
                                    v-on:change="(e) => onCurrentWordEnteredChange(e.target.value)" 
                                    type="email" 
                                    class="form-control trainig-word-input"
                                    placeholder="Listen the audio and type the word"
                                    v-bind:class="{
                                        'border-danger': privateState.isCurrentItemAnsweredCorrectly === false,
                                        'border-success': privateState.isCurrentItemAnsweredCorrectly === true,
                                    }"
                                    v-bind:disabled="privateState.isCurrentItemAnswered"
                                >
                            </div>

                            <!-- Details (when answered) -->
                            <div v-if="privateState.isShowCurrentItemDetails">
                                <hr />
                                 <div v-if="privateState.isShowCurrentItemDetails" class="card-text mb-1 training-description">
                                    <div>{{ currentItem.word.word }}</div>
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
                            <span v-if="!privateState.isCurrentItemAnswered" v-on:click="submitAnswer()" class="card-bottom-control-item text-success">
                                <i class="fas fa-check"></i>
                            </span>
                            <span v-if="privateState.isCurrentItemAnswered" v-on:click="onNextClick()" class="card-bottom-control-item" v-bind:class="{'disabled': !privateState.isCurrentItemAnswered}">
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
                                    <router-link v-bind:to="{ name: 'trainings-dashboard', params: {}}" class="btn custom-btn-normal btn-sm text-white">
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
    name: 'words-learn-listenwords',
    components: {
        RowLoader,
        KeyboardEventListener,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                itemsLimit: 5,
                currentItemIndex: 0,
                isCurrentItemAnswered: false,
                isCurrentItemAnsweredCorrectly: null,
                isShowCurrentItemDetails: false,
                isPronunciationAudioPlaying: false,
                currentWordEntered: '',
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
            return this.trainingListenWords !== null && this.trainingListenWords.items.length === 0;
        },
        currentItem: function() { 
            if(this.trainingListenWords === null || this.trainingListenWords.items.length === 0) {
                return null;
            }
            return this.trainingListenWords.items[this.privateState.currentItemIndex];
        },
        totalItemsCount: function() { 
            if(this.trainingListenWords === null) {
                return this.privateState.itemsLimit;
            }
            return this.trainingListenWords.items.length;
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            trainingListenWords: state => state.trainingListenWords,
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
            return this.$store.dispatch(storeTypes.WORD_TRAINING_LISTENWORDS_START, {
                collectionId: this.$store.getters.currentCustomCollectionId,
                limit: this.privateState.itemsLimit, 
            }).then(() => {
                this.focusInput();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        startTraining: function() {
            this.privateState.currentItemIndex = 0;
            this.privateState.isCurrentItemAnswered = false;
            this.privateState.isShowCurrentItemDetails = false;
            this.privateState.currentWordEntered = '';
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
        focusInput: function () {
            if(this.$refs.currentWordEnteredInput) {
                this.$refs.currentWordEnteredInput.focus();
            }
        },
        goToCard: function(index = 0) {
            if(this.trainingListenWords === null) {
                return;
            }
            let count = this.trainingListenWords.items.length;
            
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
            this.privateState.isCurrentItemAnsweredCorrectly = null;
            this.privateState.currentWordEntered = '';
        },
        checkCurrentWordPronunciationAudioExists: function() {
            return this.sharedState.wordsPronunciationAudio && 
                   this.sharedState.wordsPronunciationAudio[this.currentItem.word];
        },
        getCurrentWordPronunciationAudio: function() {
            if(!this.currentItem) {
                return null;
            }

            let currentWordPronunciationAudio = this.sharedState.wordsPronunciationAudio && 
                       this.sharedState.wordsPronunciationAudio[this.currentItem.word.word] &&
                       (this.sharedState.wordsPronunciationAudio[this.currentItem.word.word].audioMp3Url ||
                        this.sharedState.wordsPronunciationAudio[this.currentItem.word.word].audioOggUrl) ? this.sharedState.wordsPronunciationAudio[this.currentItem.word.word]  : null;
            
            return currentWordPronunciationAudio;
        },
        onWordPronunciationAudioClick: function() {
            this.loadWordPronunciationAudio({
                languageCode: this.currentItem.word.wordLanguageCode,
                word: this.currentItem.word.word
            }).then(() => {
                // hack to ensure audio element is rendered before playing
                const refKey = `pronunciationAudioEl_${this.currentItem.word.id}`;
                if(this.getCurrentWordPronunciationAudio() != null && this.$refs[refKey]) {
                    setTimeout(() => {
                        this.privateState.isPronunciationAudioPlaying = true;
                        this.$refs[refKey].play();

                        setTimeout(() => {
                            this.privateState.isPronunciationAudioPlaying = false;
                        }, 1200);
                    }, 500);
                } else {
                    this.$notify({
                        group: 'app',
                        type: 'info',
                        title: `Sorry, but we can't find pronunciation.`,
                        text: '',
                        duration: 5000,
                    });
                }
            });
        },
        onCurrentWordEnteredChange: function(e) {
            console.log(`onCurrentWordEnteredChange.`, e);
        },
        onPrevClick: function() {
            if(this.privateState.currentItemIndex === 0) {
                return;
            }
            this.goToCard(this.privateState.currentItemIndex - 1);
        },
        onShowClick: function() {
            this.handleItemResponse({
                wordId: this.currentItem.word.id, 
                isCorrect: false,
                answer: null,
            });
            this.privateState.isShowCurrentItemDetails = true;
            this.privateState.isCurrentItemAnswered = true;
            this.privateState.isCurrentItemAnsweredCorrectly = false;
            this.privateState.currentWordEntered = this.currentItem.word.word;
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
        submitAnswer: function() {
            const isCorrect = this.privateState.currentWordEntered.trim() === this.currentItem.correctAnswer;

            this.handleItemResponse({
                wordId: this.currentItem.word.id, 
                isCorrect: isCorrect,
                answer: this.privateState.currentWordEntered,
            });
            this.privateState.isShowCurrentItemDetails = true;
            this.privateState.isCurrentItemAnswered = true;
            this.privateState.isCurrentItemAnsweredCorrectly = isCorrect;
            this.privateState.currentWordEntered = this.privateState.currentWordEntered || this.currentItem.word.word;
        },
        handleItemResponse: function({wordId, isCorrect, answer}) {
            let isHandled = this.privateState.itemResults.some(x => x.wordId === wordId);
            if(isHandled) {
                return;
            }
            this.privateState.itemResults.push({
                wordId, 
                isCorrect,
                answer,
            });
            this.privateState.summary = {
                correctItemsCount: isCorrect ? this.privateState.summary.correctItemsCount + 1 : this.privateState.summary.correctItemsCount,
                incorrectItemsCount: !isCorrect ? this.privateState.summary.incorrectItemsCount + 1 : this.privateState.summary.incorrectItemsCount,
            }
        },
        handleKeyboardEvent: function(e) {
            // http://gcctech.org/csc/javascript/javascript_keycodes.htm
            switch(e.which) {
                // Space (conflicts with entering space character)
                // case 32:
                case 13:
                    if(this.privateState.isCurrentItemAnswered) {
                        this.onNextClick();
                    } else {
                        this.submitAnswer();
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

                // any other characters
                default:
                    break;
            }
        },
        saveTraining: function() {
            this.privateState.isTrainingFinished = true;
            return this.$store.dispatch(storeTypes.WORD_TRAINING_LISTENWORDS_SAVE, {
                data: {
                    trainingType: this.trainingListenWords.trainingType,
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
                this.handleItemResponse({
                    wordId: wordId, 
                    isCorrect: true,
                    answer: this.currentItem.correctAnswer,
                });
                this.privateState.isCurrentItemAnswered = true;
                this.onNextClick();
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        loadWordPronunciationAudio: function({ languageCode, word } = {}) {
            return this.$store.dispatch(storeTypes.WORD_PRONUNCIATION_AUDIO_LOAD, {
                languageCode,
                word, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
