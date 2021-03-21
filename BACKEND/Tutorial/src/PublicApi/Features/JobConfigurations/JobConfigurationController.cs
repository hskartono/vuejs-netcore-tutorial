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

namespace Tutorial.PublicApi.Features.JobConfigurations
{
	[Authorize]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class JobConfigurationController : BaseAPIController
	{
		private const string functionId = "scheduler_configuration";
		private readonly ILogger<JobConfigurationController> _logger;
		private readonly IJobConfigurationService _jobConfigurationService;
		private readonly IMapper _mapper;

		public JobConfigurationController(
			IJobConfigurationService jobConfigurationService,
			IMapper mapper,
			ILogger<JobConfigurationController> logger,
			IUserInfoService userInfoService) : base(userInfoService, functionId)
		{
			_jobConfigurationService = jobConfigurationService;
			_jobConfigurationService.UserName = _userName;
			_mapper = mapper;
			_logger = logger;
		}

		// GET api/v1/[controller]/[?pageSize=3&pageIndex=10&filter[propertyname]=filtervalue&sorting[propertyname]=1]
		[HttpGet]
		public async Task<ActionResult<PaginatedItemsViewModel<JobConfigurationDTO>>> ItemsAsync(
			[FromQuery] int pageSize = 10,
			[FromQuery] int pageIndex = 0,
			[FromQuery] Dictionary<string, string> filter = default,
			[FromQuery] Dictionary<string, int> sorting = default,
			CancellationToken cancellation = default)
		{
			var filterSpec = GenerateFilter(filter);
			var totalItems = await _jobConfigurationService.CountAsync(filterSpec, cancellation);
			if (totalItems < 0)
			{
				AssignToModelState(_jobConfigurationService.Errors);
				return ValidationProblem();
			}

			var pagedFilterSpec = GenerateFilter(filter, pageSize, pageIndex);
			var sortingSpec = GenerateSortingSpec(sorting);
			var items = await _jobConfigurationService.ListAsync(pagedFilterSpec, sortingSpec, cancellation);

			var model = new PaginatedItemsViewModel<JobConfigurationDTO>(pageIndex, pageSize, totalItems, items.Select(_mapper.Map<JobConfigurationDTO>));
			return Ok(model);
		}

		// GET api/v1/[controller]/1
		[HttpGet]
		[Route("{id:int}")]
		[ActionName(nameof(ItemByIdAsync))]
		public async Task<ActionResult<JobConfigurationDTO>> ItemByIdAsync(int id, CancellationToken cancellationToken)
		{
			var item = await _jobConfigurationService.GetByIdAsync(id, cancellationToken);
			if (item == null)
				throw new EntityNotFoundException(nameof(JobConfiguration), id);
			return _mapper.Map<JobConfigurationDTO>(item);
		}

		// POST api/v1/[controller]
		[HttpPost]
		public async Task<ActionResult> CreateItemAsync([FromBody] JobConfigurationDTO schedulerCronInterval, CancellationToken cancellationToken)
		{
			var newItem = _mapper.Map<JobConfiguration>(schedulerCronInterval);
			newItem = await _jobConfigurationService.AddAsync(newItem, cancellationToken);
			if (newItem == null)
			{
				AssignToModelState(_jobConfigurationService.Errors);
				return ValidationProblem();
			}

			return CreatedAtAction(nameof(ItemByIdAsync), new { id = newItem.Id }, null);
		}

		// PUT api/v1/[controller]
		[HttpPut]
		public async Task<ActionResult> UpdateItemAsync([FromBody] JobConfigurationDTO jobConfiguration, CancellationToken cancellationToken)
		{
			var specFilter = new JobConfigurationFilterSpecification(jobConfiguration.Id);
			var rowCount = await _jobConfigurationService.CountAsync(specFilter, cancellationToken);
			if (rowCount == 0)
				throw new EntityNotFoundException(nameof(JobConfiguration), jobConfiguration.Id);

			// bind to old item
			var item = _mapper.Map<JobConfiguration>(jobConfiguration);

			var result = await _jobConfigurationService.UpdateAsync(item, cancellationToken);
			if (!result)
			{
				AssignToModelState(_jobConfigurationService.Errors);
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
			var itemToDelete = await _jobConfigurationService.GetByIdAsync(id, cancellationToken);
			if (itemToDelete == null)
				throw new EntityNotFoundException(nameof(JobConfiguration), id);

			// delete data
			var result = await _jobConfigurationService.DeleteAsync(itemToDelete, cancellationToken);
			if (!result)
			{
				AssignToModelState(_jobConfigurationService.Errors);
				return ValidationProblem();
			}

			return NoContent();
		}

		private JobConfigurationFilterSpecification GenerateFilter(Dictionary<string, string> filter, int pageSize = 0, int pageIndex = 0)
		{
			string interfaceName = (filter.ContainsKey("interfaceName") ? filter["interfaceName"] : string.Empty);
			string jobName = (filter.ContainsKey("jobName") ? filter["jobName"] : string.Empty);
			bool? isStoredProcedure = (filter.ContainsKey("isStoredProcedure") ? (filter["isStoredProcedure"] == "1") : null);

			if (pageSize == 0)
				return new JobConfigurationFilterSpecification(interfaceName, jobName, isStoredProcedure);

			return new JobConfigurationFilterSpecification(
				skip: pageSize * pageIndex,
				take: pageSize,
				interfaceName: interfaceName,
				jobName: jobName,
				isStoredProcedure: isStoredProcedure
			);
		}

		private List<SortingInformation<JobConfiguration>> GenerateSortingSpec(Dictionary<string, int> sorting)
		{
			if (sorting == null || sorting.Count == 0)
				return null;

			var sortingOrder = SortingType.Ascending;
			List<SortingInformation<JobConfiguration>> sortingSpec = new List<SortingInformation<JobConfiguration>>();
			foreach (var sort in sorting)
			{
				sortingOrder = SortingType.Descending;
				if (sort.Value == 1) sortingOrder = SortingType.Ascending;

				// catatan: disini perlu dibuat seluruh property yang di tampilkan di table agar bisa di sorting
				switch (sort.Key.ToLower())
				{
					case "id":
						sortingSpec.Add(new SortingInformation<JobConfiguration>(p => p.Id, sortingOrder));
						break;

					case "jobname":
						sortingSpec.Add(new SortingInformation<JobConfiguration>(p => p.JobName, sortingOrder));
						break;

					case "isstoredprocedure":
						sortingSpec.Add(new SortingInformation<JobConfiguration>(p => p.IsStoredProcedure, sortingOrder));
						break;

					default:
						sortingSpec.Add(new SortingInformation<JobConfiguration>(p => p.CreatedBy, sortingOrder));
						break;
				}
			}

			return sortingSpec;
		}
	}
}
