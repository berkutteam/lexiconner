<!-- Time only input built on datetime -->
<template>
  <!-- Event if type is time, component stores it as datetime -->
  <datetime
    v-model="privateState.selectedDateTime"
    v-bind:type="'time'"
    v-bind:placeholder="'To'"
    v-bind:input-class="'form-control app-datetime-input'"
    v-bind:value-zone="'UTC'"
    v-bind:zone="'local'"
    v-bind:format="'HH:mm'"
    v-on:input="onChange"
  />
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from "vuex";
import moment from "moment";
import { storeTypes } from "@/constants/index";
import authService from "@/services/authService";
import notification from "@/utils/notification";
import RowLoader from "@/components/loaders/RowLoader";
import LoadingButton from "@/components/LoadingButton";

export default {
  name: "time-input",
  props: {
    /**
     * Value is time in format HH:mm (e.g. 13:09). C# TimeSpan serializes as HH:mm:ss.
     * Value is passed as v-model="<timeProp>"
     * v-model does this:
     *      v-bind:value="<timeProp>"
     *      v-on:input="<timeProp> = $event"
     */
    value: {
      type: String,
      required: true,
      default: null,
    },
  },
  components: {
    // RowLoader,
    // LoadingButton,
  },
  data: function () {
    return {
      privateState: {
        storeTypes: storeTypes,
        selectedDateTime: null,
      },
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: (state) => state,
    }),
  },
  created: function () {
    if (this.value) {
      let parts = this.value.split(":");
      let hours = parseInt(parts[0]);
      let minutes = parseInt(parts[1]);
      let seconds = parseInt(parts[2]);

      // use local time, so datetime can display time without conversion
      this.privateState.selectedDateTime = moment()
        .set("hour", hours)
        .set("minute", minutes)
        .set("second", seconds)
        .format();
      console.log(parts, this.privateState.selectedDateTime);
    }
  },
  mounted: function () {},
  updated: function () {},
  destroyed: function () {},

  methods: {
    onChange: function (value, id) {
      // tell parent that calue was changed and it can update its v-model property
      let datetime = value;

      // use local time, so datetime can display time without conversion
      let time = moment(datetime).format("HH:mm");
      this.$emit("input", time);
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss"></style>
