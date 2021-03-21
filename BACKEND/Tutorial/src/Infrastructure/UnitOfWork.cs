using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.Infrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;

		#region Framework private variable

		private IAttachmentRepository attachmentRepository;
		private ICompanyRepository companyRepository;
		private IEmailRepository emailRepository;
		private IEmailAttachmentRepository emailAttachmentRepository;
		private IFunctionInfoRepository functionInfoRepository;
		private IRoleRepository roleRepository;
		private IRoleDetailRepository roleDetailRepository;
		private IUserInfoRepository userInfoRepository;
		private IUserRoleRepository userRoleRepository;
		private IUserRoleDetailRepository userRoleDetailRepository;
		private ISchedulerCronIntervalRepository schedulerCronIntervalRepository;
		private IJobConfigurationRepository jobConfigurationRepository;
		private ISchedulerConfigurationRepository schedulerConfigurationRepository;
		private IDownloadProcessRepository downloadProcessRepository;
		private ILookupRepository lookupRepository;
		private ILookupDetailRepository lookupDetailRepository;
		private IGenericRepository genericRepository;
		private IModuleInfoRepository moduleInfoRepository;

		#endregion

		// do not remove region marker. this marker is used by code generator
		#region Application private variable

		#endregion

		#region Framework properties

		public ICompanyRepository Companies =>
			companyRepository = companyRepository ?? new CompanyRepository(_context);

		public IAttachmentRepository AttachmentRepository =>
			attachmentRepository = attachmentRepository ?? new AttachmentRepository(_context);

		public IEmailRepository Emails =>
			emailRepository = emailRepository ?? new EmailRepository(_context);

		public IEmailAttachmentRepository EmailAttachments =>
			emailAttachmentRepository = emailAttachmentRepository ?? new EmailAttachmentRepository(_context);

		public IFunctionInfoRepository FunctionInfoRepository =>
			functionInfoRepository = functionInfoRepository ?? new FunctionInfoRepository(_context);

		public IRoleDetailRepository RoleDetailRepository =>
			roleDetailRepository = roleDetailRepository ?? new RoleDetailRepository(_context);

		public IRoleRepository RoleRepository =>
			roleRepository = roleRepository ?? new RoleRepository(_context);

		IUserInfoRepository IUnitOfWork.UserInfoRepository =>
			userInfoRepository = userInfoRepository ?? new UserInfoRepository(_context);

		IUserRoleDetailRepository IUnitOfWork.UserRoleDetailRepository =>
			userRoleDetailRepository = userRoleDetailRepository ?? new UserRoleDetailRepository(_context);

		public IUserRoleRepository UserRoleRepository =>
			userRoleRepository = userRoleRepository ?? new UserRoleRepository(_context);

		public ISchedulerCronIntervalRepository SchedulerCronIntervalRepository =>
			schedulerCronIntervalRepository = schedulerCronIntervalRepository ?? new SchedulerCronIntervalRepository(_context);

		public IJobConfigurationRepository JobConfigurationRepository =>
			jobConfigurationRepository = jobConfigurationRepository ?? new JobConfigurationRepository(_context);

		public ISchedulerConfigurationRepository SchedulerConfigurationRepository =>
			schedulerConfigurationRepository = schedulerConfigurationRepository ?? new SchedulerConfigurationRepositroy(_context);

		public IDownloadProcessRepository DownloadProcessRepository =>
			downloadProcessRepository = downloadProcessRepository ?? new DownloadProcessRepository(_context);

		public ILookupRepository LookupRepository => lookupRepository = lookupRepository ?? new LookupRepository(_context);
		public ILookupDetailRepository LookupDetailRepository => lookupDetailRepository = lookupDetailRepository ?? new LookupDetailRepository(_context);

		public IModuleInfoRepository ModuleInfoRepository => moduleInfoRepository = moduleInfoRepository ?? new ModuleInfoRepository(_context);

		public IGenericRepository GenericRepository => genericRepository = genericRepository ?? new GenericRepository(_context);

		#endregion

		// do not remove region marker. this marker is used by code generator
		#region Application private properties

		#endregion

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
		}

		public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
		{
			return await _context.SaveChangesAsync(cancellationToken);
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
