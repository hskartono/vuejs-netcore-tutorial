import BaseModel from '../BaseModel';
import UserInfo from './UserInfo';
import UserRoleDetail from './UserRoleDetail';

export default class UserRole extends BaseModel {
  static entity = 'userrole'
  static entityName = "userrole";
  
  static fields () {
    return {
      id : this.uid(),
      userInfoId : this.string(null).nullable(),
      userInfo : this.belongsTo(UserInfo, "userInfoId"),
      userRoleDetails : this.hasMany(UserRoleDetail, "userRoleId")
    }
  }
}