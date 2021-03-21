using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;


namespace Tutorial.Infrastructure.Configurations
{
	public class PurchaseRequestDetailConfiguration : IEntityTypeConfiguration<PurchaseRequestDetail>
	{
		public void Configure(EntityTypeBuilder<PurchaseRequestDetail> builder)
		{
			builder
				.HasKey(e => e.Id);
			builder
				.Property(e => e.Id)
				.UseIdentityColumn();



			builder
				.HasOne(e => e.PurchaseRequest)
				.WithMany(d => d.PurchaseRequestDetails)
				.HasForeignKey(e => e.PurchaseRequestId);
			builder
				.HasOne(e => e.Part)
				.WithMany()
				.HasForeignKey(d => d.PartId);




			builder
				.HasQueryFilter(m => EF.Property<DateTime?>(m, "DeletedAt") == null);
		}
	}
}
