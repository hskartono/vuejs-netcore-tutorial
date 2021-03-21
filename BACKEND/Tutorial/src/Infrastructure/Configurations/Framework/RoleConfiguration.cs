using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;

namespace Tutorial.Infrastructure.Configurations
{
	public class RoleConfiguration : IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
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
				.Property(p => p.Description)
				.HasMaxLength(500);

			builder
				.HasMany(p => p.RoleDetails)
				.WithOne(d => d.Role)
				.HasForeignKey(d => d.RoleId);

			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
