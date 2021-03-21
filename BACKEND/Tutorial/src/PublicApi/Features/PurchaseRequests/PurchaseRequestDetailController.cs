using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Exceptions;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.PurchaseRequests
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class PurchaseRequestDetailController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "purchase_request_detail";
		private readonly ILogger<PurchaseRequestDetailController> _logger;
		private readonly IPurchaseRequestDetailService _purchaseRequestDetailService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseRequestDetailController(
			IPurchaseRequestDetailService purchaseRequestDetailService,
			IMapper mapper,
			ILogger<PurchaseRequestDetailController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_purchaseRequestDetailService = purchaseRequestDetailService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<PurchaseRequestDetailDTO>>> GetListAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			CleanFilter(filter);
			var filterSpec = GenerateFilter(filter, exact);
			var totalItems = await _purchaseRequestDetailService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _purchaseRequestDetailService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<PurchaseRequestDetailDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<PurchaseRequestDetailDTO>));
			return Ok(model);
		}
		#endregion

		#region appgen: get id list
		[HttpGet]
		[Route("ids")]
		public async Task<ActionResult<List<int>>> GetIdsAsync(
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, bool> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellation = default)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			CleanFilter(filter);
			var sortingSpec = GenerateSortingSpec(sorting);
			var filterSpec = GenerateFilter(filter, exact, 0, 0, sortingSpec);
			var items = await _purchaseRequestDetailService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<PurchaseRequestDetailDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _purchaseRequestDetailService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(PurchaseRequestDetail), id);
			return _mapper.Map<PurchaseRequestDetailDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] PurchaseRequestDetailDTO purchaseRequestDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<PurchaseRequestDetail>(purchaseRequestDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(newItem);

			newItem = await _purchaseRequestDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] PurchaseRequestDetailDTO purchaseRequestDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseRequestDetailFilterSpecification(int.Parse(purchaseRequestDetail.Id), true);
			var rowCount = await _purchaseRequestDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseRequestDetail), purchaseRequestDetail.Id);

			// bind to old item


			var objItem = _mapper.Map<PurchaseRequestDetail>(purchaseRequestDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(objItem);

			var result = await _purchaseRequestDetailService.PatchDraft(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = objItem.Id }, null);
		}
		#endregion

		#region appgen: delete record
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowDelete) return ValidationProblem();
			// validate if data exists
			var itemToDelete = await _purchaseRequestDetailService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(PurchaseRequestDetail), id);

			// delete data
			var result = await _purchaseRequestDetailService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#endregion

		#region Recovery Record Controller

		#region appgen: create draft
		[HttpPost]
		[Route("create")]
		public async Task<ActionResult> CreateDraftAsync([FromBody] PurchaseRequestDetailDTO purchaseRequestDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = _mapper.Map<PurchaseRequestDetail>(purchaseRequestDetail);
			 newItem = await _purchaseRequestDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<PurchaseRequestDetailDTO>(newItem));
		}
		#endregion

		#region appgen: edit draft
		/*
		[HttpGet]
		[Route("edit/{id}")]
		public async Task<ActionResult> CreateEditDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var editItem = await _purchaseRequestDetailService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<PurchaseRequestDetailDTO>(editItem));
		}
		*/
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] PurchaseRequestDetailDTO purchaseRequestDetailDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseRequestDetailFilterSpecification(int.Parse(purchaseRequestDetailDto.Id), true);
			var rowCount = await _purchaseRequestDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseRequestDetail), purchaseRequestDetailDto.Id);

			var purchaseRequestDetail = _mapper.Map<PurchaseRequestDetail>(purchaseRequestDetailDto);
			var result = await _purchaseRequestDetailService.PatchDraft(purchaseRequestDetail, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		}
		#endregion

		#region appgen: commit changes
		/*
		[HttpPut]
		[Route("commit/{id}")]
		public async Task<ActionResult> CommitDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var mainRecordId = await _purchaseRequestDetailService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		}
		*/
		#endregion

		#region appgen: discard changes
		[HttpDelete]
		[Route("discard/{id}")]
		public async Task<IActionResult> DiscardAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _purchaseRequestDetailService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#region appgen: get draft list
		/*
		[HttpGet]
		[Route("draftlist")]
		public async Task<IActionResult> DraftListAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var draftList = await _purchaseRequestDetailService.GetDraftList(cancellationToken);
			return Ok(draftList);
		}
		*/
		#endregion

		#region appgen: get current editor
		/*
		[HttpGet]
		[Route("currenteditor/{id}")]
		public async Task<IActionResult> CurrentEditorAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var currentEditors = await _purchaseRequestDetailService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		*/
		#endregion

		#region appgen: replace async
		[HttpPost]
		[Route("replace")]
		public async Task<ActionResult> ReplaceAsync(
			[FromBody] List<PurchaseRequestDetail> purchaseRequestDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = purchaseRequestDetails.Select(_mapper.Map<PurchaseRequestDetail>).ToList();

			foreach (var item in newItems)
				CleanReferenceObject(item);

			newItems = await _purchaseRequestDetailService.ReplaceDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: add async
		[HttpPost]
		[Route("add")]
		public async Task<ActionResult> AddAsync(
			[FromBody] List<PurchaseRequestDetail> purchaseRequestDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = purchaseRequestDetails.Select(_mapper.Map<PurchaseRequestDetail>).ToList();

			foreach(var item in newItems)
				CleanReferenceObject(item);

			newItems = await _purchaseRequestDetailService.AddDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItems[0].Id }, null);
		}
		#endregion

		#endregion

		#region PDF operation

		#region appgen: generate single pdf
		[HttpPost]
		[Route("singlepagepdf")]
		public async Task<IActionResult> SinglePdfAsync(Dictionary<string, List<int>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var id = ids["ids"][0];
			var item = await _purchaseRequestDetailService.GetByIdAsync(id, cancellationToken);
			var result = _purchaseRequestDetailService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return BadRequest(ModelState);
			}

			var downloadUrl = _uriComposer.ComposeDownloadUri(System.IO.Path.GetFileName(result));
			var pdfResult = new Dictionary<string, string>()
			{
				{"download", downloadUrl}
			};

			return Ok(pdfResult);
		}
		#endregion

		#region appgen: generate multipage pdf
		[HttpPost]
		[Route("multipagePdf")]
		public async Task<IActionResult> MultipagePdfAsync([FromBody] Dictionary<string, List<int>> ids, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowPrint) return ValidationProblem();
			var idToPrint = ids["ids"];
			string key = await _purchaseRequestDetailService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>()
			{
				{"id", key}
			};
			return Ok(result);
		}
		#endregion

		#endregion

		#region File Upload & Download Operation

		#region appgen: upload excel file
		[HttpPost]
		[Route("upload")]
		public async Task<ActionResult<IEnumerable<PurchaseRequestDetailDTO>>> UploadAsync(
			IFormFile file, 
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpload) return ValidationProblem();
			var filePath = System.IO.Path.GetTempFileName();
			using(var stream = System.IO.File.Create(filePath))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			var result = await _purchaseRequestDetailService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}
			
			List<PurchaseRequestDetailDTO> resultDto = result.Select(_mapper.Map<PurchaseRequestDetailDTO>).ToList();
			foreach(var item in resultDto)
			{
				if (string.IsNullOrEmpty(item.Id) || item.Id == "0")
				{
					var newguid = Guid.NewGuid().ToString();
					item.Id = newguid;
				}
			}


