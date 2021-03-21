<template>
	<div class="pb-5">
		<b-button class="filter" variant="light" block href="#" v-b-toggle.accordion-filter>
			<span><b-icon-funnel-fill></b-icon-funnel-fill> Filter</span>
			<span class="menu-arrow">
				<b-icon-chevron-right class="float-right chevron-right"></b-icon-chevron-right><b-icon-chevron-down class="float-right chevron-down"></b-icon-chevron-down>
			</span>
		</b-button>
		<b-collapse id="accordion-filter" accordion="accordion-filter" role="tabpanel" class="menu-accordion">
			<b-card>
				<b-row>
					<b-col cols="12" lg="6">
					<app-input-datepicker-range  id="fieldset-pr-date" label-cols-sm="4" label-cols-lg="3" label="PR Date" size="sm" :modelFrom.sync="filter.prDateFrom" :modelTo.sync="filter.prDateTo" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button />
					<app-input-textbox id="fieldset-pr-no" label-cols-sm="4" label-cols-lg="3" label="PR Number" label-for="filter-pr-no" size="sm" :model.sync="filter.prNo" />
					<app-input-textarea id="fieldset-remarks" label-cols-sm="4" label-cols-lg="3" label="Remarks" label-for="filter-remarks" size="sm" :model.sync="filter.remarksMultiple" />
					<b-button variant="danger" size="sm" class="float-right" @click="resetFilter"><span style="font-size: 0.75rem" class="mr-1"><b-icon-x></b-icon-x></span> Clear Filter</b-button>
					<b-button variant="primary" size="sm" class="float-right mr-1" @click="getPurchaseRequests"><span style="font-size: 0.75rem" class="mr-1"><b-icon-search></b-icon-search></span> Cari</b-button>
					</b-col>
				</b-row>
			</b-card>
		</b-collapse>
		<b-row class="mt-3">
			<b-col>
				<b-button variant="warning" v-if="(userRole != null ? userRole.allowUpdate : false)" class="float-right ml-2" @click="showPopupUploadData" size="sm"><b-icon-upload class="mr-1"></b-icon-upload> Upload</b-button>
				<b-overlay
					v-if="(userRole != null ? userRole.allowDownload : false)"
					:show="isDownloadDataBusy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="float-right "
					>
					<b-button variant="primary" v-if="(userRole != null ? userRole.allowDownload : false)" class="float-right ml-2" @click="downloadData" size="sm"><b-icon-download class="mr-1"></b-icon-download> Download</b-button>
				</b-overlay>
				<b-button variant="primary" v-if="(userRole != null ? userRole.allowDownload : false)" class="float-right" href="/files/PurchaseRequest/PurchaseRequest.xlsx" size="sm"><b-icon-wallet-fill class="mr-1"></b-icon-wallet-fill> Download Template</b-button>
			</b-col>
		 </b-row>
		<b-card class="mt-3 mb-3">
		<b-row class="mb-3">
			<b-col>
				<b-overlay
					v-if="(userRole != null ? userRole.allowCreate : false)"
					:show="isCreateBusy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="d-inline-block"
					>
					<b-button variant="success" v-if="(userRole != null ? userRole.allowCreate : false)" @click="createData" size="sm"><b-icon-pencil class="mr-1"></b-icon-pencil> Create</b-button>
				</b-overlay>
				<b-overlay
					v-if="(userRole != null ? userRole.allowPrint : false)"
					:show="isMultiplePrintBusy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="d-inline-block"
					>
					<b-button variant="primary" v-if="(userRole != null ? userRole.allowPrint : false)" class="ml-2" @click="printData" size="sm"><b-icon-printer-fill class="mr-1"></b-icon-printer-fill> Multiple Print</b-button>
				</b-overlay>
			</b-col>
			<b-col>
				<span class="float-right">
					<b-form-select v-model="pageSizeOptions" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
				</span>
			</b-col>
		</b-row>
		<div class="table-corner">
			<b-table id="my-table" stacked="md" head-variant="light"  :no-local-sorting="true" :items="purchaseRequests" :fields="fields" :busy="isDataLoading" :sort-by.sync="sortBy" :sort-desc.sync="sortDesc" :tbody-tr-class="rowClass" small responsive>
			<template v-slot:head(actions)>
				<b-button-group class="float-right"  size="sm">
					<b-button @click="isShowFilter = !isShowFilter" size="sm"><b-icon-funnel-fill /></b-button>
					<b-button><b-form-checkbox v-if="(userRole != null ? userRole.allowPrint : false)" id="input-select-all" class="float-right" v-model="selectAll"></b-form-checkbox></b-button>
				</b-button-group>
			</template>
			<template #top-row v-if="isShowFilter">
				<b-th></b-th>
				<b-th stacked-heading="PR Date">
					<b-form-datepicker boundary="viewport" placeholder="" id="input-pr-date-from-table" reset-button @input="getPurchaseRequestsWithoutLoading" v-model="filter.prDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB"></b-form-datepicker>
					<b-form-datepicker boundary="viewport" placeholder="" id="input-pr-date-to-table" reset-button @input="getPurchaseRequestsWithoutLoading" v-model="filter.prDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB"></b-form-datepicker>
				</b-th>
				<b-th stacked-heading="PR Number">
					<b-form-input id="input-pr-no-table" @input="getPurchaseRequestsWithoutLoading" v-model="filter.prNo" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Remarks">
					<b-form-input id="input-remarks-table" @input="getPurchaseRequestsWithoutLoading" v-model="filter.remarks" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Select All">
					<b-form-checkbox v-if="(userRole != null ? userRole.allowPrint : false)" id="input-select-all" class="float-right d-block d-md-none" v-model="selectAll"></b-form-checkbox>
				</b-th>
			</template>
			<template v-slot:cell(no)="row">
				{{ ((row.index + 1) + ( pageSizeOptions * (currentPage - 1))) }}
			</template>
			<template v-slot:cell(prDate)="row">
				{{ row.item.prDate | moment("DD-MMM-YYYY") }}
			</template>
			<template v-slot:cell(actions)="row" >
				<div class="div-actions float-right">
				<b-button size="sm" v-if="(userRole != null ? userRole.allowUpdate : false)" @click="editData(row.index, row.item.id)" variant ="success"  class="mr-2 btn-xs">
					<b-icon-pencil></b-icon-pencil>
				</b-button>
				<b-overlay
					v-if="(userRole != null ? userRole.allowDelete : false)"
					:show="busy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="d-inline-block"
					>
					<click-confirm>
					<b-button size="sm" variant="danger" @click="deleteData(row.item.id)" class="mr-2 btn-xs">
						<b-icon-trash></b-icon-trash>
					</b-button>
					</click-confirm>
				</b-overlay>
				<b-button size="sm" v-if="(userRole != null ? userRole.allowRead : false)" variant="primary" class="mr-2 mt-md-2 mt-lg-0 btn-xs" @click="showTableDetail(row.item.id)" >
					<b-icon-eye-fill></b-icon-eye-fill>
				</b-button>
				<b-button size="sm" v-if="(userRole != null ? userRole.allowPrint : false)" variant="light"  @click="printSingleData(row.item.id)" class="mr-2 mt-md-2 mt-lg-0 btn-xs">
					<b-icon-printer-fill></b-icon-printer-fill>
				</b-button>
				<b-form-checkbox v-if="(userRole != null ? userRole.allowPrint : false)" class="d-inline" :id="'input-is-print-' + (row.index + 1)" @change="toggleIsPrint(Number(row.item.id))" :checked="checkAllData.includes(Number(row.item.id))" ></b-form-checkbox>
				</div>
			</template>
			</b-table>
		</div>
		<b-row>
			<b-col cols="12" md="6" lg="6">
				<b-overlay
					v-if="(userRole != null ? userRole.allowCreate : false)"
					:show="isCreateBusy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="d-inline-block"
					>
					<b-button variant="success" v-if="(userRole != null ? userRole.allowCreate : false)" @click="createData" size="sm"><b-icon-pencil class="mr-1"></b-icon-pencil> Create</b-button>
				</b-overlay>
				<b-overlay
					v-if="(userRole != null ? userRole.allowPrint : false)"
					:show="isMultiplePrintBusy"
					rounded
					opacity="0.6"
					spinner-small
					spinner-variant="primary"
					class="d-inline-block"
					>
					<b-button variant="primary" v-if="(userRole != null ? userRole.allowPrint : false)" class="ml-2" @click="printData" size="sm"><b-icon-printer-fill class="mr-1"></b-icon-printer-fill> Multiple Print</b-button>
				</b-overlay>
			</b-col>
			<b-col cols="12" md="6" lg="6">
			<b-pagination
				class="float-md-right btn-paging"
				v-model="currentPage"
				:total-rows="(purchaseRequestsPaging != null ? purchaseRequestsPaging.dataCount : 0)"
				:per-page="(purchaseRequestsPaging != null ? purchaseRequestsPaging.pageSize : 0)"
				aria-controls="my-table"
			></b-pagination>
			</b-col>
		</b-row>
		</b-card>
		<Upload
		:openPopup="showPopupUpload"
		@resetMethod="resetModalUpload"
		@uploadButtonMethod="uploadData"
		/>
		<PopupDownload
		:openPopup="showPopupDownload"
		:processId="processId"
		@resetModalMethod="resetModal"
		label= "Popup Download"
		:contentLabel = "popupDownloadLabel"
		/>
		<PopupPDF
		:openPopup="isShowPopupPDF"
		:pdfUrl="pdfUrl"
		@resetModalMethod="resetModal"
		label= "Popup PDF"
		/>
		<PopupDataEditNotification
		entityName ="purchaserequest"
		:dataId ="currentDataId"
		@resetModalMethod ="resetModal"
		label = "Data Recovery"
		/>
	<b-card class="mb-3" header-bg-variant="transparent" v-if="isShowTablePurchaseRequestDetail">
		<template #header>
			<b-row>
				<b-col cols="9"><h5 class="card-title font-weight-bolder text-dark">Purchase Request Details</h5></b-col>
				<b-col>
					<span class="float-right">
						<button class="close" no-variant @click="isShowTablePurchaseRequestDetail = false">×</button>
					</span>
				</b-col>
			</b-row>
		</template>
		<b-row class="mb-2">
			<b-col>
				<span class="float-right">
					<b-form-select v-model="purchaseRequestDetailPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
				</span>
			</b-col>
		</b-row>
		<b-row>
		<b-col>
			<div class="table-corner table-row-selected">
			<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseRequestDetails" :fields="fieldsPurchaseRequestDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseRequestDetail" :sort-desc.sync="sortDescPurchaseRequestDetail" small responsive>
			<!--APP_TABLE_FILTER-->
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseRequestDetailFilter = !isShowPurchaseRequestDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseRequestDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseRequestDetail.partId" :reduce="item => item.id" v-model="filterPurchaseRequestDetail.partId" @search="getParts" :filterable="true"  @focus="getParts"></v-select>
				</b-th>
				<b-th stacked-heading="Qty">
					<b-form-input id="input-qty-table" v-model="filterPurchaseRequestDetail.qty" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Request Date">
					<b-form-datepicker boundary="viewport" placeholder="" id="input-request-date-from-table" v-model="filterPurchaseRequestDetail.requestDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
					<b-form-datepicker boundary="viewport" placeholder="" id="input-request-date-to-table" v-model="filterPurchaseRequestDetail.requestDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button></b-form-datepicker>
				</b-th>
				<b-th></b-th>
			</template>
			<!--END_APP_TABLE_FILTER-->
			<!--APP_TABLE_DATA-->
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
			<!--END_APP_TABLE_DATA-->
			</b-table>
			</div>
		</b-col>
		<b-col cols="12">
			<!--APP_TABLE_PAGINATION-->
			<b-pagination
			class="float-right btn-paging"
			v-model="currentPurchaseRequestDetailPage"
			:total-rows="totalPurchaseRequestDetailRow"
			:per-page="purchaseRequestDetailPageSize"
			aria-controls="my-table"
			></b-pagination>
			<!--END_APP_TABLE_PAGINATION-->
		</b-col>
		</b-row>
	</b-card>
		<RouterView />
	</div>
