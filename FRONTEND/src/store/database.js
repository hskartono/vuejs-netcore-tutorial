import VuexORM from '@vuex-orm/core'
import VuexORMAxios from '@vuex-orm/plugin-axios'
import axios from 'axios'

import Attachment from '@/models/Core/Attachment'
import UserInfo from '@/models/Core/UserInfo'
import FunctionInfo from '@/models/Core/FunctionInfo'
import UserRole from '@/models/Core/UserRole'
import UserRoleDetail from '@/models/Core/UserRoleDetail'
import Role from '@/models/Core/Role'
import RoleDetail from '@/models/Core/RoleDetail'
import ModuleInfo from '@/models/Core/ModuleInfo'

import AttachmentList from '@/models/Core/AttachmentList'
import UserInfoList from '@/models/Core/UserInfoList'
import FunctionInfoList from '@/models/Core/FunctionInfoList'
import UserRoleList from '@/models/Core/UserRoleList'
import RoleList from '@/models/Core/RoleList'

VuexORM.use(VuexORMAxios, { axios })
const database = new VuexORM.Database()

database.register(Attachment);
database.register(UserInfo);
database.register(FunctionInfo);
database.register(UserRole);
database.register(UserRoleDetail);
database.register(Role);
database.register(ModuleInfo);
database.register(RoleDetail);

database.register(AttachmentList);
database.register(UserInfoList);
database.register(FunctionInfoList);
database.register(UserRoleList);
database.register(RoleList);

export default database
