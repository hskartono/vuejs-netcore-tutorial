import Vue from 'vue';
import Router from 'vue-router';
import { authGuard } from "./auth";

import Home from './views/Home/Home.vue';
import LoginIndex from './views/Login/Index.vue';

import ModuleInfoIndex from './views/ModuleInfo/Index.vue';
import ModuleInfoInput from './views/ModuleInfo/Input.vue';
import ModuleInfoDetail from './views/ModuleInfo/Detail.vue';

import PartIndex from './views/Part/Index.vue';
import PartInput from './views/Part/Input.vue';
import PartDetail from './views/Part/Detail.vue';


import PurchaseOrderIndex from './views/PurchaseOrder/Index.vue';
import PurchaseOrderInput from './views/PurchaseOrder/Input.vue';
import PurchaseOrderDetail from './views/PurchaseOrder/Detail.vue';
import PurchaseOrderIndexDetail from './views/PurchaseOrder/IndexDetail.vue';


import PurchaseRequestIndex from './views/PurchaseRequest/Index.vue';
import PurchaseRequestInput from './views/PurchaseRequest/Input.vue';
import PurchaseRequestDetail from './views/PurchaseRequest/Detail.vue';
import PurchaseRequestIndexDetail from './views/PurchaseRequest/IndexDetail.vue';


//IMPORTROUTE

Vue.use(Router);

const baseUrl = process.env.VUE_APP_BASE_FOLDER;
export default new Router({
    base : baseUrl,
    mode: 'history',
    routes : [
        {
            path : "/login",
            name: "login",
            component: LoginIndex
        },
        {
            path : "/",
            name: "home",
            component: Home,
            beforeEnter: authGuard,
        },
        {
            path : "/home",
            name: "homeindex",
            component: Home,
            beforeEnter: authGuard,
        },

		{
			path : "/moduleinfo",
			name: "moduleinfoindex",
			component: ModuleInfoIndex,
			beforeEnter: authGuard,
			children: [
				{
					path: "/moduleinfo/create",
					name: "moduleinfocreate",
					component: ModuleInfoInput,
					beforeEnter: authGuard
				},
				{
					path: "/moduleinfo/create/:id/copydata/:copydata",
					name: "moduleinfocopydata",
					component: ModuleInfoInput,
					beforeEnter: authGuard
				},
				{
					path: "/moduleinfo/edit/:id",
					name: "moduleinfoedit",
					component: ModuleInfoInput,
					beforeEnter: authGuard
				},
				{
					path: "/moduleinfo/detail/:id",
					name: "moduleinfodetail",
					component: ModuleInfoDetail,
					beforeEnter: authGuard
				},
			]
		},
		
		{
			path : "/part",
			name: "partindex",
			component: PartIndex,
			beforeEnter: authGuard,
			children: [
				{
					path: "/part/create",
					name: "partcreate",
					component: PartInput,
					beforeEnter: authGuard
				},
				{
					path: "/part/create/:id/copydata/:copydata",
					name: "partcopydata",
					component: PartInput,
					beforeEnter: authGuard
				},
				{
					path: "/part/edit/:id",
					name: "partedit",
					component: PartInput,
					beforeEnter: authGuard
				},
				{
					path: "/part/detail/:id",
					name: "partdetail",
					component: PartDetail,
					beforeEnter: authGuard
				},
			]
		},

		{
			path : "/purchaseorder",
			name: "purchaseorderindex",
			component: PurchaseOrderIndex,// if you need change to PurchaseOrderIndexDetail 
			beforeEnter: authGuard,
			children: [
				{
					path: "/purchaseorder/create",
					name: "purchaseordercreate",
					component: PurchaseOrderInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaseorder/create/:id/copydata/:copydata",
					name: "purchaseordercopydata",
					component: PurchaseOrderInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaseorder/edit/:id",
					name: "purchaseorderedit",
					component: PurchaseOrderInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaseorder/detail/:id",
					name: "purchaseorderdetail",
					component: PurchaseOrderDetail,
					beforeEnter: authGuard
				},
			]
		},

		{
			path : "/purchaserequest",
			name: "purchaserequestindex",
			component: PurchaseRequestIndex,// if you need change to PurchaseRequestIndexDetail 
			beforeEnter: authGuard,
			children: [
				{
					path: "/purchaserequest/create",
					name: "purchaserequestcreate",
					component: PurchaseRequestInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaserequest/create/:id/copydata/:copydata",
					name: "purchaserequestcopydata",
					component: PurchaseRequestInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaserequest/edit/:id",
					name: "purchaserequestedit",
					component: PurchaseRequestInput,
					beforeEnter: authGuard
				},
				{
					path: "/purchaserequest/detail/:id",
					name: "purchaserequestdetail",
					component: PurchaseRequestDetail,
					beforeEnter: authGuard
				},
			]
		},

		//ENDROUTE
    ]
})