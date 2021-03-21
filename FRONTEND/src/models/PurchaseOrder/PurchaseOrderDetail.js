import BaseModelAPI from '../BaseModelAPI';

export default class PurchaseOrderDetail extends BaseModelAPI {
	static entity = 'purchaseorderdetail';
	static entityName = 'purchaseorderdetail';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.purchaseOrderId = '';
			this.purchaseOrder = {};
			this.partId = '';
			this.part = {};
			this.partPrice = 0;
			this.qty = 0;
			this.totalPrice = 0;
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}