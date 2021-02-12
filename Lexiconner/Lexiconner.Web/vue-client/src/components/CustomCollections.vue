<template>
    <div>
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.CUSTOM_COLLECTIONS_LOAD]"></row-loader>

        <div v-if="sharedState.customCollectionsResult">
            <folder-tree-view
                v-bind:treeItem="customCollectionsTree"
                v-bind:activeTreeItemId="$store.getters.currentCustomCollectionId"
                v-bind:onFolderClick="onFolderClick"
                v-bind:onCreateFolder="onCreateFolder"
                v-bind:onUpdateFolder="onUpdateFolder"
                v-bind:onCreateFolderItem="onCreateFolderItem"
                v-bind:onDuplicateFolder="onDuplicateFolder"
                v-bind:onDeleteFolder="onDeleteFolder"
            >
            </folder-tree-view>

            <!-- Custom collection edit -->
            <modal 
                name="custom-collection-edit" 
                height="auto"
                width="450px"
                v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                v-bind:clickToClose="false"
            >
                <div class="app-modal">
                    <div class="app-modal-header">
                        <div class="app-modal-title">
                            <span v-if="privateState.modalMode === 'create'">Create collection</span>
                            <span v-if="privateState.modalMode === 'edit'">Edit collection</span>
                        </div>
                        <div v-on:click="$modal.hide('custom-collection-edit')" class="app-modal-close">
                            <i class="fas fa-times"></i>
                        </div>
                    </div>
                    
                    <div class="app-modal-content">
                        <form v-on:submit.prevent="createEditCustomCollection(privateState.modalMode)">
                            <div class="form-group">
                                <label for="customCollectionModel__name">Name</label>
                                <input v-model="privateState.customCollectionModel.name" type="text" class="form-control" id="customCollectionModel__name" placeholder="Name" />
                            </div>
                            <loading-button 
                                type="submit"
                                v-bind:loading="sharedState.loading[privateState.storeTypes.CUSTOM_COLLECTION_CREATE] || sharedState.loading[privateState.storeTypes.CUSTOM_COLLECTION_UPDATE]"
                                class="btn btn-outline-success btn-block"
                            >Save</loading-button>
                        </form>
                    </div>
                </div>
            </modal>

            <!-- Custom collection delete  -->
            <modal 
                name="custom-collection-delete" 
                height="auto"
                width="450px"
                v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                v-bind:clickToClose="false"
            >
                <div class="app-modal">
                    <div class="app-modal-header">
                        <div class="app-modal-title">
                            <span>Delete collection</span>
                        </div>
                        <div v-on:click="$modal.hide('custom-collection-delete')" class="app-modal-close">
                            <i class="fas fa-times"></i>
                        </div>
                    </div>
                    
                    <div class="app-modal-content">
                        <form v-on:submit.prevent="deleteCustomCollection(privateState.modalMode)">
                            <div class="form-check">
                                <input v-model="privateState.customCollectionDeleteModel.isDeleteItems" class="form-check-input" type="checkbox" name="customCollectionDeleteModel_isDeleteItems" id="customCollectionDeleteModel_isDeleteItems">
                                <label class="form-check-label" for="customCollectionDeleteModel_isDeleteItems">
                                    Move items to parent collection
                                </label>
                                <small class="form-text text-muted">If not checked the items in collection will be deleted.</small>
                            </div>
                            <loading-button 
                                type="submit"
                                v-bind:loading="sharedState.loading[privateState.storeTypes.CUSTOM_COLLECTION_DELETE]"
                                class="btn btn-outline-danger btn-block mt-3"
                            >Delete</loading-button>
                        </form>
                    </div>
                </div>
            </modal>

            <!-- Study item create/edit -->
            <study-item-create-update-modal
                ref="customCollections_studyItemCreateUpdateModal"
            >
            </study-item-create-update-modal>
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
import FolderTreeView from '@/components/FolderTreeView';
import StudyItemCreateUpdateModal from '@/views/StudyItems/StudyItemCreateUpdateModal';

const customCollectionModelDefault = {
    name: null,
    parentCollectionId: null,
};
const customCollectionDeleteModelDefault = {
    customCollectionId: null,
    isDeleteItems: null,
};

