using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.PublicApi.Features.Parts;


namespace Tutorial.PublicApi.Features.PurchaseOrders
{
	public class PurchaseOrderDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string PoNumber { get; set; }
		public DateTime? PoDate { get; set; }
		public string Remarks { get; set; }
		public List<PurchaseOrderDetailDTO> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetailDTO>();

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
