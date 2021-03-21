using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class DownloadProcessRepository : AsyncRepository<DownloadProcess>, IDownloadProcessRepository
	{
		public DownloadProcessRepository(AppDbContext context) : base(context) { }
	}
}
