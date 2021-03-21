<template>
	<PopupRouterView label="Purchase Order Input"  entityName="purchaseorder" :documentId="mainRecordId"  @saveMethod="saveActionConfirmation" @hideMethod="onHidden">
		<b-card>
			<b-col cols="12" lg="6">
				<app-input-textbox id="fieldset-po-number" label-cols-sm="4" label-cols-lg="3" label="PO Number" label-for="input-po-number" size="sm" :model.sync="model.poNumber"  @change="updateData" />
				<app-input-datepicker  id="fieldset-po-date" label-cols-sm="4" label-cols-lg="3" label="PO Date" size="sm" :model.sync="model.poDate" :isRequired="true" :state="validateState('poDate')" :errorMessage="getErrorMessage(errorMessage.poDate, 'poDate')"  :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" @change="updateData" />
				<app-input-textarea id="fieldset-remarks" label-cols-sm="4" label-cols-lg="3" label="Remarks" label-for="input-remarks" size="sm" :model.sync="model.remarks"  @change="updateData" />
			</b-col>
			<b-col>
				<span class="float-right" v-if="modelId != null && modelId != undefined && modelId != ''">
					<b-button variant="success" class="float-right ml-2" @click="downloadData" size="sm"><b-icon-download class="mr-1"></b-icon-download> Download</b-button>
				</span>
			</b-col>
		</b-card>
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
			<app-table-input :items="purchaseOrderDetails" :fields="fieldsPurchaseOrderDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseOrderDetail" :sort-desc.sync="sortDescPurchaseOrderDetail" small responsive
			@editButtonMethod="editRowDataPurchaseOrderDetail"
			@deleteButtonMethod="deleteRowDataPurchaseOrderDetail"
			@showPopupAddMethod="addDataPurchaseOrderDetail()"
			@showPopupUploadMethod="showPopupUploadPurchaseOrderDetail()"
			@downloadMethod="downloadPurchaseOrderDetail()"
			@addFromClipboardMethod="addFromClipboardPurchaseOrderDetail()"
			@replaceFromClipboardMethod="replaceFromClipboardPurchaseOrderDetail()"
			:showActionButton="true"
			:showAddButton="true"
			:showAddFromClipboardButton="true"
			:showReplaceFromClipboardButton="true"
			:showUploadButton="true"
			:showDownloadButton = "true"
			:isDownloadBusy = "isDownloadPurchaseOrderDetail"
			>
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseOrderDetailFilter = !isShowPurchaseOrderDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseOrderDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseOrderDetail.partId" :reduce="item => item.id" v-model="filterPurchaseOrderDetail.partId" @search="getParts" :filterable ="true"></v-select>
				</b-th>
				<b-th stacked-heading="Part Price">
					<b-form-input id="input-part-price-table" v-model="filterPurchaseOrderDetail.partPrice" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Qty">
					<b-form-input id="input-qty-table" v-model="filterPurchaseOrderDetail.qty" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Total Price">
					<b-form-input id="input-total-price-table" v-model="filterPurchaseOrderDetail.totalPrice" size="sm"></b-form-input>
				</b-th>
				<b-th>
				</b-th>
			</template>
			<template v-slot:cell(no)="row">
				{{ ((row.index + 1) + ( purchaseOrderDetailPageSize * (currentPurchaseOrderDetailPage - 1))) }}
			</template>
			<template v-slot:table-busys>
				<div class="text-center my-2">
			<b-spinner class="align-middle text-primary"></b-spinner>
				<strong class="ml-2">Loading...</strong>
				</div>
			</template>
			<template v-slot:cell(part)="row">
				<app-input-combobox-autocomplete :options="parts" optionsLabel="partName" optionsKey="id" id="input-" size="sm" :model.sync="row.item.partId"   @change="saveInputPurchaseOrderDetail(row.item)" />
			</template>
			<template v-slot:cell(partPrice)="row">
				<app-input-textbox id="fieldset-part-price" size="sm" :thousandSeparator="true" :model.sync="row.item.partPrice"  @change="saveInputPurchaseOrderDetail(row.item)" />
			</template>
			<template v-slot:cell(qty)="row">
				<app-input-textbox id="fieldset-qty" size="sm" :model.sync="row.item.qty"   @change="saveInputPurchaseOrderDetail(row.item)" />
			</template>
			<template v-slot:cell(totalPrice)="row">
				<app-input-textbox id="fieldset-total-price" size="sm" :thousandSeparator="true" :model.sync="row.item.totalPrice"  @change="saveInputPurchaseOrderDetail(row.item)" />
			</template>
				<template v-slot:form-add-detail>
				<PurchaseOrderDetailInput 
					:model="modelPurchaseOrderDetail"
					:openPopup="isShowPopupPurchaseOrderDetail"
					@resetMethod="resetModal"
					@addButtonMethod="addRowDataPurchaseOrderDetail"/>
				</template>
				<template v-slot:upload-detail>
					<PurchaseOrderDetailUpload
					:openPopup="isShowPopupUploadPurchaseOrderDetail"
					@resetMethod="resetModal"
					@uploadButtonMethod="uploadDataPurchaseOrderDetail"
					:parentId="modelId"
				 />
				</template>
			<template v-slot:table-paging>
				<b-pagination
				class="float-right btn-paging"
				v-model="currentPurchaseOrderDetailPage"
				:total-rows="totalPurchaseOrderDetailRow"
				:per-page="purchaseOrderDetailPageSize"
				aria-controls="my-table"
				></b-pagination>
			</template>
			</app-table-input>
		</b-col>
		</b-row>
	</b-card>
		<PopupConfirmationDetail :openPopup="isShowPopupConfirmation" :modelId="modelId" @saveActionMethod="saveAction" @resetMethod="resetModal" />
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterView>
</template>
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate";
import { required, minLength } from "vuelidate/lib/validators";
import PurchaseOrder from '@/models/PurchaseOrder/PurchaseOrder';
import PurchaseOrderList from '@/models/PurchaseOrder/PurchaseOrderList';
import PurchaseOrderFilter from '@/filters/PurchaseOrder/PurchaseOrderFilter';

