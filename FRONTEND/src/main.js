import Vue from 'vue'
import router from "./router.js"

import { BootstrapVue, BootstrapVueIcons, IconsPlugin, BIcon  } from 'bootstrap-vue'
import { VclFacebook } from 'vue-content-loading';
import { Auth0Plugin } from "./auth";
import { domain, clientId, audience } from "../auth_config.json";

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import 'bootstrap-vue/dist/bootstrap-vue-icons.min.css'
import './assets/css/docs.min.css'
import './assets/css/button.css'
import 'vue-select/dist/vue-select.css';
import 'pdfvuer/dist/pdfvuer.css';

import axios from 'axios'
import App from './App.vue'
import store from './store'
import ClickConfirm from './components/ClickConfirm.vue'
import titleMixin from './mixins/titleMixin'
import PortalVue from 'portal-vue'
import "wicg-inert"
import api from "./services/api";
import { makeServer } from "./server"
import LiquorTree from 'liquor-tree'


// global registration
Vue.use(LiquorTree)

if (process.env.NODE_ENV === "development") {
  //makeServer()
}

import vSelect from 'vue-select'

Vue.component('v-select', vSelect)

Vue.mixin(titleMixin)
Vue.use(PortalVue)
Vue.use(BootstrapVue, axios)
Vue.use(BootstrapVueIcons)
Vue.use(IconsPlugin)
Vue.use(require('vue-moment'));

Vue.use(Auth0Plugin, {
  domain,
  clientId,
  audience,
  onRedirectCallback: appState => {
    router.push(
      appState && appState.targetUrl
        ? appState.targetUrl
        : window.location.pathname
    );
  }
});

Vue.component('vclFacebook', VclFacebook)
Vue.component('BIcon', BIcon)

const clickConfirmPlugin = (Vue, params) => {
  let name = 'click-confirm';
  if (typeof params === 'string') name = params;

  Vue.component(name, ClickConfirm);
};

ClickConfirm.install = clickConfirmPlugin;
Vue.component('clickConfirm', ClickConfirm);

Vue.config.productionTip = false
Vue.prototype.$http = api; 
api.defaults.timeout = 100000;

api.interceptors.request.use(
  config => {
    const token = window.localStorage.getItem("access_token");
    if (token) {
      config.headers.common["Authorization"] = 'Bearer ' + token;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app')
