using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Tutorial.Infrastructure.Services
{
	public class AsyncBaseService<T> : IAsyncBaseService<T> where T : CoreEntity
	{
		protected readonly IUnitOfWork _unitOfWork;
		private readonly List<string> _errors = new List<string>();
		private string _functionId;
		protected UserInfo _user = null;
		protected string _userName;
		protected int _companyId;

		public AsyncBaseService(IUnitOfWork unitOfWork) 
		{
			_unitOfWork = unitOfWork;
		}

		#region Error message holder

		public IReadOnlyList<string> Errors => _errors;

		public void AddError(string errorMessage)
		{
			if (_errors.Contains(errorMessage)) return;
			_errors.Add(errorMessage);
		}

		public void ClearErrors()
		{
			_errors.Clear();
		}

		public bool ServiceState => _errors.Count == 0;

		#endregion;

		#region public methods
		public UserInfo UserInfo {
			get { 
				return _user; 
			}
			set { 
				_user = value;
				if(_user != null)
				{
					_userName = _user.UserName;
					_companyId = _user.CompanyId;
				}
			}
		}

		public string UserName {
			get { 
				return _user.UserName; 
			}
			set
			{
				_userName = value;
				if(_user == null && !String.IsNullOrEmpty(_userName))
				{
					_user = _unitOfWork.UserInfoRepository.FirstOrDefaultAsync(new UserInfoFilterSpecification(_userName)).Result;
				}

				if (_user != null)
				{
					_userName = _user.UserName;
					_companyId = _user.CompanyId;
				}
			}
		}

		public string FunctionId { get => _functionId; set => _functionId = value; }

		public IUnitOfWork UnitOfWork => _unitOfWork;

		public string BuildHtmlTemplate(string htmlTemplate, Dictionary<string, object> content)
		{
			if (string.IsNullOrEmpty(htmlTemplate) || content == null)
				return string.Empty;

			List<string> keyCollections = new List<string>();

			string result = htmlTemplate;
			foreach(string key in content.Keys)
			{
				if (content[key] is IList)
				{
					keyCollections.Add(key);
					continue;
				}
				result = result.Replace("{" + key + "}", content[key]?.ToString());
			}

			if (keyCollections.Count == 0) return result;

			// jika ada data many, proses template nya
			string[] lineTemplates = result.Split(Environment.NewLine);
			foreach(string key in keyCollections)
			{
				string replaceAbleLoopContent = "";
				string lineLoopTemplate = "";
				bool isInLoop = false;
				foreach(string lineValue in lineTemplates)
				{
					if(lineValue.Contains("[" + key + "]"))
					{
						lineLoopTemplate = "";
						replaceAbleLoopContent = lineValue + Environment.NewLine;
						isInLoop = true;
						continue;
					}

					if (isInLoop)
					{
						if(lineValue.Contains("[/" + key + "]"))
						{
							replaceAbleLoopContent += lineValue + Environment.NewLine;

							// proses lineLoop ini
							string loopResult = "";
							List<Dictionary<string,string>> childLoops = (List<Dictionary<string, string>>) content[key];
							foreach(var childRow in childLoops)
							{
								string rowResult = lineLoopTemplate;
								foreach(string childKey in childRow.Keys)
								{
									rowResult = rowResult.Replace("{" + childKey + "}", childRow[childKey]);
								}
								loopResult += rowResult + Environment.NewLine;
							}
							 
							result = result.Replace(replaceAbleLoopContent, loopResult);

							// move ke key berikutnya
							break;
						} else
						{
							lineLoopTemplate += lineValue + Environment.NewLine;
							replaceAbleLoopContent += lineValue + Environment.NewLine;
						}
					}
				}
			}

			return result;
		}
		#endregion

		#region protected methods

		protected void initEPPlusLicense()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		}

		protected void AssignCreatorAndCompany(BaseEntity entity)
		{
			entity.CompanyId = _companyId;
			entity.CreatedBy = _userName;
			entity.CreatedDate = DateTime.Now;
		}

		protected void AssignUpdater(BaseEntity entity)
		{
			entity.CompanyId = _companyId;
			entity.UpdatedBy = _userName;
			entity.UpdatedDate = DateTime.Now;
		}

		protected async Task<string> GenerateNumber(string functionId, int recordId, CancellationToken cancellationToken = default)
		{
			var spLookupDetail = await _unitOfWork.LookupDetailRepository.FirstOrDefaultAsync(new LookupDetailFilterSpecification(functionId, 1), cancellationToken);
			if (spLookupDetail == null) return string.Empty;

			var result = _unitOfWork.GenericRepository.ExecStoredProcedureNumberGenerator(spLookupDetail.Value, recordId);
			if(result == null) return string.Empty;
			return result.NumberGenerated;
		}

		//protected List<Warehouse> _warehouses;

		//protected async Task<List<Warehouse>> GetMyWarehousesAsync()
		//{
		//	return await GetMyWarehousesAsync(_userName);
		//}
		//protected async Task<List<Warehouse>> GetMyWarehousesAsync(string userName)
		//{
		//	if (!string.IsNullOrEmpty(userName))
		//	{
		//		// ambil data warehouse berdasarkan user yang dikirim
		//		var result = await GetWarehouseListAsync(userName);
		//		return result;
		//	}

		//	if (_warehouses == null || _warehouses.Count <= 0) _warehouses = await GetWarehouseListAsync(_userName);
		//	return _warehouses;
		//}

		//private async Task<List<Warehouse>> GetWarehouseListAsync(string userName)
		//{
		//	return await _unitOfWork.WarehouseRepository.GetWarehousesByUser(userName);
		//}

		//protected async Task<Warehouse> GetMySingleWarehouseAsync()
		//{
		//	if (_warehouses == null || _warehouses.Count <= 0) _warehouses = await GetWarehouseListAsync(_userName);
		//	return (_warehouses == null || _warehouses.Count <= 0) ? null : _warehouses[0];
		//}

		#endregion

		#region excel helper

		protected ExcelPackage DataTableToExcel(DataTable dt)
		{
			if (dt == null)
				return null;

			var package = new ExcelPackage();
			var ws = package.Workbook.Worksheets.Add("Worksheet 1");
			int row = 1, col = 1;
			List<string> headerInfo = new List<string>();

			// draw excel header
			foreach (DataColumn colInfo in dt.Columns)
			{
				ws.Cells[row, col].Value = colInfo.ColumnName;
				headerInfo.Add(colInfo.ColumnName);
				col++;
			}
			ws.Cells[1, 1, 1, col].Style.Font.Bold = true;

			// draw excel content
			row = 2;
			foreach (DataRow rowInfo in dt.Rows)
			{
				col = 1;
				foreach (string title in headerInfo)
				{
					ws.Cells[row, col].Value = rowInfo[col - 1];
					col++;
				}
				row++;
			}

			ws.Cells[ws.Dimension.Address].AutoFitColumns();
			return package;
		}

		protected List<Dictionary<string, Object>> ExcelToDictionary(String excelFileName, bool firstRowTitle = true)
		{
			List<Dictionary<string, Object>> results = new List<Dictionary<string, object>>();

			if (!System.IO.File.Exists(excelFileName))
				return results;

			using (var package = new ExcelPackage(new FileInfo(excelFileName)))
			{
				var ws = package.Workbook.Worksheets[0];
				if (ws == null)
					return results;

				if (ws.Dimension == null || ws.Dimension.End == null)
					return results;

				int maxRow = ws.Dimension.End.Row;
				int maxCol = ws.Dimension.End.Column;

				// prepare header title, make sure there is no duplicate header title
				int startRow = 1;
				List<string> title = new List<string>();
				Dictionary<string, int> headerDuplicateCounter = new Dictionary<string, int>();
				if (firstRowTitle)
				{
					for (int col = 1; col <= maxCol; col++)
					{
						var value = ws.Cells[startRow, col].Value;
						if (value == null) value = "NULL";
						if (!headerDuplicateCounter.ContainsKey((string)value))
							headerDuplicateCounter.Add((string)value, 1);

						if (title.Contains((string)value))
						{
							title.Add($"{(string)value}{headerDuplicateCounter[(string)value]}");
							headerDuplicateCounter[(string)value]++;
						}
						else
						{
							title.Add((string)value);
						}
					}
					startRow = 2;
				}
				else
				{
					for (int col = 1; col <= maxCol; col++)
						title.Add(col.ToString());
				}

				// read all content and add to list results
				for (int row = startRow; row <= maxRow; row++)
				{
					var rowValue = new Dictionary<string, object>();
					for (int col = 1; col <= maxCol; col++)
					{
						var value = ws.Cells[row, col].Value;
						rowValue.Add(title[col - 1], value);
					}
					results.Add(rowValue);
				}
			}

			return results;
		}

		#endregion
	}
}
