using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface IRoleService : IAsyncBaseService<Role>
	{
        Task<Role> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Role>> ListAllAsync(List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Role>> ListAsync(ISpecification<Role> spec, List<SortingInformation<Role>> sorting, CancellationToken cancellationToken = default);
        Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Role entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Role entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> FirstAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> FirstOrDefaultAsync(ISpecification<Role> spec, CancellationToken cancellationToken = default);
        Task<Role> GetUserRole(string userName, CancellationToken cancellationToken = default);
    }
}
