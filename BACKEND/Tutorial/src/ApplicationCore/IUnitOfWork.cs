using Tutorial.ApplicationCore.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore
{
	public interface IUnitOfWork : IDisposable
	{
		#region Framework Unit of Work

		Task<int> CommitAsync(CancellationToken cancellationToken = default);
		ICompanyRepository Companies { get; }
		IAttachmentRepository AttachmentRepository { get; }
		IEmailRepository Emails { get; }
		IEmailAttachmentRepository EmailAttachments { get; }
		IFunctionInfoRepository FunctionInfoRepository { get; }
		IRoleDetailRepository RoleDetailRepository { get; }
		IRoleRepository RoleRepository { get; }
		IUserInfoRepository UserInfoRepository { get; }
		IUserRoleDetailRepository UserRoleDetailRepository { get; }
		IUserRoleRepository UserRoleRepository { get; }
		ISchedulerCronIntervalRepository SchedulerCronIntervalRepository { get; }
		IJobConfigurationRepository JobConfigurationRepository { get; }
		ISchedulerConfigurationRepository SchedulerConfigurationRepository { get; }
		IDownloadProcessRepository DownloadProcessRepository { get; }
		ILookupRepository LookupRepository { get; }
		ILookupDetailRepository LookupDetailRepository { get; }

		IGenericRepository GenericRepository { get; }
		IModuleInfoRepository ModuleInfoRepository { get; }
		#endregion

		// do not remove region marker. this marker is used by code generator
		#region Application

			IPartRepository PartRepository { get; }
			IPurchaseOrderRepository PurchaseOrderRepository { get; }
			IPurchaseOrderDetailRepository PurchaseOrderDetailRepository { get; }
			IPurchaseRequestRepository PurchaseRequestRepository { get; }
			IPurchaseRequestDetailRepository PurchaseRequestDetailRepository { get; }
		#endregion
	}
}
