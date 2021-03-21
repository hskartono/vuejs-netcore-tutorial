using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tutorial.ApplicationCore.Entities
{
	public class Part : BaseEntity
	{
		#region appgen: generated constructor
		public Part() { }

		public Part(string partName, string description)
		{
			PartName = partName;
			Description = description;
		}


		#endregion

		#region appgen: generated property
		public string Id { get; set; }
		public string PartName { get; set; }
		public string Description { get; set; }

		public string MainRecordId { get; set; }
		#endregion

		#region appgen: generated method

		#endregion
	}
}
