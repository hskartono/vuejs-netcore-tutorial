if(not exists(select id from Companies where name='Default')) begin
	insert into Companies (Name, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('Default',1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select UserName from UserInfos where UserName in ('admin','auth0|6006986af6fac6006a61f4dd'))) begin
	insert into UserInfos values 
	('admin','Administrator','Admin',1,1,GETDATE(),null,null,0, GETDATE(), null, null, 0, null, null, null), 
	('auth0|6006986af6fac6006a61f4dd','hskartono','Harry',1,1,GETDATE(),null,null,0, GETDATE(), null, null, 0, null, null, null);
end

if(not exists(select Id from FunctionInfos where Id in ('user_info','function_info','role','user_role'))) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('user_info','User Management','userInfo/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('function_info','Function Management','functionInfo/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('role','Role Management','role/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('role_detail','Role Management Detail','roledetail/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('user_role','User Role Management','userrole/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('user_role_detail','User Role Management Detail','userroledetail/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('attachment','Attachment','attachment/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('email','Email Service','email/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('email_attachment','Email Service Attachment','emailattachment/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('job_configuration','Job Configuration','jobconfiguration/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('scheduler_configuration','Scheduler Configuration','schedulerconfiguration/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('scheduler_cron_interval','Scheduler Cron Interval','schedulercroninterval/','',1,1,'admin',GETDATE(),0,GETDATE(),0),
	('download_process','Download Monitor','downloadprocess/','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select Name from Roles where Name='Administrator')) begin
	insert into Roles (Name, Description, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	('Administrator', 'Administrator', 1, 'admin', GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId in ('user_info','function_info','role','user_role'))) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'user_info',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'function_info',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'role',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'role_detail',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'user_role',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'user_role_detail',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'attachment',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'email',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'email_attachment',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'job_configuration',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'scheduler_configuration',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'scheduler_cron_interval',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(1,'download_process',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from UserRoles where UserInfoId in ('admin','auth0|6006986af6fac6006a61f4dd'))) begin
	insert into UserRoles (UserInfoId, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('admin',1,'admin',GETDATE(),0,GETDATE(),0),
	('auth0|6006986af6fac6006a61f4dd',1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from UserRoleDetails where UserRoleId in (1,2))) begin
	insert into UserRoleDetails (UserRoleId, RoleId, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	(1,1,1,'admin',GETDATE(),0,GETDATE(),0),
	(2,1,1,'admin',GETDATE(),0,GETDATE(),0);
end

CREATE TABLE [Event]
(
    [EventId] BIGINT IDENTITY(1,1) NOT NULL,
    [InsertedDate] DATETIME NOT NULL DEFAULT(GETUTCDATE()),
    [LastUpdatedDate] DATETIME NULL,
    [JsonData] NVARCHAR(MAX) NOT NULL,
    [EventType] NVARCHAR(100) NOT NULL,
	[User] NVARCHAR(100) NULL,
    CONSTRAINT PK_Event PRIMARY KEY (EventId)
)