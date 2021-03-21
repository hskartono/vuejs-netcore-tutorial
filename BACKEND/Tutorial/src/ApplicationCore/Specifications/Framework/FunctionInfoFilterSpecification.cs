using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;

namespace Tutorial.ApplicationCore.Specifications
{
	public class FunctionInfoFilterSpecification : Specification<FunctionInfo>
	{
		public FunctionInfoFilterSpecification(int skip, int take)
		{
			InitializeSpecification(skip, take);
		}

		public FunctionInfoFilterSpecification(int skip, int take, string name, bool? isEnabled)
		{
			InitializeSpecification(skip, take, name, isEnabled);
		}

		public FunctionInfoFilterSpecification(string name, bool? isEnabled)
		{
			InitializeSpecification(name: name, isEnabled: isEnabled);
		}

		private void InitializeSpecification(int? skip = null, int? take = null, string name = "", bool? isEnabled = false)
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(name) || e.Name.Contains(name)) &&
					(!isEnabled.HasValue || e.IsEnabled == isEnabled)
				);

			if(skip.HasValue && take.HasValue)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
