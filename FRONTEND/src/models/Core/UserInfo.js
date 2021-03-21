import BaseModel from '../BaseModel';

export default class UserInfo extends BaseModel {
  static entity = 'user_infos'
  static entityName = "userinfo";
  
  static fields () {
    return {
      id : this.uid(),
      userName : this.string(null).nullable(),
      firstName : this.string(null).nullable(),
      lastName : this.string(null).nullable(),
    }
  }
}