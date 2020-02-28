<template>
  <div class="user-profile-wrapper company-user-profile-wrapper">
    <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.SENSOR_LOAD]" class="mb-3"></row-loader>
    <div>
        <!-- Sensor -->
        <div class="row mb-5">
            <div class="col-12 col-md-5">
                <h5 class="mt-0">Sensor</h5>
                <div v-if="sensor" v-bind:class="{'block-disabled': sharedState.loading[privateState.storeTypes.SENSOR_LOAD]}">
                    <div class="card">
                        <div class="card-body">
                            <form v-on:submit.prevent="updateSensor">
                                <div>
                                    <div class="form-group">
                                        <label for=""><strong>#id</strong></label>
                                        <div>{{ sensor.id }}</div>
                                    </div>
                                    <div class="form-group">
                                        <label for="sensorModel__name"><strong>Name</strong></label>
                                        <input v-model="privateState.sensorModel.name" id="sensorModel__name" type="text" class="form-control">
                                    </div>
                                    <div class="form-group form-check">
                                        <input v-model="privateState.sensorModel.isIgnoranceIntervalEnabled" id="sensorModel__isIgnoranceIntervalEnabled" type="checkbox" class="form-check-input">
                                        <label class="form-check-label" for="sensorModel__isIgnoranceIntervalEnabled">Enable ignorance interval</label>
                                        <div><small>Ingorance interval - the interval when all sensor telemetry is ignored.</small></div>
                                    </div>
                                    <div v-if="privateState.sensorModel.isIgnoranceIntervalEnabled" class="form-group">
                                        <div class="row">
                                            <div class="col">
                                                <div class="form-group">
                                                    <label for="sensorModel__ignoranceIntervalFrom"><strong>From</strong></label>
                                                    <time-input
                                                        v-model="privateState.sensorModel.ignoranceIntervalFrom"
                                                    />
                                                </div>
                                            </div>
                                            <div class="col">
                                                <div class="form-group">
                                                    <label for="sensorModel__ignoranceIntervalTo"><strong>To</strong></label>
                                                    <time-input
                                                        v-model="privateState.sensorModel.ignoranceIntervalTo"
                                                    />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group form-check">
                                        <input v-model="privateState.sensorModel.notificationSettings.isAlarmEnabled" id="sensorModel__notificationSettings__isAlarmEnabled" type="checkbox" class="form-check-input">
                                        <label class="form-check-label" for="sensorModel__notificationSettings__isAlarmEnabled">Enable alrams</label>
                                        <div><small>If alarms are disabled you won't get notified about emergencies.</small></div>
                                    </div>
                                </div>
                                <loading-button 
                                    type="submit"
                                    v-bind:loading="sharedState.loading[privateState.storeTypes.SENSOR_UPDATE]"
                                    class="btn btn-outline-success"
                                >Save</loading-button>
                            </form>
                    </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Calibrations -->
        <div class="row mb-5">
            <div class="col-12 col-md-12">
                <h5 class="mt-0">Calibrations</h5>
                <div v-if="sensor && sensor.calibrations">
                    <div class="mb-3">
                        <button 
                            v-on:click="onCreateCalibration" type="button" class="btn btn-outline-success">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                    <div v-bind:class="{'block-disabled': isCalibrationLoading}">
                        <table class="table table-hover">
                            <thead class="">
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Name</th>
                                    <th scope="col">Channel</th>
                                    <th scope="col">Metrics</th>
                                    <th scope="col">OriginalValue</th>
                                    <th scope="col">Calibrated Value</th>
                                    <th scope="col"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr 
                                    v-for="(item) in sensor.calibrations"
                                    v-bind:key="item.id"
                                    v-bind:class="{}"
                                >
                                    <th scope="row">
                                        {{ item.id }}
                                    </th>
                                    <td>
                                        {{ item.name }}
                                    </td>
                                    <td>
                                        {{ item.channel }}
                                    </td>
                                     <td>
                                        {{ item.metrics }}
                                    </td>
                                    <td>
                                        {{ item.originalValue }}
                                    </td>
                                    <td>
                                        {{ item.calibratedValue }}
                                    </td>
                                    <td>
                                        <button 
                                            class="btn btn-outline-secondary btn-sm"
                                            v-on:click="onEditCalibration(item.id)"
                                        >
                                            <i class="far fa-edit"></i>
                                        </button>
                                        <button 
                                            class="btn btn-outline-danger btn-sm ml-1"
                                            v-on:click="deleteCalibration(item.id)"
                                        >
                                            <i class="fas fa-minus"></i>
                                        </button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <modal 
                        name="calibration-create-edit" 
                        height="auto"
                        width="450px"
                        v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                        v-bind:clickToClose="false"
                    >
                        <div class="app-modal">
                            <div class="app-modal-header">
                                <div class="app-modal-title">
                                    <span v-if="!privateState.calibrationCreateModel.isEdit">Create</span>
                                    <span v-if="privateState.calibrationCreateModel.isEdit">Edit</span>
                                    calibration
                                </div>
                                <div v-on:click="$modal.hide('calibration-create-edit')" class="app-modal-close">
                                    <i class="fas fa-times"></i>
                                </div>
                            </div>
                            
                            <div class="app-modal-content">
                                <form v-on:submit.prevent="privateState.calibrationCreateModel.isEdit ? editCalibration() : createCalibration()">
                                    <div class="form-group">
                                        <label for="calibrationCreateModel__name"><strong>Name</strong></label>
                                        <input v-model="privateState.calibrationCreateModel.name" type="text" class="form-control" id="calibrationCreateModel__name" placeholder="Name" />
                                        <!-- <small id="" class="form-text text-muted">Name must be unique.</small> -->
                                    </div>
                                    <div class="form-group">
                                        <label for="calibrationCreateModel__channel"><strong>Channel</strong></label>
                                        <select v-model="privateState.calibrationCreateModel.channel" class="form-control" id="calibrationCreateModel__channel">
                                            <option
                                                v-for="(item) in telemetryChannels" 
                                                v-bind:key="item" 
                                                v-bind:value="item"
                                                v-bind:selected="item === privateState.calibrationCreateModel.channel"
                                            >
                                                {{ item }}
                                            </option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <label for="calibrationCreateModel__metrics"><strong>Measurement unit</strong></label>
                                        <select v-model="privateState.calibrationCreateModel.metrics" class="form-control" id="calibrationCreateModel__metrics">
                                            <option
                                                v-for="(item) in measurementUnits" 
                                                v-bind:key="item" 
                                                v-bind:value="item"
                                                v-bind:selected="item === privateState.calibrationCreateModel.metrics"
                                            >
                                                {{ item }}
                                            </option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <label for="calibrationCreateModel__originalValue"><strong>Original Value</strong></label>
                                        <input v-model="privateState.calibrationCreateModel.originalValue" type="number" step="any" required class="form-control" id="calibrationCreateModel__originalValue" placeholder="Original" />
                                    </div>
                                    <div class="form-group">
                                        <label for="calibrationCreateModel__calibratedValue"><strong>Calibrated Value</strong></label>
                                        <input v-model="privateState.calibrationCreateModel.calibratedValue" type="number" step="any" required class="form-control" id="calibrationCreateModel__calibratedValue" placeholder="Calibrated" />
                                    </div>
                                    
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.SENSOR_CALIBRATION_CREATE] || sharedState.loading[privateState.storeTypes.SENSOR_CALIBRATION_UPDATE]"
                                        class="btn btn-outline-success btn-block"
                                    >
                                        <span v-if="!privateState.calibrationCreateModel.isEdit">Create</span>
                                        <span v-if="privateState.calibrationCreateModel.isEdit">Save</span>
                                    </loading-button>
                                </form>
                            </div>
                        </div>
                    </modal>
                </div>
            </div>
        </div>

        <!-- Telemetry rules -->
        <div class="row mb-5">
            <div class="col">
                <h5 class="mt-0">Telemetry rules</h5>
                <div v-if="sensor && sensor.telemetryRules">
                    <div class="mb-3">
                        <button 
                            v-on:click="onCreateTelemetryRule" type="button" class="btn btn-outline-success">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                    <div v-bind:class="{'block-disabled': isTelemetryRuleLoading}">
                        <table class="table table-hover">
                            <thead class="">
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Name</th>
                                    <th scope="col">Channel</th>
                                    <th scope="col">Metrics</th>
                                    <th scope="col">Alarm Enabled</th>
                                    <th scope="col">Min Allowed Value</th>
                                    <th scope="col">Max Allowed Value</th>
                                    <th scope="col"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr 
                                    v-for="(item) in sensor.telemetryRules"
                                    v-bind:key="item.id"
                                    v-bind:class="{}"
                                >
                                    <th scope="row">
                                        {{ item.id }}
                                    </th>
                                    <td>
                                        {{ item.name }}
                                    </td>
                                     <td>
                                        {{ item.channel }}
                                    </td>
                                     <td>
                                        {{ item.metrics }}
                                    </td>
                                    <td>
                                        {{ item.isAlarmEnabled }}
                                    </td>
                                    <td>
                                        {{ item.minAllowedValue }}
                                    </td>
                                    <td>
                                        {{ item.maxAllowedValue }}
                                    </td>
                                    <td>
                                        <button 
                                            class="btn btn-outline-secondary btn-sm"
                                            v-on:click="onEditTelemetryRule(item.id)"
                                        >
                                            <i class="far fa-edit"></i>
                                        </button>
                                        <button 
                                            class="btn btn-outline-danger btn-sm ml-1"
                                            v-on:click="deleteTelemetryRule(item.id)"
                                        >
                                            <i class="fas fa-minus"></i>
                                        </button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <modal 
                        name="telemetry-rule-create-edit" 
                        height="auto"
                        width="450px"
                        v-bind:classes="['v--modal', 'v--modal-box', 'v--modal-box--overflow-visible', 'v--modal-box--sm-fullwidth']"
                        v-bind:clickToClose="false"
                    >
                        <div class="app-modal">
                            <div class="app-modal-header">
                                <div class="app-modal-title">
                                    <span v-if="!privateState.telemetryRuleCreateModel.isEdit">Create</span>
                                    <span v-if="privateState.telemetryRuleCreateModel.isEdit">Edit</span>
                                    telemetry rule
                                </div>
                                <div v-on:click="$modal.hide('telemetry-rule-create-edit')" class="app-modal-close">
                                    <i class="fas fa-times"></i>
                                </div>
                            </div>
                            
                            <div class="app-modal-content">
                                <form v-on:submit.prevent="privateState.telemetryRuleCreateModel.isEdit ? editTelemetryRule() : createTelemetryRule()">
                                    <div class="form-group">
                                        <label for="telemetryRuleCreateModel__name"><strong>Name</strong></label>
                                        <input v-model="privateState.telemetryRuleCreateModel.name" type="text" class="form-control" id="telemetryRuleCreateModel__name" placeholder="Name" />
                                        <small id="" class="form-text text-muted">Name must be unique.</small>
                                    </div>
                                    <div class="form-group">
                                        <label for="telemetryRuleCreateModel__channel"><strong>Channel</strong></label>
                                        <select v-model="privateState.telemetryRuleCreateModel.channel" class="form-control" id="telemetryRuleCreateModel__channel">
                                            <!-- <option value="">None</option> -->
                                            <option
                                                v-for="(item) in telemetryChannels" 
                                                v-bind:key="item" 
                                                v-bind:value="item"
                                                v-bind:selected="item === privateState.telemetryRuleCreateModel.channel"
                                            >
                                                {{ item }}
                                            </option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <label for="telemetryRuleCreateModel__metrics"><strong>Measurement unit</strong></label>
                                        <select v-model="privateState.telemetryRuleCreateModel.metrics" class="form-control" id="telemetryRuleCreateModel__metrics">
                                            <!-- <option value="">None</option> -->
                                            <option
                                                v-for="(item) in measurementUnits" 
                                                v-bind:key="item" 
                                                v-bind:value="item"
                                                v-bind:selected="item === privateState.telemetryRuleCreateModel.metrics"
                                            >
                                                {{ item }}
                                            </option>
                                        </select>
                                    </div>
                                    <div class="form-group form-check">
                                        <input v-model="privateState.telemetryRuleCreateModel.isAlarmEnabled" id="telemetryRuleCreateModel__isAlarmEnabled" type="checkbox" class="form-check-input">
                                        <label class="form-check-label" for="telemetryRuleCreateModel__isAlarmEnabled">Enable alarms</label>
                                    </div>
                                    <div class="form-group">
                                        <label for="telemetryRuleCreateModel__minAllowedValue"><strong>Min Allowed Value</strong></label>
                                        <input v-model="privateState.telemetryRuleCreateModel.minAllowedValue" type="number" step="any" required class="form-control" id="telemetryRuleCreateModel__minAllowedValue" placeholder="Min" />
                                    </div>
                                    <div class="form-group">
                                        <label for="telemetryRuleCreateModel__maxAllowedValue"><strong>Max Allowed Value</strong></label>
                                        <input v-model="privateState.telemetryRuleCreateModel.maxAllowedValue" type="number" step="any" required class="form-control" id="telemetryRuleCreateModel__maxAllowedValue" placeholder="Max" />
                                    </div>
                                    
                                    <loading-button 
                                        type="submit"
                                        v-bind:loading="sharedState.loading[privateState.storeTypes.SENSOR_TELEMETRY_RULE_CREATE] || sharedState.loading[privateState.storeTypes.SENSOR_TELEMETRY_RULE_UPDATE]"
                                        class="btn btn-outline-success btn-block"
                                    >
                                        <span v-if="!privateState.telemetryRuleCreateModel.isEdit">Create</span>
                                        <span v-if="privateState.telemetryRuleCreateModel.isEdit">Save</span>
                                    </loading-button>
                                </form>
                            </div>
                        </div>
                    </modal>
                </div>
            </div>
        </div>
    </div>
  </div>
