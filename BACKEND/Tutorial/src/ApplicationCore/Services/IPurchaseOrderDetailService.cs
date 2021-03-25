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
	public interface IPurchaseOrderDetailService : IAsyncBaseService<PurchaseOrderDetail>
	{
        #region appgen: crud operations

        Task<PurchaseOrderDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseOrderDetail>> ListAllAsync(List<SortingInformation<PurchaseOrderDetail>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseOrderDetail>> ListAsync(
            ISpecification<PurchaseOrderDetail> spec, 
            List<SortingInformation<PurchaseOrderDetail>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<PurchaseOrderDetail> AddAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default);
        Task<PurchaseOrderDetail> FirstAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default);
        Task<PurchaseOrderDetail> FirstOrDefaultAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(PurchaseOrderDetail entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<int> purchaseOrders = null, List<string> parts = null, List<double> partPrices = null, List<int> qtys = null, List<double> totalPrices = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<int> purchaseOrders = null, List<string> parts = null, List<double> partPrices = null, List<int> qtys = null, List<double> totalPrices = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<PurchaseOrderDetail>> UploadExcel(string tempExcelFile, int parent_id, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<PurchaseOrderDetail> CreateDraft(PurchaseOrderDetail entity, CancellationToken cancellation);
		Task<bool> PatchDraft(PurchaseOrderDetail purchaseOrderDetail, CancellationToken cancellation);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<PurchaseOrderDetail>> AddDraftAsync(List<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken);
		Task<List<PurchaseOrderDetail>> ReplaceDraftAsync(List<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken);


		#endregion
	}
}
