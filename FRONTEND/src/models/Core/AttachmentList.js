import BaseModel from '../BaseModel';

export default class AttachmentList extends BaseModel {

	static entity = 'attachment_list'
	static entityName = 'attachment';

	static fields () {
		return {
			pageIndex : this.number(null).nullable(),
			pageSize : this.number(null).nullable(),
			pageCount : this.number(null).nullable(),
			dataCount : this.number(null).nullable(),
		}
	}
}
