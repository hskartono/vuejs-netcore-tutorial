<template>
	<PopupRouterView label="Purchase Request Input"  entityName="purchaserequest" :documentId="mainRecordId"  @saveMethod="saveActionConfirmation" @hideMethod="onHidden">
		<b-card>
			<b-col cols="12" lg="6">
				<app-input-datepicker  id="fieldset-pr-date" label-cols-sm="4" label-cols-lg="3" label="PR Date" size="sm" :model.sync="model.prDate"  :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" @change="updateData" />
				<app-input-textbox id="fieldset-pr-no" label-cols-sm="4" label-cols-lg="3" label="PR Number" label-for="input-pr-no" size="sm" :model.sync="model.prNo"  @change="updateData" />
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
			<b-col cols="9"><h5 class="card-title font-weight-bolder text-dark">Purchase Request Details</h5></b-col>
			<b-col>
				<span class="float-right">
					<b-form-select v-model="purchaseRequestDetailPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
				</span>
			</b-col>
		</b-row>
		<b-row>
		<b-col>
			<app-table-input-popup :items="purchaseRequestDetails" :fields="fieldsPurchaseRequestDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseRequestDetail" :sort-desc.sync="sortDescPurchaseRequestDetail" small responsive
			@editButtonMethod="editRowDataPurchaseRequestDetail"
			@deleteButtonMethod="deleteRowDataPurchaseRequestDetail"
			@showPopupAddMethod="resetDataPurchaseRequestDetail()"
			@showPopupUploadMethod="showPopupUploadPurchaseRequestDetail()"
			@downloadMethod="downloadPurchaseRequestDetail()"
			@addFromClipboardMethod="addFromClipboardPurchaseRequestDetail()"
			@replaceFromClipboardMethod="replaceFromClipboardPurchaseRequestDetail()"
			:showActionButton="true"
			:showAddButton="true"
			:showAddFromClipboardButton="true"
			:showReplaceFromClipboardButton="true"
			:showUploadButton="true"
			:showDownloadButton="true"
			:isDownloadBusy="isDownloadPurchaseRequestDetail"
			>
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseRequestDetailFilter = !isShowPurchaseRequestDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseRequestDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts"  :value="filterPurchaseRequestDetail.partId" :reduce="item => item.id" v-model="filterPurchaseRequestDetail.partId" @search="getParts" :filterable="true"></v-select>
				</b-th>
				<b-th stacked-heading="Qty">
					<b-form-input id="input-qty-table" v-model="filterPurchaseRequestDetail.qty" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Request Date">
					<b-form-datepicker boundary="viewport" placeholder="" id="input-request-date-from-table" v-model="filterPurchaseRequestDetail.requestDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
					<b-form-datepicker boundary="viewport" placeholder="" id="input-request-date-to-table" v-model="filterPurchaseRequestDetail.requestDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
				</b-th>
				<b-th>
				</b-th>
			</template>
			<template v-slot:cell(no)="row">
				{{ ((row.index + 1) + ( purchaseRequestDetailPageSize * (currentPurchaseRequestDetailPage - 1))) }}
			</template>
			<template v-slot:table-busys>
				<div class="text-center my-2">
			<b-spinner class="align-middle text-primary"></b-spinner>
				<strong class="ml-2">Loading...</strong>
				</div>
			</template>
			<template v-slot:cell(purchaseRequest)="row">
				{{ row.item.purchaseRequest != null ? row.item.purchaseRequest.prNo : "" }}
			</template>
			<template v-slot:cell(part)="row">
				{{ row.item.part != null ? row.item.part.partName : "" }}
			</template>
			<template v-slot:cell(requestDate)="row">
				{{ row.item.requestDate | moment("DD-MMM-YYYY")  }}
			</template>
				<template v-slot:form-add-detail>
				<PurchaseRequestDetailInput 
					:openPopup="isShowPopupPurchaseRequestDetail"
					:modelId="purchaseRequestDetailId"
					@resetMethod="resetModal"
					@addButtonMethod="addRowDataPurchaseRequestDetail"/>
				</template>
				<template v-slot:upload-detail>
					<PurchaseRequestDetailUpload
					:openPopup="isShowPopupUploadPurchaseRequestDetail"
					@resetMethod="resetModal"
					@uploadButtonMethod="uploadDataPurchaseRequestDetail"
					:parentId="modelId"
				 />
				</template>
			<template v-slot:table-paging>
				<b-pagination
				class="float-right btn-paging"
				v-model="currentPurchaseRequestDetailPage"
				:total-rows="totalPurchaseRequestDetailRow"
				:per-page="purchaseRequestDetailPageSize"
				aria-controls="my-table"
				></b-pagination>
			</template>
			</app-table-input-popup>
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
import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';
import PurchaseRequestList from '@/models/PurchaseRequest/PurchaseRequestList';
import PurchaseRequestFilter from '@/filters/PurchaseRequest/PurchaseRequestFilter';

import PopupConfirmationDetail from './PopupConfirmationDetail';

