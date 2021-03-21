//src/components/Navbar.vue
<template>
    <div style="position:fixed; width:100%; z-index:1000">
        <b-navbar type="dark" variant="dark" fixed="true" style="padding: 0px">
            <b-col>
                <b-navbar-brand href="#">AMN Core</b-navbar-brand>
            </b-col>
            <b-col cols="10" class="pl-0">
                <button id="sidebarToggleTop" class="btn mr-3">
                    <b-icon-list style="color: #eee"></b-icon-list>
                </button>
            </b-col>
        </b-navbar>
    </div>
</template>

<script>
export default {}
</script>