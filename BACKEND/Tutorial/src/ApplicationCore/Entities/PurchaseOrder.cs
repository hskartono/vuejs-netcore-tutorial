using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class PurchaseOrder : BaseEntity
	{
		#region appgen: generated constructor
		public PurchaseOrder() { }

		public PurchaseOrder(string poNumber, DateTime? poDate, string remarks)
		{
			PoNumber = poNumber;
			PoDate = poDate;
			Remarks = remarks;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public string PoNumber { get; set; }
		public DateTime? PoDate { get; set; }
		public string Remarks { get; set; }
		private IList<PurchaseOrderDetail> _purchaseOrderDetails = new List<PurchaseOrderDetail>();
		public IList<PurchaseOrderDetail> PurchaseOrderDetails { get => _purchaseOrderDetails; set => _purchaseOrderDetails = value; }

		public void AddOrReplacePurchaseOrderDetails(PurchaseOrderDetail entity)
		{
			PurchaseOrderDetail selectedItem = null;
			int index = 0;
			foreach(var item in _purchaseOrderDetails)
			{
				if(item.Id == entity.Id)
				{
					selectedItem = item;
					break;
				}
				index++;
			}

			if(selectedItem == null)
			{
				entity.PurchaseOrder = this;
				entity.PurchaseOrderId = this.Id;
				_purchaseOrderDetails.Add(entity);
			} else
			{
				entity.Id = selectedItem.Id;
				entity.PurchaseOrder = this;
				entity.PurchaseOrderId = this.Id;

				entity.CompanyId = selectedItem.CompanyId;
				entity.CreatedBy = selectedItem.CreatedBy;
				entity.CreatedDate = selectedItem.CreatedDate;
				entity.UpdatedBy = selectedItem.UpdatedBy;
				entity.UpdatedDate = selectedItem.UpdatedDate;

				selectedItem = entity;
				_purchaseOrderDetails[index] = selectedItem;
			}
		}

		public void AddPurchaseOrderDetails(string partId, double? partPrice, int? qty, double? totalPrice)
		{
			var newItem = new PurchaseOrderDetail(partId, partPrice, qty, totalPrice, this);
			_purchaseOrderDetails.Add(newItem);
		}

		public void RemovePurchaseOrderDetails(PurchaseOrderDetail entity)
		{
			var selectedItem = _purchaseOrderDetails.FirstOrDefault(e => e.Id == entity.Id);
			_purchaseOrderDetails.Remove(selectedItem);
		}

		public void ClearPurchaseOrderDetails()
		{
			_purchaseOrderDetails.Clear();
		}

		public void AddRangePurchaseOrderDetails(IList<PurchaseOrderDetail> purchaseOrderDetails)
		{
			this.ClearPurchaseOrderDetails();
			((List<PurchaseOrderDetail>)_purchaseOrderDetails).AddRange(purchaseOrderDetails);
		}

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
