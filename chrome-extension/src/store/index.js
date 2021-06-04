"use strict";

import Vue from "vue";
import Vuex from "vuex";
import _ from "lodash";
import { storeTypes } from "@/constants/index";
import api from "@/utils/api";
import { updateLocale } from "moment";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    // one loading object for all resources
    loading: {
      // <storeType>: bool
    },

    auth: {
      isAuthenticated: false,
      user: null,
      loginResult: null,
    },

    profile: null,

    // list of language DTO
    languages: null,

    wordMeanings: null,
    createdWord: null,
  },
  getters: {
    /**
     * Checks if there are any resources are loading
     */
    isAnyLoading(state, getters) {
      let isAnyLoading = Object.keys(state.loading).reduce((accum, key, i) => {
        return accum || state.loading[key];
      }, false);
      // console.log('isAnyLoading', isAnyLoading)
      return isAnyLoading;
    },
    selectedLearningLanguageCode(state, getters) {
      if (!state.profile) {
        return null;
      }
      return (
        (
          state.profile.learningLanguages.find(
            (x) => x.isSelectedForBrowserExtension
          ) || {}
        ).languageCode || null
      );
    },
  },
  mutations: {
    [storeTypes.LOADING_SET](state, payload) {
      let { target, loading } = payload;
      state.loading = {
        ...loading,
        [target]: loading,
      };
    },

    //#region Auth

    [storeTypes.AUTH_USER_SET](state, payload) {
      let { user } = payload;
      state.auth.isAuthenticated = !!user;
      state.auth.user = { ...user };
    },
    [storeTypes.AUTH_USER_RESET](state) {
      state.auth.isAuthenticated = false;
      state.auth.user = null;
    },
    [storeTypes.AUTH_LOGIN_SET](state, payload) {
      let { loginResult } = payload;
      state.auth.loginResult = loginResult;
    },

    //#endregion

    //#region Profile

    [storeTypes.PROFILE_SET](state, payload) {
      let { profile } = payload;
      state.profile = profile;
    },

    //#endregion

    //#region Reference info

    [storeTypes.LANGUAGES_SET](state, payload) {
      let { data } = payload;
      state.languages = data;
    },

    //#endregion

    //#region Words

    [storeTypes.WORD_MEANINGS_SET](state, payload) {
      let { wordMeanings } = payload;
      state.wordMeanings = wordMeanings;
    },
    [storeTypes.WORD_CREATE_SET](state, payload) {
      let { word } = payload;
      state.createdWord = word;
    },

    //#endregion
  },
  actions: {
    //#region Auth

    [storeTypes.AUTH_LOGIN_REQUEST](context, { data }) {
      let { commit, dispatch, getters, state } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.AUTH_LOGIN_REQUEST,
        loading: true,
      });
      return api
        .identity()
        .login({ ...data })
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.AUTH_LOGIN_REQUEST,
            loading: false,
          });
          commit(storeTypes.AUTH_LOGIN_SET, {
            loginResult: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.AUTH_LOGIN_REQUEST,
            loading: false,
          });
          throw err;
        });
    },

    //#endregion

    //#region Profile

    [storeTypes.PROFILE_LOAD](context) {
      let { commit, dispatch, state, getters } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.PROFILE_LOAD,
        loading: true,
      });
      return api
        .webApi()
        .getProfile()
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.PROFILE_LOAD,
            loading: false,
          });
          commit(storeTypes.PROFILE_SET, {
            profile: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.PROFILE_LOAD,
            loading: false,
          });
          throw err;
        });
    },
    [storeTypes.PROFILE_SELECT_LEARNING_LANGUAGE](context, { languageCode }) {
      let { commit, dispatch, state, getters } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.PROFILE_SELECT_LEARNING_LANGUAGE,
        loading: true,
      });
      return api
        .webApi()
        .selectProfileLearningLanguage({
          languageCode,
        })
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.PROFILE_SELECT_LEARNING_LANGUAGE,
            loading: false,
          });
          commit(storeTypes.PROFILE_SET, {
            profile: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.PROFILE_SELECT_LEARNING_LANGUAGE,
            loading: false,
          });
          throw err;
        });
    },

    //#endregion

    //#region Reference info

    [storeTypes.LANGUAGES_LOAD](context) {
      let { commit, dispatch, getters } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.LANGUAGES_LOAD,
        loading: true,
      });
      return api
        .webApi()
        .getLanguages()
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.LANGUAGES_LOAD,
            loading: false,
          });
          commit(storeTypes.LANGUAGES_SET, {
            data: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.LANGUAGES_LOAD,
            loading: false,
          });
          throw err;
        });
    },

    //#endregion

    //#region Words

    [storeTypes.WORD_MEANINGS_LOAD](
      context,
      { word, wordLanguageCode, meaningLanguageCode }
    ) {
      let { commit, dispatch, getters } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.WORD_MEANINGS_LOAD,
        loading: true,
      });
      return api
        .webApi()
        .getWordMeanings({ word, wordLanguageCode, meaningLanguageCode })
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.WORD_MEANINGS_LOAD,
            loading: false,
          });
          commit(storeTypes.WORD_MEANINGS_SET, {
            wordMeanings: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.WORD_MEANINGS_LOAD,
            loading: false,
          });
          throw err;
        });
    },
    [storeTypes.WORD_CREATE](context, { data }) {
      let { commit, dispatch, getters } = context;
      commit(storeTypes.LOADING_SET, {
        target: storeTypes.WORD_CREATE,
        loading: true,
      });
      return api
        .webApi()
        .createWord({ data })
        .then(({ data, ok }) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.WORD_CREATE,
            loading: false,
          });
          commit(storeTypes.WORD_CREATE_SET, {
            word: data,
          });
          return data;
        })
        .catch((err) => {
          commit(storeTypes.LOADING_SET, {
            target: storeTypes.WORD_CREATE,
            loading: false,
          });
          throw err;
        });
    },

    //#endregion
  },
  modules: {},
});
