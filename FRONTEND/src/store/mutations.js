export const CHECKED_DATA_STORAGE_KEY = 'data-check-all'
export const APP_BREADCRUMB = 'app-bread-crumb'
export const PDF_PROCESSED_IDS_STORAGE_KEY = 'pdf-processed-id'
export const ERROR_MESSAGE_GLOBAL = 'error-message-global'
export const SUCCESS_MESSAGE_GLOBAL = 'success-message-global'
export const APP_ROLE_LOADED = 'app-role-loaded'

// for testing
if (navigator.userAgent.indexOf('PhantomJS') > -1) {
  window.localStorage.clear()
}

export const mutations = {
  addCheckAllData (state, dataIds) {
    state.dataIds = dataIds;
  },
  removeCheckAllData (state) {
    state.dataIds = []
  },
  addCheckData (state, id) {
    state.dataIds.push(id);
  },
  removeCheckData (state, id) {
    let index = state.dataIds.indexOf(id);
    state.dataIds.splice(index, 1);
  },
  addDownloadPDFId(state, id) {
    state.dataPDFIds.push(id);
  },
  removeDownloadPDFId(state, id) {
    let index = state.dataPDFIds.indexOf(id);
    state.dataPDFIds.splice(index, 1);
  },
  addErrorMessageGlobal(state, msg) {
    let errors = Array();
    let errorExists = false;
    
    if (msg != null) {
      if (msg.response != null) {
        if (msg.response.data != null) {
          if (msg.response.data.errors != null) {
            if (msg.response.data.errors != null && typeof msg.response.data.errors === "object") {
              let errorArr = Object.values(msg.response.data.errors);
              errorArr.forEach(item => {
                if (Array.isArray(item)) {
                  if (item != "" && item != null) {
                    errors.push(item[0]);
                  }
                }
              });
            } else {
              errors = msg.response.data.errors;
            }
          }
          if (errors.length > 0) {
            errorExists = true;
          }
          if (!errorExists) {
            if (msg.response.data.title != null && msg.response.data.title != undefined && msg.response.data.title != '') {
              errors.push(msg.response.data.title);
              errorExists = true;
            }
          }
        }
      }
    }
    if (!errorExists) {
      errors.push(msg);
    }
    state.errorMessageGlobal = errors;
  },
  removeErrorMessageGlobal(state) {
    state.errorMessageGlobal = [];
  },
  addSuccessMessageGlobal(state, msg) {
    state.successMessageGlobal = msg;
  },
  removeSuccessMessageGlobal(state) {
    state.successMessageGlobal = {};
  },
  setBreadCrumb(state, datas) {
    state.breadcrumbs = datas;
  },
  setRoleLoaded(state) {
    state.isRoleLoaded = true;
  },
  unsetRoleLoaded(state) {
    state.isRoleLoaded = false;
  },
}
