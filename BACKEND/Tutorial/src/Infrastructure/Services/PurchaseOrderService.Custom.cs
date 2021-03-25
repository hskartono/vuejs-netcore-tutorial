using Ardalis.Specification;
using Hangfire;
using iText.Html2pdf;
using OfficeOpenXml;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using Tutorial.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Tutorial.Infrastructure.Services
{
	public partial class PurchaseOrderService : AsyncBaseService<PurchaseOrder>, IPurchaseOrderService
	{
		#region appgen: generate excel process
		public async Task<string> GenerateExcel(string excelFilename, int? refId = null,
			int? id = null, List<string> poNumbers = null, DateTime? poDateFrom = null, DateTime? poDateTo = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var dt = await _unitOfWork.PurchaseOrderRepository.GetDataTable(poNumbers, poDateFrom, poDateTo);
				var excel = DataTableToExcel(dt);
				if(excel != null)
				{
					FileOutputUtil.OutputDir = new DirectoryInfo(Path.GetDirectoryName(excelFilename));
					var xFile = FileOutputUtil.GetFileInfo(Path.GetFileName(excelFilename));
					excel.SaveAs(xFile);
				}

				// update database information (if needed)
				if (refId.HasValue)
				{
					if(dt == null || !File.Exists(excelFilename))
					{
						await _downloadProcessService.FailedToGenerate(refId.Value, "Gagal membuat file excel", cancellationToken);
						return string.Empty;
					}

					excelFilename = Path.GetFileName(excelFilename);
					await _downloadProcessService.SuccessfullyGenerated(refId.Value, excelFilename);
				}

				return excelFilename;
			}
			catch (Exception ex)
			{
				if (refId.HasValue)
					await _downloadProcessService.FailedToGenerate(refId.Value, ex.Message);

				throw;
			}
		}
		#endregion
	}
}
