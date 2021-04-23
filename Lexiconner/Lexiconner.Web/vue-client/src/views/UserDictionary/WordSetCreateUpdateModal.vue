<template>
    <div class="">
        <!-- Word create/edit -->
        <modal 
            name="word-set-create-edit" 
            height="auto"
            width="450px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">
                        <span v-if="privateState.modalMode === 'create'">Create word set</span>
                        <span v-if="privateState.modalMode === 'edit'">Edit word set</span>
                    </div>
                    <div v-on:click="$modal.hide('word-set-create-edit')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.WORDS_LOAD]"></row-loader>

                    <form v-on:submit.prevent="createEditWordSet()">
                        <div class="form-group">
                            <label for="wordSetModel__title">Name</label>
                            <input v-model="privateState.wordSetModel.name" type="text" class="form-control" id="wordSetModel__title" placeholder="Name" />
                        </div>
                        <loading-button 
                            type="submit"
                            v-bind:loading="sharedState.loading[privateState.storeTypes.USER_DICTIONARY_WORD_SET_CREATE] || sharedState.loading[privateState.storeTypes.USER_DICTIONARY_WORD_SET_UPDATE]"
                            class="btn custom-btn-normal btn-block"
                        >Save</loading-button>
                    </form>
                </div>
            </div>
        </modal>
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
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import TagsMultiselect from '@/components/TagsMultiselect';
import CustomCollectionsMultiselect from '@/components/CustomCollectionsMultiselect';
import CustomCollections from '@/components/CustomCollections';
import WordsFilters from '@/components/WordsFilters';
import UserWordSetSelector from '@/components/UserWordSetSelector';

import ProgressBar from 'vue-simple-progress'

const wordSetModelDefault = {
    name: null,
};

export default {
    name: 'word-set-create-update-modal',
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
                wordSetModel: _.cloneDeep(wordSetModelDefault),
                modalMode: 'create', // ['create', 'edit']
                wordSetEditingId: null,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            wordSet: state => state.wordSet,
            userDictionary: state => state.userDictionary,
        }),
    },
    watch: {
    },
    created: function() {
        // watch when edited word loaded
        this.unwatch = this.$store.watch(
            (state, getters) => state.userDictionary,
            (newValue, oldValue) => {
                if(this.privateState.modalMode === 'edit') {
                    const userDictionary = newValue;
                    const wordSet = userDictionary.wordSets.find(x => x.id === this.privateState.wordSetEditingId) || null;
                    this.privateState.wordSetModel = { ...(wordSet || {}) };
                }
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
        show: function({ wordSetId }) {
            // reset
            this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);

            if(wordSetId !== null) {
                this.privateState.modalMode = 'edit';
                this.privateState.wordSetEditingId = wordSetId;

                this.loadWordSet({ wordSetId });
            } else {
                this.privateState.modalMode = 'create';
                this.privateState.wordSetModel = {
                    ..._.cloneDeep(wordSetModelDefault),
                };
            }

            this.$modal.show('word-set-create-edit');
        },
        hide: function() {
            this.$modal.hide('word-set-create-edit');

            // reset
            this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);
        },
        loadWordSet: function({wordSetId} = {}) {
            return this.$store.dispatch(storeTypes.USER_DICTIONARY_LOAD, {}).then((data) => {

            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        createEditWordSet: function() {
            if(this.privateState.modalMode === 'create') {
                this.createWordSet();
            } else if(this.privateState.modalMode === 'edit') {
                this.updateWordSet();
            }
        },
        createWordSet: function() {
            this.$store.dispatch(storeTypes.USER_DICTIONARY_WORD_SET_CREATE, {
                data: {
                    ...this.privateState.wordSetModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Word set '${this.privateState.wordSetModel.name}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateWordSet: function() {
            this.$store.dispatch(storeTypes.USER_DICTIONARY_WORD_SET_UPDATE, {
                wordSetId: this.privateState.wordSetModel.id,
                data: {
                    ...this.privateState.wordSetModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Word set '${this.privateState.wordSetModel.name}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.wordSetModel = _.cloneDeep(wordSetModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
