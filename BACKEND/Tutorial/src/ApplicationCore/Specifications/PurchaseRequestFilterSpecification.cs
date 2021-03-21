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
	public class PurchaseRequestFilterSpecification : Specification<PurchaseRequest>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public PurchaseRequestFilterSpecification() { }

		public PurchaseRequestFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public PurchaseRequestFilterSpecification(int? id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public PurchaseRequestFilterSpecification(List<int> ids, bool withBelongsTo = true)
		{
			Query.Where(e => ids.Contains(e.Id));

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		private int? _skip = null;
		private int? _take = null;
		public PurchaseRequestFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public int? Id { get; set; }
		public List<int> Ids { get; set; } = new List<int>();
		public DateTime? PrDateFrom { get; set; } = null;
		public DateTime? PrDateTo { get; set; } = null;
		public List<string> PrNos { get; set; } = new List<string>();
		public List<string> Remarkss { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public int? MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public PurchaseRequestFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<PurchaseRequest>> orderby = null)
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

			if (PrDateFrom.HasValue || PrDateTo.HasValue)
				if (PrDateFrom.HasValue && PrDateTo.HasValue)
					Query.Where(e => e.PrDate >= PrDateFrom.Value && e.PrDate <= PrDateTo.Value);
				else if (PrDateFrom.HasValue)
					Query.Where(e => e.PrDate >= PrDateFrom.Value);
				else if (PrDateTo.HasValue)
					Query.Where(e => e.PrDate <= PrDateTo.Value);

			if(PrNos?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("prno") && _exact["prno"] == 1)
				{
					Query.Where(e => PrNos.Contains(e.PrNo));
				}
				else
				{
					var predicate = PredicateBuilder.False<PurchaseRequest>();
					foreach (var item in PrNos)
						predicate = predicate.Or(p => p.PrNo.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Remarkss?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("remarks") && _exact["remarks"] == 1)
				{
					Query.Where(e => Remarkss.Contains(e.Remarks));
				}
				else
				{
					var predicate = PredicateBuilder.False<PurchaseRequest>();
					foreach (var item in Remarkss)
						predicate = predicate.Or(p => p.Remarks.Contains(item));

					Query.Where(predicate);
				}
			}


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

				#endregion
			}

			return this;
		}
		#endregion
	}
}
