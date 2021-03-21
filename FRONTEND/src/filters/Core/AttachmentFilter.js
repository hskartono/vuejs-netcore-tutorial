import Attachment from '@/models/Core/Attachment';

export default class AttachmentFilter {
  
  constructor () {
    
      this.id = "";
      this.originalFilename = "";
      this.savedFileName = "";
      this.fileExtension = "";
      this.fileSize = "";
  }

  get QueryStore() {
		let query = Attachment.query();

		if (this.originalFilename != null && this.originalFilename != "") {
			query = query.where((query) => {
				return query.originalFilename.includes(this.originalFilename);
			});
		}

		return query;
	}
}
