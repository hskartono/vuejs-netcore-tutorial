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
							<b-tab title="Purchase Request" active>
								<b-row class="mb-2">
									<b-col>
										<span class="float-right">
											<b-form-select v-model="purchaseRequestPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
										</span>
									</b-col>
								</b-row>
								<b-row>
								<b-col>
								<div class="table-corner">
									<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseRequests" :fields="fieldsPurchaseRequest" :busy="isDataLoading" :sort-by.sync="sortByPurchaseRequest" :sort-desc.sync="sortDescPurchaseRequest" small responsive>
									<template v-slot:head(actions)>
										<b-button @click="isShowPurchaseRequestFilter = !isShowPurchaseRequestFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowPurchaseRequestFilter">
										<b-th></b-th>
										<b-th stacked-heading="PR Date">
											<b-form-datepicker boundary="viewport" placeholder="" id="input-pr-date-from-table" v-model="filterPurchaseRequest.prDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
											<b-form-datepicker boundary="viewport" placeholder="" id="input-pr-date-to-table" v-model="filterPurchaseRequest.prDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
										</b-th>
										<b-th stacked-heading="PR Number">
											<b-form-input id="input-pr-no-table" v-model="filterPurchaseRequest.prNo" size="sm"></b-form-input>
										</b-th>
										<b-th stacked-heading="Remarks">
											<b-form-input id="input-remarks-table" v-model="filterPurchaseRequest.remarks" size="sm"></b-form-input>
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
										{{ ((row.index + 1) + ( purchaseRequestPageSize * (currentPurchaseRequestPage - 1))) }}
									</template>
									<template v-slot:cell(prDate)="row">
										{{ row.item.prDate | moment("DD-MMM-YYYY")  }}
									</template>
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentPurchaseRequestPage"
									:total-rows="totalPurchaseRequestRow"
									:per-page="purchaseRequestPageSize"
									aria-controls="my-table"
									></b-pagination>
								</b-col>
								</b-row>
							</b-tab>
							<b-tab title="Purchase Request Detail" active>
								<b-row class="mb-2">
									<b-col>
										<span class="float-right">
											<b-form-select v-model="purchaseRequestDetailPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
										</span>
									</b-col>
								</b-row>
								<b-row>
								<b-col>
								<div class="table-corner">
									<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseRequestDetails" :fields="fieldsPurchaseRequestDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseRequestDetail" :sort-desc.sync="sortDescPurchaseRequestDetail" small responsive>
									<template v-slot:head(actions)>
										<b-button @click="isShowPurchaseRequestDetailFilter = !isShowPurchaseRequestDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowPurchaseRequestDetailFilter">
										<b-th></b-th>
										<b-th>
										</b-th>
										<b-th stacked-heading="Part">
											<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseRequestDetail.partId"  :reduce="item => item.id" v-model="filterPurchaseRequestDetail.partId" @search="getParts" :filterable="true"></v-select>
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
									<template v-slot:table-busys>
										<div class="text-center my-2">
									<b-spinner class="align-middle text-primary"></b-spinner>
										<strong class="ml-2">Loading..</strong>
										</div>
									</template>
									<template v-slot:cell(no)="row">
										{{ ((row.index + 1) + ( purchaseRequestDetailPageSize * (currentPurchaseRequestDetailPage - 1))) }}
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
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentPurchaseRequestDetailPage"
									:total-rows="totalPurchaseRequestDetailRow"
									:per-page="purchaseRequestDetailPageSize"
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

import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';
import PurchaseRequestList from '@/models/PurchaseRequest/PurchaseRequestList';
import PurchaseRequestFilter from '@/filters/PurchaseRequest/PurchaseRequestFilter';

