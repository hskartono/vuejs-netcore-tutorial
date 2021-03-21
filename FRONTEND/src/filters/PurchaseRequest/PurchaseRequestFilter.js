import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';

export default class PurchaseRequestFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.prDateFrom = '';
		this.prDateTo = '';
		this.prNo = '';
		this.remarksMultiple = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '';
		this.draftMode = '';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			prDateFrom : '0',
			prDateTo : '0',
			prNo : '0',
			remarksMultiple : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}