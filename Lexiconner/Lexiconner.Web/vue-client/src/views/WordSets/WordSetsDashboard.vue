<template>
    <div class="dashboard-wrapper">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORD_SETS_LOAD]" class="mb-2"></row-loader>

        <learning-language-not-selected-alert />

        <div v-if="isLearningLanguageCodeSelected === true">
            <div v-if="wordSets">
                <div>
                    <pagination-wrapper
                        v-bind:paginationResult="sharedState.wordSetsPaginationResult"
                        v-bind:loadItemsF="loadWordSets"
                        v-bind:showGoToButtons="true"
                    >
                        <!-- Card view -->
                        <div v-if="privateState.currentView === 'cards'" class="wordsets-card-list">
                            <div
                                v-for="(item) in wordSets"
                                v-bind:key="`card-${item.id}`"
                                class="card bg-light item-card" 
                            >
                                <img v-if="item.images && item.images.length !== 0 && item.images[0]" class="card-img-top item-card-image" v-bind:src="item.images[0].url" v-bind:alt="item.word">
                                <img v-else class="card-img-top item-card-image" src="/img/empty-image.png">
                                
                                <div class="card-body">
                                    <h6 class="card-title mb-0">
                                        <span>{{item.name}}</span>
                                    </h6>
                                    <span class="badge badge-secondary">{{item.words.length}}</span>
                                </div>

                                <div class="card-controls">

                                    <loading-button 
                                        type="button"
                                        v-if="!checkWordSetIsAlreadyAdded(item.id)"
                                        v-bind:disabled="checkWordSetIsAlreadyAdded(item.id)"
                                        v-bind:loading="false"
                                        v-on:click.native="onAddWordSetClick(item.id)"
                                        class="btn btn-sm custom-btn-normal mr-1"
                                    >
                                        <i class="fas fa-plus"></i>
                                    </loading-button>

                                </div>

                                <!-- Already added message -->
                                <div v-if="checkWordSetIsAlreadyAdded(item.id)" class="card-already-added-message">
                                    You've already added this word set.
                                </div>
                            </div>
                        </div>
                    </pagination-wrapper>
                </div>
            </div>
        </div>

        <!-- Word create/edit -->
        <modal 
            name="word-set-add-to-dictionary" 
            height="auto"
            width="450px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">
                        <span>Add word set to dictionary</span>
                    </div>
                    <div v-on:click="$modal.hide('word-set-add-to-dictionary')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <form v-on:submit.prevent="addWordSetToDictionaryClick()">
                        <!-- <div class="form-group">
                            <label for="wordSetModel__title">Name</label>
                            <input v-model="privateState.wordSetModel.name" type="text" class="form-control" id="wordSetModel__title" placeholder="Name" />
                        </div> -->
                        <div class="form-check">
                            <input 
                                class="form-check-input" 
                                type="checkbox" 
                                checked="checked" 
                                id="defaultCheck1" 
                                v-on:change="(e) => onSelectAllWordsChange(e, privateState.addWordSetToDictionaryModel.wordSetId)"
                            >
                            <label class="form-check-label" for="defaultCheck1">
                                Select all
                            </label>
                        </div>
                        <hr />
                        <div class="mb-2">
                            <div
                                v-for="(word) in getWordSetWords(privateState.addWordSetToDictionaryModel.wordSetId)"
                                v-bind:key="word.id"
                                class="form-check mb-1"
                            >
                                <input 
                                    class="form-check-input" 
                                    type="checkbox" 
                                    value="" 
                                    v-bind:id="`${word.id}-check`" 
                                    v-on:change="(e) => onSelectWordChange(e, word.id)"
                                    v-bind:checked="privateState.addWordSetToDictionaryModel.selectedWordsIds.includes(word.id)"
                                >
                                <label class="form-check-label" v-bind:for="`${word.id}-check`">
                                    {{ word.word }}
                                </label>
                            </div>
                        </div>
                        <loading-button 
                            type="submit"
                            v-bind:loading="sharedState.loading[privateState.storeTypes.USER_DICTIONARY_WORD_SET_ADD]"
                            class="btn custom-btn-normal btn-block"
                        >Save</loading-button>
                    </form>
                </div>
            </div>
        </modal>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import LearningLanguageNotSelectedAlert from '@/components/LearningLanguageNotSelectedAlert';

export default {
    name: 'wordsets-dashboard',
    components: {
        RowLoader,
        LoadingButton,
        PaginationWrapper,
        LearningLanguageNotSelectedAlert,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                currentView: localStorage.getItem(`wordSetsBrowse_currentView`) || 'cards', // ['cards']
                addWordSetToDictionaryModel: {
                    wordSetId: null,
                    selectedWordsIds: [],
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            wordSets: state => state.wordSetsPaginationResult ? state.wordSetsPaginationResult.items : null,
            userDictionary: state => state.userDictionary,
        }),

        // store getter
        ...mapGetters([
            'selectedLearningLanguageCode',
            'isLearningLanguageCodeSelected',
        ]),
    },
    created: function() {
       this.loadWordSets();

        if(!this.userDictionary) {
            this.$store.dispatch(storeTypes.USER_DICTIONARY_LOAD, {}).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

       this.unwatch = this.$store.watch(
            (state, getters) => getters.selectedLearningLanguageCode,
            (newValue, oldValue) => {
                this.loadWordSets({});
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
        loadWordSets: function({offset = 0, limit = 50} = {}) {
            return this.$store.dispatch(storeTypes.WORD_SETS_LOAD, {
                offset: offset, 
                limit: limit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        checkWordSetIsAlreadyAdded: function(wordSetId) {
            if(this.userDictionary) {
                return this.userDictionary.wordSets.some(x => x.sourceWordSetId === wordSetId);
            }
            return false;
        },
        getWordSetWords: function(wordSetId) {
            if(this.wordSets) {
                return (this.wordSets.find(x => x.id === wordSetId) || {}).words || [];
            }
            return [];
        },
        onAddWordSetClick: function(wordSetId) {
            this.privateState.addWordSetToDictionaryModel.wordSetId = wordSetId;
            this.privateState.addWordSetToDictionaryModel.selectedWordsIds = [
                ...this.getWordSetWords(wordSetId).map(x => x.id),
            ];
            this.$modal.show('word-set-add-to-dictionary');
        },
        addWordSetToDictionaryClick: function() {
            return this.$store.dispatch(storeTypes.USER_DICTIONARY_WORD_SET_ADD, {
                wordSetId: this.privateState.addWordSetToDictionaryModel.wordSetId, 
                selectedWordIds: this.privateState.addWordSetToDictionaryModel.selectedWordsIds,
            }).then(() => {
                this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Word set added to your dictionary.`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('word-set-add-to-dictionary');
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onSelectAllWordsChange: function(e, wordSetId) {
            if(e.target.checked) {
                this.privateState.addWordSetToDictionaryModel.selectedWordsIds = [
                    ...this.getWordSetWords(wordSetId).map(x => x.id),
                ];
            } else {
                this.privateState.addWordSetToDictionaryModel.selectedWordsIds = [];
            }
            
        },
        onSelectWordChange: function(e, wordId) {
            const isAlreadySelected = this.privateState.addWordSetToDictionaryModel.selectedWordsIds.includes(wordId);
            if(isAlreadySelected) {
                this.privateState.addWordSetToDictionaryModel.selectedWordsIds = this.privateState.addWordSetToDictionaryModel.selectedWordsIds.filter(x => x !== wordId);
            } else {
                this.privateState.addWordSetToDictionaryModel.selectedWordsIds.push(wordId);
            }
        },
    },
}
</script>
