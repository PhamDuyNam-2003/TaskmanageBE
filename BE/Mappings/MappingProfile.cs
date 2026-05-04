using AutoMapper;
using BE.DTOs;
using BE.Models;

namespace BE.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<TaskItem, TaskDetailDto>();
        }
    }
}
