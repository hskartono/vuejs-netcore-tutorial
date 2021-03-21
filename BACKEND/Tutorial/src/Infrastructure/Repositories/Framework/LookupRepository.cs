using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class LookupRepository : AsyncRepository<Lookup>, ILookupRepository
	{
		public LookupRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
