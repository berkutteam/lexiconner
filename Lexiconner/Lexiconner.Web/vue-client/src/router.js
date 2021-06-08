"use strict";

import Vue from "vue";
import Router from "vue-router";
import store from "@/store";
import { roleNames } from "@/constants/index";
import miscUtils from "@/utils/misc";
import authService from "@/services/authService";

import Home from "./views/Home.vue";
import ErrorView from "./views/ErrorView.vue";
import Login from "./views/Login.vue";
import Register from "./views/Register.vue";
import Logout from "./views/Logout.vue";

import UserDictionaryDashboard from "./views/UserDictionary/UserDictionaryDashboard.vue";

import WordsTrainingDashboard from "./views/Words/WordsTrainingDashboard.vue";
import WordsBrowse from "./views/Words/WordsBrowse.vue";
import WordsLearnFlashCards from "./views/Words/WordsLearnFlashCards.vue";
import WordsLearnWordMeaning from "./views/Words/WordsLearnWordMeaning.vue";
import WordsLearnMeaningWord from "./views/Words/WordsLearnMeaningWord.vue";
import WordsLearnMatchWords from "./views/Words/WordsLearnMatchWords.vue";
import WordsLearnBuildWords from "./views/Words/WordsLearnBuildWords.vue";
import WordsLearnListenWords from "./views/Words/WordsLearnListenWords.vue";

import WordSetsDashboard from "./views/WordSets/WordSetsDashboard.vue";
import WordSetCreate from "./views/WordSets/WordSetCreate.vue";

import UserFilmsBrowse from "./views/UserFilms/UserFilmsBrowse.vue";

import Dashboard from "./views/Dashboard.vue";
import UserProfile from "./views/UserProfile.vue";

Vue.use(Router);

/**
 * Maps route params and query params to props
 * @param {*} route
 */
function mapRouteParamsToProps(route) {
  return {
    ...route.params,
    query: {
      ...route.query,
    },
  };
}

/**
 * Waits before processing a route if there are some store actions loading.
 * The aim - to prevent a user from being redirected from protected route due to yet not loaded auth info
 * when page was refreshed.
 * NB: do not guarantee that all the required data will be loaded until wait time end.
 */
async function waitAppInitializationAsync({ to, from, next }) {
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
  next();
}

function checkAuthenticated({ to, from, next }) {
  if (store.state.auth.isAuthenticated) {
    next();
  } else {
    console.error(
      `Access denied to route ${to.name}: ${to.path}. Unauthenticated.`
    );

    Vue.notify({
      group: "error",
      type: "error",
      title: "Access denied!",
      text: "Looks like you are not logged in, or session expired. Try to relogin.",
    });

    // next(false); // abort the current navigation
    next({
      name: "home", // back to safety route //
      query: {},
    });
  }
}

async function checkRolesAsync({ to, from, next, roles }) {
  const userRoles = await authService.getUserRolesAsync();
  console.log(`checkRoles. checking required roles`, roles, `in`, userRoles);
  if (
    !roles ||
    roles.length === 0 ||
    roles.every((role) => userRoles.includes(role))
  ) {
    next();
  } else {
    console.error(
      `Access denied to route ${to.name}: ${to.path}. Unauthorized - insufficient permissions.`
    );

    Vue.notify({
      group: "error",
      type: "error",
      title: "Access denied for the page!",
      text: `Insufficient permissions to proceed to the page ${to.path}`,
    });

    next(false); // abort the current navigation
  }
}

/** Applies N guards sequentially. Guard might be sync or async. */
async function guardPipelineAsync({ to, from, next, guards }) {
  let totalSuccess = true;
  let nextMockParams = null;
  const nextMock = (successOrRoute) => {
    nextMockParams = successOrRoute === false ? false : successOrRoute;
  };

  for (const guard of guards) {
    await guard(to, from, nextMock);
    totalSuccess = totalSuccess && nextMockParams !== false;

    // if one of the guards terminates navigation then there is no need to proceed other guards
    if (nextMockParams === false) {
      break;
    }
  }

  if (totalSuccess === true) {
    next();
  } else {
    next(nextMockParams === false ? false : nextMockParams);
  }
}

