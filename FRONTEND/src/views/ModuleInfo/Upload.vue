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
							<b-tab title="Module Info" active>
								<b-row class="mb-2">
									<b-col>
										<span class="float-right">
											<b-form-select v-model="moduleInfoPageSize" size="sm" class="float-right" :options="pagingOptions"></b-form-select>
										</span>
									</b-col>
								</b-row>
								<b-row>
								<b-col>
								<div class="table-corner">
									<b-table id="my-table" stacked="md" head-variant="light" :no-local-sorting="true" :items="moduleInfos" :fields="fieldsModuleInfo" :busy="isDataLoading" :sort-by.sync="sortByModuleInfo" :sort-desc.sync="sortDescModuleInfo" small responsive>
									<template v-slot:head(actions)>
										<b-button @click="isShowModuleInfoFilter = !isShowModuleInfoFilter" class="float-right" size="sm"><b-icon-funnel-fill /></b-button>
									</template>
									<template #top-row v-if="isShowModuleInfoFilter">
										<b-th></b-th>
										<b-th stacked-heading="Name">
											<b-form-input id="input-name-table" v-model="filterModuleInfo.name" size="sm"></b-form-input>
										</b-th>
										<b-th stacked-heading="Parent Module">
											<b-form-input id="input-parent-module-id-table" v-model="filterModuleInfo.parentModuleId" size="sm"></b-form-input>
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
										{{ ((row.index + 1) + ( moduleInfoPageSize * (currentModuleInfoPage - 1))) }}
									</template>
									</b-table>
									</div>
								</b-col>
								<b-col cols="12">
									<b-pagination
									class="float-right btn-paging"
									v-model="currentModuleInfoPage"
									:total-rows="totalModuleInfoRow"
									:per-page="moduleInfoPageSize"
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

import ModuleInfo from '@/models/ModuleInfo/ModuleInfo';
import ModuleInfoList from '@/models/ModuleInfo/ModuleInfoList';
import ModuleInfoFilter from '@/filters/ModuleInfo/ModuleInfoFilter';

import AppInputFileupload from '@/components/AppInputFileupload';
export default {
	props: ['model', 'openPopup','uploadButtonMethod', 'parentId'],
	components : {AppInputFileupload,},
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
			fieldsModuleInfo: [
				{"no" : { "class" : "th-number"}}, 
				{"name": { "class" : "th-name", "label" : "Name"}, key: 'name', sortable: true}, 
				{"parentModuleId": { "class" : "th-parent-module-id", "label" : "Parent Module"}, key: 'parentModuleId', sortable: true}, 
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
			moduleInfos : [],
			moduleInfoId : '',
			totalModuleInfoRow : 0,
			isShowModuleInfoFilter: false,
			filterModuleInfo : null,
			currentModuleInfoPage : 1,
			moduleInfoPageSize: 10,
			sortByModuleInfo: '',
			sortDescModuleInfo: '',
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
				this.$http.post(process.env.VUE_APP_API_URL + 'moduleinfo/confirmUpload',
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
				this.$http.post(process.env.VUE_APP_API_URL + 'moduleinfo/upload',
					data,
					{ headers: { 'Content-Type': 'multipart/form-data' } }
					).then(async function(response){
						_this.isBusy = false;
						_this.isUploadSuccess = true;
						_this.getModuleInfos();
					})
					.catch(error => {
						_this.$store.dispatch('addErrorMessageGlobal', error);
						_this.isBusy = false;
					});
				}
			}
		},
		getModuleInfos : async function() {
			let filter = this.filterModuleInfo;
			filter.draftFromUpload = "1";
			filter.draftMode = "1";
			this.sortingByModuleInfo = Array();
			if (this.sortByModuleInfo != null) {
				this.sortingByModuleInfo[this.sortByModuleInfo] = this.sortDescModuleInfo;
			}
			let currentPage = (this.currentModuleInfoPage - 1);
			ModuleInfo.getList(this, filter, this.sortingByModuleInfo, currentPage , this.moduleInfoPageSize).then(result => {
				this.moduleInfos = result.data;
				this.totalModuleInfoRow = result.dataCount;
			}).catch(error => {});
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
			}
		},
		downloadLog() {
			let _this = this;
			this.$http.post(process.env.VUE_APP_API_URL + 'moduleinfo/downloadlog', [],
			{ headers: { 'Content-Type': 'application/json' }}
			).then(async function(response){
				await saveAs(process.env.VUE_APP_API_URL + 'moduleinfo/downloadlog', "ModuleInfo.xslx");
				_this.isBusy = false;
			})
			.catch(error => {
				_this.$store.dispatch('addErrorMessageGlobal', error);
				_this.isBusy = false;
			});
		}
	},
	beforeMount() {
		this.filterModuleInfo = new ModuleInfoFilter();
	},
	mounted(){
	},
	watch: {
		sortByModuleInfo : {
			handler: function() {
				this.getModuleInfos();
			}
		},
		sortDescModuleInfo: {
			handler: function() {
				this.getModuleInfos();
			}
		},
		currentModuleInfoPage: {
			handler: function() {
				this.getModuleInfos();
			}
		},
		moduleInfoPageSize : {
			handler: function() {
				this.getModuleInfos();
			}
		},
		filterModuleInfo: {
			handler: function() {
				this.getModuleInfos();
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
			if (this.openPopup) return true;
			else return false;
			},
			set: function (newValue) {}
		},
	}
}
</script>
