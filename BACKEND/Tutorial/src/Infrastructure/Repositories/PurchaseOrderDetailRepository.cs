using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class PurchaseOrderDetailRepository : AsyncRepository<PurchaseOrderDetail>, IPurchaseOrderDetailRepository
	{
		#region appgen: private variable
		public PurchaseOrderDetailRepository(AppDbContext context) : base(context) { }
		#endregion

		#region appgen: constructor
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
		#endregion

		#region appgen: generated methods
		public async Task<PurchaseOrderDetail> CloneEntity(int id, string userName)
		{
			var entity = await MyDbContext.Set<PurchaseOrderDetail>()
				.Where(e => e.Id == id)
				.AsNoTracking()
				.SingleOrDefaultAsync();
			entity.Id = 0;
			entity.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
			entity.MainRecordId = id;
			entity.RecordActionDate = DateTime.Now;
			entity.RecordEditedBy = userName;



			await MyDbContext.AddAsync(entity);
			await MyDbContext.SaveChangesAsync();
			return entity;
		}
		#endregion
	}
}
