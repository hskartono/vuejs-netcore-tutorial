using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class SchedulerCronIntervalRepository : AsyncRepository<SchedulerCronInterval>, ISchedulerCronIntervalRepository
	{
		public SchedulerCronIntervalRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
