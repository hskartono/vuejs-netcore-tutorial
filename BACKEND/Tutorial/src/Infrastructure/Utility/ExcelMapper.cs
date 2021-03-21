using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Utility
{
	public class ExcelMapper
	{
		public static ExcelToObjectMapConfig LoadConfig(string configFileName)
		{
			string jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", configFileName);
			if (!File.Exists(jsonFile))
				throw new FileNotFoundException("Could not load configuration", configFileName);

			return Newtonsoft.Json.JsonConvert.DeserializeObject<ExcelToObjectMapConfig>(File.ReadAllText(jsonFile));
		}

		public static List<T> ReadFromExcel<T>(string excelFileName, string configFileName)
		{
			var mapper = ExcelMapper.LoadConfig(configFileName);
			return ExcelMapper.ReadFromExcel<T>(excelFileName, mapper);
		}

		public static List<T> ReadFromExcel<T>(string excelFileName, ExcelToObjectMapConfig mapper)
		{
			if (!File.Exists(excelFileName))
				throw new FileNotFoundException("Could not load excel file", excelFileName);

			if (string.IsNullOrEmpty(excelFileName))
				throw new ArgumentNullException(nameof(excelFileName));

			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));

			List<T> result = new List<T>();
			Dictionary<int, List<string>> validations = new Dictionary<int, List<string>>();
			Dictionary<string, ExcelWorksheet> worksheets = new Dictionary<string, ExcelWorksheet>();
			Dictionary<string, List<object>> childsData = new Dictionary<string, List<object>>();
			Dictionary<string, Dictionary<int, List<string>>> childsValidation = new Dictionary<string, Dictionary<int, List<string>>>();

			// load excel
			initExcelLicense();
			using (var package = new ExcelPackage(new FileInfo(excelFileName)))
			{
				// load main worksheet
				var ws = package.Workbook.Worksheets[mapper.worksheet];
				if (ws == null)
					return null;

				ProcessReadWorksheet<T>(mapper, result, validations, ws);

				Dictionary<string, ExcelToObjectMapConfig> collectionProperties = new Dictionary<string, ExcelToObjectMapConfig>();
				// load child worksheets
				foreach (var item in mapper.columnsMap)
				{
					if (item.type.ToLower() == "collection")
					{
						var config = item.LoadRelatedCollection();
						collectionProperties.Add(item.property, config);

						ws = package.Workbook.Worksheets[config.worksheet];
						if (ws == null)
							return null;

						worksheets.Add(item.property, ws);
						childsData.Add(item.property, new List<object>());
						childsValidation.Add(item.property, new Dictionary<int, List<string>>());
						ProcessReadWorksheet(config, childsData[item.property], childsValidation[item.property], worksheets[item.property]);
					}
				}

				// assign childs data ke main worksheet
				Type mType = Type.GetType(mapper.assembly);
				foreach (var item in result)
				{
					var primaryKey = mType.GetProperty("_RefPrimaryKey").GetValue(item);
					foreach (var propertyName in collectionProperties.Keys)
					{
						Type childType = Type.GetType(collectionProperties[propertyName].assembly);
						Type genericListType = typeof(List<>);
						Type concreteListType = genericListType.MakeGenericType(childType);
						IList childs = Activator.CreateInstance(concreteListType) as IList;
						foreach (var data in childsData[propertyName])
						{
							var foreignKey = childType.GetProperty("_RefForeignKey").GetValue(data);
							if (foreignKey.Equals(primaryKey))
							{
								childs.Add(data);
							}
						}
						mType.GetProperty(propertyName).SetValue(item, childs);
					}
				}
			}

			return result;
		}

		private static void ProcessReadWorksheet<T>(ExcelToObjectMapConfig mapper, List<T> result, Dictionary<int, List<string>> validations, ExcelWorksheet ws)
		{
			int maxRow = ws.Dimension.End.Row;
			Type mType = Type.GetType(mapper.assembly);
			List<string> numericTypes = new List<string>() { "int", "long", "single", "double", "decimal" };

			for (int y = 2; y <= maxRow; y++)
			{
				int rowNumber = y - 1;
				validations.Add(rowNumber, new List<string>());

				//var itemInstance = Activator.CreateInstance<T>();
				var itemInstance = Activator.CreateInstance(mType);
				foreach (PropertyInfo prop in mType.GetProperties())
				{
					var propConfig = mapper.columnsMap.Where(e => e.property.ToLower() == prop.Name.ToLower()).SingleOrDefault();
					if (propConfig == null) continue;
					if (propConfig.type.ToLower() == "collection") continue;

					var value = ws.Cells[y, propConfig.colIndex].Value;
					if (propConfig.isRequired)
					{
						if (value == null)
						{
							// validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}

						// jika tidak null, cek jika numeric atau date/time
						if (numericTypes.Contains(propConfig.type.ToLower()) && (value.ToString() == "" || double.Parse(value.ToString()) == 0))
						{
							//validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}

						if (propConfig.type.ToLower() == "datetime" && ((DateTime)value) == DateTime.MinValue)
						{
							//validations[rowNumber].Add($"{prop.Name} pada baris {rowNumber} harus diisi");
							validations[rowNumber].Add($"{prop.Name} harus diisi.");
							continue;
						}
					}

					// object data type
					if (propConfig.type.ToLower() == "object" && value != null)
					{
						if (prop.PropertyType == typeof(int))
						{
							int valueId = 0;
							if (!int.TryParse(value.ToString(), out valueId))
							{
								//validations[rowNumber].Add($"Nilai {prop.Name} pada baris {rowNumber} harus berupa angka.");
								validations[rowNumber].Add($"Nilai {prop.Name} harus berupa angka.");
								continue;
							}
							prop.SetValue(itemInstance, valueId);
						}
						else if (prop.PropertyType == typeof(int?))
						{
							int? valueId = null;
							int parsedValueId = 0;
							if (!int.TryParse(value.ToString(), out parsedValueId))
							{
								validations[rowNumber].Add($"Nilai {prop.Name} harus berupa angka.");
								continue;
							}
							if (parsedValueId > 0) valueId = parsedValueId;
							prop.SetValue(itemInstance, valueId);
						}
						else
						{
							prop.SetValue(itemInstance, value);
						}
					}
					else if (propConfig.type.ToLower() == "collection")
					{
						// collection
					}
					else
					{
						// primitive data type
						if (propConfig.PK || propConfig.FK)
						{
							if (value != null)
								prop.SetValue(itemInstance, value.ToString());
						}
						else
						{
							if (numericTypes.Contains(propConfig.type.ToLower()))
							{
								if (value == null) value = 0;
								prop.SetValue(itemInstance, int.Parse(value.ToString()));
							}
							else if (propConfig.type.ToLower() == "datetime")
							{
								long dateNum = Convert.ToInt64(value);
								DateTime dtCheck = DateTime.FromOADate(dateNum);
								prop.SetValue(itemInstance, dtCheck);
							}
							else
							{
								prop.SetValue(itemInstance, value);
							}
						}
					}
				}

				result.Add((T)itemInstance);
			}

			int index = 1;
			foreach (var item in result)
			{
				mType.GetProperty("UploadValidationStatus").SetValue(item, "Success");
				if (validations[index].Count > 0)
				{
					string err = string.Join(Environment.NewLine, validations[index]);
					mType.GetProperty("UploadValidationMessage").SetValue(item, err);
					mType.GetProperty("UploadValidationStatus").SetValue(item, "Failed");
				}
				index++;
			}
		}

		public static bool WriteToExcel<T>(string excelFileName, string configFileName, IReadOnlyList<T> data)
		{
			var mapper = ExcelMapper.LoadConfig(configFileName);
			return ExcelMapper.WriteToExcel<T>(excelFileName, mapper, data);
		}

		public static bool WriteToExcel<T>(string excelFileName, ExcelToObjectMapConfig mapper, IReadOnlyList<T> data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			if (mapper == null)
				throw new ArgumentNullException(nameof(mapper));
			if (string.IsNullOrEmpty(excelFileName))
				throw new ArgumentNullException(nameof(excelFileName));

			initExcelLicense();
			using (var package = new ExcelPackage())
			{
				Dictionary<string, ExcelWorksheet> wsChilds = new Dictionary<string, ExcelWorksheet>();
				Dictionary<string, int> wsChildsRow = new Dictionary<string, int>();
				var ws = package.Workbook.Worksheets.Add(mapper.worksheet);
				// draw header
				int maxCol = 1;
				foreach (var item in mapper.columnsMap)
				{
					if (item.type.ToLower() == "collection") continue;
					ws.Cells[1, item.colIndex].Value = item.title;
					maxCol++;
				}
				ws.Cells[1, 1, 1, maxCol].Style.Font.Bold = true;

				Type itemType = Type.GetType(mapper.assembly);
				//var refPK = 1;
				var xlRow = 2;
				foreach (var itemObj in data)
				{
					string foreignKey = Convert.ToString(itemType.GetProperty(mapper.columnsMap[0].property).GetValue(itemObj));
					itemType.GetProperty("_RefForeignKey").SetValue(itemObj, foreignKey);
					foreach (var map in mapper.columnsMap)
					{
						if (map.type.ToLower() == "collection")
						{
							var mapChild = map.LoadRelatedCollection();
							if (!wsChilds.ContainsKey(mapChild.worksheet))
							{
								wsChilds.Add(mapChild.worksheet, package.Workbook.Worksheets.Add(mapChild.worksheet));
								wsChildsRow.Add(mapChild.worksheet, 2);

								// draw header
								maxCol = 1;
								foreach (var itemMapChild in mapChild.columnsMap)
								{
									if (itemMapChild.type.ToLower() == "collection") continue;
									wsChilds[mapChild.worksheet].Cells[1, itemMapChild.colIndex].Value = itemMapChild.title;
									maxCol++;
								}
								wsChilds[mapChild.worksheet].Cells[1,1,1,maxCol].Style.Font.Bold = true;
							}

							var wsChild = wsChilds[mapChild.worksheet];
							var childObj = (IList) itemType.GetProperty(map.property).GetValue(itemObj);
							var itemChildType = Type.GetType(mapChild.assembly);
							foreach(var itemChildObj in childObj)
							{
								itemChildType.GetProperty("_RefPrimaryKey").SetValue(itemChildObj, foreignKey);
								foreach(var itemMap in mapChild.columnsMap)
								{
									if (itemMap.type.ToLower() == "collection")
									{
										// belum support 3 level
									} else
									{
										wsChild.Cells[wsChildsRow[mapChild.worksheet], itemMap.colIndex].Value = itemChildType.GetProperty(itemMap.property).GetValue(itemChildObj);
										if (itemMap.type.ToLower() == "datetime")
											wsChild.Cells[wsChildsRow[mapChild.worksheet], itemMap.colIndex].Style.Numberformat.Format = "dd-MMM-yyyy";
									}
								}

								wsChildsRow[mapChild.worksheet]++;
							}

							wsChild.Cells[wsChild.Dimension.Address].AutoFitColumns();
						}
						else
						{
							ws.Cells[xlRow, map.colIndex].Value = itemType.GetProperty(map.property).GetValue(itemObj);
							if (map.type.ToLower() == "datetime")
								ws.Cells[xlRow, map.colIndex].Style.Numberformat.Format = "dd-MMM-yyyy";
						}
					}

					//refPK++;
					xlRow++;
				}
				ws.Cells[ws.Dimension.Address].AutoFitColumns();

				FileOutputUtil.OutputDir = new DirectoryInfo(Path.GetDirectoryName(excelFileName));
				var xFile = FileOutputUtil.GetFileInfo(Path.GetFileName(excelFileName));
				package.SaveAs(xFile);
			}

			return true;
		}

		public static void initExcelLicense()
		{
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		}
	}
}
