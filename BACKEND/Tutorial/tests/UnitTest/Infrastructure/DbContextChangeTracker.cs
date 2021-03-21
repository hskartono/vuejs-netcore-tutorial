using Microsoft.EntityFrameworkCore;
using Tutorial.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Infrastructure
{
	public static class DbContextChangeTracker
	{
        public static void DetachAllEntities(this AppDbContext db)
        {
            var changedEntriesCopy = db.ChangeTracker.Entries()
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
