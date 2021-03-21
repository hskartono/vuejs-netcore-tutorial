<template>
    <div style="position:fixed;width:100%;z-index:998;margin-right: -15px;">
        <ol class="breadcrumb" style="border-radius:0px;background:#fff;-webkit-box-shadow: 0 10px 30px 0 rgba(82,63,105,.08);box-shadow: 0 10px 30px 0 rgba(82,63,105,.08);" >
            <li class="breadcrumb-item">
                <b-link :to="{ path : '/' }" class="router-link-active" target="_self"><b-icon-house-door-fill></b-icon-house-door-fill></b-link>
            </li>
            <li class="breadcrumb-item active" v-for="item in items" :key="item">
                <span aria-current="location">{{ item }}</span>
            </li>
        </ol>
    </div>
</template>

<script>
import FunctionInfo from '@/models/Core/FunctionInfo';
export default {
    data() {
      return {
        items: [],
        routeName : {
            "homeindex" : "Home",
        }
      }
    },
    methods : {
        
    },
    mounted() {
        
    },
    watch : {
        currentPage() {
            var path = this.$route.path;
            path = path.replace(/[\/]/g, "");
            let result = FunctionInfo.query().where('uri', path).with("moduleInfo").first();
            if (result != null) {
                let functionName = result.name;
                let moduleName = result.moduleInfo ? result.moduleInfo.name : "Master Data";
                this.items = [moduleName, functionName];
            } else {
                this.items = this.$store.state.breadcrumbs;
            }
        },
    },
    computed : {
        currentPage() {
            return this.$store.state.breadcrumbs;
        },
        getBreadcrumbsItem() {
            var items = [
                {
                    text: 'Home',
                    to: '/'
                },
                
            ];
            var itemRoutes = this.routeName[this.$route.name];
            if (Array.isArray(itemRoutes)) {
                itemRoutes.forEach(route => {
                    var item = {};
                    if (route.href != undefined) {
                        item = {
                            text: route.name,
                            to: route.href
                        };
                    } else {
                        item = {
                            text: route,
                            active: true
                        };
                    }
                    
                    items.push(item);
                });
            } else {
                var item = {
                    text: this.routeName[this.$route.name],
                    active: true
                };
                items.push(item);
            }
            return items;
        },
        showBreadcrumb() {
            var show = true;
            if (this.$route.name == 'home') {
                show = false;
            }
            return show;
        }
    }
  }
</script>