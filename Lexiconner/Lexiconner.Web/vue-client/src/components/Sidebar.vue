<template>
  <div id="sidebar" class="app-sidebar no-print">
    <!-- Hide all links if not completed registration, so user complete it first -->
    <div v-if="sharedState.auth.isAuthenticated">
      <!-- User profile -->
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
        <router-link v-bind:to="{ name: 'user-dictionary'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-spell-check mr-2"></i>
          <span>Dictionary</span>
        </router-link>
      </div>
      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'words-dashboard'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-fighter-jet mr-2"></i>
          <span>Trainings</span>
        </router-link>
      </div>
      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'wordsets'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-book mr-2"></i>
          <span>Word sets</span>
        </router-link>
      </div>
      <div v-if="sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'user-films-browse'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-film mr-2"></i>
          <span>My films</span>
        </router-link>
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
        <router-link v-bind:to="{ name: 'logout'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-sign-out-alt mr-2"></i>
          <span>Logout</span>
        </router-link>
      </div>
    </div>

    <div v-if="!sharedState.auth.isAuthenticated">
      <div v-if="!sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'home'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-home mr-2"></i>
          <span>Home</span>
        </router-link>
      </div>
      <div v-if="!sharedState.auth.isAuthenticated" class="sidebar-item">
        <router-link v-bind:to="{ name: 'login'}" v-on:click.native="handleNavLinkClick()" class="sidebar-link">
          <i class="fas fa-sign-in-alt mr-2"></i>
          <span>Login</span>
        </router-link>
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
import DeviceDetectHelper from "@/helpers/deviceDetectHelper";

let Sidebar = {
  name: "sidebar",
  components: {},
  data: function() {
    return {
      privateState: {
      }
    };
  },
  computed: {
    // local computed go here

    // store state computed go here
    ...mapState({
      sharedState: state => state,
      currentRouteName() {
        // console.log(this.$route);
        return this.$route.name;
      }
    })
  },
  created: async function() {},
  mounted: function() {},
  updated: function() {},
  destroyed: function() {},
  methods: {
    handleNavLinkClick: function(e) {
      if(DeviceDetectHelper.checkIsMobile()) {
        // hide sidebar on mobile after lick
        Sidebar.toggleSidebar();
      }
    },
  }
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