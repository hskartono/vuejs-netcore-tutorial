<template>
	<b-modal v-model="isShowPopup" dialog-class="modal-detail" @hidden="resetModal" scrollable :no-close-on-backdrop=true @ok="addButtonAction" title="Add Data Purchase Request Detail">
		<b-card>
			<b-col cols="12" lg="6">
				<app-input-combobox-autocomplete :options="parts" optionsLabel="partName" optionsKey="id" label-cols-sm="4" label-cols-lg="3" label="Part" id="input-" size="sm" :model.sync="model.partId"   :selected="model.part != null ? model.part.partName : ''" @input="getParts" />
				<app-input-textbox id="fieldset-qty" label-cols-sm="4" label-cols-lg="3" label="Qty" label-for="input-qty" size="sm" :model.sync="model.qty"  />
				<app-input-datepicker id="fieldset-request-date" label-cols-sm="4" label-cols-lg="3" label="Request Date" size="sm" :model.sync="model.requestDate"  :date-format-options="{ year: 'numeric', month: '2-digit', day: '2-digit' }" locale="en-GB" />
			</b-col>
		</b-card>
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</b-modal>
</template>
<style lang="scss" scoped>
	/deep/ .modal-dialog {
		width: 70% !important; 
	}
</style> 
<script>
import axios from 'axios';
import { validationMixin } from "vuelidate";
import { required, minLength } from "vuelidate/lib/validators";
import PurchaseRequestDetail from '@/models/PurchaseRequest/PurchaseRequestDetail';
import PurchaseRequestDetailFilter from '@/filters/PurchaseRequest/PurchaseRequestDetailFilter';

import PurchaseRequest from '@/models/PurchaseRequest/PurchaseRequest';
import PurchaseRequestFilter from '@/filters/PurchaseRequest/PurchaseRequestFilter';
import AppInputComboboxAutocomplete from '@/components/AppInputComboboxAutocomplete';
import Part from '@/models/Part/Part';
import PartFilter from '@/filters/Part/PartFilter';
import AppInputTextbox from '@/components/AppInputTextbox';
import AppInputDatepicker from '@/components/AppInputDatepicker';
import AppInputDatepickerRange from '@/components/AppInputDatepickerRange';
import PopupRouterView from '@/components/PopupRouterView';
export default {
	props: ['modelId', 'openPopup','addButtonMethod', 'resetMethod'],
	components : {AppInputComboboxAutocomplete,AppInputTextbox,AppInputDatepicker,AppInputDatepickerRange,PopupRouterView,},
	mixins : [validationMixin],
	validations: {
		model: {
		}
	},
	data() {
		return {
			model : {},
			errorMessage : {
			},
			parts : [],
			isBusy: false,
		}
	},
	methods : {
		getPurchaseRequestDetail : async function(id) {
			this.isBusy = true;
			PurchaseRequestDetail.getData(this, id).then(result => { this.model = result; this.isBusy = false }).catch(error => { this.isBusy = false; });
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
		validateState(name) {
			const { $dirty, $error } = this.$v.model[name];
			return $dirty ? !$error : null;
		},
		getErrorMessage(message, name) {
			if (this.$v.model[name].required != undefined && !this.$v.model[name].required) {
				if (message.required) return message.required;
			}
		},
		addButtonAction(modalEvent) {
			modalEvent.preventDefault();

			this.$v.model.$touch(); 
			if (this.$v.model.$anyError) {
				return;
			} else {
				this.$emit("addButtonMethod", modalEvent, this.model); 
			}
		},
		resetModal() {
			this.$emit('resetMethod');
		},
	},
	beforeMount() {
		this.getParts();
	},
	async mounted(){
	},
	watch : {
		modelId(newValue) {
			if (newValue != null && newValue != undefined && newValue != '') {
				this.getPurchaseRequestDetail(newValue);
			} else {
				this.model = {};
			}
		},
		openPopup(newValue) {
			if (newValue) {
				this.getParts();
			} else {
				this.model = {};
			}
		}
	},
	computed: {
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
