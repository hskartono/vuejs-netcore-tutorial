<template>
	<b-modal v-model="isShowPopup" dialog-class="modal-detail" @hidden="resetModal" scrollable :no-close-on-backdrop=true @ok="saveButtonAction" title="Save Confirmation - Purchase Order">
<!-- APP_DETAIL_HEADER -->
		<b-card>
			<b-col cols="12" lg="6">
					<b-form-group id="fieldset-po-number" label-cols-sm="4" label-cols-lg="3" label="PO Number" label-for="filter-po-number">
						{{ model != null ? model.poNumber : "" }}
					</b-form-group>
					<b-form-group id="fieldset-po-date" label-cols-sm="4" label-cols-lg="3" label="PO Date" label-for="filter-po-date">
						{{ (model != null ? model.poDate : "") | moment("DD-MMM-YYYY") }}
					</b-form-group>
					<b-form-group id="fieldset-remarks" label-cols-sm="4" label-cols-lg="3" label="Remarks" label-for="filter-remarks">
						{{ model != null ? model.remarks : "" }}
					</b-form-group>
			</b-col>
		</b-card>
<!-- END_APP_DETAIL_HEADER -->
<!--APP_DETAIL_TABLE-->
	<b-card class="mt-3" header-bg-variant="transparent">
		<b-row class="mb-2">
			<b-col cols="9"><h5 class="card-title font-weight-bolder text-dark">Purchase Order Details</h5></b-col>
			<b-col>
				<span class="float-right">
					<b-form-select v-model="purchaseOrderDetailPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
				</span>
			</b-col>
		</b-row>
		<b-row>
		<b-col>
			<div class="table-corner">
			<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseOrderDetails" :fields="fieldsPurchaseOrderDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseOrderDetail" :sort-desc.sync="sortDescPurchaseOrderDetail" small responsive>
			<!--APP_TABLE_FILTER-->
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseOrderDetailFilter = !isShowPurchaseOrderDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseOrderDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseOrderDetail.partId" :reduce="item => item.id" v-model="filterPurchaseOrderDetail.partId" @search="getParts" :filterable="true"></v-select>
				</b-th>
				<b-th stacked-heading="Qty">
					<b-form-input id="input-qty-table" v-model="filterPurchaseOrderDetail.qty" size="sm"></b-form-input>
				</b-th>
				<b-th></b-th>
			</template>
			<!--END_APP_TABLE_FILTER-->
			<!--APP_TABLE_DATA-->
			<template v-slot:cell(no)="row">
				{{ ((row.index + 1) + ( purchaseOrderDetailPageSize * (currentPurchaseOrderDetailPage - 1))) }}
			</template>
			<template v-slot:cell(purchaseOrder)="row">
				{{ row.item.purchaseOrder != null ? row.item.purchaseOrder.poNumber : "" }}
			</template>
			<template v-slot:cell(part)="row">
				{{ row.item.part != null ? row.item.part.partName : "" }}
			</template>
			<!--END_APP_TABLE_DATA-->
			</b-table>
			</div>
		</b-col>
		<b-col cols="12">
			<!--APP_TABLE_PAGINATION-->
			<b-pagination
			class="float-right btn-paging"
			v-model="currentPurchaseOrderDetailPage"
			:total-rows="totalPurchaseOrderDetailRow"
			:per-page="purchaseOrderDetailPageSize"
			aria-controls="my-table"
			></b-pagination>
			<!--END_APP_TABLE_PAGINATION-->
		</b-col>
		</b-row>
	</b-card>
<!--END_APP_DETAIL_TABLE-->
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</b-modal>
</template>
<script>
/*APP_DETAIL_IMPORT*/
import PurchaseOrder from '@/models/PurchaseOrder/PurchaseOrder';
import PurchaseOrderList from '@/models/PurchaseOrder/PurchaseOrderList';
import PurchaseOrderFilter from '@/filters/PurchaseOrder/PurchaseOrderFilter';

