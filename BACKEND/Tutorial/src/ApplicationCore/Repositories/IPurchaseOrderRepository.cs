using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public partial interface IPurchaseOrderRepository : IAsyncRepository<PurchaseOrder>
	{
		#region appgen: repository method
		Task<PurchaseOrder> CloneEntity(int id, string userName);
		#endregion
	}
}
