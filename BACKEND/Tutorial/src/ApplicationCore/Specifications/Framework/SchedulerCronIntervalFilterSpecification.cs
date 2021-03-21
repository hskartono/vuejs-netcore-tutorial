using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;

namespace Tutorial.ApplicationCore.Specifications
{
	public class SchedulerCronIntervalFilterSpecification : Specification<SchedulerCronInterval>
	{

		public SchedulerCronIntervalFilterSpecification(int id)
		{
			InitializeFilterData(id: id);
		}

		public SchedulerCronIntervalFilterSpecification(int? skip, int? take)
		{
			InitializeFilterData(skip, take);
		}

		public SchedulerCronIntervalFilterSpecification(string code, string name)
		{
			InitializeFilterData(code: code, name: name);
		}

		public SchedulerCronIntervalFilterSpecification(int? skip, int? take, string code, string name)
		{
			InitializeFilterData(skip, take, code, name);
		}

		private void InitializeFilterData(int? skip = null, int? take = null, string code = "", string name = "", int? id = null)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(code) || e.Code == code) &&
				(string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
				(!id.HasValue || e.Id == id)
			);

			if (take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
