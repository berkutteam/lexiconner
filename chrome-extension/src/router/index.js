import Vue from "vue";
import VueRouter from "vue-router";
import store from "@/store";
import miscUtils from "@/utils/misc";

import Home from "../views/Home.vue";
import Login from "../views/Login.vue";
import Dashboard from "../views/Dashboard.vue";

Vue.use(VueRouter);

/**
 * Waits before processing a route if there are some store actions loading.
 * The aim - to prevent a user from being redirected from protected route due to yet not loaded auth info
 * when page was refreshed.
 * NB: do not guarantee that all the required data will be loaded until wait time end.
 */
async function waitAppInitialization({ to, from, next }) {
  const waitRetries = 5;
  const waitTimeStepFactor = 0.5;
  let waitTimeMs = 500;

  if (store.getters.isAnyLoading) {
    // wait a little bit and then process the route
    for (let i = 0; i < waitRetries; i++) {
      console.log(
        `Waiting app initialization before processing to route ${to.name}: ${to.path}. Wait time ms: ${waitTimeMs}`
      );
      await miscUtils.waitAsync(waitTimeMs);

      if (!store.getters.isAnyLoading) {
        break;
      }

      // continue
      waitTimeMs = waitTimeMs + waitTimeMs * waitTimeStepFactor;
    }
  }

  console.log(
    `App was initialized. Processing to route ${to.name}: ${to.path}.`
  );
}

const routes = [
  // home is unauthenticated initial page
  {
    path: "/",
    name: "home",
    component: Home,
    props: true,
    beforeEnter: async (to, from, next) => {
      await waitAppInitialization({ to, from, next });

      if (store.state.auth.isAuthenticated) {
        next({
          name: "dashboard",
          query: {},
        });
      } else {
        next();
      }
    },
  },
  {
    path: "/login",
    name: "login",
    component: Login,
    props: true,
  },
  {
    path: "/register",
    name: "register",
    component: Login,
    props: true,
  },
  {
    path: "/about",
    name: "about",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/About.vue"),
    props: true,
  },
  {
    path: "/dashboard",
    name: "dashboard",
    component: Dashboard,
  },
];

const router = new VueRouter({
  routes,
});

export default router;
