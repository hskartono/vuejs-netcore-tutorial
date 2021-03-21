using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface ISchedulerCronIntervalService : IAsyncBaseService<SchedulerCronInterval>
	{
        Task<SchedulerCronInterval> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerCronInterval>> ListAllAsync(
            List<SortingInformation<SchedulerCronInterval>> sorting, 
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchedulerCronInterval>> ListAsync(
            ISpecification<SchedulerCronInterval> spec, 
            List<SortingInformation<SchedulerCronInterval>> sorting, 
            CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> AddAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> FirstAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);
        Task<SchedulerCronInterval> FirstOrDefaultAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default);
    }
}
