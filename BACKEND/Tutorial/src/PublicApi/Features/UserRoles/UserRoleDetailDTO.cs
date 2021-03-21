using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.UserRoles
{
	public class UserRoleDetailDTO
	{
		public int? Id { get; set; }
		public int? UserRoleId { get; set; }
		public int RoleId { get; set; }
		public string RoleName { get; set; }

	}
}
