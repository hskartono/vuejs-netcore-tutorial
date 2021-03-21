<template>
	<PopupRouterViewDetail label="Part Detail" @copyDataMethod="copyDataAction">
<!-- APP_DETAIL_HEADER -->
		<b-card>
			<b-col cols="12" lg="6">
					<b-form-group id="fieldset-part-name" label-cols-sm="4" label-cols-lg="3" label="Part Name" label-for="filter-part-name">
						{{ model != null ? model.partName : "" }}
					</b-form-group>
					<b-form-group id="fieldset-description" label-cols-sm="4" label-cols-lg="3" label="Description" label-for="filter-description">
						{{ model != null ? model.description : "" }}
					</b-form-group>
			</b-col>
		</b-card>
<!-- END_APP_DETAIL_HEADER -->
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterViewDetail>
</template>
<script>
/*APP_DETAIL_IMPORT*/
import Part from '@/models/Part/Part';
import PartList from '@/models/Part/PartList';
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
			datas: Array(),
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			model: {},
			/*APP_DETAIL_PAGINATION*/
			/*END_APP_DETAIL_PAGINATION*/
			/*APP_DETAIL_FILTER*/
			filterPart : null,
			/*END_APP_DETAIL_FILTER*/
			/*APP_DETAIL_PAGING_DEFINITION*/
			currentPartPage : 1,
			partPageSize: 10,
			/*END_APP_DETAIL_PAGING_DEFINITION*/
			/*APP_DETAIL_SORT_DEFINITION*/
			sortByPart: '',
			sortDescPart: '',
			/*END_APP_DETAIL_SORT_DEFINITION*/
			/*APP_DETAIL_VARIABLE_DEFINITION*/
			/*END_APP_DETAIL_VARIABLE_DEFINITION*/
			/*APP_DETAIL_OBJ_VARIABLE*/
			/*END_APP_DETAIL_OBJ_VARIABLE*/
		}
	},
	methods : {
		/*APP_DETAIL_GETPart*/
		getPart : async function(id) {
			this.isBusy = true;
			Part.getData(this, id).then(result => { this.model = result; this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		/*END_APP_DETAIL_GETPart*/
		/*APP_DETAIL_COPYDATA*/
		copyDataAction(modelEvent) {
			this.$router.push({ name: 'partcopydata', params: { id: this.modelId, copydata : 1 } })
		},
		/*END_APP_DETAIL_COPYDATA*/
		/*APP_DETAIL_DISPLAYFILTER*/
		displayFilter() {
			this.windowWidth = window.innerWidth;
			if (this.windowWidth <= 768 ) {
			}
		},
		/*END_APP_DETAIL_DISPLAYFILTER*/
	},
	/*APP_DETAIL_BEFORE_MOUNT*/
	beforeMount() {
		this.filterPart = new PartFilter();
	},
	/*END_APP_DETAIL_BEFORE_MOUNT*/
	/*APP_DETAIL_MOUNTED*/
	mounted(){
		let id = this.$route.params.id;
		this.modelId = id;
		this.getPart(id);
		this.$nextTick(() => {
			window.addEventListener('resize', this.displayFilter);
			this.displayFilter();
		});
	},
	/*END_APP_DETAIL_MOUNTED*/
	watch : {
	},
	/*APP_DETAIL_COMPUTED*/
	computed: {
	}
	/*END_APP_DETAIL_COMPUTED*/
}
</script>
