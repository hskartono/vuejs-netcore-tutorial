<template>
	<b-modal v-model="isShowPopup" dialog-class="modal-detail" @hidden="resetModal" scrollable :no-close-on-backdrop=true id="popup-upload" @ok="uploadButtonAction" :okTitle="okTitle" title="Upload Data">
		<b-card>
			<b-row>
				<b-col cols="12">
					<app-input-fileupload label="File" size="sm" :model.sync="fileupload" />
				</b-col>
			</b-row>
		</b-card>
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</b-modal>
</template>
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate"; 
import { required, minLength } from "vuelidate/lib/validators"; 

import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';

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
			busy:false,
			isDataLoading: false,
			isBusy : false,
			selected:'',
			pageSizeOptions: 10,
			pageIndex:0,
			sortingBy: Array(),
			processId: "",
		}
	},
	methods : {
		resetModal() {
			this.$emit('resetMethod', 'PurchaseRequestDetail');
		},
		uploadButtonAction(modalEvent) {
			modalEvent.preventDefault();
			if (this.fileupload != null) {
				this.isBusy = true;
				let data = new FormData();
				data.append('file', this.fileupload);
				let _this = this;
				this.$http.post(process.env.VUE_APP_API_URL + 'purchaserequestdetail/upload',
					data,
					{ headers: { 'Content-Type': 'multipart/form-data' } }
					).then(async function(response){
						this.isBusy = false;
					})
					.catch(error => {
						this.$store.dispatch('addErrorMessageGlobal', error);
						this.isBusy = false;
					});
			}
		},
	},
	beforeMount() {
	},
	mounted(){
	},
	computed: {
		okTitle: {
			get: function() {
			if (this.datas != null) {
				if (this.datas.length > 0) {
				return "Proses";
				}
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
