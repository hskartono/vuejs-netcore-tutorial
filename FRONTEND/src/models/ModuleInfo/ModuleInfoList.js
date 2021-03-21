import BaseModelAPI from '../BaseModelAPI';

export default class ModuleInfoList extends BaseModelAPI {

	static entity = 'moduleinfolist'
	static entityName = 'moduleinfo';

	constructor() {
		this.pageIndex = 0;
		this.pageSize = 0;
		this.pageCount = 0;
		this.dataCount = 0;
	}
}
