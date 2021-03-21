import BaseModelAPI from '../BaseModelAPI';

export default class PartList extends BaseModelAPI {

	static entity = 'partlist'
	static entityName = 'part';

	constructor() {
		this.pageIndex = 0;
		this.pageSize = 0;
		this.pageCount = 0;
		this.dataCount = 0;
	}
}
