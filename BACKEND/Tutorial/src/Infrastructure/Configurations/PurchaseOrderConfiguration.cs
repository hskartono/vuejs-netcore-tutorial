using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
	{
		public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();





			builder
				.HasMany(d => d.PurchaseOrderDetails)
				.WithOne(p => p.PurchaseOrder)
				.HasForeignKey(p => p.PurchaseOrderId);


			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
