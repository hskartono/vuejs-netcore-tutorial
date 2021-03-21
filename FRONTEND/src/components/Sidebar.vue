<template>
    <div class="bd-sidebar border-bottom-0" style="border-right: none;">
        <nav id="bd-docs-nav" class="bd-links d-block" aria-label="Main navigation" style="border-right: none;">
            <div class="logo">
                <center>
                    <img src="/img/logo.png" class="menu-logo" id="menu-logo" alt="menu logo" />
                </center>
            </div>
            <b-link to="/" exact router-tag="div">
                <b-link :to="{ path : '/home' }" :class="getActiveClass('homeindex')"><b-icon-house-door-fill></b-icon-house-door-fill> Home</b-link>
                <span v-for="(item, parentName, index) in menus" :key="index">
                    <b-button block href="#" v-b-toggle="'accordion-' + index"><b-icon-people-fill></b-icon-people-fill> {{ parentName }}
                        <span class="menu-arrow">
                            <b-icon-chevron-right class="float-right chevron-right"></b-icon-chevron-right><b-icon-chevron-down class="float-right chevron-down"></b-icon-chevron-down>
                        </span>
                    </b-button>
                    <b-collapse :id="'accordion-' + index" :visible="getVisibility(item)" accordion="menu-accordion" role="tabpanel" class="menu-accordion">
                        <span v-for="itemMenu in item" :key="itemMenu.id">
                            <b-link :to="{ path : itemMenu.uri }" :class="getActiveClass(itemMenu.uri + 'index')"><b-icon-circle-fill></b-icon-circle-fill> {{ itemMenu.name }} </b-link>
                        </span>
                    </b-collapse>
                </span>
            </b-link>
        </nav>
    </div>
</template>
<style scoped>
.menu-logo {
    width: 120px;
    margin-bottom: 20px;
}
@media (max-width: 1366px) {
    .menu-logo {
        width: 100px;
        margin-bottom: 20px;
    }
}
@media (max-width: 500px) {
    .menu-logo {
        width: 80px;
        margin-bottom: 20px;
    }
}

.active {
    background-color: #1b1b28;
    color: #ffa800;;
}
.btn {
    background-color: transparent;
    color: #a2a3b7;
    border: none;
    text-align: justify;
    padding: .75rem 1.5rem;
    font-weight: 400 !important;
    font-size: 14px !important;
    color: #a2a3b7;
}
.btn .b-icon {
    color: #494b74;
    margin-right: 10px;
}

.bd-toc-link.active .b-icon {
    color: #f64e60;
    margin-right: 10px;
}

.btn:hover, .btn.not-collapsed, .btn-secondary:active {
    background-color: #1b1b28 !important;
    color: #fff !important;
}
.btn.not-collapsed .chevron-down {
    display: inline;
}

.btn-secondary:active:focus {
    box-shadow: none;
}

.btn.not-collapsed .chevron-right {
    display: none;
}

.btn.collapsed .chevron-right {
    display: inline;
}

.btn.collapsed .chevron-down {
    display: none;
}
.menu-accordion {
    border-radius: 0px;
}

.menu-accordion .bd-toc-link {
    padding-left: 35px;
}
.menu-arrow {
    font-size: 10px;
    padding-top: 2px;
}
.btn.btn-secondary.focus:not(.btn-text), .btn.btn-secondary:focus:not(.btn-text), .btn.btn-secondary:hover:not(.btn-text):not(:disabled):not(.disabled) {
    background-color: transparent !important;
    border-color: transparent !important;
}
</style>
<script>
import RoleDetail from '@/models/Core/RoleDetail';

export default {
    data() {
        return {
            activeClass:"",
            activeElement: "",
            menus : {},
        }
    },
    methods : {
       getActiveClass(text) {
           var isActive = false;
           text = text.replace(/[\/]/, "");
           var menu_name = this.$route.name;
           if (Array.isArray(text)) {
               text.forEach(item => { 
                if (menu_name != null) {
                    if (menu_name.indexOf(item) == 0 && text.length == menu_name.length) {
                        isActive = true;
                    }
                }
               });  
           } else {
               if (menu_name != null) {
                   if (menu_name.indexOf(text) == 0 && text.length == menu_name.length) {
                        isActive = true;
                    }
               }
           }
            if (isActive) {
                this.activeClass = text;
                return "bd-toc-link active";
            }
            return "bd-toc-link";
        },
        getMenu() {
            let roleDetails = RoleDetail.query().with("functionInfo").with("functionInfo.moduleInfo").get();
            let menus = {};
            if (roleDetails != null && roleDetails.length > 0) {
                let defaultMenu = "Master Data";
                menus[defaultMenu] = Array();
                roleDetails.forEach(item => {
                    if (item != null) {
                        if (item.functionInfo != null) {
                            if (item.showInMenu) {
                                let moduleInfo = item.functionInfo.moduleInfo;
                                    if (moduleInfo != null) {
                                        if (!menus[moduleInfo.name]) {
                                            menus[moduleInfo.name] = Array();
                                        }
                                        menus[moduleInfo.name].push(item.functionInfo);
                                    } else {
                                        //menus[defaultMenu].push(item.functionInfo);
                                    }
                            }
                        }
                    }
                });
            }
            
            this.menus = menus;
        },
        getVisibility(menus) {
            let isVisible = false;
            var menu_name = this.$route.name;
            menus.forEach(item => {
                let text = item.uri
                text = text.replace('/[/]/', "");
                text = text + "index";
                if (menu_name != null) {
                   if (menu_name.indexOf(text) == 0 && text.length == menu_name.length) {
                        isVisible = true;
                    }
               }
            });
            return isVisible;
        }
    },
    watch : {
        activeClass : function(val) {
            if (document.getElementById(val) != null) {
               this.activeElement = document.getElementById(val).parentElement.id;
            }
        },
        isRoleLoaded(newValue, oldValue) {
            this.getMenu();
        }
    },
    async mounted() {
        await this.getMenu();
    },
    computed : {
        isRoleLoaded() {
            return this.$store.state.isRoleLoaded;
        },
    }
}
</script>