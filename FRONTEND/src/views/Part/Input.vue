<template>
	<PopupRouterView label="Part Input"  entityName="part" :documentId="mainRecordId"  @saveMethod="saveActionConfirmation" @hideMethod="onHidden">
		<b-card>
			<b-col cols="12" lg="6">
				<app-input-textbox id="fieldset-id" label-cols-sm="4" label-cols-lg="3" label="ID" label-for="input-id" size="sm" :model.sync="model.id"   />
				<app-input-textbox id="fieldset-part-name" label-cols-sm="4" label-cols-lg="3" label="Part Name" label-for="input-part-name" size="sm" :model.sync="model.partName"   />
				<app-input-textbox id="fieldset-description" label-cols-sm="4" label-cols-lg="3" label="Description" label-for="input-description" size="sm" :model.sync="model.description"   />
			</b-col>
			<b-col>
				<span class="float-right" v-if="modelId != null && modelId != undefined && modelId != ''">
					<b-button variant="success" class="float-right ml-2" @click="downloadData" size="sm"><b-icon-download class="mr-1"></b-icon-download> Download</b-button>
				</span>
			</b-col>
		</b-card>
		<PopupConfirmationDetail :openPopup="isShowPopupConfirmation" :model="model" @saveActionMethod="saveAction" @resetMethod="resetModal" />
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterView>
</template>
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate";
import { required, minLength } from "vuelidate/lib/validators";
import Part from '@/models/Part/Part';
import PartList from '@/models/Part/PartList';
import PartFilter from '@/filters/Part/PartFilter';

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
			filterPart : null,
			currentPartPage : 1,
			partPageSize: 10,
			sortByPart: '',
			sortDescPart: '',
			isShowPopupConfirmation : false,
			isDataCommited : false,
			mainRecordId : '',
		}
	},
	methods : {
		getPart : async function(id) {
			this.isBusy = true;
			Part.getData(this, id).then(result => { this.model = result; this.mainRecordId = String(result.mainRecordId); this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		downloadData() {
			this.isDownloadPart = true;
			this.filterPart.id = this.modelId;
			Part.downloadDataDetail(this, this.filterPart).then(result => {
				this.isDownloadPart = false;
			}).catch(error => { this.isDownloadPart = false; })
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
				Part.updateData(this, model, id).then(result => {
					this.isDataCommited = true;
					this.$router.push({ name: 'partindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			} else {
				Part.insertData(this, model).then(result => {
					this.$router.push({ name: 'partindex' });
					this.isBusy = false;
				}).catch(error => { this.isBusy = false; });
			}
		},
		onHidden() {
			this.getParts();
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
		this.filterPart = new PartFilter();
	},
	async mounted(){
		let id = this.$route.params.id;
		let copyData = this.$route.params.copydata;
		if (id != '' && id != null && id != undefined) {
			if (copyData != null && copyData != undefined) {
				this.getPart(id);
			} else {
				this.modelId = id;
				this.getPart(id);
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
	},
}
</script>
