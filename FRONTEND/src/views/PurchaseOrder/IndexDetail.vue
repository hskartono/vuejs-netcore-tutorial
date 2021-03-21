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
					<app-input-textbox id="fieldset-po-number" label-cols-sm="4" label-cols-lg="3" label="PO Number" label-for="filter-po-number" size="sm" :model.sync="filter.poNumber" />
					<app-input-datepicker-range  id="fieldset-po-date" label-cols-sm="4" label-cols-lg="3" label="PO Date" size="sm" :modelFrom.sync="filter.poDateFrom" :modelTo.sync="filter.poDateTo" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" reset-button />
					<app-input-textarea id="fieldset-remarks" label-cols-sm="4" label-cols-lg="3" label="Remarks" label-for="filter-remarks" size="sm" :model.sync="filter.remarksMultiple" />
					<b-button variant="danger" size="sm" class="float-right" @click="resetFilter"><span style="font-size: 0.75rem" class="mr-1"><b-icon-x></b-icon-x></span> Clear Filter</b-button>
					<b-button variant="primary" size="sm" class="float-right mr-1" @click="getPurchaseOrders"><span style="font-size: 0.75rem" class="mr-1"><b-icon-search></b-icon-search></span> Cari</b-button>
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
				<b-button variant="primary" v-if="(userRole != null ? userRole.allowDownload : false)" class="float-right" href="/files/PurchaseOrder/PurchaseOrder.xlsx" size="sm"><b-icon-wallet-fill class="mr-1"></b-icon-wallet-fill> Download Template</b-button>
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
			<b-table id="my-table" stacked="md" head-variant="light"  :no-local-sorting="true" :items="purchaseOrders" :fields="fields" :busy="isDataLoading" :sort-by.sync="sortBy" :sort-desc.sync="sortDesc" :tbody-tr-class="rowClass" small responsive>
			<template v-slot:head(actions)>
				<b-button-group class="float-right"  size="sm">
					<b-button @click="isShowFilter = !isShowFilter" size="sm"><b-icon-funnel-fill /></b-button>
					<b-button><b-form-checkbox v-if="(userRole != null ? userRole.allowPrint : false)" id="input-select-all" class="float-right" v-model="selectAll"></b-form-checkbox></b-button>
				</b-button-group>
			</template>
			<template #top-row v-if="isShowFilter">
				<b-th></b-th>
				<b-th stacked-heading="PO Number">
					<b-form-input id="input-po-number-table" @input="getPurchaseOrdersWithoutLoading" v-model="filter.poNumber" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="PO Date">
					<b-form-datepicker boundary="viewport" placeholder="" id="input-po-date-from-table" reset-button @input="getPurchaseOrdersWithoutLoading" v-model="filter.poDateFrom" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB"></b-form-datepicker>
					<b-form-datepicker boundary="viewport" placeholder="" id="input-po-date-to-table" reset-button @input="getPurchaseOrdersWithoutLoading" v-model="filter.poDateTo" size="sm" :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB"></b-form-datepicker>
				</b-th>
				<b-th stacked-heading="Remarks">
					<b-form-input id="input-remarks-table" @input="getPurchaseOrdersWithoutLoading" v-model="filter.remarks" size="sm"></b-form-input>
				</b-th>
				<b-th stacked-heading="Select All">
					<b-form-checkbox v-if="(userRole != null ? userRole.allowPrint : false)" id="input-select-all" class="float-right d-block d-md-none" v-model="selectAll"></b-form-checkbox>
				</b-th>
			</template>
			<template v-slot:cell(no)="row">
				{{ ((row.index + 1) + ( pageSizeOptions * (currentPage - 1))) }}
			</template>
			<template v-slot:cell(poDate)="row">
				{{ row.item.poDate | moment("DD-MMM-YYYY") }}
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
				:total-rows="(purchaseOrdersPaging != null ? purchaseOrdersPaging.dataCount : 0)"
				:per-page="(purchaseOrdersPaging != null ? purchaseOrdersPaging.pageSize : 0)"
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
		entityName ="purchaseorder"
		:dataId ="currentDataId"
		@resetModalMethod ="resetModal"
		label = "Data Recovery"
		/>
	<b-card class="mb-3" header-bg-variant="transparent" v-if="isShowTablePurchaseOrderDetail">
		<template #header>
			<b-row>
				<b-col cols="9"><h5 class="card-title font-weight-bolder text-dark">Purchase Order Details</h5></b-col>
				<b-col>
					<span class="float-right">
						<button class="close" no-variant @click="isShowTablePurchaseOrderDetail = false">×</button>
					</span>
				</b-col>
			</b-row>
		</template>
		<b-row class="mb-2">
			<b-col>
				<span class="float-right">
					<b-form-select v-model="purchaseOrderDetailPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
				</span>
			</b-col>
		</b-row>
		<b-row>
		<b-col>
			<div class="table-corner table-row-selected">
			<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="purchaseOrderDetails" :fields="fieldsPurchaseOrderDetail" :busy="isDataLoading" :sort-by.sync="sortByPurchaseOrderDetail" :sort-desc.sync="sortDescPurchaseOrderDetail" small responsive>
			<!--APP_TABLE_FILTER-->
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseOrderDetailFilter = !isShowPurchaseOrderDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseOrderDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseOrderDetail.partId" :reduce="item => item.id" v-model="filterPurchaseOrderDetail.partId" @search="getParts" :filterable="true"  @focus="getParts"></v-select>
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
		<RouterView />
	</div>
