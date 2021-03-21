using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class RoleRepository : AsyncRepository<Role>, IRoleRepository
	{
		public RoleRepository(AppDbContext context) : base(context) { }
	}
}
