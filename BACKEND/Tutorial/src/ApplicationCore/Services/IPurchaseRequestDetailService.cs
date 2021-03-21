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
	public interface IPurchaseRequestDetailService : IAsyncBaseService<PurchaseRequestDetail>
	{
        #region appgen: crud operations

        Task<PurchaseRequestDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseRequestDetail>> ListAllAsync(List<SortingInformation<PurchaseRequestDetail>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseRequestDetail>> ListAsync(
            ISpecification<PurchaseRequestDetail> spec, 
            List<SortingInformation<PurchaseRequestDetail>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<PurchaseRequestDetail> AddAsync(PurchaseRequestDetail entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PurchaseRequestDetail entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(PurchaseRequestDetail entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<PurchaseRequestDetail> spec, CancellationToken cancellationToken = default);
        Task<PurchaseRequestDetail> FirstAsync(ISpecification<PurchaseRequestDetail> spec, CancellationToken cancellationToken = default);
        Task<PurchaseRequestDetail> FirstOrDefaultAsync(ISpecification<PurchaseRequestDetail> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(PurchaseRequestDetail entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, List<int> purchaseRequests = null, List<string> parts = null, List<int> qtys = null, DateTime? requestDateFrom = null, DateTime? requestDateTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, List<int> purchaseRequests = null, List<string> parts = null, List<int> qtys = null, DateTime? requestDateFrom = null, DateTime? requestDateTo = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<PurchaseRequestDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<PurchaseRequestDetail> purchaseRequestDetails, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<PurchaseRequestDetail> CreateDraft(PurchaseRequestDetail entity, CancellationToken cancellation);
		Task<bool> PatchDraft(PurchaseRequestDetail purchaseRequestDetail, CancellationToken cancellation);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<PurchaseRequestDetail>> AddDraftAsync(List<PurchaseRequestDetail> purchaseRequestDetails, CancellationToken cancellationToken);
		Task<List<PurchaseRequestDetail>> ReplaceDraftAsync(List<PurchaseRequestDetail> purchaseRequestDetails, CancellationToken cancellationToken);


		#endregion
	}
}
