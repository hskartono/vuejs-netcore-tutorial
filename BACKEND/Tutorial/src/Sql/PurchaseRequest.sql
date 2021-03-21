if(not exists(select Id from FunctionInfos where Id = 'purchase_request')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('purchase_request','Purchase Request','purchaserequest','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'purchase_request')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'purchase_request',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
if(not exists(select Id from FunctionInfos where Id = 'purchase_request')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('purchase_request','Purchase Request','purchaserequest','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'purchase_request')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'purchase_request',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select Id from FunctionInfos where Id = 'purchase_request_detail')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('purchase_request_detail','Purchase Request Detail','purchaserequestdetail','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'purchase_request_detail')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'purchase_request_detail',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
