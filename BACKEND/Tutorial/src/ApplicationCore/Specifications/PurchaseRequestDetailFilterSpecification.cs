using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Specifications
{
	public class PurchaseRequestDetailFilterSpecification : Specification<PurchaseRequestDetail>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public PurchaseRequestDetailFilterSpecification() { }

		public PurchaseRequestDetailFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public PurchaseRequestDetailFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.PurchaseRequest);
				Query.Include(e => e.Part);

				#endregion
			}
		}

		public PurchaseRequestDetailFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.PurchaseRequest);
				Query.Include(e => e.Part);

				#endregion
			}
		}

		private int? _skip = null;
		private int? _take = null;
		public PurchaseRequestDetailFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public List<int> PurchaseRequestIds { get; set; } = new List<int>();
		public List<string> PartIds { get; set; } = new List<string>();
		public List<int> Qtys { get; set; } = new List<int>();
		public DateTime? RequestDateFrom { get; set; } = null;
		public DateTime? RequestDateTo { get; set; } = null;

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.All;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public PurchaseRequestDetailFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<PurchaseRequestDetail>> orderby = null)
		{
			Query.Where(e => (!Id.HasValue || e.Id == Id.Value));
			Query.Where(e => (!MainRecordId.HasValue || e.MainRecordId == MainRecordId));
			if(MainRecordIdIsNull)
				Query.Where(e => e.MainRecordId == null);
			Query.Where(e => (string.IsNullOrEmpty(RecordEditedBy) || e.RecordEditedBy == RecordEditedBy));
			Query.Where(e => (!DraftFromUpload.HasValue || e.DraftFromUpload == DraftFromUpload.Value));

			#region appgen: generated query
			if(Ids?.Count > 0)
				Query.Where(e => Ids.Contains(e.Id));

			if(PurchaseRequestIds?.Count > 0)
					Query.Where(e => PurchaseRequestIds.Contains(e.PurchaseRequestId.Value));

			if(PartIds?.Count > 0)
				foreach (var item in PartIds)
					Query.Where(e => PartIds.Contains(e.PartId));

			if(Qtys?.Count > 0)
				Query.Where(e => Qtys.Contains(e.Qty.Value));

			if (RequestDateFrom.HasValue || RequestDateTo.HasValue)
				if (RequestDateFrom.HasValue && RequestDateTo.HasValue)
					Query.Where(e => e.RequestDate >= RequestDateFrom.Value && e.RequestDate <= RequestDateTo.Value);
				else if (RequestDateFrom.HasValue)
					Query.Where(e => e.RequestDate >= RequestDateFrom.Value);
				else if (RequestDateTo.HasValue)
					Query.Where(e => e.RequestDate <= RequestDateTo.Value);


			#endregion

			if(ShowDraftList > BaseEntity.DraftStatus.All)
				Query.Where(e => e.IsDraftRecord == (int)ShowDraftList);

			if(_skip.HasValue && _take.HasValue)
				Query
					.Skip(_skip.Value)
					.Take(_take.Value);

			if (orderby?.Count > 0)
			{
				foreach(var item in orderby)
				{
					if (item.SortType == SortingType.Ascending)
						Query.OrderBy(item.Predicate);
					else
						Query.OrderByDescending(item.Predicate);
				}
			}

			if (withBelongsTo)
			{
				#region appgen: belongsToList
				Query.Include(e => e.PurchaseRequest);
				Query.Include(e => e.Part);

				#endregion
			}

			return this;
		}
		#endregion
	}
}