export default {
    name: 'custom-collections',
    props: {
        onSelectedCollectionChange: {
            type: Function,
            required: false,
            default: null,
        },
    },
    components: {
        RowLoader,
        LoadingButton,
        FolderTreeView,
        StudyItemCreateUpdateModal,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                customCollectionModel: _.cloneDeep(customCollectionModelDefault),
                customCollectionDeleteModel: _.cloneDeep(customCollectionDeleteModelDefault),
                modalMode: 'create', // ['create', 'edit']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            customCollectionsTree: state => state.customCollectionsResult.asTree,
            customCollectionsList: state => state.customCollectionsResult.asList,
        }),
    },
    created: function() {
        let self = this;

        this.$store.dispatch(storeTypes.CUSTOM_COLLECTIONS_LOAD, {})
        .then()
        .catch(err => {
            console.error(err);
            notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
        });
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        onFolderClick: function(sourceFolder) {
            if(this.onSelectedCollectionChange) {
                this.onSelectedCollectionChange(sourceFolder.id);
            }

            // update currect collection on store
            this.$store.commit(storeTypes.CUSTOM_COLLECTION_CURRENT_SET, {
                customCollection: sourceFolder,
            });

            // mark as selected
            this.markCustomCollectionAsSelectedDebounce(sourceFolder.id);
        },
        onCreateFolder: function(parentFolder) {
            this.privateState.modalMode = 'create';
            this.privateState.customCollectionModel.parentCollectionId = parentFolder.id;
            this.$modal.show('custom-collection-edit');
        },
        onUpdateFolder: function(sourceFolder) {
            this.privateState.modalMode = 'edit';
            let collection = this.customCollectionsList.find(x => x.id === sourceFolder.id);
            this.privateState.customCollectionModel = {id: collection.id, ...collection};
            this.$modal.show('custom-collection-edit');
        },
        onCreateFolderItem: function(parentFolder) {
            this.$refs.customCollections_studyItemCreateUpdateModal.show({
                studyItemId: null,
                customCollectionIds: [parentFolder.id],
            });
        },
        onDuplicateFolder: function(sourceFolder) {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTION_DUPLICATE, {
                    customCollectionId: sourceFolder.id,
                }).then(() => {
                    this.$notify({
                        group: 'app',
                        type: 'success',
                        title: `Collection has been duplicated!`,
                        text: '',
                        duration: 5000,
                    });
                }).catch(err => {
                    console.error(err);
                    notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
                });
        },
        onDeleteFolder: function(sourceFolder) {
            this.privateState.customCollectionDeleteModel = {
                customCollectionId: sourceFolder.id,
                isDeleteItems: false,
            };
            this.$modal.show('custom-collection-delete');
        },
        createEditCustomCollection: function(mode) {
            if(mode === 'create') {
                this.createCustomCollection();
            } else if(mode === 'edit') {
                this.updateCustomCollection();
            }
        },
        createCustomCollection: function() {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTION_CREATE, {
                data: {
                    ...this.privateState.customCollectionModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Collection '${this.privateState.customCollectionModel.name}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('custom-collection-edit');

                // reset
                this.privateState.customCollectionModel = _.cloneDeep(customCollectionModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateCustomCollection: function() {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTION_UPDATE, {
                customCollectionId: this.privateState.customCollectionModel.id,
                data: {
                    ...this.privateState.customCollectionModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Collection '${this.privateState.customCollectionModel.name}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('custom-collection-edit');

                // reset
                this.privateState.customCollectionModel = _.cloneDeep(customCollectionModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markCustomCollectionAsSelected: function(customCollectionId) {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTION_MARK_AS_SELECTED, {
                customCollectionId: customCollectionId,
            }).then(() => {
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        markCustomCollectionAsSelectedDebounce: _.debounce(function(customCollectionId) {
            this.markCustomCollectionAsSelected(customCollectionId);
        }, 500),
        deleteCustomCollection: function() {
            this.$store.dispatch(storeTypes.CUSTOM_COLLECTION_DELETE, {
                customCollectionId: this.privateState.customCollectionDeleteModel.customCollectionId,
                isDeleteItems: this.privateState.customCollectionDeleteModel.isDeleteItems,
            }).then(() => {
                this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Collection has been deleted!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('custom-collection-delete');

                // reset
                this.privateState.customCollectionDeleteModel = _.cloneDeep(customCollectionDeleteModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
