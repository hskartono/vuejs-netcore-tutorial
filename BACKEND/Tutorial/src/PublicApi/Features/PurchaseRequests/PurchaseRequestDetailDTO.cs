using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.PublicApi.Features.Parts;


namespace Tutorial.PublicApi.Features.PurchaseRequests
{
	public class PurchaseRequestDetailDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string PurchaseRequestId { get; set; }
		public string PurchaseRequestPrNo { get; set; }
		public PurchaseRequestDTO PurchaseRequest { get; set; }
		public string PartId { get; set; }
		public string PartPartName { get; set; }
		public PartDTO Part { get; set; }
		public int? Qty { get; set; }
		public DateTime? RequestDate { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
