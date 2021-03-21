import BaseModelAPI from '../BaseModelAPI';

export default class ModuleInfo extends BaseModelAPI {
	static entity = 'moduleinfo';
	static entityName = 'moduleinfo';

	constructor(){

	/*APP_MODEL_DEFINITION*/
			this.id = '';
			this.name = '';
			this.parentModuleId = 0;
			this.isFromUpload = false;
			this.isSavedToBackend = true;
			this.uploadValidationMessage = '';
			this.uploadValidationStatus = '';
	/*END_APP_MODEL_DEFINITION*/
	}

}