</template>
<style lang="scss" scoped>
	@import '../../assets/scss/index-page.scss';
</style>
<script>
import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';
import PurchaseRequestList from '@/models/PurchaseRequest/PurchaseRequestList';
import PurchaseRequestFilter from '@/filters/PurchaseRequest/PurchaseRequestFilter';
import RoleDetail from '@/models/Core/RoleDetail';

import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppInputDatepicker from '@/components/AppInputDatepicker';
import AppInputDatepickerRange from '@/components/AppInputDatepickerRange';
import AppInputTextbox from '@/components/AppInputTextbox';
import AppInputTextarea from '@/components/AppInputTextarea';
import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';
import PopupDownload from '@/components/PopupDownload';
import PopupPDF from '@/components/PopupPDF';
import PopupDataEditNotification from '@/components/PopupDataEditNotification';
import Upload from './Upload';

export default {
	components : {PurchaseRequest, PurchaseRequestList, PurchaseRequestFilter, AppInputDatepicker,AppInputDatepickerRange,AppInputTextbox,AppInputTextarea,PopupDownload,PopupPDF,Upload,PopupDataEditNotification,},
	data() {
		return {
			isShowTableDetail: false,
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
			fields: [
				{"no" : { "class" : "th-number"}}, 
				{"prDate": {}, "class" : "th-pr-date", "label" : "PR Date", key: 'prDate', sortable: true}, 
				{"prNo": {}, "class" : "th-pr-no", "label" : "PR Number", key: 'prNo', sortable: true}, 
				{"remarks": {}, "class" : "th-remarks", "label" : "Remarks", key: 'remarks', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : ""}}
			],
			busy:false,
			isDataLoading: false,
			isDownloadDataBusy: false,
			isMultiplePrintBusy: false,
			showPopupUpload: false,
			showPopupDownload: false,
			isShowPopupPDF: false,
			pdfUrl: "",
			selected:'',
			pageSizeOptions: 10,
			pageIndex:0,
			sortingBy: Array(),
			filter: null,
			processId: "",
			popupDownloadLabel: "",
			hasLoadedData: false,
			showFilter: false,
			isShowFilter: false,
			isCreateBusy: false,
			btnEditData: [],
			currentDataId: null,
			modelId: "",
			purchaseRequests: [],
			purchaseRequestsPaging: {},
			fieldsPurchaseRequestDetail: [
				{"no" : {  "class" : "th-number" }},
				{"part": {},  "class" : "th-part", "label" : "Part", key: 'part', sortable: true}, 
				{"qty": {},  "class" : "th-qty", "label" : "Qty", key: 'qty', sortable: true}, 
				{"requestDate": {},  "class" : "th-request-date", "label" : "Request Date", key: 'requestDate', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
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
			isShowTablePurchaseRequestDetail: false,
			parts: [],
		}
	},
	methods : {
		getPurchaseRequests : async function() {
			this.hideAllTableDetail();
			this.modelId = '';
			this.resetChildFilter();
			this.isDataLoading = true;
			this.sortingBy = Array();
			if (this.sortBy != null) {
				this.sortingBy[this.sortBy] = this.sortDesc;
			}
			PurchaseRequest.getList(this, this.filter, this.sortingBy, (this.currentPage - 1), this.pageSizeOptions)
			.then(result => {
				if (result != null) {
					this.purchaseRequests = result.data
					this.purchaseRequestsPaging = result
				}
				this.isDataLoading = false;
			})
			.catch(error => {
				this.isDataLoading = false;
			});
		},
		getPurchaseRequestsWithoutLoading : async function() {
			this.hideAllTableDetail();
			this.modelId = '';
			this.resetChildFilter();
			this.sortingBy = Array();
			if (this.sortBy != null) {
				this.sortingBy[this.sortBy] = this.sortDesc;
			}
			PurchaseRequest.getList(this, this.filter, this.sortingBy, (this.currentPage - 1), this.pageSizeOptions)
			.then(result => {
				if (result != null) {
					this.purchaseRequests = result.data
					this.purchaseRequestsPaging = result
				}
			})
			.catch(error => {});
		},
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
		/*APP_DETAIL_GETPurchaseRequestDetail*/
		getPurchaseRequestDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseRequestDetail);
			if (this.modelId == null || this.modelId == undefined || this.modelId == '') return;
			if (!this.isShowTablePurchaseRequestDetail) return;
			filter.purchaseRequestId = this.modelId;
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
		/*END_APP_DETAIL_GETPurchaseRequestDetail*/
		resetFilter() {
			this.filter = new PurchaseRequestFilter();
		},
		showPopupUploadData() {
			this.showPopupUpload = true;
		},
		uploadData(modalEvent) {
			this.showPopupUpload = false;
		},
		resetModal() {
			this.showPopupUpload = false;
			this.showPopupDownload = false;
			this.isShowPopupPDF = false;
		},
		resetModalUpload() {
			this.showPopupUpload = false;
			this.getPurchaseRequests();
		},
		toggleIsPrint(id) {
			if (this.checkAllData.includes(id)) {
				this.$store.dispatch("removeCheckData", id);
			} else {
				this.$store.dispatch("addCheckData", id);
			}
		},
		showPopupPrint(id) {
			this.showPopupDownload = true;
			this.popupDownloadLabel = "Mempersiapkan data untuk di cetak";
			this.processId = id;
		},
		showPopupDownloadData(id) {
			this.showPopupDownload = true;
			this.popupDownloadLabel = "Mempersiapkan data untuk di unduh";
			this.processId = id;
		},
		showPopupPDF(url) {
			this.isShowPopupPDF = true;
			this.pdfUrl = url;
		},
		printData() {
			let data = this.$store.state.dataIds;
			if (data.length > 0) {
				this.isMultiplePrintBusy = true;
				PurchaseRequest.multiPagePrint(this).then(result => {
					if (result) {
						this.showPopupPrint(result.id);
					}
					this.isMultiplePrintBusy = false;
				}).catch(error => { this.isMultiplePrintBusy = false; });
			}
		},
		printSingleData(id) {
			PurchaseRequest.singleDataPagePrint(this, id).then(result => {
				if (result) {
					this.showPopupPDF(result.download);
				}
			});
		},
		downloadData() {
			if (this.sortBy != null) {
					this.sortingBy[this.sortBy] = this.sortDesc;
			}
			this.isDownloadDataBusy = true;
			PurchaseRequest.downloadData(this, this.filter, this.sortingBy).then(result => {
				if (result) {
					this.showPopupDownloadData(result.id);
				}
				this.isDownloadDataBusy = false;
			}).catch(error => { this.isDownloadDataBusy = false; })
		},
		createData() {
			this.isCreateBusy = true;
			PurchaseRequest.createData(this).then(result => {
				if (result != null) {
					if (result.id != null && result.id != "") {
						this.$router.push({ name: 'purchaserequestedit', params : { id: result.id } });
					}
				}
				this.isCreateBusy = false;
			}).catch(error => {
				this.isCreateBusy = false;
			})
		},
		editData(index, id) {
			this.btnEditData[index] = true;
			this.dataId = id;
			PurchaseRequest.editData(this, id).then(result => {
				if (result != null) {
					if (result.id != null && result.id != "") {
						this.$router.push({ name: 'purchaserequestedit', params : { id: result.id } });
					}
				}
				this.btnEditData[index] = false;
			}).catch(error => { this.btnEditData[index] = false; })
		},
		deleteData(id) {
			PurchaseRequest.deleteData(this, id).then(result => {
				if (result)
				{
					this.getPurchaseRequests();
				}
			})
		},
		showTableDetail(id) {
			this.showAllTableDetail();
			this.modelId = id;
			this.getPurchaseRequestDetails()
		},
		rowClass(item, type) {
			if (!item || type !== 'row') return;
			if (item.id === this.modelId) {
			if (this.isShowTablePurchaseRequestDetail) {
				return 'table-row-selected';
			}
			}
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768) {
				this.isShowFilter = true;
			} else {
				this.isShowFilter = this.showFilter;
			}
		},
		resetChildFilter() {
			this.filterPurchaseRequestDetail = new PurchaseRequestDetailFilter();
		},
		showAllTableDetail() {
			this.isShowTablePurchaseRequestDetail = true;
		},
		hideAllTableDetail() {
			this.isShowTablePurchaseRequestDetail = false;
		},
	},
	beforeMount() {
		this.$store.dispatch("removeCheckAllData");
		this.filter = new PurchaseRequestFilter();
		this.filterPurchaseRequestDetail = new PurchaseRequestDetailFilter();
		this.getParts();
	},
	mounted(){
		let breadcrumb =[
			"Master Data", 
			"Purchase Request"
		];
		this.getPurchaseRequests();
		this.$store.dispatch('setBreadCrumb', breadcrumb);
		this.$nextTick(() => {
			window.addEventListener('resize', this.displayFilter);
		});
	},
	watch: {
		sortBy: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		sortDesc: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		currentPage: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		pageSizeOptions: {
			handler: function() {
				this.getPurchaseRequests();
			}
		},
		selectAll(newValue) {
			if (newValue) {
				PurchaseRequest.checkAllData(this, this.filter, this.sortingBy);
			} else {
				this.$store.dispatch("removeCheckAllData");
			}
		},
		'$route'(to, from) {
			if (to != null) {
				if (to.name == "purchaserequest") {
					this.getPurchaseRequestsWithoutLoading();
				}
			}
		},
		modelId: {
			handler: function() {
				this.resetChildFilter();
			}
		},
		/*APP_DETAIL_WATCH_PURCHASEREQUESTDETAIL*/
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
		computedFilterPurchaseRequestDetail: {
			handler: function(newValue, oldValue) {
				if (JSON.stringify(newValue) !== JSON.stringify(oldValue)) {
					this.getPurchaseRequestDetails();
				}
			},
			deep: true,
		},
		/*END_APP_DETAIL_WATCH_PURCHASEREQUESTDETAIL*/
	},
	computed: {
		/*APP_DETAIL_COMPUTED_PURCHASEREQUESTDETAIL*/
		computedFilterPurchaseRequestDetail(){
			return Object.assign({}, this.filterPurchaseRequestDetail);
		},
		/*END_APP_DETAIL_COMPUTED_PURCHASEREQUESTDETAIL*/
		checkAllData() {
			return this.$store.state.dataIds;
		},
		userRole() {
			return RoleDetail.query().where("functionInfoId", "purchase_request").first();
		}
	},
	beforeDestroy() {
		this.$store.dispatch("removeCheckAllData");
	},
}
</script>
