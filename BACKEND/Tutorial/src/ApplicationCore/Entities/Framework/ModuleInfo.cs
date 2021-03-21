using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class ModuleInfo : BaseEntity
	{
		#region appgen: generated constructor
		public ModuleInfo() { }

		public ModuleInfo(string name, int? parentModuleId)
		{
			Name = name;
			ParentModuleId = parentModuleId;
		}


		#endregion

		#region appgen: generated property
		public int Id { get; set; }
		public string Name { get; set; }
		public string IconName { get; set; }
		public int? ParentModuleId { get; set; }

		#endregion

		#region appgen: generated method

		#endregion
	}
}
