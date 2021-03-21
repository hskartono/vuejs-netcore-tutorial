import BaseModel from '../BaseModel';
import RoleDetail from './RoleDetail';

export default class Role extends BaseModel {
  static entity = 'role'
  static entityName = "role";
  
  static fields () {
    return {
      id : this.uid(),
      name : this.string(null).nullable(),
      description : this.string(null).nullable(),
      roleDetails : this.hasMany(RoleDetail, "roleId")
    }
  }

  static async getMe(this_) {
    const accessToken = window.localStorage.getItem("access_token")
    return this.api().get(process.env.VUE_APP_API_URL + `role/me`, {
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
}