using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface IPurchaseOrderService : IAsyncBaseService<PurchaseOrder>
	{
        #region appgen: crud operations

        Task<PurchaseOrder> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseOrder>> ListAllAsync(List<SortingInformation<PurchaseOrder>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseOrder>> ListAsync(
            ISpecification<PurchaseOrder> spec, 
            List<SortingInformation<PurchaseOrder>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default);
        Task<PurchaseOrder> FirstAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default);
        Task<PurchaseOrder> FirstOrDefaultAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(PurchaseOrder entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<string> poNumbers = null, DateTime? poDateFrom = null, DateTime? poDateTo = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<string> poNumbers = null, DateTime? poDateFrom = null, DateTime? poDateTo = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<PurchaseOrder>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<PurchaseOrder> purchaseOrders, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<PurchaseOrder> CreateDraft(CancellationToken cancellation);
		Task<PurchaseOrder> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(PurchaseOrder purchaseOrder, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
