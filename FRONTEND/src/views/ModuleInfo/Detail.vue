<template>
	<PopupRouterViewDetail label="Module Info Detail" @copyDataMethod="copyDataAction">
<!-- APP_DETAIL_HEADER -->
		<b-card>
			<b-col cols="12" lg="6">
					<b-form-group id="fieldset-name" label-cols-sm="4" label-cols-lg="3" label="Name" label-for="filter-name">
						{{ model != null ? model.name : "" }}
					</b-form-group>
					<b-form-group id="fieldset-parent-module-id" label-cols-sm="4" label-cols-lg="3" label="Parent Module" label-for="filter-parent-module-id">
						{{ model != null ? model.parentModuleId : "" }}
					</b-form-group>
			</b-col>
		</b-card>
<!-- END_APP_DETAIL_HEADER -->
		<b-overlay :show="isBusy" no-wrap></b-overlay>
	</PopupRouterViewDetail>
</template>
<script>
/*APP_DETAIL_IMPORT*/
import ModuleInfo from '@/models/ModuleInfo/ModuleInfo';
import ModuleInfoList from '@/models/ModuleInfo/ModuleInfoList';
import ModuleInfoFilter from '@/filters/ModuleInfo/ModuleInfoFilter';

import PopupRouterViewDetail from '@/components/PopupRouterViewDetail';
/*END_APP_DETAIL_IMPORT*/
export default {
	components : {
	/*APP_DETAIL_PROP_DEFINITION*/
	PopupRouterViewDetail,
	/*APP_DETAIL_COMPONENTS_DEFINITION*/
},
	data() {
		return {
			fileupload: null,
			model: {},
			datas: Array(),
			busy:false,
			isBusy: false,
			isDataLoading: false,
			selected:'',
			model: {},
			/*APP_DETAIL_PAGINATION*/
			/*END_APP_DETAIL_PAGINATION*/
			/*APP_DETAIL_FILTER*/
			filterModuleInfo : null,
			/*END_APP_DETAIL_FILTER*/
			/*APP_DETAIL_PAGING_DEFINITION*/
			currentModuleInfoPage : 1,
			moduleInfoPageSize: 10,
			/*END_APP_DETAIL_PAGING_DEFINITION*/
			/*APP_DETAIL_SORT_DEFINITION*/
			sortByModuleInfo: '',
			sortDescModuleInfo: '',
			/*END_APP_DETAIL_SORT_DEFINITION*/
			/*APP_DETAIL_VARIABLE_DEFINITION*/
			/*END_APP_DETAIL_VARIABLE_DEFINITION*/
			/*APP_DETAIL_OBJ_VARIABLE*/
			/*END_APP_DETAIL_OBJ_VARIABLE*/
		}
	},
	methods : {
		/*APP_DETAIL_GETModuleInfo*/
		getModuleInfo : async function(id) {
			this.isBusy = true;
			ModuleInfo.getData(this, id).then(result => { this.model = result; this.isBusy = false; }).catch(error => { this.isBusy = false; });
		},
		/*END_APP_DETAIL_GETModuleInfo*/
		/*APP_DETAIL_COPYDATA*/
		copyDataAction(modelEvent) {
			this.$router.push({ name: 'moduleinfocopydata', params: { id: this.modelId, copydata : 1 } })
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
		this.filterModuleInfo = new ModuleInfoFilter();
	},
	/*END_APP_DETAIL_BEFORE_MOUNT*/
	/*APP_DETAIL_MOUNTED*/
	mounted(){
		let id = this.$route.params.id;
		this.modelId = id;
		this.getModuleInfo(id);
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