import PopupConfirmationDetail from './PopupConfirmationDetail';

import AppInputTextbox from '@/components/AppInputTextbox';
import AppInputDatepicker from '@/components/AppInputDatepicker';
import AppInputDatepickerRange from '@/components/AppInputDatepickerRange';
import AppInputTextarea from '@/components/AppInputTextarea';
import PurchaseOrderDetail from '@/models/PurchaseOrder/PurchaseOrderDetail';
import PurchaseOrderDetailFilter from '@/filters/PurchaseOrder/PurchaseOrderDetailFilter';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppTableInput from '@/components/AppTableInput';
import AppTableInputPopup from '@/components/AppTableInputPopup';
import PurchaseOrderDetailInput from '@/views/PurchaseOrder/PurchaseOrderDetailInput';
import PurchaseOrderDetailUpload from '@/views/PurchaseOrder/PurchaseOrderDetailUpload';
import PopupRouterView from '@/components/PopupRouterView';
export default {
	components : {AppInputTextbox,AppInputDatepicker,AppInputDatepickerRange,AppInputTextarea,PurchaseOrderDetailInput,PurchaseOrderDetailUpload,AppTableInput,AppTableInputPopup,PopupRouterView,PopupConfirmationDetail},
	mixins : [validationMixin],
	validations: {
		model: {
			poDate: {required},
		}
	},
	data() {
		return {
			errorMessage : {
				poDate: { required : "PO Date is Required."},
			},
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
			fieldsPurchaseOrderDetail: [
				{"no" : { "class" : "th-number"}}, 
				{"part": { "class" : "th-part"}, "label" : "Part", key: 'part', sortable: true}, 
				{"partPrice": { "class" : "th-part-price"}, "label" : "Part Price", key: 'partPrice', sortable: true}, 
				{"qty": { "class" : "th-qty"}, "label" : "Qty", key: 'qty', sortable: true}, 
				{"totalPrice": { "class" : "th-total-price"}, "label" : "Total Price", key: 'totalPrice', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : ""}}
			],
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			pageSizeOptions: 10,
			pageIndex:0,
			sortingBy: Array(),
			modelId: null,
			model: {},
			modelPurchaseOrderDetail : {},
			isShowPopupPurchaseOrderDetail : false,
			isShowPopupUploadPurchaseOrderDetail : false,
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
			isDownloadPurchaseOrderDetail: false,
			parts: [],
			isShowPopupConfirmation : false,
			isDataCommited : false,
			mainRecordId : '',
		}
	},
	methods : {
		getPurchaseOrder : async function(id) {
			this.isBusy = true;
			PurchaseOrder.getData(this, id).then(result => { this.model = result; this.mainRecordId = String(result.mainRecordId); this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		downloadData() {
			this.isDownloadPurchaseOrder = true;
			this.filterPurchaseOrder.id = this.modelId;
			PurchaseOrder.downloadDataDetail(this, this.filterPurchaseOrder).then(result => {
				this.isDownloadPurchaseOrder = false;
			}).catch(error => { this.isDownloadPurchaseOrder = false; })
		},
		getPurchaseOrderDetails : async function(isAddRow) {
			let filter = {};
			Object.assign(filter, this.filterPurchaseOrderDetail);
			if (this.modelId == null) return;
			filter.purchaseOrderId = this.modelId;
			filter.draftMode = "1";
			if (this.sortByPurchaseOrderDetail != null) {
				this.sortingByPurchaseOrderDetail[this.sortByPurchaseOrderDetail] = this.sortDescPurchaseOrderDetail;
			}
			let currentPage = (this.currentPurchaseOrderDetailPage - 1);
			if (isAddRow) {
				let totalData = this.totalPurchaseOrderDetailRow + 1;
				let page = Math.ceil(totalData/this.purchaseOrderDetailPageSize);
				currentPage = page - 1;
			}
			PurchaseOrderDetail.getList(this, filter, this.sortingByPurchaseOrderDetail, currentPage , this.purchaseOrderDetailPageSize).then(result => {
				this.currentPurchaseOrderDetailPage = currentPage + 1;
				this.purchaseOrderDetails = result.data;
				this.totalPurchaseOrderDetailRow = result.dataCount;
			}).catch(error => {});
		},
		getPurchaseOrders : async function(input) {
			let filter = new PurchaseOrderFilter();
			if (input != undefined) {
				filter.poNumber = input;
			}
			PurchaseOrder.getList(this, filter, null, 0, 10)
			.then(result => {
				if (result != null) {
					this.purchaseOrders = result.data
				}
			})
			.catch(error => {});
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
		validateState(name) {
			const { $dirty, $error } = this.$v.model[name];
			return $dirty ? !$error : null;
		},
		getErrorMessage(message, name) {
			if (this.$v.model[name].required != undefined && !this.$v.model[name].required) {
				if (message.required) return message.required;
			}
		},
		saveActionConfirmation(modalEvent) {
			modalEvent.preventDefault();
			let id = this.$route.params.id;
			this.$v.model.$touch(); 
			if (this.$v.model.$anyError) {
				return;
			} else {
				this.isShowPopupConfirmation = true;
			}
		},
		saveAction(modalEvent) {
			this.isBusy = true;
			let id = this.modelId;
			let model = this.model;

			if (id != '' && id != null && id != undefined) {
				PurchaseOrder.commitData(this, model, id).then(result => {
					this.isDataCommited = true;
					this.$router.push({ name: 'purchaseorderindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			} else {
				PurchaseOrder.insertData(this, model).then(result => {
					this.$router.push({ name: 'purchaseorderindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			}
		},
		updateData() {
			let id = this.modelId;
			let model = this.model;

			let promises = Array();

			Promise.all(promises).then((results) => {
				if (id != '' && id != null && id != undefined) {
					PurchaseOrder.updateData(this, model, id);
				}
			});
		},
		onHidden() {
			this.getPurchaseOrders();
		},
		addRowDataPurchaseOrderDetail(modalEvent, model) {
			if (model != null) {
				if (model.id != null && model.id != '') {
					model.purchaseOrderId =  this.modelId;
					PurchaseOrderDetail.updateDataDetail(this, model, model.id)
					.then(result => {
						this.getPurchaseOrderDetails(true);
						this.isShowPopupPurchaseOrderDetail = false;
					})
				} else {
					model.purchaseOrderId =  this.modelId;
					PurchaseOrderDetail.createDataDetail(this, model)
					.then(result => {
						this.getPurchaseOrderDetails(true);
						this.isShowPopupPurchaseOrderDetail = false;
					})
				}
			}
		},
		async editRowDataPurchaseOrderDetail(id) {
			this.purchaseOrderDetailId = id;
			this.isShowPopupPurchaseOrderDetail = true; 
		},
		async deleteRowDataPurchaseOrderDetail(id) {
			PurchaseOrderDetail.deleteData(this, id).then(result => { this.getPurchaseOrderDetails(); });
		},
		async resetDataPurchaseOrderDetail() {
			this.purchaseOrderDetailId = "";
			this.isShowPopupPurchaseOrderDetail = true;
		},
		async addDataPurchaseOrderDetail() {
			PurchaseOrderDetail.createDataDetail(this, { purchaseOrderId: this.modelId })
			.then(result => {
				this.getPurchaseOrderDetails(true);
			})
		},
		async addFromClipboardPurchaseOrderDetail() {
			let partPrices = new Array();
			navigator.clipboard.readText().then(value => {
				if(value.match(/(?:\r\n|\r|\n)/g)) { value=value.replace(/(?:\r\n|\r|\n)/g, ',') ; }
				let values = value.split(",");
				values.forEach(item => {
					partPrices.push({ partPrice: item, purchaseOrderId: this.modelId });
				});
				PurchaseOrderDetail.addFromClipboard(this, partPrices).then(result => { this.getPurchaseOrderDetails() });
			});
		},
		async replaceFromClipboardPurchaseOrderDetail() {
			let partPrices = new Array();
			navigator.clipboard.readText().then(value => {
				if(value.match(/(?:\r\n|\r|\n)/g)) { value=value.replace(/(?:\r\n|\r|\n)/g, ',') ; }
				let values = value.split(",");
				values.forEach(item => {
					partPrices.push({ partPrice: item, purchaseOrderId: this.modelId });
				});
				PurchaseOrderDetail.replaceFromClipboard(this, partPrices).then(result => { this.getPurchaseOrderDetails() });
			});   
		},
		showPopupUploadPurchaseOrderDetail() {
			this.isShowPopupUploadPurchaseOrderDetail = true;
		},
		uploadDataPurchaseOrderDetail(modalEvent, newDatas) {
			this.isShowPopupUploadPurchaseOrderDetail = false; 
		},
		downloadPurchaseOrderDetail(modalEvent) {
			this.isDownloadPurchaseOrderDetail = true;
			this.filterPurchaseOrderDetail.purchaseOrderId = this.modelId;
			PurchaseOrderDetail.downloadDataDetail(this, this.filterPurchaseOrderDetail).then(result => {
				this.isDownloadPurchaseOrderDetail = false;
			}).catch(error => { this.isDownloadPurchaseOrderDetail = false; })
		},
		saveInputPurchaseOrderDetail(item) {
			PurchaseOrderDetail.updateDataDetail(this, item, item.id);
		},
		resetModal(modalType) {
			this.isShowPopupUploadPurchaseOrderDetail = false; 
			this.isShowPopupPurchaseOrderDetail = false; 
			if (modalType == 'PurchaseOrderDetail') {
				this.getPurchaseOrderDetails();
			}
			this.showPopupCopyData = false; 
			this.isShowPopupConfirmation = false;
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
				this.isShowPurchaseOrderDetailFilter = true;
			}
		},
	},
	beforeMount() {
		this.filterPurchaseOrder = new PurchaseOrderFilter();
		this.filterPurchaseOrderDetail = new PurchaseOrderDetailFilter();
		this.getPurchaseOrders();
		this.getParts();
	},
	async mounted(){
		let id = this.$route.params.id;
		let copyData = this.$route.params.copydata;
		if (id != '' && id != null && id != undefined) {
			if (copyData != null && copyData != undefined) {
				this.getPurchaseOrder(id);
			} else {
				this.modelId = id;
				this.getPurchaseOrder(id);
			}
		}
		this.$nextTick(() => {
			window.addEventListener('resize', this.displayFilter);
			this.displayFilter();
		});
	},
	computed: {
	},
	watch : {
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
	},
	beforeDestroy() {
		if (!this.isDataCommited) {
			PurchaseOrder.discardData(this, this.modelId);
		}
	},
}
</script>
