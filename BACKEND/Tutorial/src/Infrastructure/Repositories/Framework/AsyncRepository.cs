using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Repositories
{
	public class AsyncRepository<T> : IAsyncRepository<T> where T : CoreEntity
	{
		protected readonly DbContext Context;

		public AsyncRepository(AppDbContext context)
		{
			Context = context;
		}

		public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			await Context.Set<T>().AddAsync(entity);
			//await Context.SaveChangesAsync(cancellationToken);

			return entity;
		}

		public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
		{
			var specificationResult = ApplySpecification(spec);
			return await specificationResult.CountAsync(cancellationToken);
		}

		public void DeleteAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
		{
			IQueryable<T> query = Context.Set<T>().AsQueryable();
			Context.Set<T>().RemoveRange(query);
		}

		public void DeleteAsync(T entity, CancellationToken cancellationToken = default)
		{
			Context.Set<T>().Remove(entity);
			//await Context.SaveChangesAsync(cancellationToken);
		}

		public async Task<T> FirstAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
		{
			var specificationResult = ApplySpecification(spec);
			return await specificationResult.FirstAsync(cancellationToken);
		}

		public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
		{
			var specificationResult = ApplySpecification(spec);
			return await specificationResult.FirstOrDefaultAsync(cancellationToken);
		}

		public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			var keyValues = new object[] { id };
			return await Context.Set<T>().FindAsync(keyValues, cancellationToken);
		}

		public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		{
			var keyValues = new object[] { id };
			return await Context.Set<T>().FindAsync(keyValues, cancellationToken);
		}

		public async Task<IReadOnlyList<T>> ListAllAsync(List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default)
		{
			IQueryable<T> query = Context.Set<T>().AsQueryable();
			query = ApplySorting(sorting, query);
			return await query.ToListAsync(cancellationToken);

			// return await Context.Set<T>().ToListAsync(cancellationToken);
		}

		public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, List<SortingInformation<T>> sorting, CancellationToken cancellationToken = default)
		{
			var specificationResult = ApplySpecification(spec, sorting);
			return await specificationResult.ToListAsync(cancellationToken);
		}

		public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
		{
			Context.Entry(entity).State = EntityState.Modified;
			// await Context.SaveChangesAsync(cancellationToken);
		}

		public async Task ReplaceAsync(T entity, int id, CancellationToken cancellationToken = default)
		{
			T originalData = await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
			if (cancellationToken.IsCancellationRequested) 
				return;

			if (originalData == null) 
				throw new Exception($"{nameof(T)} with id {id} not found.");

			//entity.CompanyId = originalData.CompanyId;
			//entity.CreatedBy = originalData.CreatedBy;
			//entity.CreatedDate = originalData.CreatedDate;

			EntityEntry<T> entry = Context.Entry<T>(originalData);
			entry.CurrentValues.SetValues(entity);
			entry.State = EntityState.Modified;
		}

		public async Task ReplaceAsync(T entity, string id, CancellationToken cancellationToken = default)
		{
			T originalData = await Context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
				return;

			if (originalData == null)
				throw new Exception($"{nameof(T)} with id {id} not found.");

			//entity.CompanyId = originalData.CompanyId;
			//entity.CreatedBy = originalData.CreatedBy;
			//entity.CreatedDate = originalData.CreatedDate;

			EntityEntry<T> entry = Context.Entry<T>(originalData);
			entry.CurrentValues.SetValues(entity);
			entry.State = EntityState.Modified;
		}

		private IQueryable<T> ApplySorting(List<SortingInformation<T>> sorting, IQueryable<T> query)
		{
			if (sorting != null && sorting.Count > 0)
			{
				foreach (var sortingInfo in sorting)
				{
					if (sortingInfo.SortType == SortingType.Descending)
					{
						query = query.AppendOrderByDescending<T, dynamic>(sortingInfo.Predicate);
					}
					else
					{
						query = query.AppendOrderBy<T, dynamic>(sortingInfo.Predicate);
					}
				}
			}

			return query;
		}

		protected IQueryable<T> ApplySpecification(ISpecification<T> spec, List<SortingInformation<T>> sorting = null)
		{
			if (spec == null)
				return Context.Set<T>().AsQueryable();
			
			var evaluator = new SpecificationEvaluator<T>();
			var queryResult = evaluator.GetQuery(Context.Set<T>().AsQueryable(), spec);

			// queryResult = ApplySorting(sorting, queryResult);

			return queryResult;
		}

		/// <summary>
		/// Method untuk mengambil data dari custom SQL menjadi datatable
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		protected async Task<DataTable> SqlToDataTableAsync(string sql)
		{
			DataTable dt = new DataTable();
			var conn = Context.Database.GetDbConnection();
			await conn.OpenAsync();
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText = sql;
				cmd.CommandType = CommandType.Text;
				var reader = await cmd.ExecuteReaderAsync();
				dt.Load(reader);
				reader.Close();
			}
			await conn.CloseAsync();
			
			return dt;
		}

		/// <summary>
		/// Method untuk mengeksekusi action query ke database
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
		protected async Task<int> ExecuteNonQueryAsync(string sql)
		{
			int result = 0;
			var conn = Context.Database.GetDbConnection();
			await conn.OpenAsync();
			using (var cmd = conn.CreateCommand())
			{
				cmd.CommandText = sql;
				cmd.CommandType = CommandType.Text;
				result = await cmd.ExecuteNonQueryAsync();
			}
			await conn.CloseAsync();

			return result;
		}

		/// <summary>
		/// Method untuk mengambil nilai dari datarow
		/// </summary>
		/// <param name="row"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		protected int GetDataRowValue(DataRow row, string field, int defaultValue = default)
		{
			if (row[field] == DBNull.Value) return defaultValue;
			return (int)row[field];
		}

		protected double GetDataRowValue(DataRow row, string field, double defaultValue = default)
		{
			if (row[field] == DBNull.Value) return defaultValue;
			return (double)row[field];
		}

		protected string GetDataRowValue(DataRow row, string field, string defaultValue = default)
		{
			if (row[field] == DBNull.Value) return defaultValue;
			return (string)row[field];
		}

		protected DateTime GetDataRowValue(DataRow row, string field, DateTime defaultValue = default)
		{
			if (row[field] == DBNull.Value) return defaultValue;
			return (DateTime)row[field];
		}

		protected bool GetDataRowValue(DataRow row, string field, bool defaultValue = default)
		{
			if (row[field] == DBNull.Value) return defaultValue;
			return (bool)row[field];
		}
	}
}
