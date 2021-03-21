import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';

export default class PurchaseRequestDetailFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.purchaseRequestId  = '';
		this.partId  = '';
		this.qty = '';
		this.requestDateFrom = '';
		this.requestDateTo = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '';
		this.draftMode = '';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			purchaseRequestId : '0',
			partId : '0',
			qty : '0',
			requestDateFrom : '0',
			requestDateTo : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}