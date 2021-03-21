import BaseModelAPI from '../BaseModelAPI';

export default class UserInfo extends BaseModelAPI {
  static entity = 'user_infos'
  static entityName = "userinfo";
  
  constructor () {
    return {
      id : '',
      userName : '',
      firstName : '',
      lastName : '',
    }
  }
}