using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Exceptions;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.DownloadProcesses
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class DownloadProcessController : BaseAPIController
	{
		private const string functionId = "download_process";
		private readonly ILogger<DownloadProcessController> _logger;
		private readonly IDownloadProcessService _downloadProcessService;
		private readonly IMapper _mapper;
		private readonly IUriComposer _uriComposer;

		public DownloadProcessController(
			IDownloadProcessService downloadProcessService,
			IMapper mapper,
			ILogger<DownloadProcessController> logger,
			IUriComposer uriComposer,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_downloadProcessService = downloadProcessService;
			_downloadProcessService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
			_uriComposer = uriComposer;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<DownloadProcessDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _downloadProcessService.CountAsync(filterSpec, cancellation);

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _downloadProcessService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<DownloadProcessDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<DownloadProcessDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<DownloadProcessDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			return _mapper.Map<DownloadProcessDTO>(item);
		}

		// GET api/v1/[controller]/status/1
		[HttpGet]
		[Route("status/{id}")]
		public async Task<ActionResult<Dictionary<string, string>>> GetStatusAsync(string id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByJobIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var downloadUrl = _uriComposer.ComposeDownloadUri(item.FileName);
			return new Dictionary<string, string>()
			{
				{"status", item.Status},
				{"filename", downloadUrl }
			};
		}

		[HttpGet]
		[Route("download/{id}")]
		public async Task<IActionResult> DownloadAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var downloadUrl = _uriComposer.ComposeDownloadUri(item.FileName);
			var result = new Dictionary<string, string>()
			{
				{"download", downloadUrl}
			};

			return Ok(result);
		}

		[HttpGet]
		[Route("directdownload/{id}")]
		public async Task<IActionResult> DirectDownloadAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			var filePath = _uriComposer.ComposeDownloadPath(item.FileName);
			var fileExtension = Path.GetExtension(filePath).ToLower();

			var mimeType = $"application/{fileExtension}";
			if (fileExtension == "pdf")
			{
				mimeType = "application/pdf";
			}
			else if (fileExtension == "xlsx")
			{
				mimeType = "application/xlsx";
			}

			var fileName = $"download{fileExtension}";
			byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
			return File(fileBytes, mimeType, fileName);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] DownloadProcessDTO downloadProcessDto, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<DownloadProcess>(downloadProcessDto);
			newItem = await _downloadProcessService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] DownloadProcessDTO downloadProcessDto, CancellationToken cancellationToken)
		{
			var specFilter = new DownloadProcessFilterSpecification(downloadProcessDto.Id);
			var rowCount = await _downloadProcessService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(downloadProcessDto), downloadProcessDto.Id);

			var item = _mapper.Map<DownloadProcess>(downloadProcessDto);

			var result = await _downloadProcessService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		// DELETE api/v1/[controller]/id
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteItemAsync(int id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _downloadProcessService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(UserInfo), id);

			// delete data
			var result = await _downloadProcessService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_downloadProcessService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private DownloadProcessFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string jobId = (filter.ContainsKey("jobid") ? filter["jobid"] : string.Empty);
			string functionId = (filter.ContainsKey("functionid") ? filter["functionid"] : string.Empty);
			string status = (filter.ContainsKey("status") ? filter["status"] : string.Empty);

			if (pageSize == 0)
				return new DownloadProcessFilterSpecification(jobId, functionId, status);

			return new DownloadProcessFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				jobId: jobId,
				functionId: functionId,
				status: status
			);
		}

		private List<SortingInformation<DownloadProcess>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<DownloadProcess>> sortingSpec = new List<SortingInformation<DownloadProcess>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.Id, sortingOrder));
						break;

					case "jobid":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.JobId, sortingOrder));
						break;

					case "functionid":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.FunctionId, sortingOrder));
						break;

					case "status":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.Status, sortingOrder));
						break;

					case "filename":
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.FileName, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<DownloadProcess>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
