import Vue from "vue";
import App from "./App.vue";
import router from "@/router";
import store from "@/store";
import api from "@/utils/api";
import authService from "@/services/authService";

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
console.log(
  "process.env.VUE_APP_IDENTITY_CLIENT_ID: ",
  process.env.VUE_APP_IDENTITY_CLIENT_ID
);

// init
authService.init({
  clientId: process.env.VUE_APP_IDENTITY_CLIENT_ID,
});
api.init({
  identityUrl: process.env.VUE_APP_IDENTITY_URL,
  apiUrl: process.env.VUE_APP_API_URL,
});

runApp();

async function runApp() {
  if (await authService.checkIsAuthenticatedAsync()) {
    // load profile before rendering the app
    // await store.dispatch(storeTypes.PROFILE_LOAD, {});
  } else {
    // redirect to login
    router.push({
      path: "/login",
    });
  }

  renderApp();
}

function renderApp() {
  /* eslint-disable no-new */
  new Vue({
    router,

    // provide the store using the "store" option.
    // this will inject the store instance to all child components.
    store,

    el: "#app",
    render: (h) => h(App),
  });
}
