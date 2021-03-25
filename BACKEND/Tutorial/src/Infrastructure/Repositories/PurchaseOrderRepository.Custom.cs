using Microsoft.EntityFrameworkCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;

namespace Tutorial.Infrastructure.Repositories
{
	public partial class PurchaseOrderRepository : AsyncRepository<PurchaseOrder>, IPurchaseOrderRepository
	{
		public async Task<DataTable> GetDataTable(List<string> poNumbers, DateTime? poDateFrom, DateTime? poDateTo)
		{
			string wherePoNumbers = "";
			if (poNumbers?.Count > 0)
			{
				string wherein = "";
				string separator = ",";
				foreach(var poNo in poNumbers)
					wherein += $"{separator}'{poNo}'";
				wherePoNumbers += $"and po.PoNumber in ({wherein})";
			}

			string wherePoDate = "";
			if(poDateFrom.HasValue && poDateTo.HasValue)
			{
				wherePoDate = $"and (po.poDate >= '{poDateFrom.Value.ToString("yyyy-MM-dd")}' and poDate <= '{poDateTo.Value.ToString("yyyy-MM-dd")}' )";
			} else if (poDateFrom.HasValue)
			{
				wherePoDate = $"and po.poDate >= '{poDateFrom.Value.ToString("yyyy-MM-dd")}'";
			} else if (poDateTo.HasValue)
			{
				wherePoDate = $"and poDate <= '{poDateTo.Value.ToString("yyyy-MM-dd")}'";
			}

			string sql = $@"
			select po.PoNumber, po.PoDate, po.Remarks, p.PartName, pod.PartPrice, pod.Qty, pod.TotalPrice
			from PurchaseOrders po
			inner join PurchaseOrderDetails pod on po.Id = pod.PurchaseOrderId
			inner join Parts p on pod.PartId = p.Id
			where po.IsDraftRecord = 0 and po.DeletedAt is null
			and pod.IsDraftRecord = 0 and pod.DeletedAt is null
			{wherePoNumbers}
			{wherePoDate}
			";

			return await SqlToDataTableAsync(sql);
		}
	}
}
