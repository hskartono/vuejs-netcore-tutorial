import ModuleInfo from '@/models/ModuleInfo/ModuleInfo';

export default class ModuleInfoFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.name = '';
		this.parentModuleId = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '0';
		this.draftMode = '0';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			name : '0',
			parentModuleId : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}