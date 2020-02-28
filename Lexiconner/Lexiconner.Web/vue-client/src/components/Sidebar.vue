<template>
  <div id="sidebar" class="app-sidebar no-print">
    <!-- Hide all links if not completed registration, so user complete it first -->
    <div v-if="sharedState.auth.isAuthenticated">
      <div class="sidebar-header" v-if="sharedState.auth.isAuthenticated">
        <router-link v-bind:to="{ name: 'user-profile'}" class="sidebar-user">
          <div class="d-flex justify-content-start align-items-start">
            <div class="mr-3">
              <img
                class="user-image"
                src="img/user.png"
                v-bind:alt="sharedState.auth.user.profile.name || sharedState.auth.user.profile.given_name"
              />
            </div>
            <div>
              <div class="mb-0">
                <small>{{sharedState.auth.user.profile.name || sharedState.auth.user.profile.given_name}}</small>
              </div>
              <div>
                <small>{{sharedState.auth.user.profile.email}}</small>
              </div>
            </div>
          </div>
        </router-link>
      </div>

      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'study-items'}" class="sidebar-link">Study items</router-link>
      </div>

      <!-- <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'dashboard-home'}" class="sidebar-link">Dashboard</router-link>
      </div>

      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'my-permissions'}" class="sidebar-link">Permissions</router-link>
      </div>
      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'my-company-invitations'}" class="sidebar-link">Invitations</router-link>
      </div> -->
      
      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'logout'}" class="sidebar-link">Logout</router-link>
      </div>
    </div>
    <div v-if="!sharedState.auth.isAuthenticated">
      <div v-if="!sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'home'}" class="sidebar-link">Home</router-link>
      </div>
      <div v-if="!sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'login'}" class="sidebar-link">Login</router-link>
      </div>
      <!-- <div v-if="!sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'register'}" class="sidebar-link">Register</router-link>
      </div> -->
    </div>
  </div>
</template>


<script>
"use strict";

import { mapState, mapGetters } from "vuex";
import authService from "@/services/authService";

let Sidebar = {
  name: "sidebar",
  components: {},
  data: function() {
    return {
      privateState: {
      }
    };
  },
  methods: {
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: state => state,
      // isReortsSelected: state => {
      //   console.log(this.$router.currentRoute);
      //   return false;
      // },
      currentRouteName() {
        console.log(this.$route);
        return this.$route.name;
      }
    })
  },
  created: async function() {},
  mounted: function() {},
  updated: function() {},
  destroyed: function() {}
};

Sidebar.toggleSidebar = function() {
  let sidebar = document.getElementById("sidebar");
  if (sidebar.classList.contains("active")) {
    sidebar.classList.remove("active");
  } else {
    sidebar.classList.add("active");
  }
};

export default Sidebar;
</script>


<style lang="scss">
.sidebar-subitems {
  padding-left: 15px;
  font-size: 12px;
}
</style>