</template>

<script>
'use strict';

import { mapState, mapGetters } from 'vuex';
import moment from 'moment';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notification from '@/utils/notification';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import TimeInput from '@/components/TimeInput';

const sensorModelDefault = {
    name: null,
    isIgnoranceIntervalEnabled: false,
    ignoranceIntervalFrom: null,
    ignoranceIntervalTo: null,
    notificationSettings: {
        isAlarmEnabled: false,
    },
};

const calibrationCreateModelDefault = {
    isEdit: false,
    id: null,
    originalValue: 0,
    calibratedValue: 0,
};

const telemetryRuleCreateModelDefault = {
    isEdit: false,
    id: null,
    isAlarmEnabled: true,
    minAllowedValue: 0,
    maxAllowedValue: 0,
};

export default {
    name: 'sensor-manage',
    components: {
        RowLoader,
        LoadingButton,
        TimeInput,
    },
    props: {
        // route props:
        companyId: String,
        sensorId: String,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                sensorModel: {
                    ...sensorModelDefault,
                },
                calibrationCreateModel: {
                    ...calibrationCreateModelDefault,
                },
                telemetryRuleCreateModel: {
                    ...telemetryRuleCreateModelDefault,
                },
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
            isCalibrationLoading: state => {
                return state.loading[storeTypes.SENSOR_LOAD] || 
                       state.loading[storeTypes.SENSOR_CALIBRATION_CREATE] ||
                       state.loading[storeTypes.SENSOR_CALIBRATION_UPDATE] ||
                       state.loading[storeTypes.SENSOR_CALIBRATION_DELETE];
            },
            isTelemetryRuleLoading: state => {
                return state.loading[storeTypes.SENSOR_LOAD] || 
                       state.loading[storeTypes.SENSOR_TELEMETRY_RULE_CREATE] ||
                       state.loading[storeTypes.SENSOR_TELEMETRY_RULE_UPDATE] ||
                       state.loading[storeTypes.SENSOR_TELEMETRY_RULE_DELETE];
            },
            sensor: function(state) {
                if(state.sensor[this.companyId] && state.sensor[this.companyId][this.sensorId]) {
                    return state.sensor[this.companyId][this.sensorId];
                }
                return null;
            },
            telemetryChannels: state => state.enums ? state.enums['telemetryChannel'] || [] : [],
            measurementUnits: state => state.enums ? state.enums['measurementUnit'] || [] : [],
        })
    },
    created: async function() {
        // load sensor
        this.$store.dispatch(storeTypes.SENSOR_LOAD, {
            companyId: this.companyId,
            sensorId: this.sensorId,
        }).then().catch(err => {
            console.error(err);
            notification.showErrorIfServerErrorResponseOrDefaultError(err);
        });

        // load enums
        if(!this.sharedState.enums) {
            this.$store.dispatch(storeTypes.ENUMS_LOAD, {}).then().catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        }

        // set initial data
        this.privateState.sensorModel = {
            ...sensorModelDefault,
            ...(this.sensor || {}),
        };

        // update model on state change
        this.$store.subscribe((mutation, state) => {
            let {type, payload} = mutation;
            if(type === storeTypes.SENSOR_SET) {
                this.privateState.sensorModel = {
                    ...this.privateState.sensorModelDefault,
                    ...(this.sensor || {}),
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
        updateSensor: function() {
            this.$store.dispatch(storeTypes.SENSOR_UPDATE, {
                companyId: this.companyId,
                sensorId: this.sensorId,
                data: {
                    name: this.privateState.sensorModel.name,
                    isIgnoranceIntervalEnabled: this.privateState.sensorModel.isIgnoranceIntervalEnabled,
                    ignoranceIntervalFrom: this.privateState.sensorModel.ignoranceIntervalFrom,
                    ignoranceIntervalTo: this.privateState.sensorModel.ignoranceIntervalTo,
                    notificationSettings: {
                        isAlarmEnabled: this.privateState.sensorModel.notificationSettings.isAlarmEnabled,
                    },
                },
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },

        onCreateCalibration: function() {
            // reset
            this.privateState.calibrationCreateModel = {
                ...calibrationCreateModelDefault,
            };

            this.$modal.show('calibration-create-edit');
        },
        onEditCalibration: function(calibrationId) {
             this.privateState.calibrationCreateModel = {
                 isEdit: true,
                 id: calibrationId,
                ...this.sensor.calibrations.find(x => x.id === calibrationId),
             };

            this.$modal.show('calibration-create-edit');
        },
        createCalibration: function() {
            this.$store.dispatch(storeTypes.SENSOR_CALIBRATION_CREATE, {
                companyId: this.companyId,
                sensorId: this.sensorId,
                data: {
                    name: this.privateState.calibrationCreateModel.name,
                    channel: this.privateState.calibrationCreateModel.channel,
                    metrics: this.privateState.calibrationCreateModel.metrics,
                    originalValue: this.privateState.calibrationCreateModel.originalValue,
                    calibratedValue: this.privateState.calibrationCreateModel.calibratedValue,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Calibration has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('calibration-create-edit');

                // reset
                this.privateState.calibrationCreateModel = {
                    ...calibrationCreateModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        editCalibration: function() {
            this.$store.dispatch(storeTypes.SENSOR_CALIBRATION_UPDATE, {
                companyId: this.companyId,
                sensorId: this.sensorId,
                calibrationId: this.privateState.calibrationCreateModel.id,
                data: {
                    name: this.privateState.calibrationCreateModel.name,
                    channel: this.privateState.calibrationCreateModel.channel,
                    metrics: this.privateState.calibrationCreateModel.metrics,
                    originalValue: this.privateState.calibrationCreateModel.originalValue,
                    calibratedValue: this.privateState.calibrationCreateModel.calibratedValue,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Calibration has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('calibration-create-edit');

                // reset
                this.privateState.calibrationCreateModel = {
                    ...calibrationCreateModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteCalibration: function(calibrationId) {
            if(window.confirm('Are you sure?')) {
                this.$store.dispatch(storeTypes.SENSOR_CALIBRATION_DELETE, {
                    companyId: this.companyId,
                    sensorId: this.sensorId,
                    calibrationId: calibrationId,
                }).then(() => {
                    this.$notify({
                        group: 'app',
                        type: 'information',
                        title: `Calibration has been deleted!`,
                        text: '',
                        duration: 5000,
                    });
                }).catch(err => {
                    console.error(err);
                    notification.showErrorIfServerErrorResponseOrDefaultError(err);
                });
            }
        },

        onCreateTelemetryRule: function() {
            // reset
            this.privateState.telemetryRuleCreateModel = {
                ...telemetryRuleCreateModelDefault,
            };

            this.$modal.show('telemetry-rule-create-edit');
        },
        onEditTelemetryRule: function(telemetryRuleId) {
             this.privateState.telemetryRuleCreateModel = {
                 isEdit: true,
                 id: telemetryRuleId,
                ...this.sensor.telemetryRules.find(x => x.id === telemetryRuleId),
             };

            this.$modal.show('telemetry-rule-create-edit');
        },
        createTelemetryRule: function() {
            this.$store.dispatch(storeTypes.SENSOR_TELEMETRY_RULE_CREATE, {
                companyId: this.companyId,
                sensorId: this.sensorId,
                data: {
                    name: this.privateState.telemetryRuleCreateModel.name,
                    channel: this.privateState.telemetryRuleCreateModel.channel,
                    metrics: this.privateState.telemetryRuleCreateModel.metrics,
                    isAlarmEnabled: this.privateState.telemetryRuleCreateModel.isAlarmEnabled,
                    minAllowedValue: this.privateState.telemetryRuleCreateModel.minAllowedValue,
                    maxAllowedValue: this.privateState.telemetryRuleCreateModel.maxAllowedValue,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Telemetry rule '${this.privateState.telemetryRuleCreateModel.name}' has been created!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('telemetry-rule-create-edit');

                // reset
                this.privateState.telemetryRuleCreateModel = {
                    ...telemetryRuleCreateModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        editTelemetryRule: function() {
            this.$store.dispatch(storeTypes.SENSOR_TELEMETRY_RULE_UPDATE, {
                companyId: this.companyId,
                sensorId: this.sensorId,
                telemetryRuleId: this.privateState.telemetryRuleCreateModel.id,
                data: {
                    name: this.privateState.telemetryRuleCreateModel.name,
                    channel: this.privateState.telemetryRuleCreateModel.channel,
                    metrics: this.privateState.telemetryRuleCreateModel.metrics,
                    isAlarmEnabled: this.privateState.telemetryRuleCreateModel.isAlarmEnabled,
                    minAllowedValue: this.privateState.telemetryRuleCreateModel.minAllowedValue,
                    maxAllowedValue: this.privateState.telemetryRuleCreateModel.maxAllowedValue,
                },
            }).then(() => {
                 this.$notify({
                    group: 'app',
                    type: 'success',
                    title: `Telemetry rule '${this.privateState.telemetryRuleCreateModel.name}' has been updated!`,
                    text: '',
                    duration: 5000,
                });

                this.$modal.hide('telemetry-rule-create-edit');

                // reset
                this.privateState.telemetryRuleCreateModel = {
                    ...telemetryRuleCreateModelDefault,
                };
            }).catch(err => {
                console.error(err);
                notification.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        deleteTelemetryRule: function(telemetryRuleId) {
            if(window.confirm('Are you sure?')) {
                this.$store.dispatch(storeTypes.SENSOR_TELEMETRY_RULE_DELETE, {
                    companyId: this.companyId,
                    sensorId: this.sensorId,
                    telemetryRuleId: telemetryRuleId,
                }).then(() => {
                    this.$notify({
                        group: 'app',
                        type: 'information',
                        title: `Telemetry rule has been deleted!`,
                        text: '',
                        duration: 5000,
                    });
                }).catch(err => {
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
