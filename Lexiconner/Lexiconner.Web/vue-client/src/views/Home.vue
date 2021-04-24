<template>
    <div class="home-page-wrapper">
        <div class="text-center">
            <div class="cover-container d-flex h-100 p-3 mx-auto flex-column">
                <header class="masthead mb-auto">
                    <div class="inner">
                    <h3 class="masthead-brand">Lexiconner</h3>
                    <nav class="nav nav-masthead justify-content-center">
                        <router-link 
                            v-if="!sharedState.auth.isAuthenticated" 
                            v-bind:to="{ name: 'home'}" 
                            v-bind:class="{'active': this.$route.name === 'home'}"
                            class="nav-link"
                        >
                            <i class="fas fa-home mr-2"></i>
                            <span>Home</span>
                        </router-link>
                        <router-link 
                            v-if="!sharedState.auth.isAuthenticated" 
                            v-bind:to="{ name: 'login'}" 
                            class="nav-link"
                        >
                            <i class="fas fa-sign-in-alt mr-2"></i>
                            <span>Login</span>
                        </router-link>
                    </nav>
                    </div>
                </header>

                <main role="main" class="inner cover">
                    <h1 class="cover-heading">Build your vocabularly.</h1>
                    <p class="lead">
                        Add your own words or use the ones from already available word sets.
                        Create your own word sets, share and build your dictionary.
                        Instead of trying to remember on your own, use trainings to both to learn and repeat.
                    </p>
                    <!-- <p class="lead">
                        <a href="#" class="btn btn-lg btn-secondary">Learn more</a>
                    </p> -->
                </main>

                <footer class="mastfoot mt-auto">
                    <div class="inner">
                    <p>Lexiconner {{ new Date().getFullYear() }}</p>
                    </div>
                </footer>
            </div>
        </div>
    </div>
</template>

<script>
// @ is an alias to /src
import { mapState, mapGetters } from 'vuex';
import { storeTypes } from '@/constants/index';

export default {
  name: 'home',
  components: {
  },
  data: function() {
        return {
            privateState: {

            },
        };
    },
    computed: {
        // local computed go here

        // store state computed go here
        ...mapState({
            sharedState: state => state
        })
    },
    created: function() {
    },
    mounted: function() {
    },
    updated: function() {
    },
    destroyed: function() {
    },

    methods: {
    },
}
</script>

<style lang="scss" src="@/styles/home.scss">
</style>
