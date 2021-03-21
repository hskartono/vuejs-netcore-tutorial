if(not exists(select Id from FunctionInfos where Id = 'car_type')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('car_type','Car Type','cartype','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'car_type')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'car_type',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
