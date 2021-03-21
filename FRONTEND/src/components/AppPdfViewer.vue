<template>
    <div>
        <div v-if="showOnPopup">
            <pdf :src=src></pdf>
        </div>
        <div v-else>
            <pdf :src=src></pdf>
        </div>
    </div>
</template>
<script>
import pdf from 'vue-pdf'

export default {
    components: {pdf},
    props: ["src", "showOnPopup"],
    data() {
        return {
            
        }
    }, 
    methods: {
        
    }
}
</script>
<style scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
</style>