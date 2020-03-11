<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                <div v-if="studyItems" class="study-items-wrapper">
                    <h5 class="mb-3">Study items:</h5>
                    
                    <div class="btn-toolbar mb-3" role="toolbar" aria-label="Toolbar with button groups">
                        <div class="btn-group mr-2" role="group" aria-label="View toggle">
                            <button 
                                v-on:click="toggleView" 
                                v-bind:class="{'btn-primary': privateState.currentView === 'list', 'btn-secondary' : privateState.currentView !== 'list'}"
                                type="button" 
                                class="btn"
                            >
                                <i class="fas fa-list"></i>
                            </button>
                            <button 
                                v-on:click="toggleView" 
                                v-bind:class="{'btn-primary': privateState.currentView === 'cards', 'btn-secondary' : privateState.currentView !== 'cards'}"
                                type="button" 
                                class="btn"
                            >
                                <i class="fas fa-th"></i>
                            </button>
                        </div>
                        <div class="btn-group mr-2" role="group" aria-label="Create a new item">
                            <button 
                                v-on:click="onCreateStudyItem" 
                                type="button" 
                                class="btn btn-success"
                            >
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                    </div>
                    
                    <div>
                        <pagination-wrapper
                            v-bind:paginationResult="sharedState.studyItemsPaginationResult"
                            v-bind:loadItemsF="loadStudyItems"
                            v-bind:showGoToButtons="true"
                        >
                            <!-- List view -->
                            <div v-if="privateState.currentView === 'list'" class="list-group study-items-list">
                                <a 
                                    v-for="(item) in studyItems"
                                    v-bind:key="`list-${item.id}`"
                                    href="#" 
                                    class="list-group-item list-group-item-action flex-column align-items-start study-item"
                                >
                                    <div class="d-flex w-100 justify-content-between">
                                        <h6 class="mb-1">{{item.title}}</h6>
                                        <div>
                                            <span class="badge badge-info mr-1">{{ item.languageCode }}</span>
                                            <span
                                                v-for="(tag) in item.tags"
                                                v-bind:key="tag"
                                                class="badge badge-secondary mr-1"
                                            >{{tag}}</span>
                                            <span v-on:click="onStudyItemFavoriteClick(item)" class="cursor-pointer">
                                                <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                                <i v-else class="far fa-star text-warning"></i>
                                            </span>
                                            <span class="ml-2 mr-2">|</span>
                                            <span>
                                                <span v-on:click="onUpdateStudyItem(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-pencil-alt"></i>
                                                </span>
                                                <span v-on:click="onDeleteStudyItem(item.id)" class="badge badge-secondary cursor-pointer">
                                                    <i class="fas fa-times"></i>
                                                </span>
                                            </span>
                                        </div>
                                    </div>
                                    <div>
                                        <small>{{ item.description }}</small>
                                    </div>
                                    <div class="text-secondary">
                                        <small><em>{{ item.exampleText }}</em></small>
                                    </div>
                                </a>
                            </div>

                            <!-- Card view -->
                            <div v-if="privateState.currentView === 'cards'" class="study-items-card-list">
                                <div
                                    v-for="(item) in studyItems"
                                    v-bind:key="`card-${item.id}`"
                                    class="card bg-light study-item-card" 
                                >
                                    <!-- <div class="card-header"></div> -->
                                    <img v-if="item.image" class="card-img-top" v-bind:src="item.image.url" v-bind:alt="item.title">
                                    <img v-else class="card-img-top" src="/img/empty-image.png">
                                    <div class="card-body">
                                        <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                            <h6 class="card-title mb-0">
                                                <span>{{item.title}}</span>
                                            </h6>
                                        </div>
                                    
                                        <div class="card-text small mb-1">
                                            <div>{{ item.description }}</div>
                                        </div>
                                        <div class="card-text small text-secondary mb-1">
                                            <em>{{ item.exampleText }}</em>
                                        </div>
                                        <div class="card-text">
                                            <span class="badge badge-info mr-1">{{ item.languageCode }}</span>
                                            <span
                                                v-for="(tag) in item.tags"
                                                v-bind:key="tag"
                                                class="badge badge-secondary mr-1"
                                            >{{tag}}</span>
                                        </div>
                                    </div>
                                    <div class="card-bottom-controls">
                                        <span v-on:click="onStudyItemFavoriteClick(item)" class="card-bottom-control-item">
                                            <i v-if="item.isFavourite" class="fas fa-star text-warning"></i>
                                            <i v-else class="far fa-star text-warning"></i>
                                        </span>
                                        <span v-on:click="onUpdateStudyItem(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-pencil-alt"></i>
                                        </span>
                                        <span v-on:click="onDeleteStudyItem(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-times"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </pagination-wrapper>
                    </div>
                </div>


                <modal 
                    name="study-item-create-edit" 
                    height="auto"
                    width="450px"
                    v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                    v-bind:clickToClose="false"
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
                            <form v-on:submit.prevent="createEditStudyItem(privateState.modalMode)">
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
                                    <textarea v-model="privateState.studyItemModel.exampleText" type="text" class="form-control" id="studyItemModel__exampleText" placeholder="Example text" />
                                </div>
                                <div class="form-group form-check">
                                    <input v-model="privateState.studyItemModel.isFavourite" class="form-check-input" type="checkbox" id="studyItemModel__isFavourite">
                                    <label class="form-check-label" for="studyItemModel__isFavourite">
                                        Favourite
                                    </label>
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
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import TagsMultiselect from '@/components/TagsMultiselect';

