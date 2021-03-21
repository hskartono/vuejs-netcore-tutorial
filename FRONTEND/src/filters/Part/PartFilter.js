import Part from '@/models/Part/Part';

export default class PartFilter {

	constructor() {
	/*APP_FILTER_DEFINITION*/
		this.id = '';
		this.partName = '';
		this.description = '';
		this.isFromUpload = false;
		this.isSavedToBackend = false;
		this.draftFromUpload = '';
		this.draftMode = '';
	/*END_APP_FILTER_DEFINITION*/
		this.exactType = {
	/*APP_APP_FILTER_CRITERIA*/
			id : '0',
			partName : '0',
			description : '0',
			isFromUpload : '0',
			isSavedToBackend : '0',
	/*END_APP_FILTER_CRITERIA*/
		};
	}

}