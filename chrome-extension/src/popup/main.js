import Vue from "vue";
import Multiselect from "vue-multiselect";
import Notifications from "vue-notification";

import App from "./App.vue";
import router from "@/router";
import store from "@/store";
import apiUtil from "@/utils/api";
import authService, { authEvents } from "@/services/authService";
import { storeTypes } from "@/constants/index";

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

// register globally
Vue.component("multiselect", Multiselect);
Vue.use(Notifications);

// init
authService.init({
  clientId: process.env.VUE_APP_IDENTITY_CLIENT_ID,
});
apiUtil.init({
  identityUrl: process.env.VUE_APP_IDENTITY_URL,
  apiUrl: process.env.VUE_APP_API_URL,
});

runApp();

async function runApp() {
  // listen to auth events
  authService.on(
    authEvents.isAuthenticatedChanged,
    async ({ isAuthenticated, user }) => {
      console.log(`isAuthenticatedChanged:`, isAuthenticated, user);

      if (isAuthenticated) {
        // store user in store
        store.commit(storeTypes.AUTH_USER_SET, {
          user,
        });
      } else {
        // logout
        store.commit(storeTypes.AUTH_USER_RESET);
        await authService.logoutAsync();
        router.push({
          name: "home",
        });
      }
    }
  );

  if (await authService.checkIsAuthenticatedAsync()) {
    // load profile before rendering the app
    await store.dispatch(storeTypes.PROFILE_LOAD);
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
