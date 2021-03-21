<template>
	<PopupRouterView label="Module Info Input"  entityName="moduleinfo" :documentId="mainRecordId"  @saveMethod="saveActionConfirmation" @hideMethod="onHidden">
		<b-card>
			<b-col cols="12" lg="6">
				<app-input-textbox id="fieldset-name" label-cols-sm="4" label-cols-lg="3" label="Name" label-for="input-name" size="sm" :model.sync="model.name"   @change="updateData" />
				<app-input-textbox id="fieldset-parent-module-id" label-cols-sm="4" label-cols-lg="3" label="Parent Module" label-for="input-parent-module-id" size="sm" :model.sync="model.parentModuleId"   @change="updateData" />
			</b-col>
			<b-col>
				<span class="float-right">
					<b-button variant="success" class="float-right ml-2" @click="downloadData" size="sm"><b-icon-download class="mr-1"></b-icon-download> Download</b-button>
				</span>
			</b-col>
		</b-card>
		<PopupConfirmationDetail :openPopup="isShowPopupConfirmation" :modelId="modelId" @saveActionMethod="saveAction" @resetMethod="resetModal" />
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterView>
</template>
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate";
import { required, minLength } from "vuelidate/lib/validators";
import ModuleInfo from '@/models/ModuleInfo/ModuleInfo';
import ModuleInfoList from '@/models/ModuleInfo/ModuleInfoList';
import ModuleInfoFilter from '@/filters/ModuleInfo/ModuleInfoFilter';

import PopupConfirmationDetail from './PopupConfirmationDetail';

import AppInputTextbox from '@/components/AppInputTextbox';
import PopupRouterView from '@/components/PopupRouterView';
export default {
	components : {AppInputTextbox,PopupRouterView,PopupConfirmationDetail},
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
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			pageSizeOptions: 10,
			pageIndex:0,
			sortingBy: Array(),
			modelId: null,
			model: {},
			filterModuleInfo : null,
			currentModuleInfoPage : 1,
			moduleInfoPageSize: 10,
			sortByModuleInfo: '',
			sortDescModuleInfo: '',
			isShowPopupConfirmation : false,
			isDataCommited : false,
			mainRecordId : '',
		}
	},
	methods : {
		getModuleInfo : async function(id) {
			this.isBusy = true;
			ModuleInfo.getData(this, id).then(result => { this.model = result; this.mainRecordId = String(result.mainRecordId); this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		downloadData() {
			this.isDownloadModuleInfo = true;
			this.filterModuleInfo.id = this.modelId;
			ModuleInfo.downloadDataDetail(this, this.filterModuleInfo).then(result => {
				this.isDownloadModuleInfo = false;
			}).catch(error => { this.isDownloadModuleInfo = false; })
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
				ModuleInfo.commitData(this, model, id).then(result => {
					this.isDataCommited = true;
					this.$router.push({ name: 'moduleinfoindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			} else {
				ModuleInfo.insertData(this, model).then(result => {
					if (result) {
						this.$router.push({ name: 'moduleinfoindex' });
					}
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
					ModuleInfo.updateData(this, model, id);
				}
			});
		},
		onHidden() {
			this.getModuleInfos();
		},
		resetModal(modalType) {
			this.showPopupCopyData = false; 
			this.isShowPopupConfirmation = false;
		},
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
			}
		},
	},
	beforeMount() {
		this.filterModuleInfo = new ModuleInfoFilter();
	},
	async mounted(){
		let id = this.$route.params.id;
		let copyData = this.$route.params.copydata;
		if (id != '' && id != null && id != undefined) {
			if (copyData != null && copyData != undefined) {
				this.getModuleInfo(id);
			} else {
				this.modelId = id;
				this.getModuleInfo(id);
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
	},
	beforeDestroy() {
		if (!this.isDataCommited) {
			ModuleInfo.discardData(this, this.modelId);
		}
	},
}
</script>
