using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure
{
	public class AppDbContext : DbContext
	{
		#region Framework

		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Email> Emails { get; set; }
		public DbSet<EmailAttachment> EmailAttachments { get; set; }
		public DbSet<FunctionInfo> FunctionInfos { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<RoleDetail> RoleDetails { get; set; }
		public DbSet<UserInfo> UserInfos { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<UserRoleDetail> UserRoleDetails { get; set; }
		public DbSet<SchedulerCronInterval> SchedulerCronIntervals { get; set; }
		public DbSet<JobConfiguration> JobConfigurations { get; set; }
		public DbSet<SchedulerConfiguration> SchedulerConfigurations { get; set; }
		public DbSet<DownloadProcess> DownloadProcesses { get; set; }
		public DbSet<Lookup> Lookups { get; set; }
		public DbSet<LookupDetail> LookupDetails { get; set; }
		public DbSet<ModuleInfo> ModuleInfos { get; set; }
		public DbSet<SpResultNumberGenerator> spResultNumberGenerators { get; set; }
		#endregion

		// do not remove region marker. this marker is used by code generator
		#region Application Variables

			public DbSet<Part> Parts { get; set; }
			public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
			public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
			public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
			public DbSet<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
		#endregion

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

		public override int SaveChanges()
		{
			UpdateSoftDeleteStatuses();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			UpdateSoftDeleteStatuses();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void UpdateSoftDeleteStatuses()
		{
			foreach (var entry in ChangeTracker.Entries())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.CurrentValues["DeletedAt"] = null;
						break;

					case EntityState.Deleted:
						entry.State = EntityState.Modified;
						entry.CurrentValues["DeletedAt"] = DateTime.Now;
						break;
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region Framework

			modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
			modelBuilder.ApplyConfiguration(new CompanyConfiguration());
			modelBuilder.ApplyConfiguration(new EmailConfiguration());
			modelBuilder.ApplyConfiguration(new EmailAttachmentConfiguration());
			modelBuilder.ApplyConfiguration(new FunctionInfoConfiguration());
			modelBuilder.ApplyConfiguration(new RoleConfiguration());
			modelBuilder.ApplyConfiguration(new RoleDetailConfiguration());
			modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
			modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
			modelBuilder.ApplyConfiguration(new UserRoleDetailConfiguration());
			modelBuilder.ApplyConfiguration(new SchedulerCronIntervalConfiguration());
			modelBuilder.ApplyConfiguration(new JobConfigurationConfiguration());
			modelBuilder.ApplyConfiguration(new SchedulerConfigurationConfiguration());
			modelBuilder.ApplyConfiguration(new DownloadProcessConfiguration());
			modelBuilder.ApplyConfiguration(new LookupConfiguration());
			modelBuilder.ApplyConfiguration(new LookupDetailConfiguration());
			modelBuilder.ApplyConfiguration(new ModuleInfoConfiguration());
			modelBuilder.ApplyConfiguration(new SpResultNumberGeneratorConfiguration());
			#endregion

			// do not remove region marker. this marker is used by code generator
			#region Application Config

			modelBuilder.ApplyConfiguration(new PartConfiguration());
			modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
			modelBuilder.ApplyConfiguration(new PurchaseOrderDetailConfiguration());
			modelBuilder.ApplyConfiguration(new PurchaseRequestConfiguration());
			modelBuilder.ApplyConfiguration(new PurchaseRequestDetailConfiguration());
			#endregion
		}
	}
}
