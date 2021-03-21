using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Tutorial.PublicApi.Features.ModuleInfos
{
	public class ModuleInfoDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string Name { get; set; }
		public string IconName { get; set; }
		public int? ParentModuleId { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
