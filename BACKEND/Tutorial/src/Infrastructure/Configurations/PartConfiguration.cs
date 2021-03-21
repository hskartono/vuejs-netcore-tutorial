using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class PartConfiguration : IEntityTypeConfiguration<Part>
	{
		public void Configure(EntityTypeBuilder<Part> builder)
		{
			builder
				.HasKey(e => e.Id);






			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
