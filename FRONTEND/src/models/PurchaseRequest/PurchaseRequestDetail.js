import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseRequestDetail extends BaseModelAPI {
	static entity = 'purchaserequestdetail';
	static entityName = 'purchaserequestdetail';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.purchaseRequestId = '';
			this.purchaseRequest = {};
			this.partId = '';
			this.part = {};
			this.qty = 0;
			this.requestDate = '';
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}