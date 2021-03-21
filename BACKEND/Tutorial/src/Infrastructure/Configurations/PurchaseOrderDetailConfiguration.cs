using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
	{
		public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();



			builder
				.HasOne(e => e.PurchaseOrder)
				.WithMany(d => d.PurchaseOrderDetails)
				.HasForeignKey(e => e.PurchaseOrderId);
			builder
				.HasOne(e => e.Part)
				.WithMany()
				.HasForeignKey(d => d.PartId);




			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
