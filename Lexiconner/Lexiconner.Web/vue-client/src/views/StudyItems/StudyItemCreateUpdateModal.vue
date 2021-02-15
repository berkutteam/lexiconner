<template>
    <div class="my-permissions-wrapper">
       <!-- Study item create/edit -->
        <modal 
            name="study-item-create-edit" 
            height="auto"
            width="450px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">
                        <span v-if="privateState.modalMode === 'create'">Create item</span>
                        <span v-if="privateState.modalMode === 'edit'">Edit item</span>
                    </div>
                    <div v-on:click="$modal.hide('study-item-create-edit')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                    <form v-on:submit.prevent="createEditStudyItem()">
                        <div class="form-group">
                            <label for="studyItemModel__title">Title</label>
                            <input v-model="privateState.studyItemModel.title" type="text" class="form-control" id="studyItemModel__title" placeholder="Title" />
                        </div>
                        <div class="form-group">
                            <label for="studyItemModel__description">Description</label>
                            <textarea v-model="privateState.studyItemModel.description" type="text" class="form-control" id="studyItemModel__description" placeholder="Description" />
                        </div>
                        <div class="form-group">
                            <label for="studyItemModel__exampleText">Example text</label>
                            <textarea 
                                v-for="(exampleText, exampleTextIndex) in privateState.studyItemModel.exampleTexts"
                                v-bind:key="`study-item-exampleText-${exampleTextIndex}`"
                                v-model="privateState.studyItemModel.exampleTexts[exampleTextIndex]" 
                                v-bind:placeholder="`Example text ${exampleTextIndex + 1}`"
                                type="text" 
                                class="form-control mb-1" 
                                id="studyItemModel__exampleText" 
                            />
                            <div class="btn-group" role="group">
                                <button 
                                    v-on:click="onAddStudyItemExampleText" 
                                    type="button" 
                                    class="btn btn-sm btn-secondary mr-0"
                                >
                                    <i class="fas fa-plus"></i>
                                </button>
                                <button 
                                    v-on:click="onRemoveStudyItemExampleText" 
                                    type="button" 
                                    class="btn btn-sm btn-secondary"
                                >
                                    <i class="fas fa-minus"></i>
                                </button>
                            </div>
                        </div>
                        <div class="form-group form-check">
                            <!-- <input v-model="privateState.studyItemModel.isFavourite" class="form-check-input" type="checkbox" id="studyItemModel__isFavourite"> -->
                            <!-- <label class="form-check-label" for="studyItemModel__isFavourite">
                                Favourite
                            </label> -->
                        </div>
                        <div class="form-group">
                            <span v-on:click="onFavoriteChange(!privateState.studyItemModel.isFavourite)" class="cursor-pointer">
                                <i v-if="privateState.studyItemModel.isFavourite" class="fas fa-star text-warning "></i>
                                <i v-else class="far fa-star text-warning"></i>
                                <span class="ml-2">Favorite</span>
                            </span>
                        </div>
                        <div class="form-group">
                            <label for="">Language</label>
                            <language-code-select
                                v-model="privateState.studyItemModel.languageCode"
                            />
                        </div>
                        <div class="form-group">
                            <label for="">Tags</label>
                            <tags-multiselect 
                                v-model="privateState.studyItemModel.tags" 
                            ></tags-multiselect>
                        </div>
                        <div class="form-group">
                            <label for="">Collections (aka folders)</label>
                            <custom-collections-multiselect
                                v-model="privateState.studyItemModel.customCollectionIds" 
                            ></custom-collections-multiselect>
                        </div>
                        <loading-button 
                            type="submit"
                            v-bind:loading="sharedState.loading[privateState.storeTypes.STUDY_ITEM_CREATE] || sharedState.loading[privateState.storeTypes.STUDY_ITEM_UPDATE]"
                            class="btn btn-outline-success btn-block"
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
import StudyItemsFilters from '@/components/StudyItemsFilters';

import ProgressBar from 'vue-simple-progress'

const studyItemModelDefault = {
    title: null,
    description: null,
    exampleTexts: [""],
    isFavourite: false,
    languageCode: "en",
    tags: [],
    customCollectionIds: [],
};

export default {
    name: 'study-item-create-update-modal',
    props: {
    },
    components: {
        RowLoader,
        LoadingButton,
        LanguageCodeSelect,
        TagsMultiselect,
        CustomCollectionsMultiselect,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                studyItemModel: _.cloneDeep(studyItemModelDefault),
                modalMode: 'create', // ['create', 'edit']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            studyItem: state => state.studyItem,
        }),
    },
    watch:  {
    },
    created: function() {
        // watch when edited tem loaded
        this.unwatch = this.$store.watch(
            (state, getters) => state.studyItem,
            (newValue, oldValue) => {
                this.privateState.studyItemModel = { ...newValue };
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
        show: function({ studyItemId, customCollectionIds = [] }) {
            // reset
            this.privateState.studyItemModel = _.cloneDeep(studyItemModelDefault);

            if(studyItemId !== null) {
                this.privateState.modalMode = 'edit';

                this.loadStudyItem({ studyItemId });
            } else {
                this.privateState.modalMode = 'create';
                this.privateState.studyItemModel = {
                    ..._.cloneDeep(studyItemModelDefault),
                    customCollectionIds: [
                        ...customCollectionIds,
                    ]
                };
            }

            this.$modal.show('study-item-create-edit');
        },
        hide: function() {
            this.$modal.hide('study-item-create-edit');

            // reset
            this.privateState.studyItemModel = _.cloneDeep(studyItemModelDefault);
        },
        loadStudyItem: function({studyItemId} = {}) {
            return this.$store.dispatch(storeTypes.STUDY_ITEM_LOAD, {
                studyItemId, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onFavoriteChange: function(value) {
            this.privateState.studyItemModel.isFavourite = value;
        },
        onAddStudyItemExampleText: function() {
            this.privateState.studyItemModel.exampleTexts.push("");
        },
        onRemoveStudyItemExampleText: function() {
            if(this.privateState.studyItemModel.exampleTexts.length > 1) {
                this.privateState.studyItemModel.exampleTexts.pop();
            }
        },
        createEditStudyItem: function() {
            if(this.privateState.modalMode === 'create') {
                this.createStudyItem();
            } else if(this.privateState.modalMode === 'edit') {
                this.updateStudyItem();
            }
        },
        createStudyItem: function() {
            this.$store.dispatch(storeTypes.STUDY_ITEM_CREATE, {
                data: {
                    ...this.privateState.studyItemModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item '${this.privateState.studyItemModel.title}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.studyItemModel = _.cloneDeep(studyItemModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateStudyItem: function() {
            this.$store.dispatch(storeTypes.STUDY_ITEM_UPDATE, {
                studyItemId: this.privateState.studyItemModel.id,
                data: {
                    ...this.privateState.studyItemModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item '${this.privateState.studyItemModel.title}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.hide();

                // reset
                this.privateState.studyItemModel = _.cloneDeep(studyItemModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
