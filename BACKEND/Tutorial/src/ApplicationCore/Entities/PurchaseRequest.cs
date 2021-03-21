using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class PurchaseRequest : BaseEntity
	{
		#region appgen: generated constructor
		public PurchaseRequest() { }

		public PurchaseRequest(DateTime? prDate, string prNo, string remarks)
		{
			PrDate = prDate;
			PrNo = prNo;
			Remarks = remarks;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public DateTime? PrDate { get; set; }
		public string PrNo { get; set; }
		public string Remarks { get; set; }
		private IList<PurchaseRequestDetail> _purchaseRequestDetails = new List<PurchaseRequestDetail>();
		public IList<PurchaseRequestDetail> PurchaseRequestDetails { get => _purchaseRequestDetails; set => _purchaseRequestDetails = value; }

		public void AddOrReplacePurchaseRequestDetails(PurchaseRequestDetail entity)
		{
			PurchaseRequestDetail selectedItem = null;
			int index = 0;
			foreach(var item in _purchaseRequestDetails)
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
				entity.PurchaseRequest = this;
				entity.PurchaseRequestId = this.Id;
				_purchaseRequestDetails.Add(entity);
			} else
			{
				entity.Id = selectedItem.Id;
				entity.PurchaseRequest = this;
				entity.PurchaseRequestId = this.Id;

				entity.CompanyId = selectedItem.CompanyId;
				entity.CreatedBy = selectedItem.CreatedBy;
				entity.CreatedDate = selectedItem.CreatedDate;
				entity.UpdatedBy = selectedItem.UpdatedBy;
				entity.UpdatedDate = selectedItem.UpdatedDate;

				selectedItem = entity;
				_purchaseRequestDetails[index] = selectedItem;
			}
		}

		public void AddPurchaseRequestDetails(string partId, int? qty, DateTime? requestDate)
		{
			var newItem = new PurchaseRequestDetail(partId, qty, requestDate, this);
			_purchaseRequestDetails.Add(newItem);
		}

		public void RemovePurchaseRequestDetails(PurchaseRequestDetail entity)
		{
			var selectedItem = _purchaseRequestDetails.FirstOrDefault(e => e.Id == entity.Id);
			_purchaseRequestDetails.Remove(selectedItem);
		}

		public void ClearPurchaseRequestDetails()
		{
			_purchaseRequestDetails.Clear();
		}

		public void AddRangePurchaseRequestDetails(IList<PurchaseRequestDetail> purchaseRequestDetails)
		{
			this.ClearPurchaseRequestDetails();
			((List<PurchaseRequestDetail>)_purchaseRequestDetails).AddRange(purchaseRequestDetails);
		}

		public int? MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
