import BaseModel from '../BaseModel';

export default class RoleList extends BaseModel {

	static entity = 'rolelist'
	static entityName = 'role';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