import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';
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
			fieldsPurchaseRequest: [
				{"no" : { "class" : "th-number"}}, 
				{"prDate": { "class" : "th-pr-date", "label" : "PR Date"}, key: 'prDate', sortable: true}, 
				{"prNo": { "class" : "th-pr-no", "label" : "PR Number"}, key: 'prNo', sortable: true}, 
				{"remarks": { "class" : "th-remarks", "label" : "Remarks"}, key: 'remarks', sortable: true}, 
				{"class" : "th-status", "label" : "Status", key: 'uploadValidationStatus', sortable: true},
				{"class" : "th-message", "label" : "Message", key: 'uploadValidationMessage', sortable: true},
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
			fieldsPurchaseRequestDetail: [
				{"no" : { "class" : "th-number"}}, 
				{"purchaseRequest": { "class" : "th-purchase-request", "label" : "PurchaseRequest"}, key: 'purchaseRequest', sortable: true}, 
				{"part": { "class" : "th-part", "label" : "Part"}, key: 'part', sortable: true}, 
				{"qty": { "class" : "th-qty", "label" : "Qty"}, key: 'qty', sortable: true}, 
				{"requestDate": { "class" : "th-request-date", "label" : "Request Date"}, key: 'requestDate', sortable: true}, 
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
			purchaseRequests : [],
			purchaseRequestId : '',
			totalPurchaseRequestRow : 0,
			isShowPurchaseRequestFilter: false,
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
			purchaseRequests: [],
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
				this.$http.post(process.env.VUE_APP_API_URL + 'purchaserequest/confirmUpload',
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
				this.$http.post(process.env.VUE_APP_API_URL + 'purchaserequest/upload',
					data,
					{ headers: { 'Content-Type': 'multipart/form-data' } }
					).then(async function(response){
						_this.isBusy = false;
						_this.isUploadSuccess = true;
						_this.getPurchaseRequests();
						_this.getPurchaseRequestDetails();
					})
					.catch(error => {
						_this.$store.dispatch('addErrorMessageGlobal', error);
						_this.isBusy = false;
					});
				}
			}
		},
		getPurchaseRequests : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseRequest);
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
			this.sortingByPurchaseRequest = Array();
			if (this.sortByPurchaseRequest != null) {
				this.sortingByPurchaseRequest[this.sortByPurchaseRequest] = this.sortDescPurchaseRequest;
			}
			let currentPage = (this.currentPurchaseRequestPage - 1);
			PurchaseRequest.getList(this, filter, this.sortingByPurchaseRequest, currentPage , this.purchaseRequestPageSize).then(result => {
				this.purchaseRequests = result.data;
				this.totalPurchaseRequestRow = result.dataCount;
			}).catch(error => {});
		},
		getPurchaseRequestDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseRequestDetail);
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
			this.sortingByPurchaseRequestDetail = Array();
			if (this.sortByPurchaseRequestDetail != null) {
				this.sortingByPurchaseRequestDetail[this.sortByPurchaseRequestDetail] = this.sortDescPurchaseRequestDetail;
			}
			let currentPage = (this.currentPurchaseRequestDetailPage - 1);
			PurchaseRequestDetail.getList(this, filter, this.sortingByPurchaseRequestDetail, currentPage , this.purchaseRequestDetailPageSize).then(result => {
				this.purchaseRequestDetails = result.data;
				this.totalPurchaseRequestDetailRow = result.dataCount;
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
				this.isShowPurchaseRequestDetailFilter = true;
			}
		},
		downloadLog() {
			let _this = this;
			this.$http.post(process.env.VUE_APP_API_URL + 'purchaserequest/downloadlog', [],
			{ headers: { 'Content-Type': 'application/json' }}
			).then(async function(response){
				await saveAs(process.env.VUE_APP_API_URL + 'purchaserequest/downloadlog', "PurchaseRequest.xslx");
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
		sortByPurchaseRequest : {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		sortDescPurchaseRequest: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		currentPurchaseRequestPage: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		purchaseRequestPageSize : {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		filterPurchaseRequest: {
			handler: function() {
				this.getPurchaseRequests();
			},
			deep: true,
		},
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
					this.filterPurchaseRequest = new PurchaseRequestFilter();
					this.filterPurchaseRequestDetail = new PurchaseRequestDetailFilter();
					return true;
				}
				else return false;
			},
			set: function (newValue) {}
		},
	}
}
</script>
