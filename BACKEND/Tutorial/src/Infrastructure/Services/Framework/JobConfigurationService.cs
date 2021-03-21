using Ardalis.Specification;
using Microsoft.Extensions.Logging;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Services
{
	public class JobConfigurationService : AsyncBaseService<JobConfiguration>, IJobConfigurationService
	{
		public JobConfigurationService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<JobConfiguration> AddAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			await _unitOfWork.JobConfigurationRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.JobConfigurationRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<JobConfiguration> FirstAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<JobConfiguration> FirstOrDefaultAsync(ISpecification<JobConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<JobConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<JobConfiguration>> ListAllAsync(List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<JobConfiguration>> ListAsync(ISpecification<JobConfiguration> spec, List<SortingInformation<JobConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.JobConfigurationRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(JobConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.JobConfigurationRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(JobConfiguration entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(JobConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(JobConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
