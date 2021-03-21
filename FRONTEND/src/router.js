import Vue from 'vue';
import Router from 'vue-router';
import { authGuard } from "./auth";

import Home from './views/Home/Home.vue';
import LoginIndex from './views/Login/Index.vue';

import ModuleInfoIndex from './views/ModuleInfo/Index.vue';
import ModuleInfoInput from './views/ModuleInfo/Input.vue';
import ModuleInfoDetail from './views/ModuleInfo/Detail.vue';

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
		
		//ENDROUTE
    ]
})