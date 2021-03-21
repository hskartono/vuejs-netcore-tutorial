using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Tutorial.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Infrastructure
{
	public class DatabaseFixture
	{
		protected DbContextOptions<AppDbContext> ContextOptions { get; }
		public AppDbContext context { get; }

		public DatabaseFixture()
		{
			string connectionString = "Data Source=.; Integrated Security=True;";

			var builder = new SqlConnectionStringBuilder(connectionString)
			{
				InitialCatalog = "UNITTESTDB"
			};
			ContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseSqlServer(builder.ToString())
				.Options;
			context = new AppDbContext(ContextOptions);

			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
		}
	}
}
