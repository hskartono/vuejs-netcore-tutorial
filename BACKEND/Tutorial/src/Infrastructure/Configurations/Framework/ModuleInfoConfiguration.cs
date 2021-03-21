using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class ModuleInfoConfiguration : IEntityTypeConfiguration<ModuleInfo>
	{
		public void Configure(EntityTypeBuilder<ModuleInfo> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();







			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
