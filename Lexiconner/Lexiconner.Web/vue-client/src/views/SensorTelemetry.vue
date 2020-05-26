<template>
    <div id="sensorTelemetry">
        <row-loader v-bind:visible="sharedState.loading[privateState.storeTypes.SENSOR_TELEMETRY_LOAD]"></row-loader>
        <div 
            v-if="sharedState.sensorTelemetries"
            v-bind:class="{'block-disabled': sharedState.loading[privateState.storeTypes.SENSOR_TELEMETRY_LOAD]}"
        >
            <div class="mb-5">
                <div class="row justify-content-between">
                    <div class="col-3">
                        <div class="d-flex">
                            <div>
                                <datetime 
                                    v-model="privateState.dateFrom"
                                    v-bind:type="'date'"
                                    v-bind:placeholder="'Date from'"
                                    v-bind:input-class="'form-control app-datetime-input'"
                                    v-bind:value-zone="'UTC'"
                                    v-bind:zone="'local'"
                                    v-bind:format="'yyyy-MM-dd'"
                                    v-on:input="onDateFromChange"
                                />
                            </div>
                            <div class="ml-1">
                                <datetime 
                                    v-model="privateState.dateTo"
                                    v-bind:type="'date'"
                                    v-bind:placeholder="'Date to'"
                                    v-bind:input-class="'form-control app-datetime-input'"
                                    v-bind:value-zone="'UTC'"
                                    v-bind:zone="'local'"
                                    v-bind:format="'yyyy-MM-dd'"
                                    v-on:input="onDateToChange"
                                />
                            </div>
                        </div>
                    </div>
                    <div class="col-3 no-print">
                        <div class="d-flex justify-content-end">
                            <button v-on:click="exportPDF" type="button" class="btn btn-sm btn-outline-secondary"><i class="fas fa-file-pdf mr-1"></i>Export PDF</button>
                            <button v-on:click="exportCSV" type="button" class="btn btn-sm btn-outline-secondary ml-1"><i class="fas fa-file-csv mr-1"></i>Export CSV</button>
                            <!-- <button v-on:click="exportPrint" type="button" class="btn btn-sm btn-outline-secondary ml-1"><i class="fas fa-print mr-1"></i>Print</button> -->
                            <loading-button 
                                type="button"
                                v-bind:loading="privateState.isExportPrintLoading"
                                class="btn btn-sm btn-outline-secondary ml-1"
                                v-on:click.native="exportPrint"
                            >
                                <i class="fas fa-print mr-1"></i>Print
                            </loading-button>
                        </div>
                    </div>
                </div>
                <sensor-telementry-chart 
                    v-bind:chartData="telemetryChartData"
                    v-bind:options="{responsive: true, maintainAspectRatio: false}"
                />
            </div>

            <table class="table table-sm table-hover">
                <thead class="thead-dark">
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">IsLimitExceeded</th>
                        <th scope="col">CreatedAt</th>
                        <th scope="col">Channel</th>
                        <th scope="col">Value</th>
                        <th scope="col">Metric</th>
                        <th scope="col">Isforced</th>
                        <th scope="col">Comment</th>
                    </tr>
                </thead>
                <tbody>
                    <tr 
                        v-for="(item) in sharedState.sensorTelemetries.data"
                        v-bind:key="item.id"
                        v-bind:class="{'table-success': !item.isLimitExceeded, 'table-danger': item.isLimitExceeded}"
                    >
                        <th scope="row">{{ item.id }}</th>
                        <td>{{ item.isLimitExceeded }}</td>
                        <td>{{ item.createdAt }}</td>
                        <td>{{ item.channel }}</td>
                        <td>{{ item.value }}</td>
                        <td>{{ item.metric }}</td>
                        <td>{{ item.isforced }}</td>
                        <td>{{ item.comment }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
'use strict';

import { mapState, mapGetters } from 'vuex';
import moment from 'moment';
import printJS from 'print-js';
import { storeTypes } from '@/constants/index';
import authService from '@/services/authService';
import notificationUtil from '@/utils/notification';
import datetimeUtil from '@/utils/datetime';
import RowLoader from '@/components/loaders/RowLoader';
import LoadingButton from '@/components/LoadingButton';
import SensorTelemetryChart from '@/components/SensorTelemetryChart';

const dateFromDefault = moment().utcOffset(0).subtract(1, 'days').startOf('day').format();
const dateToDefault = moment().utcOffset(0).endOf('day').format();

export default {
    name: 'sensor-telemetry',
    components: {
        RowLoader,
        LoadingButton,
        'sensor-telementry-chart': SensorTelemetryChart,
    },
    data: function() {
        return {
            privateState: {
                storeTypes: storeTypes,
                dateFrom: dateFromDefault,
                dateTo: dateToDefault,
                isExportPrintLoading: false,
            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state,
        }),

        telemetryChartData: function() {
            let labels = this.sharedState.sensorTelemetries.data.map(x => {
                return datetimeUtil.formatAccordingToInterval(datetimeUtil.formatInUserTimeZone(x.createdAt), this.privateState.dateFrom, this.privateState.dateTo);
            });
            return {
                labels: [...labels], // x axis labels
                datasets: [
                    {
                        label: 'Temperature (Celsius)', // channel
                        backgroundColor: '#13B1E8',
                        borderColor: '#13B1E8',
                        data: this.sharedState.sensorTelemetries.data.map(x => {
                            return parseFloat(x.value.toFixed(2));
                        }),
                        fill: false,
                    },
                ],
            };
        },
    },
    created: async function() {
        this.loadData();
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
        this.$store.commit(storeTypes.SENSOR_TELEMETRY_SET, {
            sensorTelemetries: null,
        });
    },
    methods: {
        loadData: function() {
            this.$store.dispatch(storeTypes.SENSOR_TELEMETRY_LOAD, {
                companyId: this.$store.getters.currentCompanyId,
                sensorId: this.$route.params.sensorId,
                offset: 0, 
                limit: 1000, 
                dateFrom: this.privateState.dateFrom, 
                dateTo: this.privateState.dateTo,
            }).then().catch(err => {
                console.error(err);
                notificationUtil.showErrorIfServerErrorResponseOrDefaultError(err);
            });
        },
        onDateFromChange: function(nextValue) {
            this.loadData();
        },
        onDateToChange: function(nextValue) {
            this.loadData();
        },
        exportPDF: function() {
            alert('Will be available soon.');
        },
        exportCSV: function() {
            alert('Will be available soon.');
        },
        exportPrint: function() {
            // alert('Will be available soon.');
            this.privateState.isExportPrintLoading = true;

            // hack: use timeout to avoid blocking for loading button
            setTimeout(() => {
                // scanStyles blocks styles from css, style. Looks like scanStyles applies inline styles to element
                let printStyle = ''; // content of <style></style> tag
                let printCSSUrls = ['css/print.css'];
                printJS({
                    printable: 'sensorTelemetry', 
                    type: 'html',
                    header: null,
                    maxWidth: 800,
                    css: printCSSUrls,
                    style: printStyle,
                    scanStyles: true,
                    targetStyles: ['*'],
                    onLoadingStart: () => {
                        this.privateState.isExportPrintLoading = true;
                    },
                    onLoadingEnd: () => {
                        this.privateState.isExportPrintLoading = false;
                    },
                });
            }, 10);
        },
    },
}
</script>

<style lang="scss">
</style>
