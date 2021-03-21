import BaseModel from '../BaseModel';
import Role from './Role';
import UserRole from './UserRole';

export default class UserRoleDetail extends BaseModel {
  static entity = 'user_role_details'
  static entityName = "userroledetail";
  
  static fields () {
    return {
      id : this.uid(),
      userRoleId : this.string(null).nullable(),
      userRole : this.belongsTo(UserRole, "userRoleId"),
      roleId : this.string(null).nullable(),
      role : this.belongsTo(Role, "roleId")
    }
  }
}