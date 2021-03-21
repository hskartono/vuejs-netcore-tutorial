using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;

namespace Tutorial.Infrastructure.Repositories
{
	public class EmailRepository : AsyncRepository<Email>, IEmailRepository
	{
		public EmailRepository(AppDbContext context) : base(context) { }

		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
	}
}
