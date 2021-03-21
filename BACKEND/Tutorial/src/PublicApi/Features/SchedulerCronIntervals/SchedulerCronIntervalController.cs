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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.PublicApi.Features.SchedulerCronIntervals
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SchedulerCronIntervalController : BaseAPIController
	{
		private const string functionId = "scheduler_cron_interval";
		private readonly ILogger<SchedulerCronIntervalController> _logger;
		private readonly ISchedulerCronIntervalService _schedulerCronIntervalService;
		private readonly IMapper _mapper;

		public SchedulerCronIntervalController(
			ISchedulerCronIntervalService schedulerCronIntervalService,
			IMapper mapper,
			ILogger<SchedulerCronIntervalController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_schedulerCronIntervalService = schedulerCronIntervalService;
			_schedulerCronIntervalService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<SchedulerCronIntervalDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _schedulerCronIntervalService.CountAsync(filterSpec, cancellation);
			if (totalItems < 0)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _schedulerCronIntervalService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<SchedulerCronIntervalDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<SchedulerCronIntervalDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<SchedulerCronIntervalDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _schedulerCronIntervalService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), id);
			return _mapper.Map<SchedulerCronIntervalDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] SchedulerCronIntervalDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<SchedulerCronInterval>(schedulerCronInterval);
			newItem = await _schedulerCronIntervalService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] SchedulerCronIntervalDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			var specFilter = new SchedulerCronIntervalFilterSpecification(schedulerCronInterval.Id);
			var rowCount = await _schedulerCronIntervalService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), schedulerCronInterval.Id);

			// bind to old item
			var item = _mapper.Map<SchedulerCronInterval>(schedulerCronInterval);

			var result = await _schedulerCronIntervalService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
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
			var itemToDelete = await _schedulerCronIntervalService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(SchedulerCronInterval), id);

			// delete data
			var result = await _schedulerCronIntervalService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerCronIntervalService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private SchedulerCronIntervalFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string code = (filter.ContainsKey("code") ? filter["code"] : string.Empty);
			string name = (filter.ContainsKey("name") ? filter["name"] : string.Empty);

			if (pageSize == 0)
				return new SchedulerCronIntervalFilterSpecification(code, name);

			return new SchedulerCronIntervalFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				code: code,
				name: name
			);
		}

		private List<SortingInformation<SchedulerCronInterval>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<SchedulerCronInterval>> sortingSpec = new List<SortingInformation<SchedulerCronInterval>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Id, sortingOrder));
						break;

					case "code":
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Code, sortingOrder));
						break;

					case "name":
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.Name, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<SchedulerCronInterval>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
