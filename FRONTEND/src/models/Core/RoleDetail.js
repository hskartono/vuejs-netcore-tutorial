import BaseModel from '../BaseModel';
import FunctionInfo from './FunctionInfo';
import Role from './Role';

export default class RoleDetail extends BaseModel {
  static entity = 'roledetail'
  static entityName = "roledetail";
  
  static fields () {
    return {
      id : this.uid(),
      roleId : this.string(null).nullable(),
      role : this.belongsTo(Role, "roleId"),
      functionInfoId : this.string(null).nullable(),
      functionInfo : this.belongsTo(FunctionInfo, "functionInfoId"),
      allowCreate : this.boolean(false).nullable(),
      allowRead : this.boolean(false).nullable(),
      allowUpdate : this.boolean(false).nullable(),
      allowDelete : this.boolean(false).nullable(),
      showInMenu : this.boolean(false).nullable(),
      allowDownload : this.boolean(false).nullable(),
      allowPrint : this.boolean(false).nullable(),
    }
  }
}