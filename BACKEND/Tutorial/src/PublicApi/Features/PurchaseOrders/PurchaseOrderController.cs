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

namespace Tutorial.PublicApi.Features.PurchaseOrders
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class PurchaseOrderController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "purchase_order";
		private readonly ILogger<PurchaseOrderController> _logger;
		private readonly IPurchaseOrderService _purchaseOrderService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseOrderController(
			IPurchaseOrderService purchaseOrderService,
			IMapper mapper,
			ILogger<PurchaseOrderController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_purchaseOrderService = purchaseOrderService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<PurchaseOrderDTO>>> GetListAsync(
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
			var totalItems = await _purchaseOrderService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _purchaseOrderService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<PurchaseOrderDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<PurchaseOrderDTO>));
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
			var items = await _purchaseOrderService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<PurchaseOrderDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(PurchaseOrder), id);
			return _mapper.Map<PurchaseOrderDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] PurchaseOrderDTO purchaseOrder, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)
			foreach (var item in purchaseOrder.PurchaseOrderDetails)
			{
				if (!int.TryParse(item.Id, out _)) item.Id = "0";
				if (!int.TryParse(item.PurchaseOrderId, out _)) item.PurchaseOrderId = "0";
			}

			var newItem = _mapper.Map<PurchaseOrder>(purchaseOrder);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(newItem);

			newItem = await _purchaseOrderService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] PurchaseOrderDTO purchaseOrder, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseOrderFilterSpecification(int.Parse(purchaseOrder.Id), true);
			var rowCount = await _purchaseOrderService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseOrder), purchaseOrder.Id);

			// bind to old item
			foreach (var item in purchaseOrder.PurchaseOrderDetails)
			{
				if (!int.TryParse(item.Id, out _)) item.Id = "0";
				if (!int.TryParse(item.PurchaseOrderId, out _)) item.PurchaseOrderId = "0";
			}


			var objItem = _mapper.Map<PurchaseOrder>(purchaseOrder);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(objItem);

			var result = await _purchaseOrderService.UpdateAsync(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderService.Errors);
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
			var itemToDelete = await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(PurchaseOrder), id);

			// delete data
			var result = await _purchaseOrderService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderService.Errors);
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
		public async Task<ActionResult> CreateDraftAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = await _purchaseOrderService.CreateDraft(cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<PurchaseOrderDTO>(newItem));
		}
		#endregion

		#region appgen: edit draft
		
		[HttpGet]
		[Route("edit/{id}")]
		public async Task<ActionResult> CreateEditDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var editItem = await _purchaseOrderService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<PurchaseOrderDTO>(editItem));
		}
		
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] PurchaseOrderDTO purchaseOrderDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseOrderFilterSpecification(int.Parse(purchaseOrderDto.Id), true);
			var rowCount = await _purchaseOrderService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseOrder), purchaseOrderDto.Id);

			var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderDto);
			var result = await _purchaseOrderService.PatchDraft(purchaseOrder, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		}
		#endregion

		#region appgen: commit changes
		
		[HttpPut]
		[Route("commit/{id}")]
		public async Task<ActionResult> CommitDraftAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var mainRecordId = await _purchaseOrderService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = id }, null);
		}
		
		#endregion

		#region appgen: discard changes
		[HttpDelete]
		[Route("discard/{id}")]
		public async Task<IActionResult> DiscardAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			var result = await _purchaseOrderService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}
		#endregion

		#region appgen: get draft list
		
		[HttpGet]
		[Route("draftlist")]
		public async Task<IActionResult> DraftListAsync(CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var draftList = await _purchaseOrderService.GetDraftList(cancellationToken);
			return Ok(draftList);
		}
		
		#endregion

		#region appgen: get current editor
		
		[HttpGet]
		[Route("currenteditor/{id}")]
		public async Task<IActionResult> CurrentEditorAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var currentEditors = await _purchaseOrderService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
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
			var item = await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
			var result = _purchaseOrderService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_purchaseOrderService.Errors);
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
			string key = await _purchaseOrderService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
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
		public async Task<ActionResult<IEnumerable<PurchaseOrderDTO>>> UploadAsync(
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

			var result = await _purchaseOrderService.UploadExcel(filePath, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_purchaseOrderService.Errors);
				return ValidationProblem();
			}
			
			List<PurchaseOrderDTO> resultDto = result.Select(_mapper.Map<PurchaseOrderDTO>).ToList();
			foreach(var item in resultDto)
			{
				if (string.IsNullOrEmpty(item.Id) || item.Id == "0")
				{
					var newguid = Guid.NewGuid().ToString();
					item.Id = newguid;
				}
				foreach(var subitem in item.PurchaseOrderDetails)
				{
					if (string.IsNullOrEmpty(subitem.Id) || subitem.Id == "0")
					{
						var newguid = Guid.NewGuid().ToString();
						subitem.Id = newguid;
						subitem.PurchaseOrderId = item.Id;
					}
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
			var result = await _purchaseOrderService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderService.Errors);
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
			var result = await _purchaseOrderService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "PurchaseOrder.xlsx";
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
			List<string> poNumber = (filterParams.ContainsKey("poNumber") ? filterParams["poNumber"] : null);
			DateTime? poDateFrom = (filterParams.ContainsKey("poDateFrom") ? DateTime.Parse(filterParams["poDateFrom"][0]) : null);
			DateTime? poDateTo = (filterParams.ContainsKey("poDateTo") ? DateTime.Parse(filterParams["poDateTo"][0]) : null);
			List<string> remarks = (filterParams.ContainsKey("remarks") ? filterParams["remarks"] : null);


			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);

			string key = await _purchaseOrderService.GenerateExcelBackgroundProcess(excelFile, 
				id, poNumber, poDateFrom, poDateTo, remarks, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
/*
			string generatedFilename = await _purchaseOrderService.GenerateExcel(excelFile, null,
				id, poNumber, poDateFrom, poDateTo, remarks, 
				exact, cancellationToken);
			fileName = "PurchaseOrder.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);
*/
		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private PurchaseOrderFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<PurchaseOrder>> sorting = null)
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
			List<string> poNumber = (filterParams.ContainsKey("poNumber") ? filterParams["poNumber"] : null);
			DateTime? poDateFrom = (filterParams.ContainsKey("poDateFrom") ? DateTime.Parse(filterParams["poDateFrom"][0]) : null);
			DateTime? poDateTo = (filterParams.ContainsKey("poDateTo") ? DateTime.Parse(filterParams["poDateTo"][0]) : null);
			List<string> remarks = (filterParams.ContainsKey("remarks") ? filterParams["remarks"] : null);

			
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
			int draftMode = (filterParams.ContainsKey("draftMode") ? int.Parse(filterParams["draftMode"][0]) : 0);

			if (pageSize == 0)
			{
				return new PurchaseOrderFilterSpecification(exact)
				{

					Id = id, 
					PoNumbers = poNumber, 
					PoDateFrom = poDateFrom, 
					PoDateTo = poDateTo, 
					Remarkss = remarks,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new PurchaseOrderFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					PoNumbers = poNumber, 
					PoDateFrom = poDateFrom, 
					PoDateTo = poDateTo, 
					Remarkss = remarks,

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
		private List<SortingInformation<PurchaseOrder>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private List<SortingInformation<PurchaseOrder>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<PurchaseOrder>> sortingSpec = new List<SortingInformation<PurchaseOrder>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<PurchaseOrder>(p => p.Id, sortingOrder));
						break;
					case "ponumber":
						sortingSpec.Add(new SortingInformation<PurchaseOrder>(p => p.PoNumber, sortingOrder));
						break;
					case "podate":
						sortingSpec.Add(new SortingInformation<PurchaseOrder>(p => p.PoDate, sortingOrder));
						break;
					case "remarks":
						sortingSpec.Add(new SortingInformation<PurchaseOrder>(p => p.Remarks, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<PurchaseOrder>(p => p.Id, sortingOrder));
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
		private void CleanReferenceObject(PurchaseOrder entity)
		{
			foreach (var item in entity.PurchaseOrderDetails)
			{
				item.PurchaseOrder = null;
				item.Part = null;
			}


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_purchaseOrderService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
