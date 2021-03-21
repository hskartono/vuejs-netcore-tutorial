using Ardalis.Specification;
using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Services
{
	public interface IAsyncBaseService<T> where T : CoreEntity
	{
        string UserName { get; set; }
        UserInfo UserInfo { get; set; }
        string FunctionId { get; set; }
        void AddError(string errorMessage);
        void ClearErrors();
        IReadOnlyList<string> Errors { get; }
        bool ServiceState { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
