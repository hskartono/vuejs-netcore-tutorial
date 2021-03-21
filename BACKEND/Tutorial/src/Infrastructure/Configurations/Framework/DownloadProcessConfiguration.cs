using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class DownloadProcessConfiguration : IEntityTypeConfiguration<DownloadProcess>
	{
		public void Configure(EntityTypeBuilder<DownloadProcess> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.HasOne(p => p.FunctionInfo)
				.WithMany()
				.HasForeignKey(p => p.FunctionId);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
