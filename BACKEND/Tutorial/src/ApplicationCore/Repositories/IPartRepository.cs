using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IPartRepository : IAsyncRepository<Part>
	{
		#region appgen: repository method
		Task<Part> CloneEntity(string id, string userName);
		#endregion
	}
}
