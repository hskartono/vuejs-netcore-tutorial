import BaseModelAPI from '../BaseModelAPI';

export default class Part extends BaseModelAPI {
	static entity = 'part';
	static entityName = 'part';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.partName = '';
			this.description = '';
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}