using AutoMapper;
using TalkVN.Application.Models.Dtos.Group;
namespace TalkVN.Application.Mapping;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<TalkVN.Domain.Entities.SystemEntities.Group.Group, GroupDto>();
    }
}
