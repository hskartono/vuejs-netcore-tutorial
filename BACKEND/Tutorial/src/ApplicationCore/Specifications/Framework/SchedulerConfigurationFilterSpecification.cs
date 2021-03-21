using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Specifications
{
	public class SchedulerConfigurationFilterSpecification : Specification<SchedulerConfiguration>
	{
		public SchedulerConfigurationFilterSpecification(int id)
		{
			InitializeSpecification(id: id);
		}

		public SchedulerConfigurationFilterSpecification(int skip, int take)
		{
			InitializeSpecification(skip, take);
		}

		public SchedulerConfigurationFilterSpecification(string jobType, string recurringJobId)
		{
			InitializeSpecification(jobType: jobType, recurringJobId: recurringJobId);
		}

		public SchedulerConfigurationFilterSpecification(int skip, int take, string jobType, string recurringJobId)
		{
			InitializeSpecification(skip, take, jobType, recurringJobId);
		}

		private void InitializeSpecification(int? skip = null, int? take = null, string jobType = "", string recurringJobId = "", int? id = null)
		{
			Query
				.Where(
				e => (string.IsNullOrEmpty(jobType) || e.JobType.Contains(jobType)) &&
				(string.IsNullOrEmpty(recurringJobId) || e.RecurringJobId == recurringJobId) &&
				(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
