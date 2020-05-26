<template>
  <div class="company-create-wrapper">
      <div class="row">
        <div class="col-12 col-md-4">
            <h5>Create new company</h5>
            <form v-on:submit.prevent="onSubmit">
                <div class="form-group">
                    <label for="inputName">Name</label>
                    <input v-model="privateState.model.name" type="text" class="form-control" id="inputName" aria-describedby="nameHelp" placeholder="Name">
                </div>
                <loading-button 
                    type="submit"
                    v-bind:loading="sharedState.loading[privateState.storeTypes.COMPANY_CREATE]"
                    class="btn btn-outline-success btn-block"
                >Create</loading-button>
            </form>
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

export default {
    name: 'company-create',
     components: {
        LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                model: {
                    name: '',
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
        // // load enums
        // if(!this.sharedState.enums) {
        //     this.$store.dispatch(storeTypes.ENUMS_LOAD, {}).then().catch(err => {
        //         console.error(err);
        //         notification.showErrorIfServerErrorResponseOrDefaultError(err);
        //     });
        // }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },
    methods: {
        onSubmit(e) {
            let company = {
                ...this.privateState.model,
            };

            this.$store.dispatch(storeTypes.COMPANY_CREATE, {
                company: company,
            }).then((company) => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Company '${company.name}' has beed created!`,
                    text: '',
                });

                // reload info
                this.$store.dispatch(storeTypes.USER_INFO_LOAD, {});

                // reload user permissions
                this.$store.dispatch(storeTypes.AUTH_MY_PERMISSIONS_LOAD, {});

                authService.refreshTokens({withFullscreenLoader: true});

                this.$router.push({name: 'dashboard'});
            }).catch((err) => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
    },
}
</script>

<style lang="scss">
</style>

