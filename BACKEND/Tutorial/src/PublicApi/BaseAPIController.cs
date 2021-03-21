using Audit.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using Tutorial.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Tutorial.PublicApi
{
	[ApiController]
	public class BaseAPIController : ControllerBase
	{
		protected string _userName;
		protected UserInfo _user;
		protected int _pageSize;
		private IUserInfoService _userInfoService;
		protected Dictionary<string, RoleDetail> _role = new Dictionary<string, RoleDetail>();
		protected string _functionId;

		protected const string ERR_NOT_ALLOWED= "Access denied.";

		public string UserName { get { return _userName; } set { _userName = value; } } 

		public BaseAPIController()
		{
			LoadDefaultPagingSize();
		}

		public BaseAPIController(string functionId)
		{
			_functionId = functionId;
			LoadDefaultPagingSize();
		}

		public BaseAPIController(IUserInfoService userInfoService, string functionId)
		{
			_userInfoService = userInfoService;
			_functionId = functionId;
			LoadDefaultPagingSize();
		}

		protected void LoadIdentity()
		{
			if(!string.IsNullOrEmpty(_userName) && _userInfoService != null)
			{
				var filterSpec = new UserInfoFilterSpecification(_userName, string.Empty, string.Empty);
				_user = _userInfoService.FirstOrDefaultAsync(filterSpec).Result;

				var roleService = new RoleService(_userInfoService.UnitOfWork);
				var role = roleService.GetUserRole(_userName).Result;
				if(role != null)
				{
					_role.Clear();
					foreach(var item in role.RoleDetails)
					{
						if (_role.ContainsKey(item.FunctionInfoId)) continue;
						_role.Add(item.FunctionInfoId, item);
					}
				}
			}
		}

		protected void LoadDefaultPagingSize()
		{
			_pageSize = 10;
		}

		protected void AssignToModelState(IEnumerable<string> errors)
		{
			if (errors == null) return;
			foreach(var err in errors)
			{
				ModelState.AddModelError(Guid.NewGuid().ToString(), err);
			}
		}

		private bool CheckPrivilege(bool privilege)
		{
			if (string.IsNullOrEmpty(_functionId) || !_role.ContainsKey(_functionId) || !privilege)
			{
				ModelState.AddModelError("Privilege", ERR_NOT_ALLOWED);
				return false;
			}

			return privilege;
		}

		protected bool AllowCreate
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowCreate : false);
			}
		}

		protected bool AllowRead
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowRead : false);
			}
		}

		protected bool AllowUpdate
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowUpdate : false);
			}
		}

		protected bool AllowDelete
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowDelete : false);
			}
		}

		protected bool AllowDownload
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowDownload : false);
			}
		}

		protected bool AllowPrint
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowPrint : false);
			}
		}

		protected bool AllowUpload
		{
			get
			{
				return CheckPrivilege((_role.ContainsKey(_functionId)) ? _role[_functionId].AllowUpload : false);
			}
		}
	}
}