import AppInputDatepicker from '@/components/AppInputDatepicker';
import AppInputDatepickerRange from '@/components/AppInputDatepickerRange';
import AppInputTextbox from '@/components/AppInputTextbox';
import AppInputTextarea from '@/components/AppInputTextarea';
import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppTableInput from '@/components/AppTableInput';
import AppTableInputPopup from '@/components/AppTableInputPopup';
import PurchaseRequestDetailInput from '@/views/PurchaseRequest/PurchaseRequestDetailInput';
import PurchaseRequestDetailUpload from '@/views/PurchaseRequest/PurchaseRequestDetailUpload';
import PopupRouterView from '@/components/PopupRouterView';
export default {
	components : {AppInputDatepicker,AppInputDatepickerRange,AppInputTextbox,AppInputTextarea,PurchaseRequestDetailInput,PurchaseRequestDetailUpload,AppTableInput,AppTableInputPopup,PopupRouterView,PopupConfirmationDetail},
	mixins : [validationMixin],
	validations: {
		model: {
		}
	},
	data() {
		return {
			errorMessage : {
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
			fieldsPurchaseRequestDetail: [
				{"no" : { "class" : "th-number"}}, 
				{"part": { "class" : "th-part"}, "label" : "Part", key: 'part', sortable: true}, 
				{"qty": { "class" : "th-qty"}, "label" : "Qty", key: 'qty', sortable: true}, 
				{"requestDate": { "class" : "th-request-date"}, "label" : "Request Date", key: 'requestDate', sortable: true}, 
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
			modelPurchaseRequestDetail : {},
			isShowPopupPurchaseRequestDetail : false,
			isShowPopupUploadPurchaseRequestDetail : false,
			filterPurchaseRequest : null,
			currentPurchaseRequestPage : 1,
			purchaseRequestPageSize: 10,
			sortByPurchaseRequest: '',
			sortDescPurchaseRequest: '',
			filterPurchaseRequestDetail : null,
			currentPurchaseRequestDetailPage : 1,
			purchaseRequestDetailPageSize: 10,
			sortByPurchaseRequestDetail: '',
			sortDescPurchaseRequestDetail: '',
			sortingByPurchaseRequestDetail: [],
			purchaseRequestDetails : [],
			purchaseRequestDetailId : '',
			totalPurchaseRequestDetailRow : 0,
			isShowPurchaseRequestDetailFilter: false,
			isDownloadPurchaseRequestDetail: false,
			parts: [],
			isShowPopupConfirmation : false,
			isDataCommited : false,
			mainRecordId : '',
		}
	},
	methods : {
		getPurchaseRequest : async function(id) {
			this.isBusy = true;
			PurchaseRequest.getData(this, id).then(result => { this.model = result; this.mainRecordId = String(result.mainRecordId); this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		downloadData() {
			this.isDownloadPurchaseRequest = true;
			this.filterPurchaseRequest.id = this.modelId;
			PurchaseRequest.downloadDataDetail(this, this.filterPurchaseRequest).then(result => {
				this.isDownloadPurchaseRequest = false;
			}).catch(error => { this.isDownloadPurchaseRequest = false; })
		},
		getPurchaseRequestDetails : async function(isAddRow) {
			let filter = {};
			Object.assign(filter, this.filterPurchaseRequestDetail);
			if (this.modelId == null) return;
			filter.purchaseRequestId = this.modelId;
			filter.draftMode = "1";
			if (this.sortByPurchaseRequestDetail != null) {
				this.sortingByPurchaseRequestDetail[this.sortByPurchaseRequestDetail] = this.sortDescPurchaseRequestDetail;
			}
			let currentPage = (this.currentPurchaseRequestDetailPage - 1);
			if (isAddRow) {
				let totalData = this.totalPurchaseRequestDetailRow + 1;
				let page = Math.ceil(totalData/this.purchaseRequestDetailPageSize);
				currentPage = page - 1;
			}
			PurchaseRequestDetail.getList(this, filter, this.sortingByPurchaseRequestDetail, currentPage , this.purchaseRequestDetailPageSize).then(result => {
				this.currentPurchaseRequestDetailPage = currentPage + 1;
				this.purchaseRequestDetails = result.data;
				this.totalPurchaseRequestDetailRow = result.dataCount;
			}).catch(error => {});
		},
		getPurchaseRequests : async function(input) {
			let filter = new PurchaseRequestFilter();
			if (input != undefined) {
				filter.prNo = input;
			}
			PurchaseRequest.getList(this, filter, null, 0, 10)
			.then(result => {
				if (result != null) {
					this.purchaseRequests = result.data
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
				PurchaseRequest.commitData(this, model, id).then(result => {
					this.isDataCommited = true;
					this.$router.push({ name: 'purchaserequestindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			} else {
				PurchaseRequest.insertData(this, model).then(result => {
					this.$router.push({ name: 'purchaserequestindex' });
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
					PurchaseRequest.updateData(this, model, id);
				}
			});
		},
		onHidden() {
			this.getPurchaseRequests();
		},
		addRowDataPurchaseRequestDetail(modalEvent, model) {
			if (model != null) {
				if (model.id != null && model.id != '') {
					model.purchaseRequestId =  this.modelId;
					PurchaseRequestDetail.updateDataDetail(this, model, model.id)
					.then(result => {
						this.getPurchaseRequestDetails(true);
						this.isShowPopupPurchaseRequestDetail = false;
					})
				} else {
					model.purchaseRequestId =  this.modelId;
					PurchaseRequestDetail.createDataDetail(this, model)
					.then(result => {
						this.getPurchaseRequestDetails(true);
						this.isShowPopupPurchaseRequestDetail = false;
					})
				}
			}
		},
		async editRowDataPurchaseRequestDetail(id) {
			this.purchaseRequestDetailId = id;
			this.isShowPopupPurchaseRequestDetail = true; 
		},
		async deleteRowDataPurchaseRequestDetail(id) {
			PurchaseRequestDetail.deleteData(this, id).then(result => { this.getPurchaseRequestDetails(); });
		},
		async resetDataPurchaseRequestDetail() {
			this.purchaseRequestDetailId = "";
			this.isShowPopupPurchaseRequestDetail = true;
		},
		async addDataPurchaseRequestDetail() {
			PurchaseRequestDetail.createDataDetail(this, { purchaseRequestId: this.modelId })
			.then(result => {
				this.getPurchaseRequestDetails(true);
			})
		},
		async addFromClipboardPurchaseRequestDetail() {
			let parts = new Array();
			navigator.clipboard.readText().then(value => {
				if(value.match(/(?:\r\n|\r|\n)/g)) { value=value.replace(/(?:\r\n|\r|\n)/g, ',') ; }
				let values = value.split(",");
				values.forEach(item => {
					parts.push({ part: item, purchaseRequestId: this.modelId });
				});
				PurchaseRequestDetail.addFromClipboard(this, parts).then(result => { this.getPurchaseRequestDetails() });
			});
		},
		async replaceFromClipboardPurchaseRequestDetail() {
			let parts = new Array();
			navigator.clipboard.readText().then(value => {
				if(value.match(/(?:\r\n|\r|\n)/g)) { value=value.replace(/(?:\r\n|\r|\n)/g, ',') ; }
				let values = value.split(",");
				values.forEach(item => {
					parts.push({ part: item, purchaseRequestId: this.modelId });
				});
				PurchaseRequestDetail.replaceFromClipboard(this, parts).then(result => { this.getPurchaseRequestDetails() });
			});   
		},
		showPopupUploadPurchaseRequestDetail() {
			this.isShowPopupUploadPurchaseRequestDetail = true;
		},
		uploadDataPurchaseRequestDetail(modalEvent, newDatas) {
			this.isShowPopupUploadPurchaseRequestDetail = false; 
		},
		downloadPurchaseRequestDetail(modalEvent) {
			this.isDownloadPurchaseRequestDetail = true;
			this.filterPurchaseRequestDetail.purchaseRequestId = this.modelId;
			PurchaseRequestDetail.downloadDataDetail(this, this.filterPurchaseRequestDetail).then(result => {
				this.isDownloadPurchaseRequestDetail = false;
			}).catch(error => { this.isDownloadPurchaseRequestDetail = false; })
		},
		saveInputPurchaseRequestDetail(item) {
			PurchaseRequestDetail.updateDataDetail(this, item, item.id);
		},
		resetModal(modalType) {
			this.isShowPopupUploadPurchaseRequestDetail = false; 
			this.isShowPopupPurchaseRequestDetail = false; 
			if (modalType == 'PurchaseRequestDetail') {
				this.getPurchaseRequestDetails();
			}
			this.showPopupCopyData = false; 
			this.isShowPopupConfirmation = false;
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
				this.isShowPurchaseRequestDetailFilter = true;
			}
		},
	},
	beforeMount() {
		this.filterPurchaseRequest = new PurchaseRequestFilter();
		this.filterPurchaseRequestDetail = new PurchaseRequestDetailFilter();
		this.getPurchaseRequests();
		this.getParts();
	},
	async mounted(){
		let id = this.$route.params.id;
		let copyData = this.$route.params.copydata;
		if (id != '' && id != null && id != undefined) {
			if (copyData != null && copyData != undefined) {
				this.getPurchaseRequest(id);
			} else {
				this.modelId = id;
				this.getPurchaseRequest(id);
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
		sortByPurchaseRequestDetail : {
			handler: function() {
				this.getPurchaseRequestDetails();
			}
		},
		sortDescPurchaseRequestDetail: {
			handler: function() {
				this.getPurchaseRequestDetails();
			}
		},
		currentPurchaseRequestDetailPage: {
			handler: function() {
				this.getPurchaseRequestDetails();
			}
		},
		purchaseRequestDetailPageSize : {
			handler: function() {
				this.getPurchaseRequestDetails();
			}
		},
		filterPurchaseRequestDetail: {
			handler: function() {
				this.getPurchaseRequestDetails();
			},
			deep: true,
		},
	},
	beforeDestroy() {
		if (!this.isDataCommited) {
			PurchaseRequest.discardData(this, this.modelId);
		}
	},
}
</script>
