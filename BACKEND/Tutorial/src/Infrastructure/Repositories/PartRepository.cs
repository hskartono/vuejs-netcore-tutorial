using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class PartRepository : AsyncRepository<Part>, IPartRepository
	{
		#region appgen: private variable
		public PartRepository(AppDbContext context) : base(context) { }
		#endregion

		#region appgen: constructor
		private AppDbContext MyDbContext
		{
			get { return Context as AppDbContext; }
		}
		#endregion

		#region appgen: generated methods
		public async Task<Part> CloneEntity(string id, string userName)
		{
			var entity = await MyDbContext.Set<Part>()
				.Where(e => e.Id == id)
				.AsNoTracking()
				.SingleOrDefaultAsync();
			entity.Id = "";
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
