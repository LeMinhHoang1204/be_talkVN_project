using AutoMapper;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Relationships;

namespace TalkVN.Application.Mapping;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<TalkVN.Domain.Entities.SystemEntities.Group.Group, GroupDto>();
        CreateMap<UserGroupRole, UserGroupRoleDto>();
        CreateMap<GroupInvitation, GroupInvitationDto>();
        CreateMap<UserGroup, UserGroupDto>();
        CreateMap<JoinGroupRequest, JoinGroupRequestDto>();
    }
}
