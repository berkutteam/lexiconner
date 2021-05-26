import Vue from "vue";
import App from "./App.vue";
import router from "../router";
import store from "../store";
import api from "@/utils/api";

// log envs
console.log("process.env: ", process.env);
console.log(
  "process.env.VUE_APP_ASPNETCORE_ENVIRONMENT: ",
  process.env.VUE_APP_ASPNETCORE_ENVIRONMENT
);
console.log(
  "process.env.VUE_APP_IDENTITY_URL: ",
  process.env.VUE_APP_IDENTITY_URL
);
console.log("process.env.VUE_APP_API_URL: ", process.env.VUE_APP_API_URL);

// init
// authService.init(clientAuth);
api.init({
  identityUrl: process.env.VUE_APP_IDENTITY_URL,
  apiUrl: process.env.VUE_APP_API_URL,
});

/* eslint-disable no-new */
new Vue({
  router,

  // provide the store using the "store" option.
  // this will inject the store instance to all child components.
  store,

  el: "#app",
  render: (h) => h(App),
});
