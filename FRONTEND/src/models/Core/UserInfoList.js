import BaseModel from '../BaseModel';

export default class UserInfoList extends BaseModel {

	static entity = 'userinfolist'
	static entityName = 'userinfo';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
