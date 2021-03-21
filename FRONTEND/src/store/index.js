import Vue from 'vue'
import Vuex from 'vuex'
import { mutations, CHECKED_DATA_STORAGE_KEY, PDF_PROCESSED_IDS_STORAGE_KEY, ERROR_MESSAGE_GLOBAL, APP_BREADCRUMB, APP_ROLE_LOADED, SUCCESS_MESSAGE_GLOBAL } from './mutations'
import actions from './actions'
import plugins from './plugins'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    dataIds: JSON.parse(window.localStorage.getItem(CHECKED_DATA_STORAGE_KEY) || '[]'),
    dataPDFIds: JSON.parse(window.localStorage.getItem(PDF_PROCESSED_IDS_STORAGE_KEY) || '[]'),
    errorMessageGlobal: JSON.parse(window.localStorage.getItem(ERROR_MESSAGE_GLOBAL) || '[]'),
    successMessageGlobal: JSON.parse(window.localStorage.getItem(SUCCESS_MESSAGE_GLOBAL) || '{}'),
    breadcrumbs: JSON.parse(window.localStorage.getItem(APP_BREADCRUMB) || '[]'),
    isRoleLoaded: window.localStorage.getItem(APP_ROLE_LOADED) != undefined && window.localStorage.getItem(APP_ROLE_LOADED) != 'undefined' ? JSON.parse(window.localStorage.getItem(APP_ROLE_LOADED) || '') : false,
  },
  actions,
  mutations,
  plugins
})
