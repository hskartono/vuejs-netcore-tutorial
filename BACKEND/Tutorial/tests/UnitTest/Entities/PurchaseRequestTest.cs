using Tutorial.ApplicationCore.Entities;
using Xunit;

namespace UnitTest.Entities
{
	public class PurchaseRequestTest
	{
		[Fact]
		public void can_add_new_item()
		{
			var purchaserequest = new PurchaseRequest();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			purchaserequest.AddPurchaseRequestDetails("1", 4, new System.DateTime(2021,2,14));
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);

			var newItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(2, purchaserequest.PurchaseRequestDetails.Count);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Id == newItemPurchaseRequestDetails.Id);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Qty == newItemPurchaseRequestDetails.Qty);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.RequestDate == newItemPurchaseRequestDetails.RequestDate);

		}

		[Fact]
		public void can_replace_existing_item()
		{
			var purchaserequest = new PurchaseRequest();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			var newItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);

			var repalaceItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 49, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(repalaceItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Id == repalaceItemPurchaseRequestDetails.Id);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Qty == repalaceItemPurchaseRequestDetails.Qty);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.RequestDate == repalaceItemPurchaseRequestDetails.RequestDate);

		}

		[Fact]
		public void can_remove_existing_item()
		{
			var purchaserequest = new PurchaseRequest();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			var newItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);

			purchaserequest.RemovePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			Assert.DoesNotContain(purchaserequest.PurchaseRequestDetails, e => e.Id == newItemPurchaseRequestDetails.Id);
			Assert.DoesNotContain(purchaserequest.PurchaseRequestDetails, e => e.Qty == newItemPurchaseRequestDetails.Qty);
			Assert.DoesNotContain(purchaserequest.PurchaseRequestDetails, e => e.RequestDate == newItemPurchaseRequestDetails.RequestDate);

		}

		[Fact]
		public void remove_nont_exist_item_should_return_nothing()
		{
			var purchaserequest = new PurchaseRequest();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			var newItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);

			var removeItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 2 };
			purchaserequest.RemovePurchaseRequestDetails(removeItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Id == newItemPurchaseRequestDetails.Id);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.Qty == newItemPurchaseRequestDetails.Qty);
			Assert.Contains(purchaserequest.PurchaseRequestDetails, e => e.RequestDate == newItemPurchaseRequestDetails.RequestDate);

		}

		[Fact]
		public void can_clear_existing_items()
		{
			var purchaserequest = new PurchaseRequest();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);
			var newItemPurchaseRequestDetails = new PurchaseRequestDetail("1", 4, new System.DateTime(2021,2,14), purchaserequest) { Id = 1 };
			purchaserequest.AddOrReplacePurchaseRequestDetails(newItemPurchaseRequestDetails);
			Assert.Equal(1, purchaserequest.PurchaseRequestDetails.Count);

			purchaserequest.ClearPurchaseRequestDetails();
			Assert.Equal(0, purchaserequest.PurchaseRequestDetails.Count);

		}
	}
}
