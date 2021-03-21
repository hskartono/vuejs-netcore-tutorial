<template>
  <div>
    <div v-if="isNoTemplate">
      <RouterView />
    </div>
    <div v-else>
      <b-container fluid>
        <b-row>
          <b-col cols="2">
            <LeftMenu :class="getSidebarClass" />
            <div v-on:click="isShowMenubar = false" :class="getClassOverlay">
              <b-overlay :show="overlayShown" blur="none" style="width:100%;height:100%" spinner-type="">
              </b-overlay>
            </div>
          </b-col>
          <b-col :cols="getContentCols">
            <b-col style="position:fixed;z-index:1000;margin-left:-15px" class="p-0" :cols="getContentCols">
              <div>
                <b-navbar type="dark" style="background-color: #fff !important;border-bottom:#ddd 1px solid" variant="dark" fixed="true">
                    <b-col class="col-12 pl-0">
                        <button id="sidebarToggleTop" v-on:click="toggleSidebar" style="padding: 0.3rem 0.25rem;font-size: 20px;" class="btn mr-3">
                            <b-icon-list style="color: #6c7293"></b-icon-list>
                        </button>
                        <span class="float-right">
                          <span class="mr-2 mt-3 font-base text-muted d-none d-md-inline">Hi<span>, <b class="text-dark-50">{{ ($auth != null ? ($auth.user != null && $auth.user != undefined ? $auth.user.nickname : "") : "") }}</b></span></span><b-avatar style="background: #c9f7f5;color: #1bc5bd;" size="2rem"></b-avatar>
                        </span>
                        <button class="btn float-right mr-3" style="padding:0.25rem">
                          <b-icon-bell-fill  style="color: #6c7293"></b-icon-bell-fill>
                        </button>
                    </b-col>
                </b-navbar>
              </div>
              <Breadcrumbs></Breadcrumbs>
            </b-col>
            <b-container fluid class="col-12 mb-5" style="top: 130px;">
              <PortalTarget name="app-popup" />
              <PortalTarget name="app-popup-detail" />
              <RouterView />
            </b-container>
          </b-col>
          <b-col :cols="getSidebarCols" v-if="isShowSidebar" class="pl-0 pr-0" style="border-left: 1px solid rgba(0, 0, 0, 0.1);">
          </b-col>
        </b-row>
      </b-container>
    </div>
    <PopupError :openPopup="globalErrorExists" />
  </div>
</template>
<style lang="scss">
.btn-xs {
  font-size: .675rem;
}
.col-form-label {
    font-size: 0.85rem !important;
    color: #7e8299 !important;
}
.font-base{
  font-size: 1rem;
}
.text-muted {
    color: #b5b5c3!important;
}
.text-dark-50 {
    color: #7e8299!important;
}
.showOverlay {
  width: 100%;
  height: 100%;
  position: fixed;
}
.showOverlay-active {
  z-index: 3;
}
.sidebar {
  top: 0;
  position: fixed; 
  height: 100vh;
  /*height: calc(100vh - 2.5rem);*/
  /*margin-top: 50px;*/
  /*width: 250px;*/
  left: -250px;
  background: #1E1E2D;
  z-index: 1001;
  transition: all 0.3s;
  overflow-x: hidden;
  overflow-y: hidden;
  box-shadow: 3px 3px 3px rgba(0, 0, 0, 0.2);
}
.sidebar-active {
  left: 0px;
}
.table-corner {
  border: 1px solid #dee2e6; 
  border-radius: 4px;
  margin-bottom: 1rem;
}
.table {
  margin-bottom: 0rem !important;
}
.table th {
  border-top: none !important;
}
.table thead th {
  border-bottom: 1px solid #dee2e6 !important;
}
.table td:first-child, .table th:first-child {
  padding-left: 15px;
}
.page-item.active .page-link {
    background-color: #029FE2 !important;
    border-color: #029FE2 !important;
}

.page-link {
    color: #029FE2;
}
a {
    color: #029FE2;
}
.form-group.required .control-label:after, .required:after {
  content:"*";
  color:red;
  margin-left: 5px;
}


.bd-links {
    max-height: calc(100vh) !important;
    overflow-y: auto !important;
}

.bd-toc-link {
    padding: .75rem 1.5rem;
    border-radius: 10px;
    font-weight: 400 !important;
    font-size: 14px !important;
    color: #a2a3b7;
}

.bd-toc-link .b-icon {
  color: #494b74;
  margin-right: 10px;
}

.bd-toc-link.active .b-icon {
  color: #029FE2;
  margin-right: 10px;
}

.bd-toc-link:hover {
  background-color: #1b1b28;
  color: #fff;
}

.bd-links {
    padding-top: 2rem;
}

.btn-primary {
  background-color: #1b1b28;
  border-color: #1b1b28;
}

.btn-primary:hover {
    color: #fff;
    background-color: #0493d0;
    border-color: #029FE2;
}

.btn-secondary {
    color: #222;
    background-color: #e9ecef;
    border-color: #e9ecef;
}

.btn-secondary:hover {
    color: #222;
    background-color: #d6d8da;
    border-color: #e9ecef;
}

.btn-secondary:active {
    color: #222;
    background-color: #d6d8da;
    border-color: #e9ecef;
}
.header-text {
    color: #6c7293 !important;
    font-weight: 500;
}
body {
  background: #EEF0F8 !important;
}

body, html {
    height: 100%;
    margin: 0;
    padding: 0;
    font-size: 0.95rem !important;
    font-family: Poppins,Helvetica,"sans-serif";
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}

@media (max-width: 768px) {
  body, html {
    font-size: 0.9rem !important;
  }
}

.btn:focus, .btn.focus {
  box-shadow: none;
}

