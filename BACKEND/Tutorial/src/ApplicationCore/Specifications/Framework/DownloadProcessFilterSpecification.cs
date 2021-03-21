using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;

namespace Tutorial.ApplicationCore.Specifications
{
	public class DownloadProcessFilterSpecification : Specification<DownloadProcess>
	{
		public DownloadProcessFilterSpecification(int id, bool withFunctionInfo = false)
		{
			InitializeSpecification(id: id, withFunctionInfo: withFunctionInfo);
		}

		public DownloadProcessFilterSpecification(int? skip, int? take)
		{
			InitializeSpecification(skip, take);
		}

		public DownloadProcessFilterSpecification(string jobId, string functionId, string status)
		{
			InitializeSpecification(jobId: jobId, functionId: functionId, status: status);
		}

		public DownloadProcessFilterSpecification(int? skip, int? take, string jobId, string functionId, string status)
		{
			InitializeSpecification(skip, take, jobId, functionId, status);
		}

		private void InitializeSpecification(int? skip = null, int? take = null, string jobId = "", string functionId = "", string status = "", int? id = null, bool withFunctionInfo = false)
		{
			Query
				.Where(e => (string.IsNullOrEmpty(jobId) || e.JobId == jobId) &&
					(string.IsNullOrEmpty(functionId) || e.FunctionId == functionId) &&
					(string.IsNullOrEmpty(status) || e.Status == status) &&
					(!id.HasValue || e.Id == id)
				);

			if (skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);

			if (withFunctionInfo)
				Query
					.Include(p => p.FunctionInfo);
		}
	}
}
