using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class CompanyConfiguration : IEntityTypeConfiguration<Company>
	{
		public void Configure(EntityTypeBuilder<Company> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.Property(p => p.Name)
				.HasMaxLength(100);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
