using AutoMapper;
using BE.Models;
using BE.DTOs.Users;
using BE.DTOs.Tasks;
using BE.DTOs.Projects;

namespace BE.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Username));

            CreateMap<ProjectMember, ProjectMemberDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<TaskItem, TaskDto>()
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null))
                .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => src.AssignedTo.Username))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.Username));

            CreateMap<CreateTaskDto, TaskItem>();
            CreateMap<UpdateTaskDto, TaskItem>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SubTask, SubTaskDto>();
            CreateMap<CreateSubTaskDto, SubTask>();
            CreateMap<UpdateSubTaskDto, SubTask>()
                .ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        }
    }
}