return CreatedAtAction(nameof(GetIdAsync), new { id = result[0].Id }, null);
		}
		#endregion

		#region appgen: confirm uploaded file
		[HttpPost]
		[Route("confirmUpload")]
		public async Task<IActionResult> ConfirmUploadAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpload) return ValidationProblem();
			var result = await _purchaseRequestDetailService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseRequestDetailService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: download log
		[HttpPost]
		[Route("downloadLog")]
		public async Task<IActionResult> DownloadLogAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _purchaseRequestDetailService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "PurchaseRequestDetail.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(result);
			return File(fileBytes, "application/xlsx", fileName);
		}
		#endregion

		#region appgen: download excel data
		[HttpGet]
		[Route("downloaddata")]
		public async Task<IActionResult> DownloadDataAsync(
			[FromQuery] Dictionary<string, Dictionary<string, List<string>>> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			[FromQuery] Dictionary<string, int> exact = default,
			CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowDownload) return ValidationProblem();
			CleanFilter(filter);
			Dictionary<string, List<string>> filterParams = new Dictionary<string, List<string>>();
			foreach (string fieldName in filter.Keys)
			{
				if (filter[fieldName].Count <= 0) continue;
				filterParams.Add(fieldName, new List<string>());
				foreach (var itemValue in filter[fieldName][""])
				{
					filterParams[fieldName].Add(itemValue);
				}
			}

			int? id = (filterParams.ContainsKey("id") ? int.Parse(filterParams["id"][0]) : null);
			List<int> purchaseRequestId = null;
			if (filterParams.ContainsKey("purchaseRequestId")) 
			{
				purchaseRequestId = new List<int>();
				foreach(var item in filterParams["purchaseRequestId"])
				{
					var data = int.Parse(item);
					purchaseRequestId.Add(data);
				}
			}
			List<string> partId = (filterParams.ContainsKey("partId") ? filterParams["partId"] : null);
			List<int> qty = null;
			if (filterParams.ContainsKey("qty")) 
			{
				qty = new List<int>();
				foreach(var item in filterParams["qty"])
				{
					var data = int.Parse(item);
					qty.Add(data);
				}
			}
			DateTime? requestDateFrom = (filterParams.ContainsKey("requestDateFrom") ? DateTime.Parse(filterParams["requestDateFrom"][0]) : null);
			DateTime? requestDateTo = (filterParams.ContainsKey("requestDateTo") ? DateTime.Parse(filterParams["requestDateTo"][0]) : null);


			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);
