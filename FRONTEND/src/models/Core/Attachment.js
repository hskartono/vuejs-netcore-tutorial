import BaseModel from '../BaseModel';

export default class Attachment extends BaseModel {
  static entity = 'attachment'
  static entityName = "attachment";
  
  static fields () {
    return {
      id : this.uid(),
      downloadUrl : this.string(null).nullable(),
      originalFilename : this.string(null).nullable(),
      savedFileName : this.string(null).nullable(),
      fileExtension : this.string(null).nullable(),
      fileSize : this.number(0).nullable(),
    }
  }
}
