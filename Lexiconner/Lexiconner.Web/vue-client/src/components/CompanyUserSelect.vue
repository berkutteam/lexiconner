<template>
    <multiselect 
        v-model="privateState.selectedUserOption" 
        v-bind:options="companyUserList" 
        v-bind:multiple="false" 
        v-bind:searchable="true" 
        v-bind:close-on-select="true" 
        v-bind:clear-on-select="false" 
        v-bind:preserve-search="true" 
        v-bind:show-labels="true" 
        v-bind:allow-empty="true" 
        v-bind:preselect-first="false"
        label="name" 
        v-bind:custom-label="userLabel"
        track-by="id" 
        placeholder="Select user" 
        v-bind:loading="isLoading"
        v-bind:disabled="false"
        v-on:input="onInput"
        v-on:search-change="onSearchChange"
    >
    </multiselect>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import _ from 'lodash';

export default {
    name: 'company-user-select',
    props: {
        /**
         * Value is userId that is passed as v-model="<userIdProp>"
         * v-model does this:
         *      v-bind:value="<userIdProp>"
         *      v-on:input="<userIdProp> = $event"
         */
        value: {
            // type: [String, Object],
            required: true,
            default: null,
        },
        companyId: {
            type: String,
            required: true,
            default: null,
        },
        // TODO - select only users in department
        departmentId: {
            type: String,
            required: false,
            default: null,
        },
        /**
         * Departments which users will be excluded from select
         */
        excludedDepartmentIds: {
            type: Array,
            required: false,
            default: () => [],
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
                selectedUserOption: null,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => state.loading[storeTypes.COMPANY_USERS_LOAD],
            companyUserList: function(state) {
                return (state.companyUsers[this.companyId] || {}).data || [];
            },
        }),
    },
    created: async function() {
        let self = this;

        // update model on state change
        self.$store.subscribe((mutation, state) => {
            let {type, payload} = mutation;
            if(type === storeTypes.COMPANY_USER_SET) {
                if(!self.privateState.selectedUserOption && self.value) {
                    let companyUser = state.companyUser[self.companyId][self.value];
                    if(companyUser) {
                        self.privateState.selectedUserOption = companyUser;
                    }
                }
            }
        });

        // load passed user to make it's option selected
        if(self.value) {
            self.$store.dispatch(storeTypes.COMPANY_USER_LOAD, {
                companyId: self.companyId,
                userId: self.value,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        // load users
        if(self.companyId /*&& this.companyUserList.length === 0*/) {
            self.$store.dispatch(storeTypes.COMPANY_USERS_LOAD, {
                companyId: self.companyId,
                offset: 0, 
                limit: 20,
                search: null,
                excludedDepartmentIds: self.excludedDepartmentIds,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        userLabel(option) {
            return `${option.name || 'No name'} (${option.email})`;
        },
        onInput: function(value, id) {
            // tell parent that value was changed and it can update its v-model property
            // value is user
            this.$emit('input', value.id);
        },
        onSearchChange: function(query, id) {
            this.loadCompanyUsers({
                search: query,
            });
        },
        loadCompanyUsers: _.debounce(function({search = null}) {
            // load users
            this.$store.dispatch(storeTypes.COMPANY_USERS_LOAD, {
                companyId: this.companyId,
                offset: 0, 
                limit: 20,
                search: search,
                excludedDepartmentIds: this.excludedDepartmentIds,
            }).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }, 1500),
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
