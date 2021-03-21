import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseOrder extends BaseModelAPI {
	static entity = 'purchaseorder';
	static entityName = 'purchaseorder';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.poNumber = '';
			this.poDate = '';
			this.remarks = '';
			this.purchaseOrderDetails = []
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}