import PurchaseOrderDetail from '@/models/PurchaseOrder/PurchaseOrderDetail';
import PurchaseOrderDetailFilter from '@/filters/PurchaseOrder/PurchaseOrderDetailFilter';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import PopupRouterViewDetail from '@/components/PopupRouterViewDetail';
import AppInputDatetimepicker from '@/components/AppInputDatetimepicker';
import AppInputDatetimepickerRange from '@/components/AppInputDatetimepickerRange';
/*END_APP_DETAIL_IMPORT*/
export default {
	/*APP_DETAIL_PROP_DEFINITION*/
	props : ["openPopup", "modelId", "saveActionMethod", "resetMethod"],
	/*END_APP_DETAIL_PROP_DEFINITION*/
	components : {
	/*APP_DETAIL_PROP_DEFINITION*/
	PopupRouterViewDetail,AppInputDatetimepicker,AppInputDatetimepickerRange,
	/*APP_DETAIL_COMPONENTS_DEFINITION*/
},
	data() {
		return {
			fileupload: null,
			model: {},
			datas: Array(),
			fieldsPurchaseOrderDetail: [
				{"no" : {  "class" : "th-number" }},
				{"part": {},  "class" : "th-part", "label" : "Part", key: 'part', sortable: true}, 
				{"partPrice": {},  "class" : "th-part-price", "label" : "Part Price", key: 'partPrice', sortable: true}, 
				{"qty": {},  "class" : "th-qty", "label" : "Qty", key: 'qty', sortable: true}, 
				{"totalPrice": {},  "class" : "th-total-price", "label" : "Total Price", key: 'totalPrice', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			/*APP_DETAIL_PAGINATION*/
			pagingOptions:[
				{ value : '10', text : '10', selected:true },
				{ value : '25', text : '25' },
				{ value : '50', text : '50' },
				{ value : '100', text : '100' },
			],
			/*END_APP_DETAIL_PAGINATION*/
			/*APP_DETAIL_FILTER*/
			filterPurchaseOrder : null,
			/*END_APP_DETAIL_FILTER*/
			/*APP_DETAIL_PAGING_DEFINITION*/
			currentPurchaseOrderPage : 1,
			purchaseOrderPageSize: 10,
			/*END_APP_DETAIL_PAGING_DEFINITION*/
			/*APP_DETAIL_SORT_DEFINITION*/
			sortByPurchaseOrder: '',
			sortDescPurchaseOrder: '',
			/*END_APP_DETAIL_SORT_DEFINITION*/
			/*APP_DETAIL_VARIABLE_DEFINITION*/
			filterPurchaseOrderDetail : null,
			currentPurchaseOrderDetailPage : 1,
			purchaseOrderDetailPageSize: 10,
			sortByPurchaseOrderDetail: '',
			sortDescPurchaseOrderDetail: '',
			sortingByPurchaseOrderDetail: [],
			purchaseOrderDetails : [],
			purchaseOrderDetailId : '',
			totalPurchaseOrderDetailRow : 0,
			isShowPurchaseOrderDetailFilter: false,
			/*END_APP_DETAIL_VARIABLE_DEFINITION*/
			/*APP_DETAIL_OBJ_VARIABLE*/
			parts: [],
			/*END_APP_DETAIL_OBJ_VARIABLE*/
		}
	},
	methods : {
		/*APP_DETAIL_GETPurchaseOrder*/
		getPurchaseOrder : async function(id) {
			this.isBusy = true;
			PurchaseOrder.getData(this, id).then(result => { this.model = result; this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		/*END_APP_DETAIL_GETPurchaseOrder*/
		/*APP_DETAIL_GETPurchaseOrderDetail*/
		getPurchaseOrderDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseOrderDetail);
			if (this.modelId == null) return;
			filter.purchaseOrderId = this.modelId;
			this.sortingByPurchaseOrderDetail = Array();
			if (this.sortByPurchaseOrderDetail != null) {
				this.sortingByPurchaseOrderDetail[this.sortByPurchaseOrderDetail] = this.sortDescPurchaseOrderDetail;
			}
			let currentPage = (this.currentPurchaseOrderDetailPage - 1);
			PurchaseOrderDetail.getList(this, filter, this.sortingByPurchaseOrderDetail, currentPage , this.purchaseOrderDetailPageSize).then(result => {
				this.purchaseOrderDetails = result.data;
				this.totalPurchaseOrderDetailRow = result.dataCount;
			}).catch(error => {});
		},
		/*END_APP_DETAIL_GETPurchaseOrderDetail*/
		/*APP_DETAIL_GETPART*/
		getParts : async function(input) {
			let filter = new PartFilter();
			if (input != undefined) {
				filter.partName = input;
			}
			Part.getList(this, filter, null, 0, 10)
			.then(result => {
				if (result != null) {
					this.parts = result.data
				}
			})
			.catch(error => {});
		},
		/*END_APP_DETAIL_GETPART*/
		/*APP_DETAIL_COPYDATA*/
		copyDataAction(modelEvent) {
			this.$router.push({ name: 'purchaseordercopydata', params: { id: this.modelId, copydata : 1 } })
		},
		/*END_APP_DETAIL_COPYDATA*/
		/*APP_DETAIL_SAVEBUTTONACTION*/
		saveButtonAction(modalEvent) {
			this.$emit("saveActionMethod", modalEvent);
		},
		/*END_APP_DETAIL_SAVEBUTTONACTION*/
		/*APP_DETAIL_RESETMODAL*/
		resetModal() {
			this.$emit('resetMethod'); 
		},
		/*END_APP_DETAIL_RESETMODAL*/
		/*APP_DETAIL_DISPLAYFILTER*/
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
				this.isShowPurchaseOrderDetailFilter = true;
			}
		},
		/*END_APP_DETAIL_DISPLAYFILTER*/
	},
	/*APP_DETAIL_BEFORE_MOUNT*/
	beforeMount() {
		this.filterPurchaseOrder = new PurchaseOrderFilter();
		this.filterPurchaseOrderDetail = new PurchaseOrderDetailFilter();
	},
	/*END_APP_DETAIL_BEFORE_MOUNT*/
	watch : {
		/*APP_DETAIL_OPENPOPUP*/
		openPopup(newValue, oldValue) {
			if (newValue) {
				this.getPurchaseOrder(this.modelId);
				this.getPurchaseOrderDetails();
			}
		},
		/*END_APP_DETAIL_OPENPOPUP*/
		/*APP_DETAIL_WATCH_PURCHASEORDERDETAIL*/
		sortByPurchaseOrderDetail : {
			handler: function() {
				this.getPurchaseOrderDetails();
			}
		},
		sortDescPurchaseOrderDetail: {
			handler: function() {
				this.getPurchaseOrderDetails();
			}
		},
		currentPurchaseOrderDetailPage: {
			handler: function() {
				this.getPurchaseOrderDetails();
			}
		},
		purchaseOrderDetailPageSize : {
			handler: function() {
				this.getPurchaseOrderDetails();
			}
		},
		filterPurchaseOrderDetail: {
			handler: function() {
				this.getPurchaseOrderDetails();
			},
			deep: true,
		},
		/*END_APP_DETAIL_WATCH_PURCHASEORDERDETAIL*/
	},
	/*APP_DETAIL_COMPUTED*/
	computed: {
		isShowPopup: {
			get: function(){
				if (this.openPopup) return true; 
				else return false; 
			},
			set: function(newValue){ }
		},
	}
	/*END_APP_DETAIL_COMPUTED*/
}
</script>
