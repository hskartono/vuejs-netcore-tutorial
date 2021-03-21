using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class UserInfoRepository:AsyncRepository<UserInfo>, IUserInfoRepository
	{
		public UserInfoRepository(AppDbContext context) : base(context) { }
	}
}
