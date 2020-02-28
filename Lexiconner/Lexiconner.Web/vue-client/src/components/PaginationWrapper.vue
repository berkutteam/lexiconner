<template>
     <div>
         <div v-bind:id="scrollTopElId"></div>

         <!-- Content -->
         <slot/>

         <!-- Pagination -->
         <div v-if="pageCount !== 0" class="d-flex justify-content-center mt-4">
            <paginate
                v-model="privateState.currentPage"
                :page-count="pageCount"
                :page-range="3"
                :margin-pages="2"
                :click-handler="pageChangeCallback"
                :prev-text="'Prev'"
                :next-text="'Next'"
                :container-class="'app-pagination'"
                :page-class="'page-item'"
                :page-link-class="'page-item-link'"
                :prev-class="'page-item'"
                :prev-link-class="'page-item-link'"
                :next-class="'page-item'"
                :next-link-class="'page-item-link'"
                :break-view-class="'break-view'"
                :break-view-link-class="'break-view-link'"
                :active-class="'page-item-active'"
                :disabled-class="'page-item-disabled'"
                :first-last-button="true"
                :first-button-text="'First'"
                :last-button-text="'Last'"
                :hide-prev-next="false"
            >
            </paginate>
        </div>

        <div v-bind:id="scrollBottomElId"></div>
     </div>
</template>

<script>
// @ is an alias to /src

/**
 * Pagination wrapper. Adds pagination to the content.
 * pageCount, pageSize are calculated based on paginationResult.pagination after first data load.
 * offset, limit are set externally
 */
export default {
    name: 'pagination-wrapper',
    props: {
        paginationResult: Object, // must return promise
        loadItemsF: Function, // required args: {offset, limit}
        onPageChanged: Function,
        // loading: Boolean,
    },
    components: {
    },
    data: function() {
        return {
            privateState: {
                currentPage: 1,
                scrollRequired: false,
            },
        };
    },
    computed: {
        // local computed go here
        scrollTopElId: function() {
            return `paginationTop_${this._uid || +(new Date())}`;
        },
        scrollBottomElId: function() {
            return `paginationBottom_${this._uid || +(new Date())}`;
        },
        pageCount: function() {
            if(!this.paginationResult) {
                return 0;
            }
            return Math.ceil(this.paginationResult.pagination.totalCount / this.pageSize);
        },
        pageSize: function() {
            if(this.paginationResult) {
                return this.paginationResult.pagination.limit;
            }
            return 100;
        },

        // store state computed go here
        
    },
    created: function() {
    },
    mounted: function() {
    },
    updated: function() {
        this.$nextTick(() => {
            // Code that will run only after the
            // entire view has been re-rendered
        });
    },
    destroyed: function() {
    },

    methods: {
        pageChangeCallback: function(pageNum) {
            console.log(this.$el.sele);
            if(this.onPageChanged) {
                this.onPageChanged(pageNum);
            }

            let limit = this.pageSize;
            let offset = Math.max(this.privateState.currentPage - 1, 0) * limit;
           
            this.scrollTo();
            
            this.loadItemsF({
                offset: offset,
                limit: limit,
            }).then(() => {
            });
        },
        scrollTo(direction = 'top') {
            let elSelector = direction === 'top' ? `#${this.scrollTopElId}` : `#${this.scrollBottomElId}`;
            this.$scrollTo(elSelector);
        }
    },
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">

</style>
