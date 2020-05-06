<template>
    <ul 
        v-if="treeItem"
        v-bind:class="{'folder-tree-view--root': treeItem.isRoot}"
        class="folder-tree-view"
    >
        <li>
            <div
                v-bind:class="{'font-weight-bold': true}"
                class="tree-item"
            >
                <span v-if="hasChildren" v-on:click="toggle" class="tree-item-toggle mr-1">
                    <i v-if="!privateState.isOpen" class="fas fa-chevron-down"></i>
                    <i v-if="privateState.isOpen" class="fas fa-chevron-up"></i>
                </span>
                <span class="tree-item-name" v-on:click="folderClicked(treeItem)">
                    <i v-if="hasChildren" class="fas fa-folder"></i>
                    <i v-if="!hasChildren" class="far fa-folder"></i>
                    {{treeItem.name}}
                </span>
                <!-- <span v-if="hasChildren">[{{ privateState.isOpen ? '-' : '+' }}]</span> -->
                
                <span class="tree-item-controls ml-3">
                    <span 
                        v-on:click="createFolder(treeItem)"
                        class="control-item mr-2"
                    >
                        <i class="fas fa-folder-plus"></i>
                    </span>
                    <span 
                        v-on:click="updateFolder(treeItem)"
                        class="control-item mr-2"
                    >
                        <i class="fas fa-edit"></i>
                    </span>
                    <span 
                        v-on:click="createFolderItem(treeItem)"
                        class="control-item mr-2"
                    >
                        <i class="fas fa-file-medical"></i>
                    </span>
                    <span 
                        v-if="!treeItem.isRoot"
                        v-on:click="duplicateFolder(treeItem)"
                        class="control-item mr-2"
                    >
                        <i class="far fa-copy"></i>
                    </span>
                    <span 
                        v-if="!treeItem.isRoot"
                        v-on:click="deleteFolder(treeItem)"
                        class="control-item mr-2"
                    >
                        <i class="fas fa-folder-minus"></i>
                    </span>
                </span>
            </div>
            <!--If `children` is undefined this will not render-->
            <div
                v-show="privateState.isOpen"
            >
                <folder-tree-view
                    v-for="child in treeItem.children" 
                    v-bind:key="child.id"
                    v-bind:treeItem="child"
                    v-bind:onFolderClick="folderClicked"
                    v-bind:onCreateFolder="createFolder"
                    v-bind:onUpdateFolder="updateFolder"
                    v-bind:onCreateFolderItem="createFolderItem"
                    v-bind:onDuplicateFolder="duplicateFolder"
                    v-bind:onDeleteFolder="deleteFolder"
                >
                </folder-tree-view>
            </div>
        </li>
    </ul>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import _ from 'lodash';

export default {
    name: 'folder-tree-view',
    props: {
        /*
            {
                name: "My Tree",
                isRoot: true,
                children: [
                { name: "hello1" },
                { name: "wat1" },
                {
                    name: "child folder1",
                    children: [
                    {
                        name: "child folder2",
                        children: [{ name: "hello3" }, { name: "wat3" }]
                    },
                    { name: "hello2" },
                    { name: "wat2" },
                    {
                        name: "child folder2",
                        children: [{ name: "hello3" }, { name: "wat3" }]
                    }
                    ]
                }
                ]
            }
        */
        treeItem: {
            type: Object,
            required: true,
        },
        onFolderClick: {
            type: Function,
            required: false,
            default: null,
        },
        onCreateFolder: {
            type: Function,
            required: false,
            default: null,
        },
        onUpdateFolder: {
            type: Function,
            required: false,
            default: null,
        },
        onCreateFolderItem: {
            type: Function,
            required: false,
            default: null,
        },
        onDuplicateFolder: {
            type: Function,
            required: false,
            default: null,
        },
        onDeleteFolder: {
            type: Function,
            required: false,
            default: null,
        },
    },
    components: {
        // RowLoader,
        // LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                isOpen: false,
            },
        };
    },
    computed: {
        // local computed go here
        hasChildren: function() { return this.treeItem.children && this.treeItem.children.length > 0 },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        }),
    },
    created: function() {
        let self = this;
        if(this.treeItem) {
            this.privateState.isOpen = this.treeItem.isRoot === true;
        }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        toggle: function() {
            this.privateState.isOpen = !this.privateState.isOpen;
        },
        folderClicked: function(sourceFolder) {
            if(this.onFolderClick) {
                this.onFolderClick(sourceFolder);
            }
        },
        createFolder: function(parentFolder) {
            if(this.onCreateFolder) {
                this.onCreateFolder(parentFolder);
            }
        },
        updateFolder: function(sourceFolder) {
            if(this.onUpdateFolder) {
                this.onUpdateFolder(sourceFolder);
            }
        },
        createFolderItem: function(parentFolder) {
            if(this.onCreateFolderItem) {
                this.onCreateFolderItem(parentFolder);
            }
        },
        duplicateFolder: function(sourceFolder) {
            if(this.onDuplicateFolder) {
                this.onDuplicateFolder(sourceFolder);
            }
        },
        deleteFolder: function(sourceFolder) {
            if(this.onDeleteFolder) {
                this.onDeleteFolder(sourceFolder);
            }
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
