<template>
    <div class="dashboard-wrapper">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORD_TRAINING_STATS_LOAD]" class="mb-2"></row-loader>

        <learning-language-not-selected-alert />

        <div v-if="isLearningLanguageCodeSelected === true">
            <!-- Collections -->
            <!-- <custom-collections>
            </custom-collections> -->

            <!-- WordSet selector -->
            <user-word-set-selector class="mb-4" />

            <!-- Nav -->
            <div class="app-card-nav mb-2">
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-falshcards', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-red-card-80.png" alt="">
                        <span class="app-card-nav-text">Flash cards</span>
                    </router-link>
                </div>
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-wordmeaning', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-rich-text-converter-96.png" alt="">
                        <span class="app-card-nav-text">Word-Meaning</span>
                    </router-link>
                </div>
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-meaningword', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-dictionary-64.png" alt="">
                        <span class="app-card-nav-text">Meaning-Word</span>
                    </router-link>
                </div>
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-matchwords', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-compare-64.png" alt="">
                        <span class="app-card-nav-text">Match words</span>
                    </router-link>
                </div>
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-buildwords', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-brick-wall-64.png" alt="">
                        <span class="app-card-nav-text">Build word</span>
                    </router-link>
                </div>
                <div class="app-card-nav-item">
                    <router-link v-bind:to="{ name: 'words-learn-listenwords', params: {}}" class="app-card-nav-link">
                        <img class="app-card-nav-image app-card-nav-image--64x64" src="img/app-card-nav/icons8-foreign-language-sound-64.png" alt="">
                        <span class="app-card-nav-text">Listen word</span>
                    </router-link>
                </div>
            </div>

            <!-- Stats -->
            <div class="mb-2">
                <div v-if="trainingStats">
                    <div class="card mb-4">
                        <div class="card-body">
                            <h4>Training stats</h4>
                            <hr/>
                            <div class="mb-3">
                                <div class="h5">
                                    <span>Total</span>
                                    <span class="badge badge-secondary ml-1">{{trainingStats.totalItemCount}}</span>
                                </div>
                                <div class="h5">
                                    <span>Trained</span>
                                    <span class="badge badge-success ml-1">{{trainingStats.trainedItemCount}}</span>
                                </div>     
                                <div class="h5">
                                    <span>On training</span>
                                    <span class="badge badge-info ml-1">{{trainingStats.onTrainingItemCount}}</span>
                                </div>               
                            </div>
                            <div>
                                <ul>
                                    <li
                                        v-for="(item) in trainingStats.trainingStats"
                                        v-bind:key="`key-training-${item.trainingType}`"
                                    >
                                        <span class="badge badge-secondary">{{item.trainingTypeFormatted}}</span>
                                        <!-- <span class="ml-1">
                                            <span>Trained</span>
                                            <span class="badge badge-success ml-1">{{item.trainedItemCount}}</span>
                                        </span>
                                        <span class="ml-1">
                                            <span>On training</span>
                                            <span class="badge badge-info ml-1">{{item.onTrainingItemCount}}</span>
                                        </span> -->
                                        <ul>
                                            <li>
                                                <span>Trained</span>
                                                <span class="badge badge-success ml-1">{{item.trainedItemCount}}</span>
                                            </li>
                                            <li>
                                                <span>Training</span>
                                                <span class="badge badge-info ml-1">{{item.onTrainingItemCount}}</span>
                                            </li>
                                        </ul>
                                    </li>
                                </ul>
                            </div>
                        </div>
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
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import CustomCollections from '@/components/CustomCollections';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import LearningLanguageNotSelectedAlert from '@/components/LearningLanguageNotSelectedAlert';
import UserWordSetSelector from '@/components/UserWordSetSelector';

export default {
    name: 'words-training-dashboard',
    components: {
        RowLoader,
        // CustomCollections,
        LearningLanguageNotSelectedAlert,
        UserWordSetSelector,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                learningLanguageCode: null,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            trainingStats: state => state.trainingStats,
        }),

        // store getter
        ...mapGetters([
            'selectedLearningLanguageCode',
            'isLearningLanguageCodeSelected',
        ]),
    },
    created: function() {
       this.loadTrainingsStats();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadTrainingsStats: function() {
            return this.$store.dispatch(storeTypes.WORD_TRAINING_STATS_LOAD, {})
                .then()
                .catch(err => {
                    console.error(err);
                    notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
                });
        },
        onLearningLanguageCodeChange: function(code) {
            
        },
    },
}
</script>
