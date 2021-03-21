using Ardalis.Specification;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Services
{
	public class FunctionInfoService : AsyncBaseService<FunctionInfo>, IFunctionInfoService
	{
		public FunctionInfoService(IUnitOfWork unitOfWork) : base(unitOfWork) { }

		public async Task<FunctionInfo> AddAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnInsert(entity))
				return null;

			AssignCreatorAndCompany(entity);
			await _unitOfWork.FunctionInfoRepository.AddAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return entity;
		}

		public async Task<int> CountAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.CountAsync(spec, cancellationToken);
		}

		public async Task<bool> DeleteAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			_unitOfWork.FunctionInfoRepository.DeleteAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		public async Task<FunctionInfo> FirstAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.FirstAsync(spec, cancellationToken);
		}

		public async Task<FunctionInfo> FirstOrDefaultAsync(ISpecification<FunctionInfo> spec, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.FirstOrDefaultAsync(spec, cancellationToken);
		}

		public async Task<FunctionInfo> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.GetByIdAsync(id, cancellationToken);
		}

		public async Task<IReadOnlyList<FunctionInfo>> ListAllAsync(List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.ListAllAsync(sorting, cancellationToken);
		}

		public async Task<IReadOnlyList<FunctionInfo>> ListAsync(ISpecification<FunctionInfo> spec, List<SortingInformation<FunctionInfo>> sorting, CancellationToken cancellationToken = default)
		{
			return await _unitOfWork.FunctionInfoRepository.ListAsync(spec, sorting, cancellationToken);
		}

		public async Task<bool> UpdateAsync(FunctionInfo entity, CancellationToken cancellationToken = default)
		{
			if (!ValidateOnUpdate(entity))
				return false;

			AssignUpdater(entity);
			await _unitOfWork.FunctionInfoRepository.UpdateAsync(entity, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);
			return true;
		}

		private bool ValidateBase(FunctionInfo functionInfo)
		{

			return ServiceState;
		}

		private bool ValidateOnInsert(FunctionInfo functionInfo)
		{
			ValidateBase(functionInfo);

			return ServiceState;
		}

		private bool ValidateOnUpdate(FunctionInfo functionInfo)
		{
			ValidateBase(functionInfo);

			return ServiceState;
		}
	}
}
