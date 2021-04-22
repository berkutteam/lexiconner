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
                                    <div class="">
                                        <h6 class="card-title mb-0">
                                            <span>{{item.name}}</span>
                                        </h6>
                                    </div>
                                    
                                    <div class="card-word-count">
                                        <span class="badge badge-secondary">{{item.words.length}}</span>
                                    </div>
                                </div>

                                <!-- Overlay controls -->
                                <div 
                                    v-if="!checkWordSetIsAlreadyAdded(item.id)"
                                    class="item-card-overlay-controls"
                                >
                                    <div class="w-100 d-flex justify-content-center">
                                        <loading-button 
                                            type="button"
                                            v-bind:loading="sharedState.loading[privateState.storeTypes.USER_DICTIONARY_WORD_SET_ADD]"
                                            v-on:click.native="onAddWordSetClick(item.id)"
                                            class="btn btn-sm custom-btn-normal"
                                        >
                                            <i class="fas fa-plus mr-2"></i> Add to my dictionary
                                        </loading-button>
                                    </div>
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
        onAddWordSetClick: function(wordSetId) {
            return this.$store.dispatch(storeTypes.USER_DICTIONARY_WORD_SET_ADD, {
                wordSetId, 
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
        }
    },
}
</script>
