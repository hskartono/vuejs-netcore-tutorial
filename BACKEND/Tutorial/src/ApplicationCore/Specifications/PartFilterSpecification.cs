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
	public class PartFilterSpecification : Specification<Part>
	{
		private Dictionary<string, int> _exact = null;

		#region appgen: predefined constructor
		public PartFilterSpecification() { }

		public PartFilterSpecification(Dictionary<string, int> exact) 
		{
			_exact = exact;
		}

		public PartFilterSpecification(string id, bool withBelongsTo = true)
		{
			Query.Where(e => e.Id == id);

			if (withBelongsTo)
			{
				#region appgen: belongsToList

				#endregion
			}
		}

		public PartFilterSpecification(List<string> ids, bool withBelongsTo = true)
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
		public PartFilterSpecification(int skip, int take, Dictionary<string, int> exact = null)
		{
			_skip = skip;
			_take = take;
		}
		#endregion

		#region appgen: generated property list
		public string Id { get; set; }
		public List<string> Ids { get; set; } = new List<string>();
		public List<string> PartNames { get; set; } = new List<string>();
		public List<string> Descriptions { get; set; } = new List<string>();

		#endregion

		#region appgen: recovery property list
		public BaseEntity.DraftStatus ShowDraftList { get; set; } = BaseEntity.DraftStatus.MainRecord;
		public string MainRecordId { get; set; } = null;
		public bool MainRecordIdIsNull { get; set; } = false;
		public string RecordEditedBy { get; set; } = string.Empty;
		public bool? DraftFromUpload { get; set; }
		#endregion

		#region appgen: buildspecification method
		public PartFilterSpecification BuildSpecification(bool withBelongsTo = true, List<SortingInformation<Part>> orderby = null)
		{
			Query.Where(e => (string.IsNullOrEmpty(Id) || e.Id == Id));
			Query.Where(e => (string.IsNullOrEmpty(MainRecordId) || e.MainRecordId == MainRecordId));
			if(MainRecordIdIsNull)
				Query.Where(e => e.MainRecordId == null);
			Query.Where(e => (string.IsNullOrEmpty(RecordEditedBy) || e.RecordEditedBy == RecordEditedBy));
			Query.Where(e => (!DraftFromUpload.HasValue || e.DraftFromUpload == DraftFromUpload.Value));

			#region appgen: generated query
			if(Ids?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("id") && _exact["id"] == 1)
				{
					Query.Where(e => Ids.Contains(e.Id));
				}
				else
				{
					var predicate = PredicateBuilder.False<Part>();
					foreach (var item in Ids)
						predicate = predicate.Or(p => p.Id.Contains(item));

					Query.Where(predicate);
				}
			}

			if(PartNames?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("partname") && _exact["partname"] == 1)
				{
					Query.Where(e => PartNames.Contains(e.PartName));
				}
				else
				{
					var predicate = PredicateBuilder.False<Part>();
					foreach (var item in PartNames)
						predicate = predicate.Or(p => p.PartName.Contains(item));

					Query.Where(predicate);
				}
			}

			if(Descriptions?.Count > 0)
			{
				if (_exact != null && _exact.ContainsKey("description") && _exact["description"] == 1)
				{
					Query.Where(e => Descriptions.Contains(e.Description));
				}
				else
				{
					var predicate = PredicateBuilder.False<Part>();
					foreach (var item in Descriptions)
						predicate = predicate.Or(p => p.Description.Contains(item));

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
