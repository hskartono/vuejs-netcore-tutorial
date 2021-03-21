using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial.PublicApi.Features.Parts;


namespace Tutorial.PublicApi.Features.PurchaseOrders
{
	public class PurchaseOrderDetailDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string PurchaseOrderId { get; set; }
		public string PurchaseOrderPoNumber { get; set; }
		public PurchaseOrderDTO PurchaseOrder { get; set; }
		public string PartId { get; set; }
		public string PartPartName { get; set; }
		public PartDTO Part { get; set; }
		public double? PartPrice { get; set; }
		public int? Qty { get; set; }
		public double? TotalPrice { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