export default new Router({
  // on local serve browser urls work bad
  ...(process.env.VUE_APP_ASPNETCORE_ENVIRONMENT === "DevelopmentLocalhost"
    ? {}
    : {
        mode: "history",
      }),
  routes: [
    // home is unauthenticated landing home
    {
      path: "/",
      name: "home",
      component: Home,
      props: true,
      meta: { layout: "home" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });

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
    // authenticated home
    {
      path: "/dashboard",
      name: "dashboard",
      component: Dashboard,
      props: true,
      meta: { layout: "default" },
    },
    {
      path: "/error",
      name: "error",
      component: ErrorView,
      props: true,
      meta: { layout: "default" },
    },
    {
      path: "/user-dictionary",
      name: "user-dictionary",
      component: UserDictionaryDashboard,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/user-dictionary/wordset/:userWordSetId/words",
      name: "user-dictionary-wordset-words",
      component: WordsBrowse,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/trainings-dashboard",
      name: "trainings-dashboard",
      component: WordsTrainingDashboard,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/browse",
      name: "words-browse",
      component: WordsBrowse,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/falshcards",
      name: "words-learn-falshcards",
      component: WordsLearnFlashCards,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/wordmeaning",
      name: "words-learn-wordmeaning",
      component: WordsLearnWordMeaning,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/meaningword",
      name: "words-learn-meaningword",
      component: WordsLearnMeaningWord,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/matchwords",
      name: "words-learn-matchwords",
      component: WordsLearnMatchWords,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/buildword",
      name: "words-learn-buildwords",
      component: WordsLearnBuildWords,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/words/learn/listenwords",
      name: "words-learn-listenwords",
      component: WordsLearnListenWords,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/wordsets",
      name: "wordsets",
      component: WordSetsDashboard,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/wordsets/create",
      name: "wordset-create-create",
      component: WordSetCreate,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await guardPipelineAsync({
          to,
          from,
          next,
          guards: [
            async (to, from, next) =>
              await waitAppInitializationAsync({ to, from, next }),
            (to, from, next) => checkAuthenticated({ to, from, next }),
            async (to, from, next) =>
              await checkRolesAsync({
                to,
                from,
                next,
                roles: [roleNames.rootAdmin],
              }),
          ],
        });
      },
    },
    {
      path: "/wordsets/:wordSetId/edit",
      name: "wordset-create-update",
      component: WordSetCreate,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await guardPipelineAsync({
          to,
          from,
          next,
          guards: [
            async (to, from, next) =>
              await waitAppInitializationAsync({ to, from, next }),
            (to, from, next) => checkAuthenticated({ to, from, next }),
            async (to, from, next) =>
              await checkRolesAsync({
                to,
                from,
                next,
                roles: [roleNames.rootAdmin],
              }),
          ],
        });
      },
    },
    {
      path: "/user-films/browse",
      name: "user-films-browse",
      component: UserFilmsBrowse,
      props: true,
      meta: { layout: "default" },
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
    {
      path: "/login",
      name: "login",
      component: Login,
      props: true,
    },
    {
      path: "/register", //'?invitationId=&preRegistrationUserId='
      name: "register",
      component: Register,
      props: mapRouteParamsToProps,
    },
    {
      path: "/logout",
      name: "logout",
      component: Logout,
      props: true,
    },
    {
      path: "/profile",
      name: "user-profile",
      component: UserProfile,
      props: true,
      beforeEnter: async (to, from, next) => {
        await waitAppInitializationAsync({ to, from, next });
        checkAuthenticated({ to, from, next });
      },
    },
  ],
});
