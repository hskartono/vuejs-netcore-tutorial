using Ardalis.Specification;
using Hangfire;
using iText.Html2pdf;
using OfficeOpenXml;
using Tutorial.ApplicationCore;
using Tutorial.ApplicationCore.Entities;
using Tutorial.ApplicationCore.Repositories;
using Tutorial.ApplicationCore.Services;
using Tutorial.ApplicationCore.Specifications;
using Tutorial.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Tutorial.Infrastructure.Services
{
	public partial class PurchaseOrderDetailService : AsyncBaseService<PurchaseOrderDetail>, IPurchaseOrderDetailService
	{
		#region appgen: upload excel
		public async Task<List<PurchaseOrderDetail>> UploadExcel(string tempExcelFile, int parent_id, CancellationToken cancellationToken = default)
		{
			var result = new List<PurchaseOrderDetail>();

			// convert dari excel menjadi dictionary
			var resultDictionary = ExcelToDictionary(tempExcelFile, true);
			if (resultDictionary?.Count <= 0)
				return null;

			// field pada excel: Part Number, Qty, Price, Sub Total
			// ambil daftar part number untuk diambi objeknya
			List<string> partNumbers = new List<string>();
			foreach (var row in resultDictionary)
			{
				if (row["Part Number"] == DBNull.Value) continue;
				partNumbers.Add(row["Part Number"].ToString());
			}

			// ambil daftar part yang ada di excel
			var partSpecification = new PartFilterSpecification()
			{
				Ids = partNumbers
			}.BuildSpecification();
			var parts = await _unitOfWork.PartRepository.ListAsync(partSpecification, null, cancellationToken);
			if (parts == null)
			{
				AddError("Data part tidak ada yang ditemukan. Periksa kembali Part Number yang dimasukkan.");
				return null;
			}

			// bind informasi excel ke dalam objek PurchaseOrderDetail, termasuk objek part
			var parent = await _unitOfWork.PurchaseOrderRepository.GetByIdAsync(parent_id, cancellationToken);
			foreach (var row in resultDictionary)
			{
				var part = parts.Where(e => e.PartName == row["Part Number"].ToString()).FirstOrDefault();
				double partPrice = (row["Price"] == DBNull.Value) ? 0 : (double)row["Price"];
				int partQty = (row["Qty"] == DBNull.Value) ? 0 : (int)row["Qty"];
				double partTotal = (row["Sub Total"] == DBNull.Value) ? 0 : (double)row["Sub Total"];
				var poDetail = new PurchaseOrderDetail(part.Id, partPrice, partQty, partTotal, parent)
				{
					Part = part
				};
				result.Add(poDetail);
			}

			// set flag upload & draft
			SetUploadDraftFlags(result);

			// validasi master data part
			await RunMasterDataValidation(result, cancellationToken);

			// save ke database
			foreach (var item in result)
			{
				var id = item.Id;
				if (id > 0)
					await _unitOfWork.PurchaseOrderDetailRepository.UpdateAsync(item, cancellationToken);
				else
					await _unitOfWork.PurchaseOrderDetailRepository.AddAsync(item, cancellationToken);

			}
			await _unitOfWork.CommitAsync();

			return result;
		}

		private async Task RunMasterDataValidation(List<PurchaseOrderDetail> result, CancellationToken cancellationToken)
		{
			foreach (var item in result)
			{
				if (item.Part == null)
				{
					item.AddValidationMessage("Nomor Part tidak ditemukan");
					item.UploadValidationStatus = "Failed";
				}
			}
		}

		private void SetUploadDraftFlags(List<PurchaseOrderDetail> result)
		{
			foreach (var item in result)
			{
				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;

				item.isFromUpload = true;
				item.DraftFromUpload = true;
				item.IsDraftRecord = (int)BaseEntity.DraftStatus.DraftMode;
				item.RecordActionDate = DateTime.Now;
				item.RecordEditedBy = _userName;

			}
		}
		#endregion
	}
}
