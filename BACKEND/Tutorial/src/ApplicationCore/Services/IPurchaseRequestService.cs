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
	public interface IPurchaseRequestService : IAsyncBaseService<PurchaseRequest>
	{
        #region appgen: crud operations

        Task<PurchaseRequest> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseRequest>> ListAllAsync(List<SortingInformation<PurchaseRequest>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PurchaseRequest>> ListAsync(
            ISpecification<PurchaseRequest> spec, 
            List<SortingInformation<PurchaseRequest>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<PurchaseRequest> AddAsync(PurchaseRequest entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PurchaseRequest entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(PurchaseRequest entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default);
        Task<PurchaseRequest> FirstAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default);
        Task<PurchaseRequest> FirstOrDefaultAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(PurchaseRequest entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			int? id = null, DateTime? prDateFrom = null, DateTime? prDateTo = null, List<string> prNos = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			int? id = null, DateTime? prDateFrom = null, DateTime? prDateTo = null, List<string> prNos = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<PurchaseRequest>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<PurchaseRequest> purchaseRequests, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<PurchaseRequest> CreateDraft(CancellationToken cancellation);
		Task<PurchaseRequest> CreateEditDraft(int id, CancellationToken cancellation);
		Task<bool> PatchDraft(PurchaseRequest purchaseRequest, CancellationToken cancellation);
		Task<int> CommitDraft(int id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(int id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken);


		#endregion
	}
}
