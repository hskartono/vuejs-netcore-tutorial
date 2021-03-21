<template>
	<b-modal v-model="isShowPopup" dialog-class="modal-detail" @hidden="resetModal" scrollable :no-close-on-backdrop=true id="popup-upload" @ok="uploadButtonAction" :okTitle="okTitle" title="Upload Data">
		<b-card>
			<b-row>
				<b-col cols="12">
					<app-input-fileupload label="File" size="sm" :model.sync="fileupload" />
				</b-col>
			</b-row>
			<b-row v-if="isUploadSuccess">
				<b-col cols="12">
					<b-card no-body>
						<b-tabs card>
							<b-tab title="Purchase Order" active>
								<b-row class="mb-2">
									<b-col>
										<span class="float-right">
											<b-form-select v-model="purchaseOrderPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
										</span>
									</b-col>
								</b-row>
								<b-row>
								<b-col>
								<div class="table-corner">
									<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseOrders" :fields="fieldsPurchaseOrder" :busy="isDataLoading" :sort-by.sync="sortByPurchaseOrder" :sort-desc.sync="sortDescPurchaseOrder" small responsive>
									<template v-slot:head(actions)>
										<b-button @click="isShowPurchaseOrderFilter = !isShowPurchaseOrderFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowPurchaseOrderFilter">
										<b-th></b-th>
										<b-th stacked-heading="PO Number">
											<b-form-input id="input-po-number-table" v-model="filterPurchaseOrder.poNumber" size="sm"></b-form-input>
										</b-th>
										<b-th stacked-heading="PO Date">
											<b-form-datepicker boundary="viewport" placeholder="" id="input-po-date-from-table" v-model="filterPurchaseOrder.poDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
											<b-form-datepicker boundary="viewport" placeholder="" id="input-po-date-to-table" v-model="filterPurchaseOrder.poDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
										</b-th>
										<b-th stacked-heading="Remarks">
											<b-form-input id="input-remarks-table" v-model="filterPurchaseOrder.remarks" size="sm"></b-form-input>
										</b-th>
										<b-th>
										</b-th>
									</template>
									<template v-slot:table-busys>
										<div class="text-center my-2">
									<b-spinner class="align-middle text-primary"></b-spinner>
										<strong class="ml-2">Loading..</strong>
										</div>
									</template>
									<template v-slot:cell(no)="row">
										{{ ((row.index + 1) + ( purchaseOrderPageSize * (currentPurchaseOrderPage - 1))) }}
									</template>
									<template v-slot:cell(poDate)="row">
										{{ row.item.poDate | moment("DD-MMM-YYYY")  }}
									</template>
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentPurchaseOrderPage"
									:total-rows="totalPurchaseOrderRow"
									:per-page="purchaseOrderPageSize"
									aria-controls="my-table"
									></b-pagination>
								</b-col>
								</b-row>
							</b-tab>
							<b-tab title="Purchase Order Detail" active>
								<b-row class="mb-2">
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
									<template v-slot:head(actions)>
										<b-button @click="isShowPurchaseOrderDetailFilter = !isShowPurchaseOrderDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowPurchaseOrderDetailFilter">
										<b-th></b-th>
										<b-th>
										</b-th>
										<b-th stacked-heading="Part">
											<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseOrderDetail.partId"  :reduce="item => item.id" v-model="filterPurchaseOrderDetail.partId" @search="getParts" :filterable="true"></v-select>
										</b-th>
										<b-th stacked-heading="Qty">
											<b-form-input id="input-qty-table" v-model="filterPurchaseOrderDetail.qty" size="sm"></b-form-input>
										</b-th>
										<b-th>
										</b-th>
									</template>
									<template v-slot:table-busys>
										<div class="text-center my-2">
									<b-spinner class="align-middle text-primary"></b-spinner>
										<strong class="ml-2">Loading..</strong>
										</div>
									</template>
									<template v-slot:cell(no)="row">
										{{ ((row.index + 1) + ( purchaseOrderDetailPageSize * (currentPurchaseOrderDetailPage - 1))) }}
									</template>
									<template v-slot:cell(purchaseOrder)="row">
										{{ row.item.purchaseOrder != null ? row.item.purchaseOrder.poNumber : "" }}
									</template>
									<template v-slot:cell(part)="row">
										{{ row.item.part != null ? row.item.part.partName : "" }}
									</template>
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentPurchaseOrderDetailPage"
									:total-rows="totalPurchaseOrderDetailRow"
									:per-page="purchaseOrderDetailPageSize"
									aria-controls="my-table"
									></b-pagination>
								</b-col>
								</b-row>
							</b-tab>
						</b-tabs>
					</b-card>
				</b-col>
			</b-row>
		</b-card>
		<template #modal-footer="{ ok, cancel }">
			<b-button v-if="isUploadSuccess" size="sm" variant="primary" @click="downloadLog()">
				Download Log
			</b-button>
			<b-button size="sm" variant="success" class="float-right" @click="ok()">
				{{ okTitle }}
			</b-button>
			<b-button size="sm" variant="secondary" class="float-right" @click="cancel()">
				Cancel
			</b-button>
		</template>
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</b-modal>
</template>
<style lang="scss" scoped>
.card {
	border: 1px solid rgba(0, 0, 0, 0.125) !important;
}
/deep/.th-message {
	color: red;
}
/deep/.modal-footer {
	display: block;
}
</style>
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate"; 
import { required, minLength } from "vuelidate/lib/validators"; 
import { saveAs } from 'file-saver';

