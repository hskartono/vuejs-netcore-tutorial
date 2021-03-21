export default {
  addCheckAllData ({ commit }, dataIds) {
    commit('addCheckAllData', dataIds)
  },

  removeCheckAllData ({ commit }) {
    commit('removeCheckAllData')
  },

  removeCheckData ({ commit }, id) {
    commit('removeCheckData', id)
  },

  addCheckData ({ commit }, id) {
    commit('addCheckData', id)
  },
  addDownloadPDFId({ commit }, id) {
    commit('addDownloadPDFId', id)
  },
  removeDownloadPDFId({ commit }, id) {
    commit('removeDownloadPDFId', id)
  },
  addErrorMessageGlobal({ commit }, msg) {
    commit('addErrorMessageGlobal', msg)
  },
  removeErrorMessageGlobal({ commit }) {
    commit('removeErrorMessageGlobal')
  },
  addSuccessMessageGlobal({ commit }, msg) {
    commit('addSuccessMessageGlobal', msg)
  },
  removeSuccessMessageGlobal({ commit }) {
    commit('removeSuccessMessageGlobal')
  },
  setBreadCrumb({ commit }, datas) {
    commit('setBreadCrumb', datas)
  },

  setRoleLoaded({ commit }) {
    commit('setRoleLoaded')
  },
  unsetRoleLoaded({ commit }) {
    commit('unsetRoleLoaded')
  },
}
