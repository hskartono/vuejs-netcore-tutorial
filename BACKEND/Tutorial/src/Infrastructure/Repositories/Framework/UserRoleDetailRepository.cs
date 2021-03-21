using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class UserRoleDetailRepository : AsyncRepository<UserRoleDetail>, IUserRoleDetailRepository
	{
		public UserRoleDetailRepository(AppDbContext context) : base(context) { }
	}
}
