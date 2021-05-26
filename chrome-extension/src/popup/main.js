import Vue from "vue";
import App from "./App.vue";
import router from "../router";
import store from "../store";

/* eslint-disable no-new */
new Vue({
  router,

  // provide the store using the "store" option.
  // this will inject the store instance to all child components.
  store,

  el: "#app",
  render: (h) => h(App),
});
