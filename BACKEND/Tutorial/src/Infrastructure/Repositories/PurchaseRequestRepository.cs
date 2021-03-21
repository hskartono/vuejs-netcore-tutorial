using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class PurchaseRequestRepository : AsyncRepository<PurchaseRequest>, IPurchaseRequestRepository
	{
		#region appgen: private variable
		public PurchaseRequestRepository(AppDbContext context) : base(context) { }
		#endregion

		#region appgen: constructor
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
		#endregion

		#region appgen: generated methods
		public async Task<PurchaseRequest> CloneEntity(int id, string userName)
		{
			var entity = await MyDbContext.Set<PurchaseRequest>()
				.Where(e => e.Id == id)
				.AsNoTracking()
				.SingleOrDefaultAsync();
			entity.Id = 0;
			entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
			entity.MainRecordId = id;
			entity.RecordActionDate = DateTime.Now;
			entity.RecordEditedBy = userName;

			var purchaseRequestDetails = await MyDbContext.Set<PurchaseRequestDetail>()
				.Where(e => e.PurchaseRequest.Id == id)
				.AsNoTracking()
				.ToListAsync();
			entity.ClearPurchaseRequestDetails();
			entity.AddRangePurchaseRequestDetails	(purchaseRequestDetails
				.Select(e => {
					e.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
					e.MainRecordId = e.Id;
					e.RecordActionDate = DateTime.Now;
					e.RecordEditedBy = userName;
					e.Id = 0;
					return e;
				})
				.ToList());


			await MyDbContext.AddAsync(entity);
			await MyDbContext.SaveChangesAsync();
			return entity;
		}
		#endregion
	}
}
