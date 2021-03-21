import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseRequestList extends BaseModelAPI {

	static entity = 'purchaserequestlist'
	static entityName = 'purchaserequest';

	constructor() {
		this.pageIndex = 0;
		this.pageSize = 0;
		this.pageCount = 0;
		this.dataCount = 0;
	}
}
