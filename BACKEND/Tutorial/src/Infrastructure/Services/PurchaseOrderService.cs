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

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseOrderService(
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
		public async Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);

			if (entity.PurchaseOrderDetails?.Count > 0)
				foreach (var item in entity.PurchaseOrderDetails)
				{
					AssignCreatorAndCompany(item);
					item.PurchaseOrder = entity;
				}


			await _unitOfWork.PurchaseOrderRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.PurchaseOrderRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<PurchaseOrder> FirstAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<PurchaseOrder> FirstOrDefaultAsync(ISpecification<PurchaseOrder> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<PurchaseOrder> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<PurchaseOrder> GetByIdAsync(int id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new PurchaseOrderFilterSpecification(id, true);
			var purchaseOrder = await _unitOfWork.PurchaseOrderRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (purchaseOrder == null || includeChilds == false)
				return purchaseOrder;

			var purchaseOrderDetailsFilter = new PurchaseOrderDetailFilterSpecification()
			{
				PurchaseOrderIds = new List<int>() { id },
				ShowDraftList = BaseEntity.DraftStatus.All
			}.BuildSpecification();
			var purchaseOrderDetailss = await _unitOfWork.PurchaseOrderDetailRepository.ListAsync(purchaseOrderDetailsFilter, null, cancellationToken);
			purchaseOrder.AddRangePurchaseOrderDetails(purchaseOrderDetailss.ToList());


			return purchaseOrder;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<PurchaseOrder>> ListAllAsync(List<SortingInformation<PurchaseOrder>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PurchaseOrderRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<PurchaseOrder>> ListAsync(
			ISpecification<PurchaseOrder> spec, 
			List<SortingInformation<PurchaseOrder>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var purchaseOrders = await _unitOfWork.PurchaseOrderRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && purchaseOrders?.Count > 0)
			{
				var results = new List<PurchaseOrder>(purchaseOrders);
				var purchaseOrderIds = purchaseOrders.Select(e => e.Id).ToList();

			var purchaseOrderDetailsFilter = new PurchaseOrderDetailFilterSpecification()
			{
				PurchaseOrderIds = purchaseOrderIds,
				ShowDraftList = BaseEntity.DraftStatus.All
			}.BuildSpecification();
			var purchaseOrderDetailss = await _unitOfWork.PurchaseOrderDetailRepository.ListAsync(purchaseOrderDetailsFilter, null, cancellationToken);
			results.ForEach(c => c.AddRangePurchaseOrderDetails(
				purchaseOrderDetailss
				.Where(e=>e.PurchaseOrderId == c.Id).ToList()
				));


				return results;
			}

			return purchaseOrders;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.PurchaseOrderRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			
			var oldEntity = await _unitOfWork.PurchaseOrderRepository.GetByIdAsync(entity.Id, cancellationToken);
			if (oldEntity == null)
			{
			AddError($"Could not load {nameof(entity)} data with id {entity.Id}.");
			return false;
			}
			await SmartUpdatePurchaseOrderDetails(oldEntity, entity); 


			// update & commit
			//await _unitOfWork.PurchaseOrderRepository.UpdateAsync(oldEntity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private async Task SmartUpdatePurchaseOrderDetails(PurchaseOrder oldEntity, PurchaseOrder entity, CancellationToken cancellationToken = default)
		{
			List<PurchaseOrderDetail> oldEntityToBeDeleted = new List<PurchaseOrderDetail>();
			if (oldEntity.PurchaseOrderDetails.Count > 0)
			{
				foreach (var item in oldEntity.PurchaseOrderDetails)
					oldEntityToBeDeleted.Add(item);
			}

			if (entity.PurchaseOrderDetails.Count > 0)
			{
				foreach (var item in entity.PurchaseOrderDetails)
				{
					var hasUpdate = false;
					if (oldEntity.PurchaseOrderDetails.Count > 0)
					{
						var data = oldEntity.PurchaseOrderDetails.SingleOrDefault(p => p.Id == item.Id);
						if (data != null)
						{
							AssignUpdater(item);
							await _unitOfWork.PurchaseOrderDetailRepository.ReplaceAsync(item, item.Id, cancellationToken);

							oldEntityToBeDeleted.Remove(data);
						}
					}

					if (!hasUpdate)
					{
						AssignCreatorAndCompany(item);
						oldEntity.AddOrReplacePurchaseOrderDetails(item);
					}
				}
			}

			if (oldEntityToBeDeleted.Count > 0)
			{
				foreach (var item in oldEntityToBeDeleted)
					oldEntity.RemovePurchaseOrderDetails(item);
			}
		}
		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(PurchaseOrder entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");

			if(!entity.PoDate.HasValue || entity.PoDate.Value == DateTime.MinValue)
			AddError($"{rowInfo} PO Date harus diisi.");


			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(PurchaseOrder entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(PurchaseOrder entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(PurchaseOrder entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("purchase_order.html");
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

			var items = await this.ListAsync(new PurchaseOrderFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("purchase_order.html");
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
			var downloadProcess = new DownloadProcess("purchase_order") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, PurchaseOrder entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"PoNumber",""},
					{"PoDate",""},
					{"Remarks",""},
					{"PurchaseOrderDetails", new List<Dictionary<string, string>>()},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id.ToString();
				mapper["PoNumber"] = entity.PoNumber;
				mapper["PoDate"] = entity.PoDate?.ToString("dd-MMM-yyyy");
				mapper["Remarks"] = entity.Remarks;
				if (entity.PurchaseOrderDetails.Count > 0)
				{
					foreach (var item in entity.PurchaseOrderDetails)
					{
						var purchaseOrderDetails = new Dictionary<string, string>()
						{
							{"PurchaseOrderDetailsId", item.Id.ToString()},
							{"PurchaseOrderDetailsPartPrice", item.PartPrice?.ToString("#,#0")},
							{"PurchaseOrderDetailsQty", item.Qty?.ToString("#,#0")},
							{"PurchaseOrderDetailsTotalPrice", item.TotalPrice?.ToString("#,#0")},
						};
						((List<Dictionary<string, string>>)mapper["PurchaseOrderDetails"]).Add(purchaseOrderDetails);
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
			int? id = null, List<string> poNumbers = null, DateTime? poDateFrom = null, DateTime? poDateTo = null, List<string> remarkss = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("purchase_order") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, poNumbers, poDateFrom, poDateTo, remarkss,
				exact,
				cancellationToken));

			result.JobId = jobId;
			await _downloadProcessService.UpdateAsync(result, cancellationToken);

			// return background job id
			return jobId;
		}
		#endregion

		

		#region appgen: upload excel
		public async Task<List<PurchaseOrder>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<PurchaseOrder>(tempExcelFile, "purchaseOrder.json");
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
					await _unitOfWork.PurchaseOrderRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.PurchaseOrderRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<PurchaseOrder> result, CancellationToken cancellationToken)
		{

		}

		private void SetUploadDraftFlags(List<PurchaseOrder> result)
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
			foreach (var subitem in item.PurchaseOrderDetails)
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
			var spec = new PurchaseOrderFilterSpecification()
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<PurchaseOrder> purchaseOrders, CancellationToken cancellationToken = default)
		{
			if (purchaseOrders == null)
				throw new ArgumentNullException(nameof(purchaseOrders));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in purchaseOrders)
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
			var spec = new PurchaseOrderFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("PurchaseOrder");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "PO Number";
				ws.Cells[1, 4].Value = "PO Date";
				ws.Cells[1, 5].Value = "Remarks";
				ws.Cells[1, 6].Value = "Purchase Order Detail";
				ws.Cells[1, 7].Value = "Status";
				ws.Cells[1, 8].Value = "Message";

				var wsPurchaseOrderDetails = package.Workbook.Worksheets.Add("PurchaseOrderDetail");
				wsPurchaseOrderDetails.Cells[1, 1].Value = "ID";
				wsPurchaseOrderDetails.Cells[1, 2].Value = "PK";
				wsPurchaseOrderDetails.Cells[1, 3].Value = "PurchaseOrder";
				wsPurchaseOrderDetails.Cells[1, 4].Value = "Part";
				wsPurchaseOrderDetails.Cells[1, 5].Value = "Part Price";
				wsPurchaseOrderDetails.Cells[1, 6].Value = "Qty";
				wsPurchaseOrderDetails.Cells[1, 7].Value = "Total Price";
				wsPurchaseOrderDetails.Cells[1, 8].Value = "Status";
				wsPurchaseOrderDetails.Cells[1, 9].Value = "Message";


				int row = 2;
				int rowPurchaseOrderDetails = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.PoNumber;
					ws.Cells[row, 4].Value = item.PoDate;
					ws.Cells[row, 4].Style.Numberformat.Format = "dd-MMM-yyyy";
					ws.Cells[row, 5].Value = item.Remarks;
					ws.Cells[row, 6].Value = item.UploadValidationStatus;
					ws.Cells[row, 7].Value = item.UploadValidationMessage;
					foreach(var itemPurchaseOrderDetails in item.PurchaseOrderDetails)
					{
						wsPurchaseOrderDetails.Cells[rowPurchaseOrderDetails, 1].Value = itemPurchaseOrderDetails.Id;
						wsPurchaseOrderDetails.Cells[rowPurchaseOrderDetails, 2].Value = pk;
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
		public async Task<PurchaseOrder> CreateDraft(CancellationToken cancellation)
		{
			var spec = new PurchaseOrderFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.PurchaseOrderRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.PurchaseOrderRepository.FirstOrDefaultAsync(spec, cancellation);

			var entity = new PurchaseOrder();

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.PurchaseOrderRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<PurchaseOrder> CreateEditDraft(int id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new PurchaseOrderFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Purchase Order dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new PurchaseOrderFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.PurchaseOrderRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.PurchaseOrderRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Purchase Order");
				return null;
			}

			return await _unitOfWork.PurchaseOrderRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(PurchaseOrder purchaseOrder, CancellationToken cancellationToken)
		{
			var id = purchaseOrder.Id;
			var originalValue = await _unitOfWork.PurchaseOrderRepository.FirstOrDefaultAsync(
				new PurchaseOrderFilterSpecification(id));

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(purchaseOrder.PoNumber)) originalValue.PoNumber = purchaseOrder.PoNumber;
			if (purchaseOrder.PoDate.HasValue && purchaseOrder.PoDate > DateTime.MinValue) originalValue.PoDate = purchaseOrder.PoDate.Value;
			if (!string.IsNullOrEmpty(purchaseOrder.Remarks)) originalValue.Remarks = purchaseOrder.Remarks;


			// pastikan data belongsTo & hasMany tidak ikut
			purchaseOrder.PurchaseOrderDetails = null;


			AssignUpdater(originalValue);
			await _unitOfWork.PurchaseOrderRepository.UpdateAsync(originalValue, cancellationToken);
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

			PurchaseOrder destinationRecord = null;
			if (recoveryRecord.MainRecordId.HasValue)
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId.Value, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.PoNumber = recoveryRecord.PoNumber;
				destinationRecord.PoDate = recoveryRecord.PoDate;
				destinationRecord.Remarks = recoveryRecord.Remarks;
				this.SmartUpdateRecoveryPurchaseOrderDetails(destinationRecord, recoveryRecord, cancellationToken);


				await _unitOfWork.PurchaseOrderRepository.UpdateAsync(destinationRecord, cancellationToken);
				resultId = destinationRecord.Id;
			}

			// header recovery
			int draftStatus = (int)BaseEntity.DraftStatus.MainRecord;
			if (destinationRecord != null)
				draftStatus = (int)BaseEntity.DraftStatus.Saved;
			
			recoveryRecord.IsDraftRecord = draftStatus;
			recoveryRecord.RecordActionDate = DateTime.Now;
			recoveryRecord.DraftFromUpload = false;

			foreach (var item in recoveryRecord.PurchaseOrderDetails)
			{
				item.IsDraftRecord = draftStatus;
				item.RecordActionDate = DateTime.Now;
				item.DraftFromUpload = false;
			}


			// save ke database
			await _unitOfWork.PurchaseOrderRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
		}
		
		private void SmartUpdateRecoveryPurchaseOrderDetails(PurchaseOrder destinationRecord, PurchaseOrder recoveryRecord, CancellationToken cancellationToken)
		{
			List<PurchaseOrderDetail> destinationToBeDeleted = new List<PurchaseOrderDetail>();
			if (destinationRecord.PurchaseOrderDetails.Count > 0)
			{
				foreach (var item in destinationRecord.PurchaseOrderDetails)
					destinationToBeDeleted.Add(item);
			}

			if (recoveryRecord.PurchaseOrderDetails.Count > 0)
			{
				foreach (var item in recoveryRecord.PurchaseOrderDetails)
				{
					var hasUpdate = false;
					if (destinationRecord.PurchaseOrderDetails.Count > 0)
					{
						var data = destinationRecord.PurchaseOrderDetails.SingleOrDefault(p => p.Id == item.MainRecordId);
						if (data != null)
						{
							data.PartId = item.PartId;
							data.PartPrice = item.PartPrice;
							data.Qty = item.Qty;
							data.TotalPrice = item.TotalPrice;

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
						destinationRecord.AddPurchaseOrderDetails( item.PartId, item.PartPrice, item.Qty, item.TotalPrice);

						item.IsDraftRecord = (int)BaseEntity.DraftStatus.Saved;
						item.RecordActionDate = DateTime.Now;
					}
				}
			}

			if (destinationToBeDeleted.Count > 0)
			{
				foreach (var item in destinationToBeDeleted)
					destinationRecord.RemovePurchaseOrderDetails(item);
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
			foreach (var item in recoveryRecord.PurchaseOrderDetails)
			{
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
				item.RecordActionDate = DateTime.Now;
			}


			// save ke database
			await _unitOfWork.PurchaseOrderRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new PurchaseOrderFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.PurchaseOrderRepository.ListAsync(spec, null, cancellationToken);
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
			var spec = new PurchaseOrderFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.PurchaseOrderRepository.ListAsync(spec, null, cancellationToken);
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
