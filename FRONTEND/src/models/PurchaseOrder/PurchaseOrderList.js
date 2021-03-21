import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseOrderList extends BaseModelAPI {

	static entity = 'purchaseorderlist'
	static entityName = 'purchaseorder';

	constructor() {
		this.pageIndex = 0;
		this.pageSize = 0;
		this.pageCount = 0;
		this.dataCount = 0;
	}
}
