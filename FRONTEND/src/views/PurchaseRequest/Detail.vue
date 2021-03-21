<template>
	<PopupRouterViewDetail label="Purchase Request Detail" @copyDataMethod="copyDataAction">
<!-- APP_DETAIL_HEADER -->
		<b-card>
			<b-col cols="12" lg="6">
					<b-form-group id="fieldset-pr-date" label-cols-sm="4" label-cols-lg="3" label="PR Date" label-for="filter-pr-date">
						{{ (model != null ? model.prDate : "") | moment("DD-MMM-YYYY") }}
					</b-form-group>
					<b-form-group id="fieldset-pr-no" label-cols-sm="4" label-cols-lg="3" label="PR Number" label-for="filter-pr-no">
						{{ model != null ? model.prNo : "" }}
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
			<b-col cols="9"><h5 class="card-title font-weight-bolder text-dark">Purchase Request Details</h5></b-col>
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
			<!--APP_TABLE_FILTER-->
			<template v-slot:head(actions)>
				<b-button @click="isShowPurchaseRequestDetailFilter = !isShowPurchaseRequestDetailFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
			</template>
			<template #top-row v-if="isShowPurchaseRequestDetailFilter">
				<b-th></b-th>
				<b-th stacked-heading="Part">
					<v-select append-to-body label="partName" :options="parts" :value="filterPurchaseRequestDetail.partId" :reduce="item => item.id" v-model="filterPurchaseRequestDetail.partId" @search="getParts" :filterable="true"></v-select>
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
<!--END_APP_DETAIL_TABLE-->
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterViewDetail>
</template>
<script>
/*APP_DETAIL_IMPORT*/
import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';
import PurchaseRequestList from '@/models/PurchaseRequest/PurchaseRequestList';
import PurchaseRequestFilter from '@/filters/PurchaseRequest/PurchaseRequestFilter';

import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import PopupRouterViewDetail from '@/components/PopupRouterViewDetail';
import AppInputDatetimepicker from '@/components/AppInputDatetimepicker';
import AppInputDatetimepickerRange from '@/components/AppInputDatetimepickerRange';
/*END_APP_DETAIL_IMPORT*/
export default {
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
			fieldsPurchaseRequestDetail: [
				{"no" : {  "class" : "th-number" }},
				{"part": {},  "class" : "th-part", "label" : "Part", key: 'part', sortable: true}, 
				{"qty": {},  "class" : "th-qty", "label" : "Qty", key: 'qty', sortable: true}, 
				{"requestDate": {},  "class" : "th-request-date", "label" : "Request Date", key: 'requestDate', sortable: true}, 
				{"actions": { "class" : "th-actions", "label" : "" }}
			],
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			model: {},
			/*APP_DETAIL_PAGINATION*/
			pagingOptions:[
				{ value : '10', text : '10', selected:true },
				{ value : '25', text : '25' },
				{ value : '50', text : '50' },
				{ value : '100', text : '100' },
			],
			/*END_APP_DETAIL_PAGINATION*/
			/*APP_DETAIL_FILTER*/
			filterPurchaseRequest : null,
			/*END_APP_DETAIL_FILTER*/
			/*APP_DETAIL_PAGING_DEFINITION*/
			currentPurchaseRequestPage : 1,
			purchaseRequestPageSize: 10,
			/*END_APP_DETAIL_PAGING_DEFINITION*/
			/*APP_DETAIL_SORT_DEFINITION*/
			sortByPurchaseRequest: '',
			sortDescPurchaseRequest: '',
			/*END_APP_DETAIL_SORT_DEFINITION*/
			/*APP_DETAIL_VARIABLE_DEFINITION*/
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
			/*END_APP_DETAIL_VARIABLE_DEFINITION*/
			/*APP_DETAIL_OBJ_VARIABLE*/
			parts: [],
			/*END_APP_DETAIL_OBJ_VARIABLE*/
		}
	},
	methods : {
		/*APP_DETAIL_GETPurchaseRequest*/
		getPurchaseRequest : async function(id) {
			this.isBusy = true;
			PurchaseRequest.getData(this, id).then(result => { this.model = result; this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		/*END_APP_DETAIL_GETPurchaseRequest*/
		/*APP_DETAIL_GETPurchaseRequestDetail*/
		getPurchaseRequestDetails : async function() {
			let filter = {};
			Object.assign(filter, this.filterPurchaseRequestDetail);
			if (this.modelId == null) return;
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
			this.$router.push({ name: 'purchaserequestcopydata', params: { id: this.modelId, copydata : 1 } })
		},
		/*END_APP_DETAIL_COPYDATA*/
		/*APP_DETAIL_DISPLAYFILTER*/
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
				this.isShowPurchaseRequestDetailFilter = true;
			}
		},
		/*END_APP_DETAIL_DISPLAYFILTER*/
	},
	/*APP_DETAIL_BEFORE_MOUNT*/
	beforeMount() {
		this.filterPurchaseRequest = new PurchaseRequestFilter();
		this.filterPurchaseRequestDetail = new PurchaseRequestDetailFilter();
	},
	/*END_APP_DETAIL_BEFORE_MOUNT*/
	/*APP_DETAIL_MOUNTED*/
	mounted(){
		let id = this.$route.params.id;
		this.modelId = id;
		this.getPurchaseRequest(id);
		this.getPurchaseRequestDetails();
		this.$nextTick(() => {
			window.addEventListener('resize', this.displayFilter);
			this.displayFilter();
		});
	},
	/*END_APP_DETAIL_MOUNTED*/
	watch : {
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
		filterPurchaseRequestDetail: {
			handler: function() {
				this.getPurchaseRequestDetails();
			},
			deep: true,
		},
		/*END_APP_DETAIL_WATCH_PURCHASEREQUESTDETAIL*/
	},
	/*APP_DETAIL_COMPUTED*/
	computed: {
	}
	/*END_APP_DETAIL_COMPUTED*/
}
</script>
