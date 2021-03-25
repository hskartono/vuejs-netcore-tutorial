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
	public class PurchaseOrderDetailController : BaseAPIController
	{

		#region appgen: private variable

		private const string functionId = "purchase_order_detail";
		private readonly ILogger<PurchaseOrderDetailController> _logger;
		private readonly IPurchaseOrderDetailService _purchaseOrderDetailService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		#endregion

		#region appgen: constructor

		public PurchaseOrderDetailController(
			IPurchaseOrderDetailService purchaseOrderDetailService,
			IMapper mapper,
			ILogger<PurchaseOrderDetailController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_purchaseOrderDetailService = purchaseOrderDetailService;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		#endregion

		#region CRUD Operation

		#region appgen: get list
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<PurchaseOrderDetailDTO>>> GetListAsync(
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
			var totalItems = await _purchaseOrderDetailService.CountAsync(filterSpec, cancellation);

			var sortingSpec = GenerateSortingSpec(sorting);
			var pagedFilterSpec = GenerateFilter(filter, exact, pageSize, pageIndex, sortingSpec);
			var items = await _purchaseOrderDetailService.ListAsync(pagedFilterSpec, sortingSpec, false, cancellation);

			var model = new PaginatedItemsViewModel<PurchaseOrderDetailDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<PurchaseOrderDetailDTO>));
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
			var items = await _purchaseOrderDetailService.ListAsync(filterSpec, sortingSpec, false, cancellation);

			var model = items.Select(e => e.Id);
			return Ok(model);
		}
		#endregion

		#region appgen: get by id
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(GetIdAsync))]
		public async Task<ActionResult<PurchaseOrderDetailDTO>> GetIdAsync(int id, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowRead) return ValidationProblem();
			var item = await _purchaseOrderDetailService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(PurchaseOrderDetail), id);
			return _mapper.Map<PurchaseOrderDetailDTO>(item);
		}
		#endregion

		#region appgen: create record
		[HttpPost]
		public async Task<ActionResult> CreateAsync([FromBody] PurchaseOrderDetailDTO purchaseOrderDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			// remove temporary id (if any)

			var newItem = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(newItem);

			newItem = await _purchaseOrderDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(GetIdAsync), new { id = newItem.Id }, null);
		}
		#endregion

		#region appgen: update record
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult> UpdateAsync([FromBody] PurchaseOrderDetailDTO purchaseOrderDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseOrderDetailFilterSpecification(int.Parse(purchaseOrderDetail.Id), true);
			var rowCount = await _purchaseOrderDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseOrderDetail), purchaseOrderDetail.Id);

			// bind to old item


			var objItem = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetail);

			// untuk data yang mereference object, perlu di set null agar tidak insert sebagai data baru
			CleanReferenceObject(objItem);

			var result = await _purchaseOrderDetailService.PatchDraft(objItem, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var itemToDelete = await _purchaseOrderDetailService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(PurchaseOrderDetail), id);

			// delete data
			var result = await _purchaseOrderDetailService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
		public async Task<ActionResult> CreateDraftAsync([FromBody] PurchaseOrderDetailDTO purchaseOrderDetail, CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate) return ValidationProblem();
			var newItem = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetail);
			 newItem = await _purchaseOrderDetailService.CreateDraft(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
				return ValidationProblem();
			}

			return Ok(_mapper.Map<PurchaseOrderDetailDTO>(newItem));
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
			var editItem = await _purchaseOrderDetailService.CreateEditDraft(id, cancellationToken);
			if (editItem == null)
				return NotFound();

			return Ok(_mapper.Map<PurchaseOrderDetailDTO>(editItem));
		}
		*/
		#endregion

		#region appgen: update patch
		[HttpPut]
		[Route("update/{id}")]
		public async Task<ActionResult> UpdateDraftAsync(int id, [FromBody] PurchaseOrderDetailDTO purchaseOrderDetailDto, CancellationToken cancellationToken = default)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var specFilter = new PurchaseOrderDetailFilterSpecification(int.Parse(purchaseOrderDetailDto.Id), true);
			var rowCount = await _purchaseOrderDetailService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(PurchaseOrderDetail), purchaseOrderDetailDto.Id);

			var purchaseOrderDetail = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetailDto);
			var result = await _purchaseOrderDetailService.PatchDraft(purchaseOrderDetail, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var mainRecordId = await _purchaseOrderDetailService.CommitDraft(id, cancellationToken);
			if (mainRecordId <= 0)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var result = await _purchaseOrderDetailService.DiscardDraft(id, cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var draftList = await _purchaseOrderDetailService.GetDraftList(cancellationToken);
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
			var currentEditors = await _purchaseOrderDetailService.GetCurrentEditors(id, cancellationToken);
			return Ok(currentEditors);
		}
		*/
		#endregion

		#region appgen: replace async
		[HttpPost]
		[Route("replace")]
		public async Task<ActionResult> ReplaceAsync(
			[FromBody] List<PurchaseOrderDetail> purchaseOrderDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = purchaseOrderDetails.Select(_mapper.Map<PurchaseOrderDetail>).ToList();

			foreach (var item in newItems)
				CleanReferenceObject(item);

			newItems = await _purchaseOrderDetailService.ReplaceDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
				return ValidationProblem();
			}

			return Ok();
		}
		#endregion

		#region appgen: add async
		[HttpPost]
		[Route("add")]
		public async Task<ActionResult> AddAsync(
			[FromBody] List<PurchaseOrderDetail> purchaseOrderDetails,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowCreate && !AllowUpdate) return ValidationProblem();
			var newItems = purchaseOrderDetails.Select(_mapper.Map<PurchaseOrderDetail>).ToList();

			foreach(var item in newItems)
				CleanReferenceObject(item);

			newItems = await _purchaseOrderDetailService.AddDraftAsync(newItems, cancellationToken);
			if (newItems == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var item = await _purchaseOrderDetailService.GetByIdAsync(id, cancellationToken);
			var result = _purchaseOrderDetailService.GeneratePdf(item, cancellationToken);
			if (result == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			string key = await _purchaseOrderDetailService.GeneratePdfMultiPageBackgroundProcess(idToPrint, cancellationToken);
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
		[Route("upload/{id}")]
		public async Task<ActionResult<IEnumerable<PurchaseOrderDetailDTO>>> UploadAsync(
			IFormFile file,
			[FromQuery] int id,
			CancellationToken cancellationToken)
		{
			InitUserInfo();
			if (!AllowUpload) return ValidationProblem();
			var filePath = System.IO.Path.GetTempFileName();
			using(var stream = System.IO.File.Create(filePath))
			{
				await file.CopyToAsync(stream, cancellationToken);
			}

			var result = await _purchaseOrderDetailService.UploadExcel(filePath, id, cancellationToken);
			if(result == null)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
				return ValidationProblem();
			}
			
			List<PurchaseOrderDetailDTO> resultDto = result.Select(_mapper.Map<PurchaseOrderDetailDTO>).ToList();
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
			var result = await _purchaseOrderDetailService.CommitUploadedFile(cancellationToken);
			if (!result)
			{
				AssignToModelState(_purchaseOrderDetailService.Errors);
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
			var result = await _purchaseOrderDetailService.GenerateUploadLogExcel(cancellationToken);

			var fileName = "PurchaseOrderDetail.xlsx";
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
			List<int> purchaseOrderId = null;
			if (filterParams.ContainsKey("purchaseOrderId")) 
			{
				purchaseOrderId = new List<int>();
				foreach(var item in filterParams["purchaseOrderId"])
				{
					var data = int.Parse(item);
					purchaseOrderId.Add(data);
				}
			}
			List<string> partId = (filterParams.ContainsKey("partId") ? filterParams["partId"] : null);
			List<double> partPrice = null;
			if (filterParams.ContainsKey("partPrice")) 
			{
				partPrice = new List<double>();
				foreach(var item in filterParams["partPrice"])
				{
					var data = double.Parse(item);
					partPrice.Add(data);
				}
			}
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
			List<double> totalPrice = null;
			if (filterParams.ContainsKey("totalPrice")) 
			{
				totalPrice = new List<double>();
				foreach(var item in filterParams["totalPrice"])
				{
					var data = double.Parse(item);
					totalPrice.Add(data);
				}
			}


			string fileName = Guid.NewGuid().ToString() + ".xlsx";
			var excelFile = _uriComposer.ComposeDownloadPath(fileName);
/*
			string key = await _purchaseOrderDetailService.GenerateExcelBackgroundProcess(excelFile, 
				id, purchaseOrderId, partId, partPrice, qty, totalPrice, 
				exact, cancellationToken);
			Dictionary<string, string> result = new Dictionary<string, string>() { {"id", key} };
			return Ok(result);
*/
			string generatedFilename = await _purchaseOrderDetailService.GenerateExcel(excelFile, null,
				id, purchaseOrderId, partId, partPrice, qty, totalPrice, 
				exact, cancellationToken);
			fileName = "PurchaseOrderDetail.xlsx";
			byte[] fileBytes = System.IO.File.ReadAllBytes(generatedFilename);
			return File(fileBytes, "application/xlsx", fileName);

		}
		#endregion

		#endregion

		#region Private Methods

		#region appgen: generate filter
		private PurchaseOrderDetailFilterSpecification GenerateFilter(Dictionary<string, Dictionary<string, List<string>>> filters, 
			Dictionary<string, int> exact,
			int pageSize = 0, int pageIndex = 0,
			List<SortingInformation<PurchaseOrderDetail>> sorting = null)
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
			List<int> purchaseOrderId = null;
			if (filterParams.ContainsKey("purchaseOrderId")) 
			{
				purchaseOrderId = new List<int>();
				foreach(var item in filterParams["purchaseOrderId"])
				{
					var data = int.Parse(item);
					purchaseOrderId.Add(data);
				}
			}
			List<string> partId = (filterParams.ContainsKey("partId") ? filterParams["partId"] : null);
			List<double> partPrice = null;
			if (filterParams.ContainsKey("partPrice")) 
			{
				partPrice = new List<double>();
				foreach(var item in filterParams["partPrice"])
				{
					var data = double.Parse(item);
					partPrice.Add(data);
				}
			}
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
			List<double> totalPrice = null;
			if (filterParams.ContainsKey("totalPrice")) 
			{
				totalPrice = new List<double>();
				foreach(var item in filterParams["totalPrice"])
				{
					var data = double.Parse(item);
					totalPrice.Add(data);
				}
			}

			
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
				return new PurchaseOrderDetailFilterSpecification(exact)
				{

					Id = id, 
					PurchaseOrderIds = purchaseOrderId, 
					PartIds = partId, 
					PartPrices = partPrice, 
					Qtys = qty, 
					TotalPrices = totalPrice,

					MainRecordId = mainRecordId,
					MainRecordIdIsNull = mainRecordIsNull,
					RecordEditedBy = recordEditedBy,
					DraftFromUpload = draftFromUpload,
					ShowDraftList = (BaseEntity.DraftStatus) draftMode
				}
				.BuildSpecification(true, sorting);
			}

			return new PurchaseOrderDetailFilterSpecification(skip: pageSize * pageIndex, take: pageSize, exact)
			{

					Id = id, 
					PurchaseOrderIds = purchaseOrderId, 
					PartIds = partId, 
					PartPrices = partPrice, 
					Qtys = qty, 
					TotalPrices = totalPrice,

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
		private List<SortingInformation<PurchaseOrderDetail>> GenerateSortingSpec(Dictionary<string, bool> sorting)
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
		
		private List<SortingInformation<PurchaseOrderDetail>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting.ContainsKey("pageIndex")) sorting.Remove("pageIndex");
			if (sorting.ContainsKey("pageSize")) sorting.Remove("pageSize");
			if (sorting.ContainsKey("exact")) sorting.Remove("exact");
			if (sorting.ContainsKey("filter")) sorting.Remove("filter");

			if (sorting == null || sorting.Count == 0)
				sorting = new Dictionary<string, int>() { { "createddate", 0 } };

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<PurchaseOrderDetail>> sortingSpec = new List<SortingInformation<PurchaseOrderDetail>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.Id, sortingOrder));
						break;
					case "purchaseorder":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.PurchaseOrder.PoNumber, sortingOrder));
						break;
					case "part":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.Part.PartName, sortingOrder));
						break;
					case "partprice":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.PartPrice, sortingOrder));
						break;
					case "qty":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.Qty, sortingOrder));
						break;
					case "totalprice":
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.TotalPrice, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<PurchaseOrderDetail>(p => p.Id, sortingOrder));
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
		private void CleanReferenceObject(PurchaseOrderDetail entity)
		{
			entity.PurchaseOrder = null;
			entity.Part = null;


		}
		#endregion

		#region appgen: init user
		private void InitUserInfo()
		{
			LoadIdentity();
			_purchaseOrderDetailService.UserInfo = _user;
		}
		#endregion

		#endregion
	}
}
