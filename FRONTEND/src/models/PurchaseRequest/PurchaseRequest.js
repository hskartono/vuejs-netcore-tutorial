import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseRequest extends BaseModelAPI {
	static entity = 'purchaserequest';
	static entityName = 'purchaserequest';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.prDate = '';
			this.prNo = '';
			this.remarks = '';
			this.purchaseRequestDetails = []
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}