import PurchaseOrder from '@/models/PurchaseOrder/PurchaseOrder';
import PurchaseOrderList from '@/models/PurchaseOrder/PurchaseOrderList';
import PurchaseOrderFilter from '@/filters/PurchaseOrder/PurchaseOrderFilter';

import PurchaseOrderDetail from '@/models/PurchaseOrder/PurchaseOrderDetail';
import PurchaseOrderDetailFilter from '@/filters/PurchaseOrder/PurchaseOrderDetailFilter';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppInputFileupload from '@/components/AppInputFileupload';
import AppInputDatetimepicker from '@/components/AppInputDatetimepicker';
import AppInputDatetimepickerRange from '@/components/AppInputDatetimepickerRange';
export default {
	props: ['model', 'openPopup','uploadButtonMethod', 'parentId'],
	components : {AppInputFileupload,AppInputDatetimepicker,AppInputDatetimepickerRange,},
	data() {
		return {
			fileupload: null,
			datas: Array(),
			selectAll:false,
			sortBy: '',
			sortDesc: '',
			pagingOptions:[
				{ value : '10', text : '10', selected:true },
				{ value : '25', text : '25' },
				{ value : '50', text : '50' },
				{ value : '100', text : '100' },
			],
			perPage: 10,
			currentPage: 1,
			totalData:0,
			fieldsPurchaseOrder: [
				{"no" : { "class" : "th-number"}}, 
				{"poNumber": { "class" : "th-po-number", "label" : "PO Number"}, key: 'poNumber', sortable: true}, 
				{"poDate": { "class" : "th-po-date", "label" : "PO Date"}, key: 'poDate', sortable: true}, 
				{"remarks": { "class" : "th-remarks", "label" : "Remarks"}, key: 'remarks', sortable: true}, 
				{"class" : "th-status", "label" : "Status", key: 'uploadValidationStatus', sortable: true},
				{"class" : "th-message", "label" : "Message", key: 'uploadValidationMessage', sortable: true},
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
			fieldsPurchaseOrderDetail: [
				{"no" : { "class" : "th-number"}}, 
				{"purchaseOrder": { "class" : "th-purchase-order", "label" : "PurchaseOrder"}, key: 'purchaseOrder', sortable: true}, 
				{"part": { "class" : "th-part", "label" : "Part"}, key: 'part', sortable: true}, 
				{"partPrice": { "class" : "th-part-price", "label" : "Part Price"}, key: 'partPrice', sortable: true}, 
				{"qty": { "class" : "th-qty", "label" : "Qty"}, key: 'qty', sortable: true}, 
				{"totalPrice": { "class" : "th-total-price", "label" : "Total Price"}, key: 'totalPrice', sortable: true}, 
				{"class" : "th-status", "label" : "Status", key: 'uploadValidationStatus', sortable: true},
				{"class" : "th-message", "label" : "Message", key: 'uploadValidationMessage', sortable: true},
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
			busy:false,
			isDataLoading: false,
			isBusy : false,
			selected:'',
			pageSizeOptions: 10,
			pageIndex:0,
			sortingBy: Array(),
			purchaseOrders : [],
			purchaseOrderId : '',
			totalPurchaseOrderRow : 0,
			isShowPurchaseOrderFilter: false,
			filterPurchaseOrder : null,
			currentPurchaseOrderPage : 1,
			purchaseOrderPageSize: 10,
			sortByPurchaseOrder: '',
			sortDescPurchaseOrder: '',
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
			purchaseOrders: [],
			parts: [],
			processId: "",
			isUploadSuccess: false,
		}
	},
	methods : {
		resetModal() {
			this.datas = Array();
			this.$emit('resetMethod');
		},
		uploadButtonAction(modalEvent) {
			modalEvent.preventDefault();
			this.isBusy = true;
			if (this.isUploadSuccess) {
				let _this = this;
				this.$http.post(process.env.VUE_APP_API_URL + 'purchaseorder/confirmUpload',
					[],
					).then(async function(response){
						if (response != null) {
						if (response.data != null) {
							_this.resetModal();
							_this.datas = Array();
							_this.fileupload = null;
							_this.isUploadSuccess = false;
						}
						}
						_this.isBusy = false;
					})
					.catch(error => {
						_this.$store.dispatch('addErrorMessageGlobal', error);
						_this.isBusy = false;
					});
			} else {
				if (this.fileupload != null) {
				let data = new FormData();
				data.append('file', this.fileupload);
				let _this = this;
				this.$http.post(process.env.VUE_APP_API_URL + 'purchaseorder/upload',
					data,
					{ headers: { 'Content-Type': 'multipart/form-data' } }
					).then(async function(response){
						_this.isBusy = false;
						_this.isUploadSuccess = true;
						_this.getPurchaseOrders();
						_this.getPurchaseOrderDetails();
					})
					.catch(error => {
						_this.$store.dispatch('addErrorMessageGlobal', error);
						_this.isBusy = false;
					});
				}
			}
		},
		getPurchaseOrders : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseOrder);
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
			this.sortingByPurchaseOrder = Array();
			if (this.sortByPurchaseOrder != null) {
				this.sortingByPurchaseOrder[this.sortByPurchaseOrder] = this.sortDescPurchaseOrder;
			}
			let currentPage = (this.currentPurchaseOrderPage - 1);
			PurchaseOrder.getList(this, filter, this.sortingByPurchaseOrder, currentPage , this.purchaseOrderPageSize).then(result => {
				this.purchaseOrders = result.data;
				this.totalPurchaseOrderRow = result.dataCount;
			}).catch(error => {});
		},
		getPurchaseOrderDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseOrderDetail);
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
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
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
				this.isShowPurchaseOrderDetailFilter = true;
			}
		},
		downloadLog() {
			let _this = this;
			this.$http.post(process.env.VUE_APP_API_URL + 'purchaseorder/downloadlog', [],
			{ headers: { 'Content-Type': 'application/json' }}
			).then(async function(response){
				await saveAs(process.env.VUE_APP_API_URL + 'purchaseorder/downloadlog', "PurchaseOrder.xslx");
				_this.isBusy = false;
			})
			.catch(error => {
				_this.$store.dispatch('addErrorMessageGlobal', error);
				_this.isBusy = false;
			});
		}
	},
	beforeMount() {
	},
	mounted(){
	},
	watch: {
		sortByPurchaseOrder : {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		sortDescPurchaseOrder: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		currentPurchaseOrderPage: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		purchaseOrderPageSize : {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		filterPurchaseOrder: {
			handler: function() {
				this.getPurchaseOrders();
			},
			deep: true,
		},
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
		fileupload(newValue, oldValue) {
			this.datas = Array();
			this.isUploadSuccess = false;
		}
	},
	computed: {
		okTitle: {
			get: function() {
				if (this.isUploadSuccess) {
					return "Proses";
				}
				return "Simpan";
				}
		},
		isShowPopup : {
			get: function () {
				if (this.openPopup) {
					this.filterPurchaseOrder = new PurchaseOrderFilter();
					this.filterPurchaseOrderDetail = new PurchaseOrderDetailFilter();
					return true;
				}
				else return false;
			},
			set: function (newValue) {}
		},
	}
}
</script>
