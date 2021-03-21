import createLogger from 'vuex/dist/logger'
import VuexORM from '@vuex-orm/core'

import { mutations, CHECKED_DATA_STORAGE_KEY, PDF_PROCESSED_IDS_STORAGE_KEY, ERROR_MESSAGE_GLOBAL, SUCCESS_MESSAGE_GLOBAL, APP_BREADCRUMB, APP_ROLE_LOADED } from './mutations'

import database from './database';


const localStoragePlugin = store => {
  store.subscribe((mutation, { dataIds }) => {
    window.localStorage.setItem(CHECKED_DATA_STORAGE_KEY, JSON.stringify(dataIds))
  })
  store.subscribe((mutation, { breadcrumbs }) => {
    window.localStorage.setItem(APP_BREADCRUMB, JSON.stringify(breadcrumbs))
  })
  store.subscribe((mutation, { dataPDFIds }) => {
    window.localStorage.setItem(PDF_PROCESSED_IDS_STORAGE_KEY, JSON.stringify(dataPDFIds))
  })
  store.subscribe((mutation, { errorMessageGlobal }) => {
    window.localStorage.setItem(ERROR_MESSAGE_GLOBAL, JSON.stringify(errorMessageGlobal))
  })
  store.subscribe((mutation, { successMessageGlobal }) => {
    window.localStorage.setItem(SUCCESS_MESSAGE_GLOBAL, JSON.stringify(successMessageGlobal))
  })
  store.subscribe((mutation, { appRoleLoaded }) => {
    window.localStorage.setItem(APP_ROLE_LOADED, JSON.stringify(appRoleLoaded))
  })
}

export default process.env.NODE_ENV !== 'production'
  ? [createLogger(), localStoragePlugin, VuexORM.install(database)]
  : [localStoragePlugin, VuexORM.install(database)]
