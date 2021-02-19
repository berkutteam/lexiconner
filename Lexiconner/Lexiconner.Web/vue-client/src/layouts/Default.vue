<template>
  <div class="container-fluid app-wrapper">
    <div class="row">
      <div class="app-sidebar-parent">
        <!-- Sidebar -->
        <sidebar />

        <!-- Page Content -->
        <div class="col app-page-content">
          <!-- Sidebar toggle button -->
          <nav class="navbar navbar-expand-lg navbar-light bg-light app-navbar no-print">
            <div class>
              <button v-on:click="toggleSidebar" type="button" class="btn custom-btn-normal">
                <i class="fas fa-align-left"></i>
              </button>
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

export default {
  name: "default-layout",
  components: {
    Sidebar
  },
  methods: {
    toggleSidebar() {
      Sidebar.toggleSidebar();
    }
  }
};
</script>
