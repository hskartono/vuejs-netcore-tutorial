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
	public class PartService : AsyncBaseService<Part>, IPartService
	{

		#region appgen: private variable

		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PartService(
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
		public async Task<Part> AddAsync(Part entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);



			await _unitOfWork.PartRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: count
		public async Task<int> CountAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PartRepository.CountAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: delete
		public async Task<bool> DeleteAsync(Part entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.PartRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: first record
		public async Task<Part> FirstAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PartRepository.FirstAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: first or default
		public async Task<Part> FirstOrDefaultAsync(ISpecification<Part> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PartRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}
		#endregion

		#region appgen: get by id
		public async Task<Part> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			return await GetByIdAsync(id, false, cancellationToken);
		}

		private async Task<Part> GetByIdAsync(string id, bool includeChilds = false, CancellationToken cancellationToken = default)
		{
			var specFilter = new PartFilterSpecification(id, true);
			var part = await _unitOfWork.PartRepository.FirstOrDefaultAsync(specFilter, cancellationToken);
			if (part == null || includeChilds == false)
				return part;



			return part;
		}
		#endregion

		#region appgen: list all
		public async Task<IReadOnlyList<Part>> ListAllAsync(List<SortingInformation<Part>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.PartRepository.ListAllAsync(sorting, cancellationToken);
		}
		#endregion

		#region appgen: get list
		public async Task<IReadOnlyList<Part>> ListAsync(
			ISpecification<Part> spec, 
			List<SortingInformation<Part>> sorting,
			bool withChilds = false,
			CancellationToken cancellationToken = default)
		{
			var parts = await _unitOfWork.PartRepository.ListAsync(spec, sorting, cancellationToken);
			if (withChilds && parts?.Count > 0)
			{
				var results = new List<Part>(parts);
				var partIds = parts.Select(e => e.Id).ToList();



				return results;
			}

			return parts;
		}
		#endregion

		#region appgen: update
		public async Task<bool> UpdateAsync(Part entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			cancellationToken.ThrowIfCancellationRequested();

			// update header
			AssignUpdater(entity);
			await _unitOfWork.PartRepository.ReplaceAsync(entity, entity.Id, cancellationToken);
			


			// update & commit
			//await _unitOfWork.PartRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}


		#endregion

		#endregion

		#region Validation Operations

		#region appgen: validatebase
		private bool ValidateBase(Part entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			if (entity == null)
				AddError("Tidak dapat menyimpan data kosong.");



			return ServiceState;
		}
		#endregion

		#region appgen: validateoninsert
		private bool ValidateOnInsert(Part entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#region appgen: validateonupdate
		private bool ValidateOnUpdate(Part entity, int? rowNum = null)
		{
			string rowInfo = (rowNum.HasValue) ? $"Baris {rowNum.Value}:" : string.Empty;

			ValidateBase(entity, rowNum);

			return ServiceState;
		}
		#endregion

		#endregion

		#region PDF Related

		#region appgen: generate pdf single
		public string GeneratePdf(Part entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));

			cancellationToken.ThrowIfCancellationRequested();

			// read template
			string templateFile = _uriComposer.ComposeTemplatePath("part.html");
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
		public async Task<string> GeneratePdfMultiPage(List<string> ids, int? refId = null, CancellationToken cancellationToken = default)
		{
			if (ids == null)
				throw new ArgumentNullException(nameof(ids));

			cancellationToken.ThrowIfCancellationRequested();

			var items = await this.ListAsync(new PartFilterSpecification(ids), null, true, cancellationToken);
			if (items == null || items.Count <= 0)
			{
				AddError($"Could not get data for list of id {ids.ToArray()}");
				return null;
			}

			string templateFile = _uriComposer.ComposeTemplatePath("part.html");
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
		public async Task<string> GeneratePdfMultiPageBackgroundProcess(List<string> ids, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("part") { StartTime = DateTime.Now };
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
		private string MapTemplateValue(string htmlContent, Part entity)
		{
			Dictionary<string, object> mapper = new Dictionary<string, object>()
				{
					{"Id",""},
					{"PartName",""},
					{"Description",""},

				};

			if (entity != null)
			{
				mapper["Id"] = entity.Id;
				mapper["PartName"] = entity.PartName;
				mapper["Description"] = entity.Description;

			}

			return BuildHtmlTemplate(htmlContent, mapper);
		}
		#endregion

		#endregion

		#region Excel Related

		#region appgen: generate excel background process
		public async Task<string> GenerateExcelBackgroundProcess(string excelFilename,
			string id = "", List<string> partNames = null, List<string> descriptions = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// insert dulu ke database
			var downloadProcess = new DownloadProcess("part") { StartTime = DateTime.Now };
			var result = await _downloadProcessService.AddAsync(downloadProcess, cancellationToken);
			if (result == null)
			{
				AddError("Failed to insert download process");
				return null;
			}

			// lempar ke background process
			var jobId = BackgroundJob.Enqueue(() => GenerateExcel(excelFilename, result.Id,
				id, partNames, descriptions,
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
			string id = "", List<string> partNames = null, List<string> descriptions = null,
			Dictionary<string, int> exact = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				PartFilterSpecification filterSpec = null;
				if (!string.IsNullOrEmpty(id))
					filterSpec = new PartFilterSpecification(id);
				else
					filterSpec = new PartFilterSpecification(exact)
					{

						Id = id, 
						PartNames = partNames, 
						Descriptions = descriptions
					}.BuildSpecification();

				var results = await this.ListAsync(filterSpec, null, true, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				if (ExcelMapper.WriteToExcel<Part>(excelFilename, "part.json", results) == false)
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
		public async Task<List<Part>> UploadExcel(string tempExcelFile, CancellationToken cancellationToken = default)
		{
			var result = ExcelMapper.ReadFromExcel<Part>(tempExcelFile, "part.json");
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
				if (!string.IsNullOrEmpty(id))
					await _unitOfWork.PartRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.PartRepository.AddAsync(item, cancellationToken);

			}

			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<Part> result, CancellationToken cancellationToken)
		{

		}

		private void SetUploadDraftFlags(List<Part> result)
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
			var spec = new PartFilterSpecification()
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
					if(string.IsNullOrEmpty(id))
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
		public async Task<bool> ProcessUploadedFile(IEnumerable<Part> parts, CancellationToken cancellationToken = default)
		{
			if (parts == null)
				throw new ArgumentNullException(nameof(parts));

			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				foreach (var item in parts)
				{
					var id = item.Id;
					if (string.IsNullOrEmpty(id))
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
			var spec = new PartFilterSpecification()
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
				var ws = package.Workbook.Worksheets.Add("Part");
				ws.Cells[1, 1].Value = "ID";
				ws.Cells[1, 2].Value = "PK";
				ws.Cells[1, 3].Value = "Part Name";
				ws.Cells[1, 4].Value = "Description";
				ws.Cells[1, 5].Value = "Status";
				ws.Cells[1, 6].Value = "Message";



				int row = 2;

				int pk = 1;
				foreach (var item in draftDatas)
				{
					ws.Cells[row, 1].Value = item.Id;
					ws.Cells[row, 2].Value = pk;
					ws.Cells[row, 3].Value = item.PartName;
					ws.Cells[row, 4].Value = item.Description;
					ws.Cells[row, 5].Value = item.UploadValidationStatus;
					ws.Cells[row, 6].Value = item.UploadValidationMessage;


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
		public async Task<Part> CreateDraft(CancellationToken cancellation)
		{
			var spec = new PartFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordIdIsNull = true,
				RecordEditedBy = _userName
			}.BuildSpecification();
			var count = await _unitOfWork.PartRepository.CountAsync(spec, cancellation);
			if (count > 0)
				return await _unitOfWork.PartRepository.FirstOrDefaultAsync(spec, cancellation);

			var entity = new Part();

			entity.IsDraftRecord = 1;
			entity.MainRecordId = null;
			entity.RecordEditedBy = _userName;
			entity.RecordActionDate = DateTime.Now;

			AssignCreatorAndCompany(entity);

			await _unitOfWork.PartRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync();
			return entity;
		}
		#endregion

		#region appgen: create edit draft
		public async Task<Part> CreateEditDraft(string id, CancellationToken cancellation)
		{

			var count = await this.CountAsync(new PartFilterSpecification(id), cancellation);
			if(count <= 0)
			{
				AddError($"Data Part dengan id {id} tidak ditemukan.");
				return null;
			}

			// cek apakah object dengan mode draft sudah ada
			var spec = new PartFilterSpecification()
			{
				MainRecordId = id,
				RecordEditedBy = _userName,
				ShowDraftList = BaseEntity.DraftStatus.DraftMode
			}.BuildSpecification();
			var previousDraft = await _unitOfWork.PartRepository.FirstOrDefaultAsync(spec, cancellation);
			if (previousDraft != null)
				return previousDraft;

			// clone data
			var cloneResult = await _unitOfWork.PartRepository.CloneEntity(id, _userName);
			if (cloneResult == null)
			{
				AddError($"Gagal membuat record Part");
				return null;
			}

			return await _unitOfWork.PartRepository.GetByIdAsync(cloneResult.Id, cancellation);

		}
		#endregion

		#region appgen: patch draft
		public async Task<bool> PatchDraft(Part part, CancellationToken cancellationToken)
		{
			var id = part.Id;
			var originalValue = await _unitOfWork.PartRepository.FirstOrDefaultAsync(
				new PartFilterSpecification(id));

			if(originalValue == null)
			{
				AddError($"Data dengan id {id} tidak ditemukan.");
				return false;
			}

			if (!string.IsNullOrEmpty(part.PartName)) originalValue.PartName = part.PartName;
			if (!string.IsNullOrEmpty(part.Description)) originalValue.Description = part.Description;


			// pastikan data belongsTo & hasMany tidak ikut


			AssignUpdater(originalValue);
			await _unitOfWork.PartRepository.UpdateAsync(originalValue, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}
		#endregion

		#region appgen: commit draft
		public async Task<string> CommitDraft(string id, CancellationToken cancellationToken)
		{
			string resultId = "";
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return "";

			Part destinationRecord = null;
			if (!string.IsNullOrEmpty(recoveryRecord.MainRecordId))
			{
				destinationRecord = await GetByIdAsync(recoveryRecord.MainRecordId, true, cancellationToken);
			}
			
			if (destinationRecord != null)
			{
				// recovery mode edit

				// header
				destinationRecord.PartName = recoveryRecord.PartName;
				destinationRecord.Description = recoveryRecord.Description;


				await _unitOfWork.PartRepository.UpdateAsync(destinationRecord, cancellationToken);
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
			await _unitOfWork.PartRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return (destinationRecord == null) ? recoveryRecord.Id : resultId;
		}
		

		#endregion

		#region appgen: discard draft
		public async Task<bool> DiscardDraft(string id, CancellationToken cancellationToken)
		{
			var recoveryRecord = await GetByIdAsync(id, true, cancellationToken);
			if (recoveryRecord == null) return false;

			// header
			recoveryRecord.IsDraftRecord = (int)BaseEntity.DraftStatus.Discarded;
			recoveryRecord.RecordActionDate = DateTime.Now;


			// save ke database
			await _unitOfWork.PartRepository.UpdateAsync(recoveryRecord, cancellationToken);
			await _unitOfWork.CommitAsync();

			return true;
		}
		#endregion

		#region appgen: get draft list
		public async Task<List<DocumentDraft>> GetDraftList(CancellationToken cancellationToken)
		{
			var spec = new PartFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				RecordEditedBy = _userName
			}.BuildSpecification();

			List<DocumentDraft> documentDrafts = new List<DocumentDraft>();
			var datas = await _unitOfWork.PartRepository.ListAsync(spec, null, cancellationToken);
			if (datas == null) return documentDrafts;

			foreach (var item in datas)
				documentDrafts.Add(
					new DocumentDraft($"Ada dokumen yang di edit terakhir pada {item.RecordActionDate?.ToString("dd-MMM-yyyy")} Jam {item.RecordActionDate?.ToString("hh:mm")} dan belum di simpan.", item.Id));

			return documentDrafts;
		}
		#endregion

		#region appgen: get current editor
		public async Task<List<string>> GetCurrentEditors(string id, CancellationToken cancellationToken)
		{
			var spec = new PartFilterSpecification()
			{
				ShowDraftList = BaseEntity.DraftStatus.DraftMode,
				MainRecordId = id
			}.BuildSpecification();

			var datas = await _unitOfWork.PartRepository.ListAsync(spec, null, cancellationToken);
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
