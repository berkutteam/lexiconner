<template>
  <div class="home-page-wrapper">
    <p>Welcome to Lexiconner browser extension!</p>
    <p>Start by <router-link to="/login">loggin in</router-link> into your account. Or <router-link to="/register">create</router-link> a new account.</p>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from "@/components/HelloWorld.vue";

export default {
  name: "Home",
  components: {
    // HelloWorld,
  },
  mounted: function() {
    console.log(1, window.location)
  }
};
</script>
