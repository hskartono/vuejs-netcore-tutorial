using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IGenericRepository
	{
		SpResultNumberGenerator ExecStoredProcedureNumberGenerator(string spName, int recordId);
	}
}
