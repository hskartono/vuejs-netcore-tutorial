import BaseModel from '../BaseModel';

export default class FunctionInfoList extends BaseModel {

	static entity = 'functioninfolist'
	static entityName = 'functioninfo';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