const studyItemModelDefault = {
    title: null,
    description: null,
    exampleText: null,
    isFavourite: false,
    languageCode: "en",
    tags: [],
};

export default {
    name: 'study-items-browse',
    components: {
        RowLoader,
        LoadingButton,
        PaginationWrapper,
        LanguageCodeSelect,
        TagsMultiselect,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                currentView: 'list', // ['list', 'cards']
                studyItemModel: {
                    ...studyItemModelDefault,
                },
                modalMode: 'create', // ['create', 'edit']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            studyItems: state => state.studyItemsPaginationResult ? state.studyItemsPaginationResult.items : null,
        }),
    },
    created: async function() {
        this.loadStudyItems();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadStudyItems: function({offset = 0, limit = 50} = {}) {
            return this.$store.dispatch(storeTypes.STUDY_ITEMS_LOAD, {
                offset: offset, 
                limit: limit, 
                search: null,
                isFavourite: null,
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        toggleView: function() {
            this.privateState.currentView = this.privateState.currentView === 'list' ? 'cards' : 'list';
        },
        onCreateStudyItem: function() {
            this.privateState.modalMode = 'create';
            this.$modal.show('study-item-create-edit');
        },
        onUpdateStudyItem: function(studyItemId) {
            this.privateState.modalMode = 'edit';
            let studyItem = this.studyItems.find(x => x.id === studyItemId);
            this.privateState.studyItemModel = {id: studyItem.id, ...studyItem};
            this.$modal.show('study-item-create-edit');
        },
        onDeleteStudyItem: function(studyItemId) {
            if(confirm('Are you sure?')) {
                this.deleteStudyItem(studyItemId);
            }
        },
        onStudyItemFavoriteClick: function(studyItem) {
            if(studyItem.isFavourite) {
                this.deleteStudyItemFromFavourites(studyItem.id);
            } else {
                this.addStudyItemToFavourites(studyItem.id);
            }
        },
        createEditStudyItem: function(mode) {
            if(mode === 'create') {
                this.createStudyItem();
            } else if(mode === 'edit') {
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

                this.$modal.hide('study-item-create-edit');

                // reset
                this.privateState.studyItemModel = {
                    ...studyItemModelDefault,
                };
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

                this.$modal.hide('study-item-create-edit');

                // reset
                this.privateState.studyItemModel = {
                    ...studyItemModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteStudyItem: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_DELETE, {
                studyItemId: studyItemId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Item has been deleted!`,
                    text: '',
                    duration: 5000,
                });

                const itemCountThresholdBeforeReload = 3;
                if(this.studyItems.length <= itemCountThresholdBeforeReload) {
                    // reload
                    this.loadStudyItems();
                }
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        addStudyItemToFavourites: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_ADD_TO_FAVOURITES, {
                studyItemId: studyItemId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteStudyItemFromFavourites: function(studyItemId) {
            this.$store.dispatch(storeTypes.STUDY_ITEM_DELETE_FROM_FAVOURITES, {
                studyItemId: studyItemId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
