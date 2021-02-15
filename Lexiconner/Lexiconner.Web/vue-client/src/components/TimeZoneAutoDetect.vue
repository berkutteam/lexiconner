<template>
     <div>
        <modal 
            name="user-timezone-confirm" 
            height="auto"
            width="400px"
            v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
            v-bind:clickToClose="false"
            v-bind:scrollable="true"
        >
            <div class="app-modal">
                <div class="app-modal-header">
                    <div class="app-modal-title">Confirm your timezone</div>
                    <div v-on:click="$modal.hide('user-timezone-confirm')" class="app-modal-close">
                        <i class="fas fa-times"></i>
                    </div>
                </div>
                
                <div class="app-modal-content">
                    <form ref="confirmForm" v-on:submit.prevent="">
                        <div class="form-group">
                            <h6>Your timezone is {{timezoneDetected}}?</h6>
                        </div>
                        <loading-button 
                            type="button"
                            v-bind:loading="isLoading"
                            class="btn btn-outline-success mr-1"
                            name="isConfirmed"
                            value="yes"
                            v-on:click.native="confirmUserTimeZone(true)"
                        >Yes</loading-button>
                        <loading-button 
                            type="button"
                            v-bind:loading="false"
                            class="btn btn-outline-secondary"
                            name="isConfirmed"
                            value="no"
                            v-on:click.native="confirmUserTimeZone(false)"
                        >No</loading-button>
                    </form>
                </div>
            </div>
        </modal>
     </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import moment from 'moment-timezone';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import LoadingButton from '@/components/LoadingButton';

/**
 * Auto detects user's timezone if it isn't set
 * and show confirm modal after some period of time
 */
export default {
    name: 'time-zone-auto-detect',
    props: {
    },
    components: {
        LoadingButton,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                showModalAfterMs: 5 * 1000, // 5 sec, when to show timezone confirm modal
            },
        };
    },
    computed: {
        // local computed go here
        timezoneDetected: function() {
            var timezoneId = moment.tz.guess();
            return timezoneId;
        },

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isLoading: state => state.loading[storeTypes.USER_INFO_TIMEZONE_SET],
        }),
    },
    created: function() {
        this.$store.watch(
            (state, getters) => {
                return state.userInfo;
            },
            (nextValue, prevValue) => {
                if(nextValue && !this.isUserTimeZoneSet(nextValue)) {
                    console.log('Auto detected timezone: ', this.timezoneDetected);

                    if(this.timezoneDetected) {
                        // modal timeout
                        setTimeout(() => {
                            this.showModal();
                        }, this.privateState.showModalAfterMs);
                    }
                    
                }
            }
        );
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
        isUserTimeZoneSet: function(userInfo) {
            return userInfo.timeZone && userInfo.timeZone.timeZoneId;
        },
        showModal: function() {
            this.$modal.show('user-timezone-confirm');
        },
        confirmUserTimeZone: function(isConfirmed) {
            if(isConfirmed) {
                this.$store.dispatch(storeTypes.USER_INFO_TIMEZONE_SET, {
                    timeZoneId: this.timezoneDetected,
                }).then(() => {
                    this.$notify({
                        group: 'app',
                        type: 'success',
                        title: `Timezone is set to ${this.timezoneDetected}.`,
                        text: '',
                        duration: 5000,
                    });

                    this.$modal.hide('user-timezone-confirm');
                }).catch(err => {
                    console.error(err);
                    notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            } else {
                this.$modal.hide('user-timezone-confirm');

                this.$notify({
                    group: 'app-important',
                    type: 'warn',
                    title: `Note:`,
                    text: 'Go to settings and set your timezone to see date and time in your timezone. Until then, UTC will be used.',
                    duration: 10000,
                });
            }
        },
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
