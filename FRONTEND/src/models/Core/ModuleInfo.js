import BaseModel from '../BaseModel';

export default class ModuleInfo extends BaseModel {
  static entity = 'moduleinfo'
  static entityName = "moduleinfo";
  
  static fields () {
    return {
      id : this.uid(),
      name : this.string(null).nullable(),
      parentModuleId : this.string(null).nullable(),
      uploadValidationStatus : this.string(null).nullable(),
      uploadValidationMessage : this.string(null).nullable(),
      isFromUpload : this.boolean(null).nullable(),
    }
  }
}