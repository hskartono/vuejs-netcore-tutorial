using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
	{
		public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();





			builder
				.HasMany(d => d.PurchaseRequestDetails)
				.WithOne(p => p.PurchaseRequest)
				.HasForeignKey(p => p.PurchaseRequestId);


			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
