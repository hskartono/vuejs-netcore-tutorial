using Tutorial.PublicApi.Features.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Tutorial.PublicApi.Features.Parts
{
	public class PartDTO : BaseDTO
	{
		#region appgen: property list
		public string Id { get; set; }
		public string PartName { get; set; }
		public string Description { get; set; }

		#endregion

		#region appgen: property collection list

		#endregion
	}
}
