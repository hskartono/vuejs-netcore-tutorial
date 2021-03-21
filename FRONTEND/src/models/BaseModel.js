import { Model } from '@vuex-orm/core';
import axios from 'axios';

export default class BaseModel extends Model {
    static entityName;
    static listModel;
    static useAuth = true;
    
    static getSavedData() {
        return this.query().where("isSavedToBackend", true).where("isFromUpload", false);
    }

    static async getList(this_, filter, sorting, pageIndex, pageSize, persistBy) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";
        
        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        if (sorting != null && sorting != undefined) {
            Object.entries(sorting).map(([key, value]) => { if(value != '') filterArray.push("sorting[" + key +"]=" + (value ? "1" : "0")); });
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

        if (persistBy == undefined) {
            persistBy = "create";
        }
        return this.api().get(process.env.VUE_APP_API_URL + `${this.entityName}` + params, {
            persistBy: persistBy,
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            dataTransformer: (response) => {
                if (this.entity != undefined && this.entity != null) {
                    this_.$store.dispatch('entities/create', { entity: this.entity + 'list', data:  response.data  })
                }
                return response.data.data;
            }
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
            return false;
        });
    }

    static async getListPaging(this_, filter) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        let filterArray = Array();
        if (filter != null && filter != undefined) {
            filterArray = this.getFilter(filter);
        }
        
        let params = "";
        if (filterArray.length > 0) {
            params = "?" + filterArray.join("&");
        }

        return this.api().get(process.env.VUE_APP_API_URL + `${this.entityName}` + params, {
            persistBy: "create",
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            dataTransformer: (response) => {
                return response.data;
            }
        }).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });
    }

    static async getData(this_, id) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";
        let _this = this;
        return await this.api().get(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`, {
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            dataTransformer: (response) => {
                let responseData = response.data;
                if (_this.normalizeResponse != undefined) {
                    responseData = _this.normalizeResponse(this_, response);
                }
                return responseData;
            }
        })
        .catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }

    static async insertData(this_, data) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        return this.api().post(process.env.VUE_APP_API_URL + `${this.entityName}`, data, {
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            dataTransformer: (response) => {
                return response.data;
            }
        }).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
            return false;
        });;
    }
    
    static async updateData(this_, data, id) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        return this.api().put(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`, data, {
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            dataTransformer: (response) => {
                console.log(response.data);
                return response.data;
            }
        }).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }

    static async deleteData(this_, id) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        return this.api().delete(process.env.VUE_APP_API_URL + `${this.entityName}/${id}`, {
            delete: id,
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
        }).catch(function(error){
            this_.$store.dispatch('addErrorMessageGlobal', error);
            throw error;
        });;
    }

    static async checkAllData(this_, filter, sorting) {
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

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
        this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/ids` + params, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                },
            })
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
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        let _this = this_;
        let ids = _this.$store.state.dataIds;
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/multipagepdf`, {
                ids
                }, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                },
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
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        let _this = this_;
        let ids = _this.$store.state.dataIds;
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/singlepagepdf`, {
                ids
            }, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
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
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

        let _this = this_;
        let ids = Array();
        ids.push(id);
        return this_.$http.post(process.env.VUE_APP_API_URL + `${this.entityName}/singlepagepdf`, {
            ids
            }, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
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
        const accessToken = this.useAuth ?  await this_.$auth.getTokenSilently() : "";

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

        return this_.$http.get(process.env.VUE_APP_API_URL + `${this.entityName}/downloaddata` + params, {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
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

    static apiConfig = {
        actions: {
          getList() {
            return this.get(process.env.VUE_APP_API_URL + `${entityName}`)
          },
          getData (id) {
            return this.get(process.env.VUE_APP_API_URL + `${entityName}/${id}`)
          },

        }
    }
}