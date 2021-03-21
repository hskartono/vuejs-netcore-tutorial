if(not exists(select Id from FunctionInfos where Id = 'supplier_hierarchy')) begin
	insert into FunctionInfos (Id, Name, Uri, IconName, IsEnabled, CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values 
	('supplier_hierarchy','Supplier Hierarchy','supplierhierarchy','',1,1,'admin',GETDATE(),0,GETDATE(),0);
end

if(not exists(select id from RoleDetails where FunctionInfoId = 'supplier_hierarchy')) begin
	insert into RoleDetails (RoleId, FunctionInfoId, AllowCreate, AllowRead, AllowUpdate, AllowDelete, ShowInMenu, AllowDownload, AllowPrint, AllowUpload,
	CompanyId, CreatedBy, CreatedDate, IsDraftRecord, RecordActionDate, DraftFromUpload) values
	(1,'supplier_hierarchy',1,1,1,1,1,1,1,1,1,'admin',GETDATE(),0,GETDATE(),0);
end
