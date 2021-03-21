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
	public interface IPartService : IAsyncBaseService<Part>
	{
        #region appgen: crud operations

        Task<Part> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Part>> ListAllAsync(List<SortingInformation<Part>> sorting, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Part>> ListAsync(
            ISpecification<Part> spec, 
            List<SortingInformation<Part>> sorting, 
            bool withChilds = false,
            CancellationToken cancellationToken = default);
        Task<Part> AddAsync(Part entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Part entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Part entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default);
        Task<Part> FirstAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default);
        Task<Part> FirstOrDefaultAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: pdf operations

        string GeneratePdf(Part entity, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPage(List<string> ids, int? refId = null, CancellationToken cancellationToken = default);
        Task<string> GeneratePdfMultiPageBackgroundProcess(List<string> ids, CancellationToken cancellationToken = default);

        #endregion

        #region appgen: excel operations

        Task<string> GenerateExcelBackgroundProcess(
            string excelFilename,
			string id = "", List<string> partNames = null, List<string> descriptions = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<string> GenerateExcel(
            string excelFilename, int? refId = null, 
			string id = "", List<string> partNames = null, List<string> descriptions = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default);
        Task<List<Part>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default);
        Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default);
        Task<bool> ProcessUploadedFile(IEnumerable<Part> parts, CancellationToken cancellationToken = default);
        Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default);

        #endregion

        #region appgen: recovery mode operations

		Task<Part> CreateDraft(CancellationToken cancellation);
		Task<Part> CreateEditDraft(string id, CancellationToken cancellation);
		Task<bool> PatchDraft(Part part, CancellationToken cancellation);
		Task<string> CommitDraft(string id, CancellationToken cancellationToken);
		Task<bool> DiscardDraft(string id, CancellationToken cancellationToken);
		Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken);
		Task<List<string>> GetCurrentEditors(string id, CancellationToken cancellationToken);


		#endregion
	}
}
