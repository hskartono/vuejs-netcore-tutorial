<template>
    <div class="menu-group mt-2">
        <b-card header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <vue-content-loading :width="100" :height="15">
                    <rect rx="4" ry="4" width="100" height="15" />
                </vue-content-loading>
            </template>
            <center>
                <vue-content-loading :width="100" :height="90">
                    <rect rx="4" ry="4" width="100" height="90" />
                </vue-content-loading>
            </center>
            <template v-slot:footer class="p-0">
                <b-row>
                    <b-col cols="6" style="padding-left:13px">
                        <vue-content-loading :width="50" :height="15">
                            <rect rx="4" ry="4" width="50" height="15" />
                        </vue-content-loading>
                    </b-col>
                    <b-col cols="6" style="padding-left:3px">
                        <vue-content-loading :width="50" :height="15">
                            <rect rx="4" ry="4" width="50" height="15" />
                        </vue-content-loading>
                    </b-col>
                </b-row>
            </template>
        </b-card>
    </div>
</template>
<script>
import VueContentLoading from "vue-content-loading";
export default {
    components : {
        VueContentLoading
    }
}
</script>