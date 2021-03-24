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
	public class PurchaseRequestService : AsyncBaseService<PurchaseRequest>, IPurchaseRequestService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseRequestService(
			IUnitOfWork unitOfWork,
			IDownloadProcessService downloadProcessService,
			IUriComposer uriComposer) : base(unitOfWork)
		{
			_downloadProcessService = downloadProcessService;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operations

		#region appgen: add
		public async Task<PurchaseRequest> AddAsync(PurchaseRequest entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);

			if (entity.PurchaseRequestDetails?.Count > 0)
				foreach (var item in entity.PurchaseRequestDetails)
				{
					AssignCreatorAndCompany(item);
					item.PurchaseRequest = entity;
				}


			await _unitOfWork.PurchaseRequestRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseRequestRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(PurchaseRequest entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.PurchaseRequestRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<PurchaseRequest> FirstAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseRequestRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<PurchaseRequest> FirstOrDefaultAsync(ISpecification<PurchaseRequest> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseRequestRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<PurchaseRequest> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<PurchaseRequest> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new PurchaseRequestFilterSpecification(id, true);
			var purchaseRequest = await _unitOfWork.PurchaseRequestRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (purchaseRequest == null || includeChilds == false)
				return purchaseRequest;

			var purchaseRequestDetailsFilter = new PurchaseRequestDetailFilterSpecification()
			{
				PurchaseRequestIds = new List<int>() { id },
				ShowDraftList = BaseEntity.DraftStatus.All
			}.BuildSpecification();
			var purchaseRequestDetailss = await _unitOfWork.PurchaseRequestDetailRepository.ListAsync(purchaseRequestDetailsFilter, null, cancellationToken);
			purchaseRequest.AddRangePurchaseRequestDetails(purchaseRequestDetailss.ToList());


			return purchaseRequest;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<PurchaseRequest>> ListAllAsync(List<SortingInformation<PurchaseRequest>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseRequestRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<PurchaseRequest>> ListAsync(
			ISpecification<PurchaseRequest> spec,
			List<SortingInformation<PurchaseRequest>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var purchaseRequests = await _unitOfWork.PurchaseRequestRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && purchaseRequests?.Count > 0)
			{
				var results = new List<PurchaseRequest>(purchaseRequests);
				var purchaseRequestIds = purchaseRequests.Select(e => e.Id).ToList();

				var purchaseRequestDetailsFilter = new PurchaseRequestDetailFilterSpecification()
				{
					PurchaseRequestIds = purchaseRequestIds,
					ShowDraftList = BaseEntity.DraftStatus.All
				}.BuildSpecification();
				var purchaseRequestDetailss = await _unitOfWork.PurchaseRequestDetailRepository.ListAsync(purchaseRequestDetailsFilter, null, cancellationToken);
				results.ForEach(c => c.AddRangePurchaseRequestDetails(
					purchaseRequestDetailss
					.Where(e => e.PurchaseRequestId == c.Id).ToList()
					));


				return results;
			}

			return purchaseRequests;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(PurchaseRequest entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.PurchaseRequestRepository.ReplaceAsync(entity, entity.Id, cancellationToken);

			var oldEntity = await _unitOfWork.PurchaseRequestRepository.GetByIdAsync(entity.Id, cancellationToken);
			if (oldEntity == null)
			{
				AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
				return false;
			}
			await SmartUpdatePurchaseRequestDetails(oldEntity, entity);


			// update & commit
			//await _unitOfWork.PurchaseRequestRepository.UpdateAsync(oldEntity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private async Task SmartUpdatePurchaseRequestDetails(PurchaseRequest oldEntity, PurchaseRequest entity, CancellationToken cancellationToken = default)
		{
			List<PurchaseRequestDetail> oldEntityToBeDeleted = new List<PurchaseRequestDetail>();
			if (oldEntity.PurchaseRequestDetails.Count > 0)
			{
				foreach (var item in oldEntity.PurchaseRequestDetails)
					oldEntityToBeDeleted.Add(item);
			}

			if (entity.PurchaseRequestDetails.Count > 0)
			{
				foreach (var item in entity.PurchaseRequestDetails)
				{
					var hasUpdate = false;
					if (oldEntity.PurchaseRequestDetails.Count > 0)
					{
						var data = oldEntity.PurchaseRequestDetails.SingleOrDefault(p => p.Id == item.Id);
						if (data != null)
						{
							AssignUpdater(item);
							await _unitOfWork.PurchaseRequestDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

							oldEntityToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						AssignCreatorAndCompany(item);
						oldEntity.AddOrReplacePurchaseRequestDetails(item);
					}
				}
			}

			if (oldEntityToBeDeleted.Count > 0)
			{
				foreach (var item in oldEntityToBeDeleted)
					oldEntity.RemovePurchaseRequestDetails(item);
			}
		}
		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(PurchaseRequest entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");



			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(PurchaseRequest entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(PurchaseRequest entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(PurchaseRequest entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("purchase_request.html");
			string htmlContent = File.ReadAllText(templateFile);

			htmlContent = MapTemplateValue(htmlContent, entity);

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new ConverterProperties();
			HtmlConverter.ConvertToPdf(htmlContent, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

			return fullPdfFileName;
		}
		#endregion

		#region appgen: generate pdf multipage
		public async Task<string> GeneratePdfMultiPage(List<int> ids, int? refId = null, CancellationToken cancellationToken = default)
		{
			if (ids == null)
				throw new ArgumentNullException(nameof(ids));

			cancellationToken.ThrowIfCancellationRequested();

			var items = await this.ListAsync(new PurchaseRequestFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("purchase_request.html");
			string htmlContent = File.ReadAllText(templateFile);
			string result = "";

			foreach (var item in items)
			{
				result += MapTemplateValue(htmlContent, item);
			}

			// prepare destination pdf
			string pdfFileName = DateTime.Now.ToString("yyyyMMdd_hhmmss_") + Guid.NewGuid().ToString() + ".pdf";
			string fullPdfFileName = _uriComposer.ComposeDownloadPath(pdfFileName);
			ConverterProperties converterProperties = new ConverterProperties();
			HtmlConverter.ConvertToPdf(result, new FileStream(fullPdfFileName, FileMode.Create), converterProperties);

			if (refId.HasValue && refId.Value > 0)
			{
				await _downloadProcessService.SuccessfullyGenerated(refId.Value, pdfFileName, cancellationToken);
			}

			return fullPdfFileName;
		}
		#endregion

		#region appgen: generate pdf background process
		public async Task<string> GeneratePdfMultiPageBackgroundProcess(List<int> ids, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("purchase_request") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GeneratePdfMultiPage(ids, result.Id, cancellationToken));

			// update background job id
			result.JobId = jobId;
			await _downloadProcessService.UpdateAsync(result, cancellationToken);

			// return background job id
			return jobId;
		}
		#endregion

		#region appgen: mapp templates
		private string MapTemplateValue(string htmlContent, PurchaseRequest entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"PrDate",""},
					{"PrNo",""},
					{"Remarks",""},
					{"PurchaseRequestDetails", new List<Dictionary<string, string>>()},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["PrDate"] = entity.PrDate?.ToString("dd-MMM-yyyy");
				mapper["PrNo"] = entity.PrNo;
				mapper["Remarks"] = entity.Remarks;
				if (entity.PurchaseRequestDetails.Count > 0)
				{
					foreach (var item in entity.PurchaseRequestDetails)
					{
						var purchaseRequestDetails = new Dictionary<string, string>()
						{
							{"PurchaseRequestDetailsId", item.Id.ToString()},
							{"PurchaseRequestDetailsQty", item.Qty?.ToString("#,#0")},
							{"PurchaseRequestDetailsRequestDate", item.RequestDate?.ToString("dd-MMM-yyyy")},
						};
						((List<Dictionary<string, string>>)mapper["PurchaseRequestDetails"]).Add(purchaseRequestDetails);
					}
				}


			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, DateTime? prDateFrom = null, DateTime? prDateTo = null, List<string> prNos = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("purchase_request") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, prDateFrom, prDateTo, prNos, remarkss,
				exact,
				cancellationToken));

			result.JobId = jobId;
			await _downloadProcessService.UpdateAsync(result, cancellationToken);

			// return background job id
			return jobId;
		}
		#endregion

		#region appgen: generate excel process
		public async Task<string> GenerateExcel(string excelFilename, int? refId = null,
			int? id = null, DateTime? prDateFrom = null, DateTime? prDateTo = null, List<string> prNos = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				PurchaseRequestFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new PurchaseRequestFilterSpecification(id.Value);
				else
					filterSpec = new PurchaseRequestFilterSpecification(exact)
					{

						Id = id,
						PrDateFrom = prDateFrom,
						PrDateTo = prDateTo,
						PrNos = prNos,
						Remarkss = remarkss
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<PurchaseRequest>(excelFilename, "purchaseRequest.json", results) == false)
				{
					if (refId.HasValue)
						await _downloadProcessService.FailedToGenerate(refId.Value, "Failed to generate excel file");
					return "";
				}

				// update database information (if needed)
				if (refId.HasValue)
				{
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

		#region appgen: upload excel
		public async Task<List<PurchaseRequest>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<PurchaseRequest>(tempExcelFile, "purchaseRequest.json");
			if (result == null)
			{
				AddError("Format template excel tidak dikenali. Silahkan download template dari menu download.");
				return null;
			}

			SetUploadDraftFlags(result);

			await RunMasterDataValidation(result, cancellationToken);

			foreach (var item in result)
			{
				var id = item.Id;
				if (id > 0)
					await _unitOfWork.PurchaseRequestRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.PurchaseRequestRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<PurchaseRequest> result, CancellationToken cancellationToken)
		{

		}

		private void SetUploadDraftFlags(List<PurchaseRequest> result)
		{
			foreach (var item in result)
			{
				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;

				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;
				foreach (var subitem in item.PurchaseRequestDetails)
				{
					item.isFromUpload = true;
					item.DraftFromUpload = true;
					item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
					item.RecordActionDate = DateTime.Now;
					item.RecordEditedBy = _userName;
				}

			}
		}
		#endregion

		#region appgen: commit uploaded excel fiel
		public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
		{
			var spec = new PurchaseRequestFilterSpecification()
			{
				RecordEditedBy = _userName,
				DraftFromUpload = true,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification(true);
			var draftDatas = await ListAsync(spec, null, true, cancellationToken);
			int rowNum = 1;
			foreach (var item in draftDatas)
			{
				ValidateOnInsert(item, rowNum);
				rowNum++;
			}

			if (!ServiceState)
				return false;

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in draftDatas)
				{
					var id = await CommitDraft(item.Id, cancellationToken);
					if (id <= 0)
					{
						AddError("Terjadi kesalahan ketika menyimpan data.");
						return false;
					}
				}

				scope.Complete();
			}

			return true;
		}
		#endregion

		#region appgen: process uploaded file
		public async Task<bool> ProcessUploadedFile(IEnumerable<PurchaseRequest> purchaseRequests, CancellationToken cancellationToken = default)
		{
			if (purchaseRequests == null)
				throw new ArgumentNullException(nameof(purchaseRequests));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in purchaseRequests)
				{
					var id = item.Id;
					if (id <= 0)
					{
						var result = await this.AddAsync(item, cancellationToken);
						if (result == null)
							return false;
					}
					else
					{
						var result = await this.UpdateAsync(item, cancellationToken);
						if (result == false)
							return false;
					}
				}

				scope.Complete();
			}

			return true;
		}
		#endregion

		#region appgen: generate upload log
		public async Task<string> GenerateUploadLogExcel(CancellationToken cancellationToken = default)
		{
			var spec = new PurchaseRequestFilterSpecification()
			{
				RecordEditedBy = _userName,
				DraftFromUpload = true,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification(true);
			var draftDatas = await ListAsync(spec, null, true, cancellationToken);

			var tempExcelFile = Path.GetTempFileName();
			initEPPlusLicense();
			using (var package = new ExcelPackage())
			{
				var ws = package.Workbook.Worksheets.Add("PurchaseRequest");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "PR Date";
				ws.Cells[1, 4].Value = "PR Number";
				ws.Cells[1, 5].Value = "Remarks";
				ws.Cells[1, 6].Value = "Purcahse Request Details";
				ws.Cells[1, 7].Value = "Status";
				ws.Cells[1, 8].Value = "Message";

				var wsPurchaseRequestDetails = package.Workbook.Worksheets.Add("PurchaseRequestDetail");
				wsPurchaseRequestDetails.Cells[1, 1].Value = "ID";
				wsPurchaseRequestDetails.Cells[1, 2].Value = "PK";
				wsPurchaseRequestDetails.Cells[1, 3].Value = "PurchaseRequest";
				wsPurchaseRequestDetails.Cells[1, 4].Value = "Part";
				wsPurchaseRequestDetails.Cells[1, 5].Value = "Qty";
				wsPurchaseRequestDetails.Cells[1, 6].Value = "Request Date";
				wsPurchaseRequestDetails.Cells[1, 7].Value = "Status";
				wsPurchaseRequestDetails.Cells[1, 8].Value = "Message";


				int row = 2;
				int rowPurchaseRequestDetails = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.PrDate;
					ws.Cells[row, 3].Style.Numberformat.Format = "dd-MMM-yyyy";
					ws.Cells[row, 4].Value = item.PrNo;
					ws.Cells[row, 5].Value = item.Remarks;
					ws.Cells[row, 6].Value = item.UploadValidationStatus;
					ws.Cells[row, 7].Value = item.UploadValidationMessage;
					foreach (var itemPurchaseRequestDetails in item.PurchaseRequestDetails)
					{
						wsPurchaseRequestDetails.Cells[rowPurchaseRequestDetails, 1].Value = itemPurchaseRequestDetails.Id;
						wsPurchaseRequestDetails.Cells[rowPurchaseRequestDetails, 2].Value = pk;
						ws.Cells[row, 3].Value = item.UploadValidationStatus;
						ws.Cells[row, 4].Value = item.UploadValidationMessage;
					}


					row++;
					pk++;
				}

				package.SaveAs(new FileInfo(tempExcelFile));
			}

			return tempExcelFile;
		}
		#endregion

		#endregion

		#region Recovery Record Service

		#region appgen: create draft
		public async Task<PurchaseRequest> CreateDraft(CancellationToken cancellation)
		{
			var spec = new PurchaseRequestFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.PurchaseRequestRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.PurchaseRequestRepository.FirstOrDefaultAsync(spec, cancellation);

			var entity = new PurchaseRequest();

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.PurchaseRequestRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<PurchaseRequest> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new PurchaseRequestFilterSpecification(id), cancellation);
			if (count <= 0)
			{
				AddError($"Data Purchase Request dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new PurchaseRequestFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.PurchaseRequestRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.PurchaseRequestRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Purchase Request");
				return null;
			}

			return await _unitOfWork.PurchaseRequestRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(PurchaseRequest purchaseRequest, CancellationToken cancellationToken)
		{
			var id = purchaseRequest.Id;
			var originalValue = await _unitOfWork.PurchaseRequestRepository.FirstOrDefaultAsync(
				new PurchaseRequestFilterSpecification(id));

			if (originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (purchaseRequest.PrDate.HasValue && purchaseRequest.PrDate > DateTime.MinValue) originalValue.PrDate = purchaseRequest.PrDate.Value;
			if (!string.IsNullOrEmpty(purchaseRequest.PrNo)) originalValue.PrNo = purchaseRequest.PrNo;
			if (!string.IsNullOrEmpty(purchaseRequest.Remarks)) originalValue.Remarks = purchaseRequest.Remarks;


			// pastikan data belongsTo & hasMany tidak ikut
			purchaseRequest.PurchaseRequestDetails = null;


			AssignUpdater(originalValue);
			await _unitOfWork.PurchaseRequestRepository.UpdateAsync(originalValue, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: commit draft
		public async Task<int> CommitDraft(int id, CancellationToken cancellationToken)
		{
			int resultId = 0;
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return 0;

			PurchaseRequest destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}

			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.PrDate = recoveryRecord.PrDate;
				destinationRecord.PrNo = recoveryRecord.PrNo;
				destinationRecord.Remarks = recoveryRecord.Remarks;
				this.SmartUpdateRecoveryPurchaseRequestDetails(destinationRecord, recoveryRecord, cancellationToken);


				await _unitOfWork.PurchaseRequestRepository.UpdateAsync(destinationRecord, cancellationToken);
				resultId = destinationRecord.Id;
			}

			// header recovery
			int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
			if (destinationRecord != null)
				draftStatus = (int)BaseEntity.DraftStatus.Saved;

			recoveryRecord.IsDraftRecord = draftStatus;
			recoveryRecord.RecordActionDate = DateTime.Now;
			recoveryRecord.DraftFromUpload = false;

			foreach (var item in recoveryRecord.PurchaseRequestDetails)
			{
				item.IsDraftRecord = draftStatus;
				item.RecordActionDate = DateTime.Now;
				item.DraftFromUpload = false;
			}


			// save ke database
			await _unitOfWork.PurchaseRequestRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
		}

		private void SmartUpdateRecoveryPurchaseRequestDetails(PurchaseRequest destinationRecord, PurchaseRequest recoveryRecord, CancellationToken cancellationToken)
		{
			List<PurchaseRequestDetail> destinationToBeDeleted = new List<PurchaseRequestDetail>();
			if (destinationRecord.PurchaseRequestDetails.Count > 0)
			{
				foreach (var item in destinationRecord.PurchaseRequestDetails)
					destinationToBeDeleted.Add(item);
			}

			if (recoveryRecord.PurchaseRequestDetails.Count > 0)
			{
				foreach (var item in recoveryRecord.PurchaseRequestDetails)
				{
					var hasUpdate = false;
					if (destinationRecord.PurchaseRequestDetails.Count > 0)
					{
						var data = destinationRecord.PurchaseRequestDetails.SingleOrDefault(p => p.Id == item.MainRecordId);
						if (data != null)
						{
							data.PartId = item.PartId;
							data.Qty = item.Qty;
							data.RequestDate = item.RequestDate;

							AssignUpdater(data);

							item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
							item.RecordActionDate = DateTime.Now;

							hasUpdate = true;
							destinationToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						//AssignCreatorAndCompany(item);
						destinationRecord.AddPurchaseRequestDetails(item.PartId, item.Qty, item.RequestDate);

						item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
						item.RecordActionDate = DateTime.Now;
					}
				}
			}

			if (destinationToBeDeleted.Count > 0)
			{
				foreach (var item in destinationToBeDeleted)
					destinationRecord.RemovePurchaseRequestDetails(item);
			}
		}

		#endregion

		#region appgen: discard draft
		public async Task<bool> DiscardDraft(int id, CancellationToken cancellationToken)
		{
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return false;

			// header
			recoveryRecord.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
			recoveryRecord.RecordActionDate = DateTime.Now;
			foreach (var item in recoveryRecord.PurchaseRequestDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
				item.RecordActionDate = DateTime.Now;
			}


			// save ke database
			await _unitOfWork.PurchaseRequestRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new PurchaseRequestFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.PurchaseRequestRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return documentDrafts;

			foreach (var item in datas)
				documentDrafts.Add(
					new DocumentDraft($"Ada dokumen yang di edit terakhir pada {item.RecordActionDate?.ToString("dd-MMM-yyyy")} Jam {item.RecordActionDate?.ToString("hh:mm")} dan belum di simpan.", item.Id));

			return documentDrafts;
		}
		#endregion

		#region appgen: get current editor
		public async Task<List<string>> GetCurrentEditors(int id, CancellationToken cancellationToken)
		{
			var spec = new PurchaseRequestFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.PurchaseRequestRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return null;

			List<string> users = new List<string>();
			foreach (var item in datas)
				users.Add(item.RecordEditedBy);

			var editorUsers = await _unitOfWork.UserInfoRepository.ListAsync(new UserInfoFilterSpecification(users), null, cancellationToken);
			users.Clear();
			if (editorUsers == null) return users;

			foreach (var item in editorUsers)
				users.Add(item.FullName);

			return users;
		}
		#endregion



		#endregion

	}
}
