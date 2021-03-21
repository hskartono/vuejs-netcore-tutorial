using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IModuleInfoRepository : IAsyncRepository<ModuleInfo>
	{
		#region appgen: repository method
		Task<ModuleInfo> CloneEntity(int id, string userName);
		#endregion
	}
}
