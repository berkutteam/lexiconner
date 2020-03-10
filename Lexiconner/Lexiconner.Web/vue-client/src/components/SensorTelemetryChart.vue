
<!-- 
Do not include the <template> tag to your .vue files. Vue can not merge templates. 
If you add an empty <template> tag, Vue will take the template from your component and not from the extended one, 
which will result in an empty template and errors.
-->
<script>
'use strict';

import { Line, mixins } from 'vue-chartjs';
const { reactiveProp } = mixins;

export default {
    extends: Line,
    // name: 'sensor-telementry-chart', // ignores name for some reason
    mixins: [reactiveProp],
    props: {
        chartData: {
            type: Object,
            default: null
        },
        options: {
            type: Object,
            default: null
        }
    },
    data: () => ({
        // chartdata: {
        //     labels: ['January', 'February'],
        //     datasets: [
        //     {
        //         label: 'Data One',
        //         backgroundColor: '#f87979',
        //         data: [40, 20]
        //     }
        //     ]
        // },
        // options: {
        //     responsive: true,
        //     maintainAspectRatio: false
        // }
    }),

    mounted () {
        this.renderChart(this.chartData, this.options)
    },
}
</script>

<style>
</style>
