import BaseModel from '../BaseModel';
import ModuleInfo from './ModuleInfo';

export default class FunctionInfo extends BaseModel {
  static entity = 'functioninfo'
  static entityName = "functioninfo";
  
  static fields () {
    return {
      id : this.uid(),
      name : this.string(null).nullable(),
      uri : this.string(null).nullable(),
      iconName : this.string(null).nullable(),
      isEnabled : this.boolean(null).nullable(),
      moduleInfoId : this.string(null).nullable(),
      moduleInfo : this.belongsTo(ModuleInfo, "moduleInfoId"),
    }
  }
}