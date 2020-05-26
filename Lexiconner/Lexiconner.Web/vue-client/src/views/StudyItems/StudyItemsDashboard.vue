<template>
    <div class="dashboard-wrapper">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD]"></row-loader>

        <!-- Collections -->
        <custom-collections>
        </custom-collections>

        <!-- Stats -->
        <div class="mb-2">
            <div v-if="trainingStats">
                <div class="card mb-4">
                    <div class="card-body">
                        <h4>Training stats</h4>
                        <hr/>
                        <div class="d-flex mb-3">
                            <div class="h5">
                                <span>Total</span>
                                <span class="badge badge-secondary ml-1">{{trainingStats.totalItemCount}}</span>
                            </div>
                            <div class="h5 ml-2">
                                <span>Trained</span>
                                <span class="badge badge-success ml-1">{{trainingStats.trainedItemCount}}</span>
                            </div>     
                            <div class="h5 ml-2">
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
                                    <span class="ml-1">
                                        <span>Trained</span>
                                        <span class="badge badge-success ml-1">{{item.trainedItemCount}}</span>
                                    </span>
                                     <span class="ml-1">
                                        <span>On training</span>
                                        <span class="badge badge-info ml-1">{{item.onTrainingItemCount}}</span>
                                    </span>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>             
            </div>
        </div>

        <!-- Nav -->
        <div class="app-card-nav">
            <div class="app-card-nav-item" v-bind:style="{'background-image': `url(img/app-card-nav/icons8-browse-folder-96.png)`}">
                <router-link v-bind:to="{ name: 'study-items-browse', params: {}}" class="app-card-nav-link">
                    <span>Browse items</span>
                </router-link>
            </div>
            <div class="app-card-nav-item" v-bind:style="{'background-image': `url(img/app-card-nav/icons8-red-card-80.png)`}">
                <router-link v-bind:to="{ name: 'study-items-learn-falshcards', params: {}}" class="app-card-nav-link">
                    <span>Flash cards</span>
                </router-link>
            </div>
            <div class="app-card-nav-item" v-bind:style="{'background-image': `url(img/app-card-nav/icons8-rich-text-converter-96.png)`}">
                <router-link v-bind:to="{ name: 'study-items-learn-falshcards', params: {}}" class="app-card-nav-link">
                    <span>Word meaning</span>
                </router-link>
            </div>
            <div class="app-card-nav-item" v-bind:style="{'background-image': `url(img/app-card-nav/icons8-dictionary-64.png)`}">
                <router-link v-bind:to="{ name: 'study-items-learn-falshcards', params: {}}" class="app-card-nav-link">
                    <span>Meaning of a word</span>
                </router-link>
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

export default {
    name: 'study-items-dashboard',
    components: {
        RowLoader,
        CustomCollections,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
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
            return this.$store.dispatch(storeTypes.STUDY_ITEM_TRAINING_STATS_LOAD, {})
                .then()
                .catch(err => {
                    console.error(err);
                    notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
                });
        },
    },
}
</script>
