<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <div v-if="userFilms" class="user-films-wrapper">
                    <h5 class="mb-3">My films:</h5>

                    <!-- Toolbar -->
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
                                v-on:click="onCreateUserFilm" 
                                type="button" 
                                class="btn btn-success"
                            >
                                <i class="fas fa-plus"></i>
                            </button>
                        </div>
                    </div>

                    <!-- Filters -->
                    <user-films-filters
                        v-bind:onChange="loadUserFilms"
                    >
                    </user-films-filters>
                    
                    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.USER_FILMS_LOAD]"></row-loader>

                    <div>
                        <pagination-wrapper
                            v-bind:paginationResult="sharedState.userFilmsPaginationResult"
                            v-bind:loadItemsF="loadUserFilms"
                            v-bind:showGoToButtons="true"
                        >
                            <!-- List view -->
                            <div v-if="privateState.currentView === 'list'" class="list-group user-items-list">
                                <a 
                                    v-for="(item) in userFilms"
                                    v-bind:key="`list-${item.id}`"
                                    href="#" 
                                    class="list-group-item list-group-item-action flex-column align-items-start user-item"
                                >
                                    <div class="d-flex w-100 justify-content-between">
                                        <h6 class="mb-1">
                                            <strong>{{item.title}}</strong>
                                        </h6>
                                        <div>
                                            <span class="badge badge-info mr-1">{{ item.languageCode }}</span>
                                            <!-- <span
                                                v-for="(tag) in item.tags"
                                                v-bind:key="tag"
                                                class="badge badge-secondary mr-1"
                                            >{{tag}}</span> -->
                                            <span class="ml-2 mr-2">|</span>
                                            <span>
                                                <span v-on:click="onUpdateUserFilm(item.id)" class="badge badge-secondary mr-1 cursor-pointer">
                                                    <i class="fas fa-pencil-alt"></i>
                                                </span>
                                                <span v-on:click="onDeleteUserFilm(item.id)" class="badge badge-secondary cursor-pointer">
                                                    <i class="fas fa-times"></i>
                                                </span>
                                            </span>
                                        </div>
                                    </div>
                                    <div class="small">
                                        <div v-if="item.myRating">
                                            <i class="fas fa-award text-danger mr-1"></i>
                                            <span class="mr-1">{{item.myRating}}</span>
                                            <!-- <small class="text-muted">(My rating)</small> -->
                                        </div>
                                        <div v-if="item.watchedAt">
                                            <i class="far fa-eye text-secondary mr-1"></i>
                                            <span class="mr-1">{{moment(item.watchedAt).format('YYYY-MM-DD')}}</span>
                                        </div>
                                        <div v-if="item.releaseYear">
                                            <i class="fas fa-calendar text-secondary mr-1"></i>
                                            <span class="mr-1">{{item.releaseYear}}</span>
                                        </div>
                                        <div v-if="item.genres && item.genres.length > 0">
                                            <i class="fas fa-film"></i>
                                            {{item.genres.join(', ')}}
                                        </div>
                                        <div v-if="item.comment">
                                            <i class="fas fa-comment text-secondary mr-1"></i>
                                            <span class="mr-1">{{item.comment}}</span>
                                        </div>
                                    </div>
                                </a>
                            </div>

                            <!-- Card view -->
                            <div v-if="privateState.currentView === 'cards'" class="items-card-list">
                                <div
                                    v-for="(item) in userFilms"
                                    v-bind:key="`card-${item.id}`"
                                    class="card bg-light item-card" 
                                >
                                    <!-- <div class="card-header"></div> -->
                                    <img v-if="item.details && item.details.image" class="card-img-top item-card-image" v-bind:src="item.details.image.posterUrl">
                                    <img v-else class="card-img-top item-card-image" src="/img/empty-image.png">
                                    <div class="card-body">
                                        <div class="d-flex w-100 justify-content-between align-items-center mb-1">
                                            <h6 class="card-title mb-0">
                                                <strong>{{item.title}}</strong>
                                            </h6>
                                        </div>
                                    
                                        <div class="card-text">
                                            <div class="small">
                                                <div v-if="item.myRating">
                                                    <i class="fas fa-award text-danger mr-1"></i>
                                                    <span class="mr-1">{{item.myRating}}</span>
                                                    <!-- <small class="text-muted">(My rating)</small> -->
                                                </div>
                                                <div v-if="item.watchedAt">
                                                    <i class="far fa-eye text-secondary mr-1"></i>
                                                    <span class="mr-1">{{moment(item.watchedAt).format('YYYY-MM-DD')}}</span>
                                                </div>
                                                <div v-if="item.releaseYear">
                                                    <i class="fas fa-calendar text-secondary mr-1"></i>
                                                    <span class="mr-1">{{item.releaseYear}}</span>
                                                </div>
                                                <div v-if="item.genres && item.genres.length > 0">
                                                    <i class="fas fa-film"></i>
                                                    {{item.genres.join(', ')}}
                                                </div>
                                                <div v-if="item.comment">
                                                    <i class="fas fa-comment text-secondary mr-1"></i>
                                                    <span class="mr-1">{{item.comment}}</span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="card-bottom-controls">
                                        <span v-on:click="onUpdateUserFilm(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-pencil-alt"></i>
                                        </span>
                                        <span v-on:click="onDeleteUserFilm(item.id)" class="card-bottom-control-item">
                                            <i class="fas fa-times"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </pagination-wrapper>
                    </div>
                </div>


                <!-- Study item create/edit -->
                <modal 
                    name="user-film-create-edit" 
                    height="auto"
                    width="450px"
                    v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                    v-bind:clickToClose="false"
                >
                    <div class="app-modal">
                        <div class="app-modal-header">
                            <div class="app-modal-title">
                                <span v-if="privateState.modalMode === 'create'">Create film</span>
                                <span v-if="privateState.modalMode === 'edit'">Edit film</span>
                            </div>
                            <div v-on:click="$modal.hide('user-film-create-edit')" class="app-modal-close">
                                <i class="fas fa-times"></i>
                            </div>
                        </div>
                        
                        <div class="app-modal-content">
                            <form v-on:submit.prevent="createEditUserFilm(privateState.modalMode)">
                                <div class="form-group">
                                    <label for="userFilmModel__title">Title</label>
                                    <input v-model="privateState.userFilmModel.title" type="text" class="form-control" id="userFilmModel__title" placeholder="Title" />
                                </div>
                                <div class="form-group">
                                    <label for="userFilmModel__myRating">My rating</label>
                                    <input v-model="privateState.userFilmModel.myRating" type="number" step="0.1" class="form-control" id="userFilmModel__myRating" placeholder="My rating" />
                                </div>
                                <div class="form-group">
                                    <label for="userFilmModel__watchedAt">Watched at</label>
                                    <datetime 
                                        v-model="privateState.userFilmModel.watchedAt"
                                        v-bind:type="'date'"
                                        v-bind:placeholder="'Watched at'"
                                        v-bind:input-class="'form-control app-datetime-input'"
                                        v-bind:value-zone="'UTC'"
                                        v-bind:zone="'local'"
                                        v-bind:format="'yyyy-MM-dd'"
                                        v-on:input="(nextValue) => {}"
                                    />
                                </div>
                                <div class="form-group">
                                    <label for="userFilmModel__releaseYear">Release year</label>
                                    <input v-model="privateState.userFilmModel.releaseYear" type="number" step="1" class="form-control" id="userFilmModel__releaseYear" placeholder="Release year" />
                                </div>
                                <div class="form-group">
                                    <label for="">Genres</label>
                                    <tags-multiselect 
                                        v-model="privateState.userFilmModel.genres"
                                        v-bind:placeholder="'Genres'"
                                    ></tags-multiselect>
                                </div>
                                <div class="form-group">
                                    <label for="userFilmModel__comment">Comment</label>
                                    <input v-model="privateState.userFilmModel.comment" type="text" class="form-control" id="userFilmModel__comment" placeholder="Comment" />
                                </div>
                                <div class="form-group">
                                    <label for="">Language <small class="text-muted">(of entered data, not a film)</small></label>
                                    <language-code-select
                                        v-model="privateState.userFilmModel.languageCode"
                                    />
                                </div>
                                <loading-button 
                                    type="submit"
                                    v-bind:loading="sharedState.loading[privateState.storeTypes.USER_FILM_CREATE] || sharedState.loading[privateState.storeTypes.USER_FILM_UPDATE]"
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
'use strict';

// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import _ from 'lodash';
import moment from 'moment';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import datetimeUtil from '@/utils/datetime';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';
import LanguageCodeSelect from '@/components/LanguageCodeSelect';
import TagsMultiselect from '@/components/TagsMultiselect';
import UserFilmsFilters from '@/components/UserFilmsFilters';

const userFilmModelDefault = {
    title: null,
    myRating: null,
    watchedAt: moment().utc().format(),
    releaseYear: null,
    genres: [],
    comment: null,
    languageCode: "en",
};

export default {
    name: 'user-films-browse',
    components: {
        RowLoader,
        LoadingButton,
        PaginationWrapper,
        LanguageCodeSelect,
        TagsMultiselect,
        UserFilmsFilters,
    },
    data: function() {
        return {
            moment,
            privateState: {
                storeTypes: storeTypes,
                currentView: 'list', // ['list', 'cards']
                userFilmModel: _.cloneDeep(userFilmModelDefault),
                modalMode: 'create', // ['create', 'edit']
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            userFilms: state => state.userFilmsPaginationResult ? state.userFilmsPaginationResult.items : null,
        }),
    },
    created: function() {
        this.loadUserFilms({});
    },
    mounted: function() {
    },
    updated: function() {
    },
    beforeDestroy: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadUserFilms: function({offset = 0, limit = 50} = {}) {
            return this.$store.dispatch(storeTypes.USER_FILMS_LOAD, {
                offset: offset, 
                limit: limit, 
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        toggleView: function() {
            this.privateState.currentView = this.privateState.currentView === 'list' ? 'cards' : 'list';
        },
        onCreateUserFilm: function() {
            this.privateState.modalMode = 'create';

            // reset
            this.privateState.userFilmModel = _.cloneDeep(userFilmModelDefault);
            
            this.$modal.show('user-film-create-edit');
        },
        onUpdateUserFilm: function(userFilmId) {
            this.privateState.modalMode = 'edit';
            let userFilm = this.userFilms.find(x => x.id === userFilmId);
            this.privateState.userFilmModel = {id: userFilm.id, ...userFilm};
            this.$modal.show('user-film-create-edit');
        },
        onDeleteUserFilm: function(userFilmId) {
            if(confirm('Are you sure?')) {
                this.deleteUserFilm(userFilmId);
            }
        },
        createEditUserFilm: function(mode) {
            if(mode === 'create') {
                this.createUserFilm();
            } else if(mode === 'edit') {
                this.updateUserFilm();
            }
        },
        createUserFilm: function() {
            this.$store.dispatch(storeTypes.USER_FILM_CREATE, {
                data: {
                    ...this.privateState.userFilmModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Film '${this.privateState.userFilmModel.title}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('user-film-create-edit');

                // reset
                this.privateState.userFilmModel = _.cloneDeep(userFilmModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        updateUserFilm: function() {
            this.$store.dispatch(storeTypes.USER_FILM_UPDATE, {
                userFilmId: this.privateState.userFilmModel.id,
                data: {
                    ...this.privateState.userFilmModel,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Film '${this.privateState.userFilmModel.title}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('user-film-create-edit');

                // reset
                this.privateState.userFilmModel = _.cloneDeep(userFilmModelDefault);
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteUserFilm: function(userFilmId) {
            this.$store.dispatch(storeTypes.USER_FILM_DELETE, {
                userFilmId: userFilmId,
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Film has been deleted!`,
                    text: '',
                    duration: 5000,
                });

                const itemCountThresholdBeforeReload = 3;
                if(this.userFilms.length <= itemCountThresholdBeforeReload) {
                    // reload
                    this.loadUserFilms();
                }
            }).catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>
