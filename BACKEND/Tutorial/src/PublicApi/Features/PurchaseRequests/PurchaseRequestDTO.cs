using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.PublicApi.Features.Parts;


namespace Tutorial.PublicApi.Features.PurchaseRequests
{
	public class PurchaseRequestDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public DateTime? PrDate { get; set; }
		public string PrNo { get; set; }
		public string Remarks { get; set; }
		public List<PurchaseRequestDetailDTO> PurchaseRequestDetails { get; set; } = new List<PurchaseRequestDetailDTO>();

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
