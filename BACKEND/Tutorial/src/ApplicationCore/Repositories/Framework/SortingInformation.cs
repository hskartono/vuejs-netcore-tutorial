using System;
using System.Linq.Expressions;

namespace Tutorial.ApplicationCore.Repositories
{
	public class SortingInformation<T> where T : class
	{
		private Expression<Func<T, dynamic>> _predicate;
		private SortingType _sortType;

		public SortingInformation(Expression<Func<T, dynamic>> predicate, SortingType sortType)
		{
			_predicate = predicate;
			_sortType = sortType;
		}

		public Expression<Func<T, dynamic>> Predicate {
			get 
			{
				return _predicate;
			}

			set 
			{
				_predicate = value;
			}
		}

		public SortingType SortType
		{
			get
			{
				return _sortType;
			}

			set
			{
				_sortType = value;
			}
		}
	}

	public enum SortingType
	{
		Ascending,
		Descending
	}
}
