using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class JobConfigurationConfiguration : IEntityTypeConfiguration<JobConfiguration>
	{
		public void Configure(EntityTypeBuilder<JobConfiguration> builder)
		{
			builder
				.HasKey(p => p.Id);
			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
