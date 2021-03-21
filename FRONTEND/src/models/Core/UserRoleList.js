import BaseModel from '../BaseModel';

export default class UserRoleList extends BaseModel {

	static entity = 'userrolelist'
	static entityName = 'userrole';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
