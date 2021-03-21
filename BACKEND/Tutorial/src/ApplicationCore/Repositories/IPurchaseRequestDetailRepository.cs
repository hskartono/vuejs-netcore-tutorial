using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IPurchaseRequestDetailRepository : IAsyncRepository<PurchaseRequestDetail>
	{
		#region appgen: repository method
		Task<PurchaseRequestDetail> CloneEntity(int id, string userName);
		#endregion
	}
}
