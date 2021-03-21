using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface IJobConfigurationService : IAsyncBaseService<JobConfiguration>
	{
        Task<JobConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobConfiguration>> ListAllAsync(List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<JobConfiguration>> ListAsync(ISpecification<JobConfiguration> spec, List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<JobConfiguration> AddAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(JobConfiguration entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);
        Task<JobConfiguration> FirstAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);
        Task<JobConfiguration> FirstOrDefaultAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default);
    }
}
