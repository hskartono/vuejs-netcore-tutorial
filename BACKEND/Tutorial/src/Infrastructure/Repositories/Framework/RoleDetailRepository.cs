using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class RoleDetailRepository : AsyncRepository<RoleDetail>, IRoleDetailRepository
	{
		public RoleDetailRepository(AppDbContext context) : base(context) { }
	}
}
