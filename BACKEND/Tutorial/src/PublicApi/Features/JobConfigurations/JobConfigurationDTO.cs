using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.JobConfigurations
{
	public class JobConfigurationDTO
	{
		public int Id { get; set; }
		public string InterfaceName { get; set; }
		public string JobName { get; set; }
		public string JobDescription { get; set; }
		public bool IsStoredProcedure { get; set; }
	}
}
