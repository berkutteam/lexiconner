<template>
  <div>
   <h5>Login</h5>
   <form v-on:submit.prevent="onLoginSubmit()">
    <div class="form-group">
        <label for="exampleInputEmail1">Email</label>
        <input v-model="privateState.loginModel.email" type="email" class="form-control" id="exampleInputEmail1" aria-describedby="emailHelp" placeholder="Email">
    </div>
    <div class="form-group">
        <label for="exampleInputPassword1">Password</label>
        <input v-model="privateState.loginModel.password" type="password" class="form-control" id="exampleInputPassword1" placeholder="Password">
    </div>
    <div class="form-group">
        <small class="form-text text-muted">
            By loggin in you are giving the Lexiconner extension a limited access to your profile. For instance, it can read/write your dictionary, retrieve basic profile info. However, this info never shared with someone else.
        </small>
    </div>
    <button 
      type="submit" 
      class="btn btn-primary"
      v-bind:disabled="sharedState.loading[privateState.storeTypes.LOGIN_REQUEST]"
    >Login</button>
    </form>
  </div>
</template>

<script>
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';

export default {
  name: "login",
  components: {
  },
  data: function() {
      return {
          privateState: {
              storeTypes: storeTypes,
              loginModel: {
                email: '',
                password: '',
                extensionVersion: '',
              },
          },
      };
  },
  computed: {
      // local computed go here

      // store state computed go here
      ...mapState({
          sharedState: state => state,
      }),

      // store getter
      ...mapGetters([
      ]),
  },
  mounted: function() {
  },
  updated: function() {
  },
  beforeDestroy: function() {
  },
  destroyed: function() {
  },
  methods: {
    onLoginSubmit: function() {

    },
  },
};
</script>

<style scoped lang="scss">
</style>
