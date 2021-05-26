'use strict';

import Vue from "vue";
import Vuex from "vuex";
import _ from 'lodash';
import { storeTypes } from '@/constants/index';
import api from '@/utils/api';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    // one loading object for all resources
    loading: {
      // <storeType>: bool
    },

    loginResult: null,
  },
  getters:  {
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
  },
  mutations: {
    [storeTypes.LOADING_SET](state, payload) {
      let { target, loading } = payload;
      state.loading = {
          ...loading,
          [target]: loading,
      };
    },

    [storeTypes.LOGIN_SET](state, payload) {
      let { data } = payload;
      state.loginResult = data;
    },
  },
  actions: {
     //#region Login

     [storeTypes.LOGIN_REQUEST](context, { data }) {
      let { commit, dispatch, getters, state } = context;
      commit(storeTypes.LOADING_SET, {
          target: storeTypes.LOGIN_REQUEST,
          loading: true,
      });
      return api.identity().login({ data }).then(({data, ok}) => {
          commit(storeTypes.LOADING_SET, {
              target: storeTypes.LOGIN_REQUEST,
              loading: false,
          });
          commit(storeTypes.LOGIN_SET, {
            data: data
          });
          return data;
      }).catch(err => {
          commit(storeTypes.LOADING_SET, {
              target: storeTypes.LOGIN_REQUEST,
              loading: false,
          });
          throw err;
      });
    },

    //#endregion
  },
  modules: {},
});
