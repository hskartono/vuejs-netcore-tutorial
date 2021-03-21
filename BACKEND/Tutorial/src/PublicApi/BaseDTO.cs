using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.PublicApi
{
	public class BaseDTO
	{
		public virtual string UploadValidationStatus { get; set; }
		public virtual string UploadValidationMessage { get; set; }
		public virtual bool isFromUpload { get; set; }
		public int? MainRecordId { get; set; }
	}
}
