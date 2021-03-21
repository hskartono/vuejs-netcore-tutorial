using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class PurchaseOrderDetail : BaseEntity
	{
		#region appgen: generated constructor
		public PurchaseOrderDetail() { }

		public PurchaseOrderDetail(string partId, double? partPrice, int? qty, double? totalPrice, PurchaseOrder parent)
		{
			PartId = partId;
			PartPrice = partPrice;
			Qty = qty;
			TotalPrice = totalPrice;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public int? PurchaseOrderId { get; set; }
		public virtual PurchaseOrder PurchaseOrder { get; set; }
		public string PartId { get; set; }
		public virtual Part Part { get; set; }
		public double? PartPrice { get; set; }
		public int? Qty { get; set; }
		public double? TotalPrice { get; set; }

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
