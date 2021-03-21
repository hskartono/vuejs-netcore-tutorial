using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class LookupConfiguration : IEntityTypeConfiguration<Lookup>
	{
		public void Configure(EntityTypeBuilder<Lookup> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).UseIdentityColumn();
			builder.HasMany(e => e.LookupDetails).WithOne(d => d.Lookup).HasForeignKey(d => d.LookupId);
			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
