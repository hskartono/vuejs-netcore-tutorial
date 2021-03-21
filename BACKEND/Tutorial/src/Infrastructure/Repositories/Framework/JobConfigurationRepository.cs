using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class JobConfigurationRepository : AsyncRepository<JobConfiguration>, IJobConfigurationRepository
	{
		public JobConfigurationRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
