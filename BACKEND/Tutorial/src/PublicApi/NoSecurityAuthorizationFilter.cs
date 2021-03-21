using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tutorial.PublicApi
{
	public class NoSecurityAuthorizationFilter : IDashboardAuthorizationFilter
	{
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
