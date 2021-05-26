<template>
  <div class="container-fluid app-wrapper">
    <div class="row">
      <div class="app-sidebar-parent">
        <!-- Sidebar -->
        <sidebar />

        <!-- Page Content -->
        <div class="col app-page-content">
          <!-- Top navbar -->
          <nav
            class="
              navbar navbar-expand-lg navbar-light
              bg-light
              app-navbar
              no-print
            "
          >
            <div class="w-100 d-flex justify-content-start">
              <div class>
                <!-- Sidebar toggle button -->
                <button
                  v-on:click="toggleSidebar"
                  type="button"
                  class="btn custom-btn-normal"
                >
                  <i class="fas fa-align-left"></i>
                </button>
              </div>

              <!-- Learning language selector -->
              <div class="ml-auto">
                <learning-language-selector> </learning-language-selector>
              </div>
            </div>
          </nav>

          <!-- Route component is rendered here -->
          <slot></slot>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
"use strict";

import Sidebar from "@/components/Sidebar";
import LearningLanguageSelector from "@/components/LearningLanguageSelector";

export default {
  name: "default-layout",
  components: {
    Sidebar,
    LearningLanguageSelector,
  },
  methods: {
    toggleSidebar() {
      Sidebar.toggleSidebar();
    },
  },
};
</script>
