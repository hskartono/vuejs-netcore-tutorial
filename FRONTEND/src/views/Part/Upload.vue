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
							<b-tab title="Part" active>
								<b-row class="mb-2">
									<b-col>
										<span class="float-right">
											<b-form-select v-model="partPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
										</span>
									</b-col>
								</b-row>
								<b-row>
								<b-col>
								<div class="table-corner">
									<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="parts" :fields="fieldsPart" :busy="isDataLoading" :sort-by.sync="sortByPart" :sort-desc.sync="sortDescPart" small responsive>
									<template v-slot:head(actions)>
										<b-button @click="isShowPartFilter = !isShowPartFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowPartFilter">
										<b-th></b-th>
										<b-th stacked-heading="ID">
											<b-form-input id="input-id-table" v-model="filterPart.id" size="sm"></b-form-input>
										</b-th>
										<b-th stacked-heading="Part Name">
											<b-form-input id="input-part-name-table" v-model="filterPart.partName" size="sm"></b-form-input>
										</b-th>
										<b-th stacked-heading="Description">
											<b-form-input id="input-description-table" v-model="filterPart.description" size="sm"></b-form-input>
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
										{{ ((row.index + 1) + ( partPageSize * (currentPartPage - 1))) }}
									</template>
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentPartPage"
									:total-rows="totalPartRow"
									:per-page="partPageSize"
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

import Part from '@/models/Part/Part';
import PartList from '@/models/Part/PartList';
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
			fieldsPart: [
				{"no" : { "class" : "th-number"}}, 
				{"partName": { "class" : "th-part-name", "label" : "Part Name"}, key: 'partName', sortable: true}, 
				{"description": { "class" : "th-description", "label" : "Description"}, key: 'description', sortable: true}, 
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
			parts : [],
			partId : '',
			totalPartRow : 0,
			isShowPartFilter: false,
			filterPart : null,
			currentPartPage : 1,
			partPageSize: 10,
			sortByPart: '',
			sortDescPart: '',
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
				this.$http.post(process.env.VUE_APP_API_URL + 'part/confirmUpload',
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
				this.$http.post(process.env.VUE_APP_API_URL + 'part/upload',
					data,
					{ headers: { 'Content-Type': 'multipart/form-data' } }
					).then(async function(response){
						_this.isBusy = false;
						_this.isUploadSuccess = true;
						_this.getParts();
					})
					.catch(error => {
						_this.$store.dispatch('addErrorMessageGlobal', error);
						_this.isBusy = false;
					});
				}
			}
		},
		getParts : async function() {
			let filter = {};
			Object.assign(filter, this.filterPart);
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
			this.sortingByPart = Array();
			if (this.sortByPart != null) {
				this.sortingByPart[this.sortByPart] = this.sortDescPart;
			}
			let currentPage = (this.currentPartPage - 1);
			Part.getList(this, filter, this.sortingByPart, currentPage , this.partPageSize).then(result => {
				this.parts = result.data;
				this.totalPartRow = result.dataCount;
			}).catch(error => {});
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
			}
		},
		downloadLog() {
			let _this = this;
			this.$http.post(process.env.VUE_APP_API_URL + 'part/downloadlog', [],
			{ headers: { 'Content-Type': 'application/json' }}
			).then(async function(response){
				await saveAs(process.env.VUE_APP_API_URL + 'part/downloadlog', "Part.xslx");
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
		sortByPart : {
			handler: function() {
				this.getParts();
			}
		},
		sortDescPart: {
			handler: function() {
				this.getParts();
			}
		},
		currentPartPage: {
			handler: function() {
				this.getParts();
			}
		},
		partPageSize : {
			handler: function() {
				this.getParts();
			}
		},
		filterPart: {
			handler: function() {
				this.getParts();
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
					this.filterPart = new PartFilter();
					return true;
				}
				else return false;
			},
			set: function (newValue) {}
		},
	}
}
</script>
