using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class RoleDetailConfiguration : IEntityTypeConfiguration<RoleDetail>
	{
		public void Configure(EntityTypeBuilder<RoleDetail> builder)
		{
			builder
				.HasKey(p => p.Id);

			builder
				.Property(p => p.Id)
				.UseIdentityColumn();

			builder
				.HasOne(p => p.Role)
				.WithMany(d => d.RoleDetails)
				.HasForeignKey(p => p.RoleId);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
