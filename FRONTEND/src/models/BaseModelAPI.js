import { Model } from '@vuex-orm/core';
import axios from 'axios';
import { saveAs } from 'file-saver';

export default class BaseModelAPI {
    static entityName;
    static listModel;
    static useAuth = true;
    
    static getSavedData() {
        return this.query().where("isSavedToBackend", true).where("isFromUpload", false);
    }

    static async getList(this_, filter, sorting, pageIndex, pageSize, persistBy) {
        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        if (sorting != null && sorting != undefined) {
            Object.entries(sorting).map(([key, value]) => { 
                if(key != '') {
                    filterArray.push("sorting[" + key +"]=" + (value ? "1" : "0"));
                }
            });
                
        }
        if (pageIndex != null) {
            filterArray.push("pageIndex=" + pageIndex);
        }
        if (pageSize != null) {
            filterArray.push("pageSize=" + pageSize);
        }
        let params = "";
        if (filterArray.length > 0) {
            params = "?" + filterArray.join("&");
        }

        return this_.$http.get(`${this.entityName}` + params)
        .then(response => {
            if (response != null) {
                return response.data;
            }
            return response;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async getData(this_, id) {
        return this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`)
        .then(response => {
            let responseData = response.data;
            return responseData;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }
    
    static async createData(this_, data) {
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/create`, data)
        .then(response => {
                return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async createDataDetail(this_, data) {
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}`, data)
        .then(response => {
                return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async insertData(this_, data) {
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}`, data)
        .then(response => {
                return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }
    
    static async updateData(this_, data, id) {
        return this_.$http.put(process.env.VUE_APP_API_URL + `${this.entityName}/update/${id}`, data)
        .then(response => {
            return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async updateDataDetail(this_, data, id) {
        return this_.$http.put(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`, data)
        .then(response => {
            return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async editData(this_, id) {
        return this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/edit/${id}`)
        .then(response => {
            return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }
    static async commitData(this_, data, id) {
        

        return this_.$http.put(process.env.VUE_APP_API_URL + `${this.entityName}/commit/${id}`, data)
        .then(response => {
            return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async deleteData(this_, id) {
        

        return this_.$http.delete(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`, {
            delete: id,

        }).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }

    static async discardData(this_, id) {
        return this_.$http.delete(process.env.VUE_APP_API_URL + `${this.entityName}/discard/${id}`).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }

    static async addFromClipboard(this_, data) {
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/add`, data)
        .then(response => {
                return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async replaceFromClipboard(this_, data) {
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/replace`, data)
        .then(response => {
                return response.data;
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async checkAllData(this_, filter, sorting) {
        

        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        if (sorting != null && sorting != undefined) {
            Object.entries(sorting).map(([key, value]) => { if(value != '') filterArray.push("sorting[" + key +"]=" + value); });
        }
        let params = "";
        if (filterArray.length > 0) {
            params = "?" + filterArray.join("&");
        }
        let _this = this_;
        this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/ids` + params)
            .then(async function(response){
                if (response.data) {
                    _this.$store.dispatch('addCheckAllData', response.data);
                }
                return true;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static async multiPagePrint(this_) {
        let _this = this_;
        let ids = _this.$store.state.dataIds;
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/multipagepdf`, {
                ids
                })
            .then(async function(response){
                if (response.data) {
                    _this.$store.dispatch('addDownloadPDFId', response.data);
                    return response.data;
                }
                return null;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static async singlePagePrint(this_) {
        

        let _this = this_;
        let ids = _this.$store.state.dataIds;
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/singlepagepdf`, {
                ids
            })
            .then(async function(response){
                if (response.data) {
                    return response.data;
                }
                return null;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static async singleDataPagePrint(this_, id) {
         let _this = this_;
        let ids = Array();
        ids.push(id);
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/singlepagepdf`, {
            ids
            })
            .then(async function(response){
                if (response.data) {
                    return response.data;
                }
                return null;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static async downloadData(this_, filter, sorting) {
        

        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        if (sorting != null && sorting != undefined) {
            Object.entries(sorting).map(([key, value]) => { if(value != '') filterArray.push("sorting[" + key +"]=" + (value ? "1" : "0")); });
        }

        let params = "";
        if (filterArray.length > 0) {
            params = "?" + filterArray.join("&");
        }
        let _this = this;
        return this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/downloaddata` + params)
            .then(async function(response){
                if (response.data) {
                    return response.data;
                }
                return null;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static async downloadDataDetail(this_, filter, sorting) {
        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        if (sorting != null && sorting != undefined) {
            Object.entries(sorting).map(([key, value]) => { if(value != '') filterArray.push("sorting[" + key +"]=" + (value ? "1" : "0")); });
        }

        let params = "";
        if (filterArray.length > 0) {
            params = "?" + filterArray.join("&");
        }
        let _this = this;
        return await this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/downloaddata` + params, {responseType: 'blob'})
            .then(async function(response){
                if (response.data) {
                    await saveAs(response.data, _this.entityName + ".xlsx");
                }
                return null;
            })
            .catch(function(error){
                this_.$store.dispatch('addErrorMessageGlobal', error);
                throw error;
            });
    }

    static getFilter(filter) {
        let filterArray = Array();
        Object.entries(filter).map(([key, value]) => { 
            if (key != "exactType") {
                if(value != '' && value != null) { 
                    let values = Array();
                    if (typeof value === "string") {
                        if(value.match(/(?:\r\n|\r|\n)/g)) { 
                            value=value.replace(/(?:\r\n|\r|\n)/g, ','); 
                        }
                        values = value.split(","); 
                    } else {
                        values.push(value);
                    }
                    values.forEach(itemValue => filterArray.push("filter[" + key +"][]=" + itemValue));
                    if (filter.exactType != undefined && filter.exactType != null) {
                        if (filter.exactType[key] != undefined) {
                            filterArray.push("exact[" + key +"]=" + filter.exactType[key]);
                        }
                    }
                }
            }
        });

        return filterArray;
    }

    static async downloadFile(this_, dataUrl, fileName) {
        this_.$http.get(dataUrl, {responseType: 'blob'}).then(async response => {
            console.log(response);
            if (response != null) {
                await saveAs(response.data, fileName);
            }
        }).catch(error => {
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async checkProgress(this_, processId) {
        let progressInterval = setInterval(() => {
            this_.$http.get(process.env.VUE_APP_API_URL + 'DownloadProcess/status/' + processId).then(response => {
                if (response.data != null) {
                    if (response.data.status == "SUCCESS") {
                        let dataUrl = response.data.filename;
                        let fileName = this.GetFilename(dataUrl);
                        this.downloadFile(this_, dataUrl, fileName)
                        clearInterval(progressInterval);
                    } else if (response.data.status == "FAILED") {
                        clearInterval(progressInterval);
                    }
              } else {
                clearInterval(progressInterval);
              }
              return true;
            }).catch(error => {
              clearInterval(progressInterval);
              this_.$store.dispatch('addErrorMessageGlobal', error);
              throw error;
            })
        }, 500);
      }

      static GetFilename(url)
      {
        if (url)
        {
            var m = url.toString().match(/.*\/(.+?)\./);
            if (m && m.length > 1)
            {
              return m[1];
            }
        }
        return "";
      }
}