using Tutorial.PublicApi.Features.ModuleInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.FunctionInfos
{
	public class FunctionInfoDTO
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Uri { get; set; }
		public string IconName { get; set; }
		public bool IsEnabled { get; set; }
		public string ModuleName { get; set; }

		public ModuleInfoDTO ModuleInfo { get; set; }

	}
}
