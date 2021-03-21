using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class FunctionInfoRepository : AsyncRepository<FunctionInfo>, IFunctionInfoRepository
	{
		public FunctionInfoRepository(AppDbContext context) : base(context) { }
	}
}
