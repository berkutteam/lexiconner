<template>
    <div class="company-manage-wrapper">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.COMPANY_LOAD] || sharedState.loading[privateState.storeTypes.COMPANY_UPDATE]"></row-loader>
        <div>
            <div class="row">
                <div class="col col-md-5">
                    <h5 class="mt-0">Manage company</h5>
                    <div v-if="sharedState.company">
                        <div class="card mb-3">
                            <div class="card-body">
                                <form v-on:submit.prevent="updateCompany">
                                    <div class="form-group">
                                        <label for="inputName">Name</label>
                                        <input v-model="privateState.companyModel.name" type="text" class="form-control" id="inputName" aria-describedby="nameHelp" placeholder="Name">
                                    </div>
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_UPDATE]"
                                        class="btn btn-outline-success"
                                    >Save</loading-button>
                                </form>
                            </div>
                        </div>

                        <div class="card border-danger text-danger">
                            <div class="card-body">
                                <loading-button 
                                    type="button"
                                    v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_DELETE]"
                                    class="btn-danger"
                                    v-on:click.native="deleteCompany()"
                                >Delete</loading-button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
'use strict';

import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import utils from '@/utils/index';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';

const companyModelDefault = {
    name: '',
};

export default {
    name: 'company-manage',
    components: {
        RowLoader,
        LoadingButton,
    },
    props: {
        companyId: {
            type: String,
            required: true,
            default: null,
        },
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                companyModel: {
                    ...companyModelDefault,
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            // measurementUnits: state => state.enums ? state.enums['measurementUnit'] || [] : [],
        }),
    },
    created: async function() {
        let self = this;

        // // load enums
        // if(!this.sharedState.enums) {
        //     this.$store.dispatch(storeTypes.ENUMS_LOAD, {}).then().catch(err => {
        //         console.error(err);
        //         notification.showErrorIfServerErrorResponseOrDefaultError(err);
        //     });
        // }

        // load company
        if(!self.sharedState.company || self.sharedState.company.id !== self.companyId) {
            self.$store.dispatch(storeTypes.COMPANY_LOAD, {companyId: self.companyId}).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        // set initial data
        self.privateState.companyModel = {
            ...companyModelDefault,
            ...(self.sharedState.company || {}),
        };

        // update model on state change
        self.$store.subscribe((mutation, state) => {
            let {type, payload} = mutation;
            if(type === storeTypes.COMPANY_SET) {
                self.privateState.companyModel = {
                    ...self.privateState.companyModel,
                    ...state.company,
                };
            }
        });
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },
    methods: {
        updateCompany() {
            this.$store.dispatch(storeTypes.COMPANY_UPDATE, {
                companyId: this.companyId,
                data: {
                    name: this.privateState.companyModel.name,
                },
            }).then((company) => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Company '${this.privateState.companyModel.name}' has beed updated!`,
                    text: '',
                });

                // reload info
                this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});
            }).catch((err) => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteCompany() {
            if(window.confirm('Are you sure?')) {
                this.$store.dispatch(storeTypes.COMPANY_DELETE, {
                    companyId: this.companyId,
                }).then((company) => {
                    this.$notify({
                        group: 'app',
                        type: 'information',
                        title: `Company '${this.privateState.companyModel.name}' has beed deleted!`,
                        text: '',
                    });

                    // reload info
                    this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});

                    // refresh tokens
                    authService.refreshTokens({withFullscreenLoader: false});

                    this.$router.push({name: 'dashboard-home'});
                }).catch((err) => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },
    },
}
</script>

<style lang="scss">
</style>

