using Tutorial.ApplicationCore.Entities;
using Xunit;

namespace UnitTest.Entities
{
	public class PurchaseOrderTest
	{
		[Fact]
		public void can_add_new_item()
		{
			var purchaseorder = new PurchaseOrder();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			purchaseorder.AddPurchaseOrderDetails("1", 4, 5, 6);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);

			var newItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(2, purchaseorder.PurchaseOrderDetails.Count);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Id == newItemPurchaseOrderDetails.Id);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.PartPrice == newItemPurchaseOrderDetails.PartPrice);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Qty == newItemPurchaseOrderDetails.Qty);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.TotalPrice == newItemPurchaseOrderDetails.TotalPrice);

		}

		[Fact]
		public void can_replace_existing_item()
		{
			var purchaseorder = new PurchaseOrder();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			var newItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);

			var repalaceItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 49, 59, 69, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(repalaceItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Id == repalaceItemPurchaseOrderDetails.Id);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.PartPrice == repalaceItemPurchaseOrderDetails.PartPrice);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Qty == repalaceItemPurchaseOrderDetails.Qty);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.TotalPrice == repalaceItemPurchaseOrderDetails.TotalPrice);

		}

		[Fact]
		public void can_remove_existing_item()
		{
			var purchaseorder = new PurchaseOrder();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			var newItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);

			purchaseorder.RemovePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			Assert.DoesNotContain(purchaseorder.PurchaseOrderDetails, e => e.Id == newItemPurchaseOrderDetails.Id);
			Assert.DoesNotContain(purchaseorder.PurchaseOrderDetails, e => e.PartPrice == newItemPurchaseOrderDetails.PartPrice);
			Assert.DoesNotContain(purchaseorder.PurchaseOrderDetails, e => e.Qty == newItemPurchaseOrderDetails.Qty);
			Assert.DoesNotContain(purchaseorder.PurchaseOrderDetails, e => e.TotalPrice == newItemPurchaseOrderDetails.TotalPrice);

		}

		[Fact]
		public void remove_nont_exist_item_should_return_nothing()
		{
			var purchaseorder = new PurchaseOrder();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			var newItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);

			var removeItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 2 };
			purchaseorder.RemovePurchaseOrderDetails(removeItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Id == newItemPurchaseOrderDetails.Id);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.PartPrice == newItemPurchaseOrderDetails.PartPrice);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.Qty == newItemPurchaseOrderDetails.Qty);
			Assert.Contains(purchaseorder.PurchaseOrderDetails, e => e.TotalPrice == newItemPurchaseOrderDetails.TotalPrice);

		}

		[Fact]
		public void can_clear_existing_items()
		{
			var purchaseorder = new PurchaseOrder();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);
			var newItemPurchaseOrderDetails = new PurchaseOrderDetail("1", 4, 5, 6, purchaseorder) { Id = 1 };
			purchaseorder.AddOrReplacePurchaseOrderDetails(newItemPurchaseOrderDetails);
			Assert.Equal(1, purchaseorder.PurchaseOrderDetails.Count);

			purchaseorder.ClearPurchaseOrderDetails();
			Assert.Equal(0, purchaseorder.PurchaseOrderDetails.Count);

		}
	}
}
