using AutoMapper;
using System;
using Tutorial.ApplicationCore.Entities;
using Tutorial.Infrastructure.Utility;
using Tutorial.PublicApi.Features.Attachments;
using Tutorial.PublicApi.Features.DownloadProcesses;
using Tutorial.PublicApi.Features.Emails;
using Tutorial.PublicApi.Features.FunctionInfos;
using Tutorial.PublicApi.Features.JobConfigurations;
using Tutorial.PublicApi.Features.Roles;
using Tutorial.PublicApi.Features.SchedulerConfigurations;
using Tutorial.PublicApi.Features.SchedulerCronIntervals;
using Tutorial.PublicApi.Features.UserInfos;
using Tutorial.PublicApi.Features.UserRoles;
using Tutorial.PublicApi.Features.Parts;
using Tutorial.PublicApi.Features.PurchaseOrders;
using Tutorial.PublicApi.Features.PurchaseRequests;
using Tutorial.PublicApi.Features.ModuleInfos;

namespace Tutorial.PublicApi
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// function info
			CreateMap<FunctionInfo, FunctionInfoDTO>()
				.ForMember(dto => dto.ModuleName, opt => opt.MapFrom(src => src.ModuleInfo.Name));

			// email
			CreateMap<Email, EmailDTO>();
			CreateMap<EmailAttachment, EmailAttachmentDTO>()
				.ForMember(dto => dto.AttachmentId, opt => opt.MapFrom(src => src.AttachmentId))
				.ForMember(dto => dto.FileExtension, opt => opt.MapFrom(src => src.Attachment.FileExtension))
				.ForMember(dto => dto.FileSize, opt => opt.MapFrom(src => src.Attachment.FileSize))
				.ForMember(dto => dto.OriginalFileName, opt => opt.MapFrom(src => src.Attachment.OriginalFileName))
				.ForMember(dto => dto.SavedFileName, opt => opt.MapFrom(src => src.Attachment.SavedFileName));

			// role
			CreateMap<Role, RoleDTO>().ReverseMap();
			CreateMap<RoleDetail, RoleDetailDTO>()
				.ForMember(dto => dto.FunctionName, opt => opt.MapFrom(src => src.FunctionInfo.Name))
				.ForMember(dto => dto.ModuleName, opt => opt.MapFrom(src => src.FunctionInfo.ModuleInfo.Name));
			CreateMap<RoleDetailDTO, RoleDetail>();

			// user info
			CreateMap<UserInfo, UserInfoDTO>().ReverseMap();

			// user role
			CreateMap<UserRole, UserRoleDTO>().ReverseMap();
			CreateMap<UserRoleDetail, UserRoleDetailDTO>()
				.ForMember(dto => dto.RoleName, opt => opt.MapFrom(src => src.Role.Name));
			CreateMap<UserRoleDetailDTO, UserRoleDetail>();

			// scheduler cron interval
			CreateMap<SchedulerCronInterval, SchedulerCronIntervalDTO>().ReverseMap();

			// job configuration
			CreateMap<JobConfiguration, JobConfigurationDTO>().ReverseMap();

			// scheduler configuration
			CreateMap<SchedulerConfiguration, SchedulerConfigurationDTO>().ReverseMap();

			// download process
			CreateMap<DownloadProcess, DownloadProcessDTO>()
				.ForMember(dto => dto.FunctionName, opt => opt.MapFrom(src => src.FunctionInfo.Name))
				.ReverseMap();


			// attachment
			var baseUri = new Uri(ConfigurationManager.AppSetting["baseUri"]);
			var downloadUri = new Uri(baseUri, ConfigurationManager.AppSetting["uploadUri"]);
			CreateMap<Attachment, AttachmentDTO>()
				.ForMember(dto=>dto.DownloadUrl, opt=>opt.MapFrom(
					(s,d)=> d.DownloadUrl = downloadUri.AbsoluteUri + s.SavedFileName)
				)
				.ReverseMap();

			CreateMap<ModuleInfo, ModuleInfoDTO>();

			// do not remove region marker. this marker is used by code generator
			#region Application Entity

			CreateMap<Part, PartDTO>().ReverseMap();

			CreateMap<PurchaseOrder, PurchaseOrderDTO>().ReverseMap();
			CreateMap<PurchaseOrderDetail, PurchaseOrderDetailDTO>().ForMember(dto => dto.PartPartName, opt => opt.MapFrom(src => src.Part.PartName)).ReverseMap();

			CreateMap<PurchaseRequest, PurchaseRequestDTO>().ReverseMap();
			CreateMap<PurchaseRequestDetail, PurchaseRequestDetailDTO>().ForMember(dto => dto.PartPartName, opt => opt.MapFrom(src => src.Part.PartName)).ReverseMap();

			#endregion
		}
	}
}
