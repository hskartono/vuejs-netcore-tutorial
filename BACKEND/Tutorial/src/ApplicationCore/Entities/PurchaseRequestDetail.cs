using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class PurchaseRequestDetail : BaseEntity
	{
		#region appgen: generated constructor
		public PurchaseRequestDetail() { }

		public PurchaseRequestDetail(string partId, int? qty, DateTime? requestDate, PurchaseRequest parent)
		{
			PartId = partId;
			Qty = qty;
			RequestDate = requestDate;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public int? PurchaseRequestId { get; set; }
		public virtual PurchaseRequest PurchaseRequest { get; set; }
		public string PartId { get; set; }
		public virtual Part Part { get; set; }
		public int? Qty { get; set; }
		public DateTime? RequestDate { get; set; }

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
