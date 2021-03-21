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
	public class PurchaseOrderDetailService : AsyncBaseService<PurchaseOrderDetail>, IPurchaseOrderDetailService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseOrderDetailService(
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
		public async Task<PurchaseOrderDetail> AddAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);



			await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderDetailRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.PurchaseOrderDetailRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<PurchaseOrderDetail> FirstAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderDetailRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<PurchaseOrderDetail> FirstOrDefaultAsync(ISpecification<PurchaseOrderDetail> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderDetailRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<PurchaseOrderDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<PurchaseOrderDetail> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new PurchaseOrderDetailFilterSpecification(id, true);
			var purchaseOrderDetail = await _unitOfWork.PurchaseOrderDetailRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (purchaseOrderDetail == null || includeChilds == false)
				return purchaseOrderDetail;



			return purchaseOrderDetail;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<PurchaseOrderDetail>> ListAllAsync(List<SortingInformation<PurchaseOrderDetail>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderDetailRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<PurchaseOrderDetail>> ListAsync(
			ISpecification<PurchaseOrderDetail> spec, 
			List<SortingInformation<PurchaseOrderDetail>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var purchaseOrderDetails = await _unitOfWork.PurchaseOrderDetailRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && purchaseOrderDetails?.Count > 0)
			{
				var results = new List<PurchaseOrderDetail>(purchaseOrderDetails);
				var purchaseOrderDetailIds = purchaseOrderDetails.Select(e => e.Id).ToList();



				return results;
			}

			return purchaseOrderDetails;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(PurchaseOrderDetail entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.PurchaseOrderDetailRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			
			var oldEntity = await _unitOfWork.PurchaseOrderDetailRepository.GetByIdAsync(entity.Id, cancellationToken);
			if (oldEntity == null)
			{
			AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
			return false;
			}


			// update & commit
			//await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(oldEntity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}


		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(PurchaseOrderDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");



			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(PurchaseOrderDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(PurchaseOrderDetail entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(PurchaseOrderDetail entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("purchase_order_detail.html");
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

			var items = await this.ListAsync(new PurchaseOrderDetailFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("purchase_order_detail.html");
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
			var downloadProcess = new DownloadProcess("purchase_order_detail") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, PurchaseOrderDetail entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"PartPrice",""},
					{"Qty",""},
					{"TotalPrice",""},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["PurchaseOrder"] = entity.PurchaseOrder.PoNumber;
				mapper["Part"] = entity.Part.PartName;
				mapper["PartPrice"] = entity.PartPrice?.ToString("#,#0");
				mapper["Qty"] = entity.Qty?.ToString("#,#0");
				mapper["TotalPrice"] = entity.TotalPrice?.ToString("#,#0");

			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			int? id = null, List<int> purchaseOrders = null, List<string> parts = null, List<double> partPrices = null, List<int> qtys = null, List<double> totalPrices = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("purchase_order_detail") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, purchaseOrders, parts, partPrices, qtys, totalPrices,
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
			int? id = null, List<int> purchaseOrders = null, List<string> parts = null, List<double> partPrices = null, List<int> qtys = null, List<double> totalPrices = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				PurchaseOrderDetailFilterSpecification filterSpec = null;
				if (id.HasValue)
					filterSpec = new PurchaseOrderDetailFilterSpecification(id.Value);
				else
					filterSpec = new PurchaseOrderDetailFilterSpecification(exact)
					{

						Id = id, 
						PurchaseOrderIds = purchaseOrders, 
						PartIds = parts, 
						PartPrices = partPrices, 
						Qtys = qtys, 
						TotalPrices = totalPrices
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<PurchaseOrderDetail>(excelFilename, "purchaseOrderDetail.json", results) == false)
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
		public async Task<List<PurchaseOrderDetail>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<PurchaseOrderDetail>(tempExcelFile, "purchaseOrderDetail.json");
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
					await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<PurchaseOrderDetail> result, CancellationToken cancellationToken)
		{
			var excelPurchaseOrderIds1 = result.Where(s => s.PurchaseOrderId.HasValue).Select(s => s.PurchaseOrderId.Value).ToList();
			var PurchaseOrders = await _unitOfWork.PurchaseOrderRepository.ListAsync(new PurchaseOrderFilterSpecification(excelPurchaseOrderIds1), null, cancellationToken);
			var PurchaseOrderIds = PurchaseOrders.Select(e => e.Id);
			var excelPartIds1 = result.Where(s => !string.IsNullOrEmpty(s.PartId)).Select(s => s.PartId).ToList();
			var Parts = await _unitOfWork.PartRepository.ListAsync(new PartFilterSpecification(excelPartIds1), null, cancellationToken);
			var PartIds = Parts.Select(e => e.Id);

		}

		private void SetUploadDraftFlags(List<PurchaseOrderDetail> result)
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

			}
		}
		#endregion

		#region appgen: commit uploaded excel fiel
		public async Task<bool> CommitUploadedFile(CancellationToken cancellationToken = default)
		{
			var spec = new PurchaseOrderDetailFilterSpecification()
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
					if(id <= 0)
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken = default)
		{
			if (purchaseOrderDetails == null)
				throw new ArgumentNullException(nameof(purchaseOrderDetails));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in purchaseOrderDetails)
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
			var spec = new PurchaseOrderDetailFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("PurchaseOrderDetail");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "PurchaseOrder";
				ws.Cells[1, 4].Value = "Part";
				ws.Cells[1, 5].Value = "Part Price";
				ws.Cells[1, 6].Value = "Qty";
				ws.Cells[1, 7].Value = "Total Price";
				ws.Cells[1, 8].Value = "Status";
				ws.Cells[1, 9].Value = "Message";



				int row = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.PartPrice;
					ws.Cells[row, 4].Value = item.Qty;
					ws.Cells[row, 5].Value = item.TotalPrice;
					ws.Cells[row, 6].Value = item.UploadValidationStatus;
					ws.Cells[row, 7].Value = item.UploadValidationMessage;


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
		public async Task<PurchaseOrderDetail> CreateDraft(PurchaseOrderDetail entity, CancellationToken cancellation)
		{
			entity.PurchaseOrder = null;
			entity.Part = null;

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<PurchaseOrderDetail> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new PurchaseOrderDetailFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Purchase Order Detail dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new PurchaseOrderDetailFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.PurchaseOrderDetailRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.PurchaseOrderDetailRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Purchase Order Detail");
				return null;
			}

			return await _unitOfWork.PurchaseOrderDetailRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(PurchaseOrderDetail purchaseOrderDetail, CancellationToken cancellationToken)
		{
			var id = purchaseOrderDetail.Id;
			var originalValue = await _unitOfWork.PurchaseOrderDetailRepository.FirstOrDefaultAsync(
				new PurchaseOrderDetailFilterSpecification(id));

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(purchaseOrderDetail.PartId)) originalValue.PartId = purchaseOrderDetail.PartId;
			if (purchaseOrderDetail.PartPrice.HasValue && purchaseOrderDetail.PartPrice > 0) originalValue.PartPrice = purchaseOrderDetail.PartPrice.Value;
			if (purchaseOrderDetail.Qty.HasValue && purchaseOrderDetail.Qty > 0) originalValue.Qty = purchaseOrderDetail.Qty.Value;
			if (purchaseOrderDetail.TotalPrice.HasValue && purchaseOrderDetail.TotalPrice > 0) originalValue.TotalPrice = purchaseOrderDetail.TotalPrice.Value;


			// pastikan data belongsTo & hasMany tidak ikut
			purchaseOrderDetail.PurchaseOrder = null;
			purchaseOrderDetail.Part = null;


			AssignUpdater(originalValue);
			await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(originalValue, cancellationToken);
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

			PurchaseOrderDetail destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.PurchaseOrder = recoveryRecord.PurchaseOrder;
				destinationRecord.PurchaseOrder = null;
				destinationRecord.Part = recoveryRecord.Part;
				destinationRecord.Part = null;
				destinationRecord.PartPrice = recoveryRecord.PartPrice;
				destinationRecord.Qty = recoveryRecord.Qty;
				destinationRecord.TotalPrice = recoveryRecord.TotalPrice;


				await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(destinationRecord, cancellationToken);
				resultId = destinationRecord.Id;
			}

			// header recovery
			int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
			if (destinationRecord != null)
				draftStatus = (int)BaseEntity.DraftStatus.Saved;
			
			recoveryRecord.IsDraftRecord = draftStatus;
			recoveryRecord.RecordActionDate = DateTime.Now;
			recoveryRecord.DraftFromUpload = false;



			// save ke database
			await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
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


			// save ke database
			await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new PurchaseOrderDetailFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.PurchaseOrderDetailRepository.ListAsync(spec, null, cancellationToken);
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
			var spec = new PurchaseOrderDetailFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.PurchaseOrderDetailRepository.ListAsync(spec, null, cancellationToken);
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

		#region appgen: add draft async
		public async Task<List<PurchaseOrderDetail>> AddDraftAsync(List<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken)
		{
			if (purchaseOrderDetails == null || purchaseOrderDetails.Count <= 0)
				throw new ArgumentNullException(nameof(purchaseOrderDetails));

			foreach(var item in purchaseOrderDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.DraftFromUpload = false;
				item.RecordEditedBy = _userName;
				item.RecordActionDate = DateTime.Now;
				await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(item, cancellationToken);
			}

			await _unitOfWork.CommitAsync();
			return purchaseOrderDetails;
		}
		#endregion

		#region appgen: replace draft async
		public async Task<List<PurchaseOrderDetail>> ReplaceDraftAsync(List<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken cancellationToken)
		{
			if (purchaseOrderDetails == null || purchaseOrderDetails.Count <= 0)
				throw new ArgumentNullException(nameof(purchaseOrderDetails));

			// hapus seluruh data sesuai filter
			int parentId = (purchaseOrderDetails[0].PurchaseOrderId.HasValue) ? purchaseOrderDetails[0].PurchaseOrderId.Value : 0;
			var spec = new PurchaseOrderDetailFilterSpecification()
			{
				PurchaseOrderIds = new List<int>() { parentId }
			}.BuildSpecification();
			_unitOfWork.PurchaseOrderDetailRepository.DeleteAsync(spec, cancellationToken);

			// insert data baru
			foreach (var item in purchaseOrderDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.DraftFromUpload = false;
				item.RecordEditedBy = _userName;
				item.RecordActionDate = DateTime.Now;
				await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(item, cancellationToken);
			}

			await _unitOfWork.CommitAsync();

			return purchaseOrderDetails;
		}
		#endregion

		#endregion

	}
}
