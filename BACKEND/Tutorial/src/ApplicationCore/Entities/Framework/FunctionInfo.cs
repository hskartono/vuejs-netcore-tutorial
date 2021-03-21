using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Entities
{
	public class FunctionInfo : BaseEntity
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Uri { get; set; }
		public string IconName { get; set; }
		public bool IsEnabled { get; set; }
		public int? ModuleInfoId { get; set; }
		public virtual ModuleInfo ModuleInfo { get; set; }
	}
}
