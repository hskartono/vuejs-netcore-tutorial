using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class GenericRepository : IGenericRepository
	{
		private readonly AppDbContext _context;
		public GenericRepository(AppDbContext context)
		{
			_context = context;
		}

		public SpResultNumberGenerator ExecStoredProcedureNumberGenerator(string spName, int recordId)
		{
			var param = new SqlParameter("@record_id", recordId);
			var result = _context.spResultNumberGenerators.FromSqlRaw(spName + " @record_id", param).AsEnumerable<SpResultNumberGenerator>().SingleOrDefault();
			return result;
		}
	}
}
