using Tutorial.ApplicationCore.Entities;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System;

namespace Tutorial.ApplicationCore.Repositories
{
	public partial interface IPurchaseOrderRepository : IAsyncRepository<PurchaseOrder>
	{
		Task<DataTable> GetDataTable(List<string> poNumbers, DateTime? poDateFrom, DateTime? poDateTo);
	}
}