</template>
<style lang="scss" scoped>
	@import '../../assets/scss/index-page.scss';
</style>
<script>
import PurchaseOrder from '@/models/PurchaseOrder/PurchaseOrder';
import PurchaseOrderList from '@/models/PurchaseOrder/PurchaseOrderList';
import PurchaseOrderFilter from '@/filters/PurchaseOrder/PurchaseOrderFilter';
import RoleDetail from '@/models/Core/RoleDetail';

import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppInputTextbox from '@/components/AppInputTextbox';
import AppInputDatepicker from '@/components/AppInputDatepicker';
import AppInputDatepickerRange from '@/components/AppInputDatepickerRange';
import AppInputTextarea from '@/components/AppInputTextarea';
import PurchaseOrderDetail from '@/models/PurchaseOrder/PurchaseOrderDetail';
import PurchaseOrderDetailFilter from '@/filters/PurchaseOrder/PurchaseOrderDetailFilter';
import PopupDownload from '@/components/PopupDownload';
import PopupPDF from '@/components/PopupPDF';
import PopupDataEditNotification from '@/components/PopupDataEditNotification';
import Upload from './Upload';

export default {
	components : {PurchaseOrder, PurchaseOrderList, PurchaseOrderFilter, AppInputTextbox,AppInputDatepicker,AppInputDatepickerRange,AppInputTextarea,PopupDownload,PopupPDF,Upload,PopupDataEditNotification,},
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
				{"poNumber": {}, "class" : "th-po-number", "label" : "PO Number", key: 'poNumber', sortable: true}, 
				{"poDate": {}, "class" : "th-po-date", "label" : "PO Date", key: 'poDate', sortable: true}, 
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
			purchaseOrders: [],
			purchaseOrdersPaging: {},
			fieldsPurchaseOrderDetail: [
				{"no" : {  "class" : "th-number" }},
				{"part": {},  "class" : "th-part", "label" : "Part", key: 'part', sortable: true}, 
				{"partPrice": {},  "class" : "th-part-price", "label" : "Part Price", key: 'partPrice', sortable: true}, 
				{"qty": {},  "class" : "th-qty", "label" : "Qty", key: 'qty', sortable: true}, 
				{"totalPrice": {},  "class" : "th-total-price", "label" : "Total Price", key: 'totalPrice', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
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
			isShowTablePurchaseOrderDetail: false,
			parts: [],
		}
	},
	methods : {
		getPurchaseOrders : async function() {
			this.hideAllTableDetail();
			this.modelId = '';
			this.resetChildFilter();
			this.isDataLoading = true;
			this.sortingBy = Array();
			if (this.sortBy != null) {
				this.sortingBy[this.sortBy] = this.sortDesc;
			}
			PurchaseOrder.getList(this, this.filter, this.sortingBy, (this.currentPage - 1), this.pageSizeOptions)
			.then(result => {
				if (result != null) {
					this.purchaseOrders = result.data
					this.purchaseOrdersPaging = result
				}
				this.isDataLoading = false;
			})
			.catch(error => {
				this.isDataLoading = false;
			});
		},
		getPurchaseOrdersWithoutLoading : async function() {
			this.hideAllTableDetail();
			this.modelId = '';
			this.resetChildFilter();
			this.sortingBy = Array();
			if (this.sortBy != null) {
				this.sortingBy[this.sortBy] = this.sortDesc;
			}
			PurchaseOrder.getList(this, this.filter, this.sortingBy, (this.currentPage - 1), this.pageSizeOptions)
			.then(result => {
				if (result != null) {
					this.purchaseOrders = result.data
					this.purchaseOrdersPaging = result
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
		/*APP_DETAIL_GETPurchaseOrderDetail*/
		getPurchaseOrderDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseOrderDetail);
			if (this.modelId == null || this.modelId == undefined || this.modelId == '') return;
			if (!this.isShowTablePurchaseOrderDetail) return;
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
		resetFilter() {
			this.filter = new PurchaseOrderFilter();
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
			this.getPurchaseOrders();
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
				PurchaseOrder.multiPagePrint(this).then(result => {
					if (result) {
						this.showPopupPrint(result.id);
					}
					this.isMultiplePrintBusy = false;
				}).catch(error => { this.isMultiplePrintBusy = false; });
			}
		},
		printSingleData(id) {
			PurchaseOrder.singleDataPagePrint(this, id).then(result => {
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
			PurchaseOrder.downloadData(this, this.filter, this.sortingBy).then(result => {
				if (result) {
					this.showPopupDownloadData(result.id);
				}
				this.isDownloadDataBusy = false;
			}).catch(error => { this.isDownloadDataBusy = false; })
		},
		createData() {
			this.isCreateBusy = true;
			PurchaseOrder.createData(this).then(result => {
				if (result != null) {
					if (result.id != null && result.id != "") {
						this.$router.push({ name: 'purchaseorderedit', params : { id: result.id } });
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
			PurchaseOrder.editData(this, id).then(result => {
				if (result != null) {
					if (result.id != null && result.id != "") {
						this.$router.push({ name: 'purchaseorderedit', params : { id: result.id } });
					}
				}
				this.btnEditData[index] = false;
			}).catch(error => { this.btnEditData[index] = false; })
		},
		deleteData(id) {
			PurchaseOrder.deleteData(this, id).then(result => {
				if (result)
				{
					this.getPurchaseOrders();
				}
			})
		},
		showTableDetail(id) {
			this.showAllTableDetail();
			this.modelId = id;
			this.getPurchaseOrderDetails()
		},
		rowClass(item, type) {
			if (!item || type !== 'row') return;
			if (item.id === this.modelId) {
			if (this.isShowTablePurchaseOrderDetail) {
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
			this.filterPurchaseOrderDetail = new PurchaseOrderDetailFilter();
		},
		showAllTableDetail() {
			this.isShowTablePurchaseOrderDetail = true;
		},
		hideAllTableDetail() {
			this.isShowTablePurchaseOrderDetail = false;
		},
	},
	beforeMount() {
		this.$store.dispatch("removeCheckAllData");
		this.filter = new PurchaseOrderFilter();
		this.filterPurchaseOrderDetail = new PurchaseOrderDetailFilter();
		this.getParts();
	},
	mounted(){
		let breadcrumb =[
			"Master Data", 
			"Purchase Order"
		];
		this.getPurchaseOrders();
		this.$store.dispatch('setBreadCrumb', breadcrumb);
		this.$nextTick(() => {
			window.addEventListener('resize', this.displayFilter);
		});
	},
	watch: {
		sortBy: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		sortDesc: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		currentPage: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		pageSizeOptions: {
			handler: function() {
				this.getPurchaseOrders();
			}
		},
		selectAll(newValue) {
			if (newValue) {
				PurchaseOrder.checkAllData(this, this.filter, this.sortingBy);
			} else {
				this.$store.dispatch("removeCheckAllData");
			}
		},
		'$route'(to, from) {
			if (to != null) {
				if (to.name == "purchaseorder") {
					this.getPurchaseOrdersWithoutLoading();
				}
			}
		},
		modelId: {
			handler: function() {
				this.resetChildFilter();
			}
		},
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
		computedFilterPurchaseOrderDetail: {
			handler: function(newValue, oldValue) {
				if (JSON.stringify(newValue) !== JSON.stringify(oldValue)) {
					this.getPurchaseOrderDetails();
				}
			},
			deep: true,
		},
		/*END_APP_DETAIL_WATCH_PURCHASEORDERDETAIL*/
	},
	computed: {
		/*APP_DETAIL_COMPUTED_PURCHASEORDERDETAIL*/
		computedFilterPurchaseOrderDetail(){
			return Object.assign({}, this.filterPurchaseOrderDetail);
		},
		/*END_APP_DETAIL_COMPUTED_PURCHASEORDERDETAIL*/
		checkAllData() {
			return this.$store.state.dataIds;
		},
		userRole() {
			return RoleDetail.query().where("functionInfoId", "purchase_order").first();
		}
	},
	beforeDestroy() {
		this.$store.dispatch("removeCheckAllData");
	},
}
</script>
