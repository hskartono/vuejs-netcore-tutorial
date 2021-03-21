using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class SchedulerConfigurationRepositroy : AsyncRepository<SchedulerConfiguration>, ISchedulerConfigurationRepository
	{
		public SchedulerConfigurationRepositroy(AppDbContext context) : base(context) { }
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
