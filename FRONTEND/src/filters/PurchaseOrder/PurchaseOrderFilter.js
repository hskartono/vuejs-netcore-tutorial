import PurchaseOrder from '@/models/PurchaseOrder/PurchaseOrder';

export default class PurchaseOrderFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.poNumber = '';
		this.poDateFrom = '';
		this.poDateTo = '';
		this.remarksMultiple = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '';
		this.draftMode = '';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			poNumber : '0',
			poDateFrom : '0',
			poDateTo : '0',
			remarksMultiple : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}