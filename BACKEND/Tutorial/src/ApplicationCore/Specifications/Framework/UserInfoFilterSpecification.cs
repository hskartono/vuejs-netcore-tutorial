using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using System.Collections.Generic;

namespace Tutorial.ApplicationCore.Specifications
{
	public class UserInfoFilterSpecification : Specification<UserInfo>
	{
		public UserInfoFilterSpecification(string userName)
		{
			InitializeFilterData(userName: userName);
		}

		public UserInfoFilterSpecification(List<string> userNames)
		{
			foreach (var user in userNames)
				Query.Where(e => userNames.Contains(user));
		}

		public UserInfoFilterSpecification(int skip, int take)
		{
			InitializeFilterData(skip, take);
		}

		public UserInfoFilterSpecification(string userName, string firstName, string lastName)
		{
			InitializeFilterData(userName: userName, firstName: firstName, lastName: lastName);
		}

		public UserInfoFilterSpecification(int skip, int take, string userName, string firstName, string lastName)
		{
			InitializeFilterData(skip, take, userName, firstName, lastName);
		}

		private void InitializeFilterData(int? skip = null, int? take = null, string userName = "", string firstName = "", string lastName = "")
		{
			Query
				.Where(
					e => (string.IsNullOrEmpty(userName) || e.UserName == userName) &&
					(string.IsNullOrEmpty(firstName) || e.FirstName.Contains(firstName)) &&
					(string.IsNullOrEmpty(lastName) || e.LastName.Contains(lastName))
				);

			if (take.HasValue && take.Value > 0)
				Query
					.Skip(skip.Value)
					.Take(take.Value);
		}
	}
}
