using Ardalis.Specification;
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
	public class SchedulerConfigurationService : AsyncBaseService<SchedulerConfiguration>, ISchedulerConfigurationService
	{
		public SchedulerConfigurationService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<SchedulerConfiguration> AddAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			await _unitOfWork.SchedulerConfigurationRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.SchedulerConfigurationRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<SchedulerConfiguration> FirstAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<SchedulerConfiguration> FirstOrDefaultAsync(ISpecification<SchedulerConfiguration> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<SchedulerConfiguration> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<SchedulerConfiguration>> ListAllAsync(List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<SchedulerConfiguration>> ListAsync(ISpecification<SchedulerConfiguration> spec, List<SortingInformation<SchedulerConfiguration>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerConfigurationRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(SchedulerConfiguration entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.SchedulerConfigurationRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(SchedulerConfiguration entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(SchedulerConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(SchedulerConfiguration entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
