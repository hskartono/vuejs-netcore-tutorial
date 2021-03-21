using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IPurchaseOrderDetailRepository : IAsyncRepository<PurchaseOrderDetail>
	{
		#region appgen: repository method
		Task<PurchaseOrderDetail> CloneEntity(int id, string userName);
		#endregion
	}
}
