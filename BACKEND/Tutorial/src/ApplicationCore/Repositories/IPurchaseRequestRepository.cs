using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IPurchaseRequestRepository : IAsyncRepository<PurchaseRequest>
	{
		#region appgen: repository method
		Task<PurchaseRequest> CloneEntity(int id, string userName);
		#endregion
	}
}
