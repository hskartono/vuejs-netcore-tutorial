using Ardalis.Specification;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Services
{
	public class SchedulerCronIntervalService : AsyncBaseService<SchedulerCronInterval>, ISchedulerCronIntervalService
	{
		public SchedulerCronIntervalService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<SchedulerCronInterval> AddAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			await _unitOfWork.SchedulerCronIntervalRepository.AddAsync(entity);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.SchedulerCronIntervalRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<SchedulerCronInterval> FirstAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<SchedulerCronInterval> FirstOrDefaultAsync(ISpecification<SchedulerCronInterval> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<SchedulerCronInterval> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<SchedulerCronInterval>> ListAllAsync(
			List<SortingInformation<SchedulerCronInterval>> sorting, 
			CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<SchedulerCronInterval>> ListAsync(
			ISpecification<SchedulerCronInterval> spec, List<SortingInformation<SchedulerCronInterval>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.SchedulerCronIntervalRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(SchedulerCronInterval entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			await _unitOfWork.SchedulerCronIntervalRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync();
			return true;
		}

		private bool ValidateBase(SchedulerCronInterval entity)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(SchedulerCronInterval entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}

		private bool ValidateOnUpdate(SchedulerCronInterval entity)
		{
			ValidateBase(entity);

			return ServiceState;
		}
	}
}
