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

namespace Tutorial.PublicApi.Features.SchedulerConfigurations
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SchedulerConfigurationController : BaseAPIController
	{
		private const string functionId = "scheduler_configuration";
		private readonly ILogger<SchedulerConfigurationController> _logger;
		private readonly ISchedulerConfigurationService _schedulerConfigurationService;
		private readonly IMapper _mapper;

		public SchedulerConfigurationController(
			ISchedulerConfigurationService schedulerConfigurationService,
			IMapper mapper,
			ILogger<SchedulerConfigurationController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_schedulerConfigurationService = schedulerConfigurationService;
			_schedulerConfigurationService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<SchedulerConfigurationDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _schedulerConfigurationService.CountAsync(filterSpec, cancellation);
			if (totalItems < 0)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _schedulerConfigurationService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<SchedulerConfigurationDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<SchedulerConfigurationDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<SchedulerConfigurationDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _schedulerConfigurationService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), id);
			return _mapper.Map<SchedulerConfigurationDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] SchedulerConfigurationDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<SchedulerConfiguration>(schedulerCronInterval);
			newItem = await _schedulerConfigurationService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] SchedulerConfigurationDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			var specFilter = new SchedulerConfigurationFilterSpecification(schedulerCronInterval.Id);
			var rowCount = await _schedulerConfigurationService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), schedulerCronInterval.Id);

			// bind to old item
			var item = _mapper.Map<SchedulerConfiguration>(schedulerCronInterval);

			var result = await _schedulerConfigurationService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
		}

		// DELETE api/v1/[controller]/id
		[Route("{id}")]
		[HttpDelete]
		public async Task<IActionResult> DeleteItemAsync(int id, CancellationToken cancellationToken)
		{
			// validate if data exists
			var itemToDelete = await _schedulerConfigurationService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(SchedulerConfiguration), id);

			// delete data
			var result = await _schedulerConfigurationService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_schedulerConfigurationService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private SchedulerConfigurationFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string jobType = (filter.ContainsKey("jobType") ? filter["jobType"] : string.Empty);
			string recurringJobId = (filter.ContainsKey("recurringJobId") ? filter["recurringJobId"] : string.Empty);

			if (pageSize == 0)
				return new SchedulerConfigurationFilterSpecification(jobType, recurringJobId);

			return new SchedulerConfigurationFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				jobType: jobType,
				recurringJobId: recurringJobId
			);
		}

		private List<SortingInformation<SchedulerConfiguration>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<SchedulerConfiguration>> sortingSpec = new List<SortingInformation<SchedulerConfiguration>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.Id, sortingOrder));
						break;

					case "jobtype":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.JobType, sortingOrder));
						break;

					case "recurringjobid":
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.RecurringJobId, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<SchedulerConfiguration>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
