using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface ISchedulerConfigurationService : IAsyncBaseService<SchedulerConfiguration>
	{
        Task<SchedulerConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerConfiguration>> ListAllAsync(List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerConfiguration>> ListAsync(ISpecification<SchedulerConfiguration> spec, List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> AddAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> FirstAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);
        Task<SchedulerConfiguration> FirstOrDefaultAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default);
    }
}
