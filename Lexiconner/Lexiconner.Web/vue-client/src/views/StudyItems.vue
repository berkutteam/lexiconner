<template>
    <div class="my-permissions-wrapper">
        <div class="row">
            <div class="col-12">
                <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.STUDY_ITEMS_LOAD]"></row-loader>

                <div v-if="studyItems">
                    <h5 class="mb-3">Study items:</h5>
                    <div>
                        <pagination-wrapper
                            v-bind:paginationResult="sharedState.studyItemsPaginationResult"
                            v-bind:loadItemsF="loadStudyItems"
                        >
                            <!-- Card view -->
                            <div class="d-flex flex-row justify-content-start align-items-start flex-wrap">
                                <div 
                                    v-for="(item) in studyItems"
                                    v-bind:key="item.id"
                                    class="card bg-light mr-md-2 mr-0 mb-2" 
                                    style="width: 18%;"
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
                                            <i v-if="item.isFavourite" class="fas fa-star text-warning mr-1"></i>
                                            <i v-else class="far fa-star text-warning mr-1"></i>

                                            <span class="badge badge-info mr-1">{{ item.languageCode }}</span>
                                            <span
                                                v-for="(tag) in item.tags"
                                                v-bind:key="tag"
                                                class="badge badge-secondary"
                                            >{{tag}}</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </pagination-wrapper>
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
import datetimeUtil from '@/utils/datetime';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import PaginationWrapper from '@/components/PaginationWrapper';

export default {
    name: 'study-items',
    components: {
        RowLoader,
        // LoadingButton,
        PaginationWrapper,
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
            studyItems: state => state.studyItemsPaginationResult ? state.studyItemsPaginationResult.items : null,
        }),
    },
    created: async function() {
        this.loadStudyItems({offset: 0, limit: 100});
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        loadStudyItems: function({offset = 0, limit = 100} = {}) {
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
    },
}
</script>
