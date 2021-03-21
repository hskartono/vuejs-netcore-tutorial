using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class UserRoleRepository : AsyncRepository<UserRole>, IUserRoleRepository
	{
		public UserRoleRepository(AppDbContext context) : base(context) { }
	}
}
