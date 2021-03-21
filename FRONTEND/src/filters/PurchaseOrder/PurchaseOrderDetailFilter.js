import PurchaseOrderDetail from '@/models/PurchaseOrder/PurchaseOrderDetail';

export default class PurchaseOrderDetailFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.purchaseOrderId  = '';
		this.partId  = '';
		this.partPrice = '';
		this.qty = '';
		this.totalPrice = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '';
		this.draftMode = '';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			purchaseOrderId : '0',
			partId : '0',
			partPrice : '0',
			qty : '0',
			totalPrice : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}