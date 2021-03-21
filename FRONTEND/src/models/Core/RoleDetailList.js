import BaseModel from '../BaseModel';

export default class RoleDetaiList extends BaseModel {

	static entity = 'roledetaillist'
	static entityName = 'roledetail';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
