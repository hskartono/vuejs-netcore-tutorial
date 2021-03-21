using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Specifications
{
	public class JobConfigurationFilterSpecification : Specification<JobConfiguration>
	{
		public JobConfigurationFilterSpecification(int id)
		{
			InitializeFilterData(id: id);
		}

		public JobConfigurationFilterSpecification(int? skip, int? take)
		{
			InitializeFilterData(skip, take);
		}

		public JobConfigurationFilterSpecification(string interfaceName, string jobName, bool? isStoredProcedure)
		{
			InitializeFilterData(interfaceName: interfaceName, jobName: jobName, isStoredProcedure: isStoredProcedure);
		}

		public JobConfigurationFilterSpecification(int? skip, int? take, string interfaceName, string jobName, bool? isStoredProcedure)
		{
			InitializeFilterData(skip, take, interfaceName, jobName, isStoredProcedure);
		}

		private void InitializeFilterData(int? skip = null, int? take = null, string interfaceName = "", string jobName = "", bool? isStoredProcedure = null, int? id = null)
		{
			Query
				.Where(
				e => (string.IsNullOrEmpty(interfaceName) || e.InterfaceName.Contains(interfaceName)) &&
				(string.IsNullOrEmpty(jobName) || e.JobName.Contains(jobName)) &&
				(!isStoredProcedure.HasValue || e.IsStoredProcedure == isStoredProcedure) &&
				(!id.HasValue || e.Id == id)
			);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