/*
			string key = await _purchaseRequestDetailService.GenerateExcelBackgroundProcess(excelFile, 
				id, purchaseRequestId, partId, qty, requestDateFrom, requestDateTo, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
*/
			string generatedFilename = await _purchaseRequestDetailService.GenerateExcel(excelFile, null,
				id, purchaseRequestId, partId, qty, requestDateFrom, requestDateTo, 
				exact, cancellationToken);
			fileName = "PurchaseRequestDetail.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);

		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private PurchaseRequestDetailFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<PurchaseRequestDetail>> sorting = null)
		{
			Dictionary<string, List<string>> filterParams = new Dictionary<string, List<string>>();
			foreach (string fieldName in filters.Keys)
			{
				if (filters[fieldName].Count <= 0) continue;
				filterParams.Add(fieldName, new List<string>());
				foreach (var itemValue in filters[fieldName][""])
				{
					filterParams[fieldName].Add(itemValue);
				}
			}
			int? id = (filterParams.ContainsKey("id") ? int.Parse(filterParams["id"][0]) : null);
			List<int> purchaseRequestId = null;
			if (filterParams.ContainsKey("purchaseRequestId")) 
			{
				purchaseRequestId = new List<int>();
				foreach(var item in filterParams["purchaseRequestId"])
				{
					var data = int.Parse(item);
					purchaseRequestId.Add(data);
				}
			}
			List<string> partId = (filterParams.ContainsKey("partId") ? filterParams["partId"] : null);
			List<int> qty = null;
			if (filterParams.ContainsKey("qty")) 
			{
				qty = new List<int>();
				foreach(var item in filterParams["qty"])
				{
					var data = int.Parse(item);
					qty.Add(data);
				}
			}
			DateTime? requestDateFrom = (filterParams.ContainsKey("requestDateFrom") ? DateTime.Parse(filterParams["requestDateFrom"][0]) : null);
			DateTime? requestDateTo = (filterParams.ContainsKey("requestDateTo") ? DateTime.Parse(filterParams["requestDateTo"][0]) : null);

			
			// RECOVERY FILTER	
			int? mainRecordId = (filterParams.ContainsKey("mainRecordId") ? int.Parse(filterParams["mainRecordId"][0]) : null);
			bool mainRecordIsNull = (filterParams.ContainsKey("mainRecordIsNull") ? bool.Parse(filterParams["mainRecordIsNull"][0]) : false);
			string recordEditedBy = (filterParams.ContainsKey("recordEditedBy") ? filterParams["recordEditedBy"][0] : null);
			if (filterParams.ContainsKey("draftFromUpload"))
			{
				if (filterParams["draftFromUpload"][0] == "1") 
					filterParams["draftFromUpload"][0] = "true";
				else
					filterParams["draftFromUpload"][0] = "false";
			}
			bool? draftFromUpload = (filterParams.ContainsKey("draftFromUpload") ? bool.Parse(filterParams["draftFromUpload"][0]) : false);
			if (filterParams.ContainsKey("draftMode"))
			{
				if (filterParams["draftMode"][0] == "true") filterParams["draftMode"][0] = "1";
				if (filterParams["draftMode"][0] == "false") filterParams["draftMode"][0] = "0";
			}
			int draftMode = (filterParams.ContainsKey("draftMode") ? int.Parse(filterParams["draftMode"][0]) : -1);

			if (pageSize == 0)
			{
				return new PurchaseRequestDetailFilterSpecification(exact)
				{

					Id = id, 
					PurchaseRequestIds = purchaseRequestId, 
					PartIds = partId, 
					Qtys = qty, 
					RequestDateFrom = requestDateFrom, 
					RequestDateTo = requestDateTo,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new PurchaseRequestDetailFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					PurchaseRequestIds = purchaseRequestId, 
					PartIds = partId, 
					Qtys = qty, 
					RequestDateFrom = requestDateFrom, 
					RequestDateTo = requestDateTo,

				MainRecordId = mainRecordId,
				MainRecordIdIsNull = mainRecordIsNull,
				RecordEditedBy = recordEditedBy,
				DraftFromUpload = draftFromUpload,
				ShowDraftList = (BaseEntity.DraftStatus) draftMode
			}
			.BuildSpecification(true, sorting);
		}
		#endregion

		#region appgen: generate sorting
		private List<SortingInformation<PurchaseRequestDetail>> GenerateSortingSpec(Dictionary<string, bool> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;
			var newDict = new Dictionary<string, int>();
			foreach(var key in sorting.Keys)
			{
				if (sorting[key] == true)
					newDict.Add(key, 1);
				else
					newDict.Add(key, 0);
			}

			return GenerateSortingSpec(newDict);
		}
		
		private List<SortingInformation<PurchaseRequestDetail>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<PurchaseRequestDetail>> sortingSpec = new List<SortingInformation<PurchaseRequestDetail>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.Id, sortingOrder));
						break;
					case "purchaserequest":
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.PurchaseRequest.PrNo, sortingOrder));
						break;
					case "part":
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.Part.PartName, sortingOrder));
						break;
					case "qty":
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.Qty, sortingOrder));
						break;
					case "requestdate":
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.RequestDate, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<PurchaseRequestDetail>(p => p.Id, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
		#endregion

		#region appgen: clean filter parameters
		private void CleanFilter(Dictionary<string, Dictionary<string, List<string>>> filter)
		{
			if (filter.ContainsKey("exact")) filter.Remove("exact");
			if (filter.ContainsKey("sorting")) filter.Remove("sorting");
			if (filter.ContainsKey("pageSize")) filter.Remove("pageSize");
			if (filter.ContainsKey("pageIndex")) filter.Remove("pageIndex");
		}
		#endregion

		#region appgen: clean reference object
		private void CleanReferenceObject(PurchaseRequestDetail entity)
		{
			entity.PurchaseRequest = null;
			entity.Part = null;


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_purchaseRequestDetailService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
