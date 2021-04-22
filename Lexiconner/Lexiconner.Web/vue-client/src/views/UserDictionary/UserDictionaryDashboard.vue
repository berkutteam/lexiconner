<template>
    <div>
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.USER_DICTIONARY_LOAD]" class="mb-2"></row-loader>

        <h4>Added word sets:</h4>
        <div v-if="userDictionary" class="wordsets-card-list">
            <div
                v-for="(item) in userDictionary.wordSets"
                v-bind:key="item.id"
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
                        <span class="badge badge-secondary">{{item.wordCount}}</span>
                    </div>
                </div>

                <!-- Overlay controls -->
                <div  class="item-card-overlay-controls">
                    <div class="w-100 d-flex justify-content-center">
                        <loading-button 
                            type="button"
                            v-bind:loading="sharedState.loading[privateState.storeTypes.USER_DICTIONARY_WORD_SET_DELETE]"
                            v-on:click.native="onDeleteWordSetClick(item.id)"
                            class="btn btn-sm custom-btn-normal"
                        >
                            <i class="fas fa-plus mr-2"></i> Delete from my dictionary
                        </loading-button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';

export default {
    name: 'user-dictionary',
    props: {
    },
    components: {
        RowLoader,
        LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
            },
        };
    },
    computed: {
        // store state computed go here
        ...mapState({
            sharedState: state => state,
            profile: state => state.profile,
            userDictionary: state => state.userDictionary,
        }),
    },
    created: async function() {
        if(!this.userDictionary) {
            this.loadUserDictionary();
        }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadUserDictionary: function() {
            return this.$store.dispatch(storeTypes.USER_DICTIONARY_LOAD_SET, {}).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onDeleteWordSetClick: function(wordSetId) {
            if(confirm(`You will lost words in word set and its' training status. Are you sure?`)) {
                return this.$store.dispatch(storeTypes.USER_DICTIONARY_WORD_SET_DELETE, {
                    wordSetId, 
                }).then().catch(err => {
                    console.error(err);
                    notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