.card {
  border: none !important;
}
h5.card-title, .card-label, h5.modal-title {
    font-weight: 500;
    font-size: 1rem;
    color: #181c32;
}

h5.card-title, h5.modal-title {
    display: -webkit-box;
    display: -ms-flexbox;
    display: flex;
    -webkit-box-align: center;
    -ms-flex-align: center;
    align-items: center;
    margin: .5rem;
    margin-left: 0;
}

h5.modal-title {
  font-weight: 600!important;
}

.text-dark {
    color: #181c32!important;
}

.font-weight-bolder {
    font-weight: 600!important;
}

@media (max-width:768px) {
  .navbar-brand {
    font-size: 1.1rem !important;
  }
}

.table-responsive:has(.dropdown-menu) {
  min-height: 370px;
}

.vs__dropdown-menu {
  z-index: 1300 !important;
}

.boundary-datepicker {
  position: static;
}
.required {
  color: red;
}
.table-row-selected {
  background-color: #fffaeb;
}

@media (min-width: 576px) {
  .modal-sm {
    max-width: 500px !important;
  }
}
.row-danger {
	color: red;
}
.b-table thead th, .b-table tbody td {
    white-space: nowrap;
}
</style>
<script>
import Breadcrumbs from '@/components/Breadcrumbs'
import PopupError from '@/components/PopupError'
import LeftMenu from './components/Sidebar'
import Role from '@/models/Core/Role'

export default {
  name: 'App',
  title: 'Training VueNetCore',
  components : {
    Breadcrumbs,
    LeftMenu,
    PopupError
  },
  methods : {
    toggleSidebar() {
      this.isShowMenubar = !this.isShowMenubar;
    },
    onResize() {
      this.windowWidth = window.innerWidth
      if (this.windowWidth < 1200 && this.windowWidth >= 768) {
        this.isShowMenubar = false;
      }
      else if (this.windowWidth < 768  && this.windowWidth >= 576) {
        this.isShowMenubar = false;
      } else if (this.windowWidth < 576) {
        this.isShowMenubar = false;
      } else {
        this.isShowMenubar = true;
      }
    },
    checkWindowSize() {
      if (this.windowWidth < 1200 && this.windowWidth >= 768) {
        this.isShowMenubar = false;
      }
      else if (this.windowWidth < 768  && this.windowWidth >= 576) {
        this.isShowMenubar = false;
      } else if (this.windowWidth < 576) {
        this.isShowMenubar = false;
      } else {
        this.isShowMenubar = true;
      }
    },
    isLoginPage() {
      var menu_name = this.$route.name != null && this.$route.name != undefined  ? this.$route.name : "";
      if (menu_name.toLowerCase() == "login") {
        this.isNoTemplate = true;
      } else {
        this.isNoTemplate = false;
      }
    },
    async getUserRole() {
      if (this.$auth != null) {
        if (this.accessToken != null && this.accessToken != undefined && this.accessToken != "") {
          Role.getMe(this).then(result => {
            this.$store.dispatch('setRoleLoaded');
          });
        }
      }
    },
    makeToast(variant = null) {
      this.$bvToast.toast(this.$store.state.successMessageGlobal, {
          title: "Success",
          variant: variant,
          autoHideDelay: '1000',
          solid: true
      });
      this.$store.dispatch('removeSuccessMessageGlobal');
    }
  },
  data() {
    return {
      isShowMenubar: true,
      isNoTemplate: false,
      contentCols : 9,
      sidebarCols : 3,
      windowWidth: window.innerWidth,
    }
  },
  beforeMount() {
    this.$store.dispatch('removeSuccessMessageGlobal');
  },
  mounted() {
      this.checkWindowSize();
      this.isLoginPage();
      this.$nextTick(() => {
        window.addEventListener('resize', this.onResize);
        this.$store.dispatch('unsetRoleLoaded');
        this.getUserRole();
      });
  },
  beforeDestroy() { 
    window.removeEventListener('resize', this.onResize); 
    this.$store.dispatch('unsetRoleLoaded');
  },
  computed : {
    accessToken() {
      return this.$auth.accessToken;
    },
    isShowSidebar() {
      return this.$store.state.isShowSidebar;
    },
    overlayShown() {
      return this.isShowMenubar;
    },
    globalErrorExists() {
      return this.$store.state.errorMessageGlobal != null && this.$store.state.errorMessageGlobal != "" ? true : false;
    },
    getClassOverlay() {
      if (this.isShowMenubar) {
        return "showOverlay showOverlay-active d-block d-lg-none";
      }
      return "showOverlay d-block d-lg-none";
    },
    getSidebarCols() {
      if (this.$store.state.isShowSidebar) {
        return 3;
      } else {
        return 0;
      }
    },
    getSidebarClass() {
      if (this.isShowMenubar) {
        let cols = "col-2";
        if (this.windowWidth < 1200 && this.windowWidth >= 768) {
          cols = "col-4";
        }
        else if (this.windowWidth < 768  && this.windowWidth >= 576) {
          cols = "col-6";
        } else if (this.windowWidth < 576) {
          cols = "col-8";
        }
        return "sidebar sidebar-active " + cols;
      }
      return "sidebar";
    },
    getContentCols() {
      if (this.isShowMenubar) {
        if (this.windowWidth < 1200) {
          return 12;
        }
        return 10;
      }
      return 12;
    },
  },
  watch : {
    accessToken : function(newValue, oldValue) {
      if (newValue != null && newValue != "") {
        this.getUserRole();
      }
    },
    '$store.state.successMessageGlobal' : function(newValue, oldValue) {
      if (typeof newValue == "string") {
        this.makeToast("success");
      }
    }
  }
}
</script>

<style>

</style>
