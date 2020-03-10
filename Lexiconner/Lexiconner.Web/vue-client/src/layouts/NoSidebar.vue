<template>
    <div class="container-fluid app-wrapper">
        <div class="row justify-content-center">
            <div class="col-10">
                <!-- route component is rendered here -->
                <slot>
                </slot>
            </div>
        </div>
    </div>
</template>


<script>
'use strict';

export default {
    name: 'no-sidebar-layout',
    components: {

    },
}
</script>
