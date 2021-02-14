<template>
    <div class="mb-3">
        <form v-on:submit.prevent="() => {}" class="form-inline">
            <div class="form-group mr-2">
                <label class="sr-only" for="studyItemsRequestParams__search">Search</label>
                <input 
                    v-bind:value="sharedState.studyItemsRequestParams.search"
                    v-on:input="onSearchChange"
                    type="text" 
                    class="form-control" 
                    id="studyItemsRequestParams__search" 
                    placeholder="Search"
                >
            </div>

            <!-- <toggle-button 
                v-on:value="sharedState.studyItemsRequestParams.isFavourite || false"
                v-bind:sync="true"
                v-on:change="onIsFavoriteChange"
                v-bind:labels="{checked: '★', unchecked: '⁂'}"
                v-bind:font-size="16"
                v-bind:color="{checked: '#ffc107', unchecked: '#6c757d'}"
                v-bind:class="' mr-2'"
            /> -->

            <!-- Favorite -->
            <div class="form-group mr-2 cursor-pointer">
                <i v-on:click="setIsFavorite(null)" v-bind:class="{'text-warning': sharedState.studyItemsRequestParams.isFavourite === null}" class="fas fa-star-half-alt mr-1"></i>
                <i v-on:click="setIsFavorite(true)" v-bind:class="{'text-warning': sharedState.studyItemsRequestParams.isFavourite === true}" class="fas fa-star mr-1"></i>
                <i v-on:click="setIsFavorite(false)" v-bind:class="{'text-warning': sharedState.studyItemsRequestParams.isFavourite === false}" class="far fa-star"></i>
            </div>

            <!-- Shuffle -->
            <div class="form-group mr-2 cursor-pointer">
                <i 
                    v-on:click="setIsShuffle(!sharedState.studyItemsRequestParams.isShuffle)" 
                    v-bind:class="{'text-info': sharedState.studyItemsRequestParams.isShuffle === true}"
                    class="fas fa-random"
                ></i>
            </div>

            <!-- Reload -->
            <div class="form-group mr-2">
                <button v-on:click="reload" type="button" class="btn btn-outline-secondary">
                    <i class="fas fa-sync-alt"></i>
                </button>
            </div>

            <!-- Reset -->
            <div class="form-group mr-2">
                <button v-on:click="resetRequestParams" type="button" class="btn btn-outline-secondary">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        </form>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import { ToggleButton } from 'vue-js-toggle-button';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

export default {
    name: 'study-items-filters',
    props: {
        onChange: {
            type: Function,
            required: false,
            default: null,
        },
    },
    components: {
        // RowLoader,
        // LoadingButton,
        // ToggleButton,
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
        }),
    },
    created: async function() {
        
    },
    mounted: function() {
    },
    updated: function() {
    },
    beforeDestroy: function() {
    },
    destroyed: function() {
        this.resetRequestParams();
    },

    methods: {
        callOnChange: function() {
            if(this.onChange) {
                this.onChange();
            }
        },
        callOnChangeDebounce: _.debounce(function() {
             if(this.onChange) {
                this.onChange();
            }
        }, 500),
        onSearchChange: function(e) {
            this.$store.commit(storeTypes.STUDY_ITEMS_REQUEST_PARAMS_SET, {
                search: e.target.value,
            });
            this.callOnChangeDebounce();
        },
        // onIsFavoriteChange: function({value, tag, srcEvent}) {
            // this.$store.commit(storeTypes.STUDY_ITEMS_REQUEST_PARAMS_SET, {
            //     isFavourite: value,
            // });
        // },
        setIsFavorite: function(value) {
            this.$store.commit(storeTypes.STUDY_ITEMS_REQUEST_PARAMS_SET, {
                isFavourite: value,
            });
            this.callOnChange();
        },
        setIsShuffle: function(value) {
            this.$store.commit(storeTypes.STUDY_ITEMS_REQUEST_PARAMS_SET, {
                isShuffle: value,
            });
            this.callOnChange();
        },
        reload: function() {
            this.callOnChange();
        },
        resetRequestParams: function() {
            this.$store.commit(storeTypes.STUDY_ITEMS_REQUEST_PARAMS_RESET, {});
            this.callOnChange();
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
