using TalkVN.Application.Services.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Relationships;
using TalkVN.Domain.Enums;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IBaseRepository<UserGroupRole> _userGroupRoleRepository;
        private readonly IBaseRepository<JoinGroupRequest> _joinGroupRequestRepository;
        private readonly IBaseRepository<GroupInvitation> _groupInvitationRepository;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupService> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public GroupService(IGroupRepository groupRepository,
            IClaimService claimService,
            IRepositoryFactory repositoryFactory,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GroupService> logger,
            RoleManager<ApplicationRole> roleManager)
        {
            _groupRepository = groupRepository;
            _userGroupRoleRepository = repositoryFactory.GetRepository<UserGroupRole>();
            _joinGroupRequestRepository = repositoryFactory.GetRepository<JoinGroupRequest>();
            _groupInvitationRepository = repositoryFactory.GetRepository<GroupInvitation>();
            _claimService = claimService;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<List<GroupDto>>GetAllGroupsAsync(PaginationFilter paginationFilter)
        {
            var userId = _claimService.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            _logger.LogInformation("GetAllGroupAsync started for UserId: {UserId}", userId);

            var paginationResponse = await _groupRepository.GetAllAsync(
                g => g.CreatorId == userId,
                g => g.OrderByDescending(x => x.UpdatedOn),
                paginationFilter.PageIndex,
                paginationFilter.PageSize,
                query => query.Include(g => g.Creator)
            );

            _logger.LogInformation("PaginationResponse: {PaginationResponse}", paginationResponse);
            List<GroupDto> response = new();
            foreach (var group in paginationResponse.Items)
            {
                var groupDto = _mapper.Map<GroupDto>(group);
                groupDto.Creator = _mapper.Map<UserDto>(group.Creator);
                response.Add(groupDto);
            }
            return response;
        }


        public async Task<List<UserGroupRoleDto>> GetMembersByGroupIdAsync(Guid groupId)
        {
            var userId = _claimService.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            var members = await _userGroupRoleRepository.GetAllAsync(
                x => x.GroupId == groupId,
                query => query.Include(x => x.User)
            );
            List<UserGroupRoleDto> response = new();
            foreach (var member in members)
            {
                var userGroupRoleDto = _mapper.Map<UserGroupRoleDto>(member);
                userGroupRoleDto.User = _mapper.Map<UserDto>(member.User);
                response.Add(userGroupRoleDto);
            }
            return response;
        }


        public async Task<GroupDto> CreateGroupAsync(RequestCreateGroupDto request)
        {
            var userId = _claimService.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            //create group
            var group = new Group
            {
                Name = request.Name,
                Description = request.Description,
                Password = request.Password,
                CreatorId = userId,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Status = GroupStatus.Active,
                IsPrivate = request.IsPrivate,
                MaxQuantity = request.MaxQuantity,
                Avatar = "https://img.freepik.com/free-vector/cute-penguin-gaming-cartoon-illustration_138676-2741.jpg?t=st=1745331555~exp=1745335155~hmac=0f64be923aed944b3db068cec9ab991950fba62a5bd0991a52a6157651438784&w=826",
                Url = Guid.NewGuid().ToString().Substring(0, 8), // tạo URL
            };

            await _groupRepository.AddAsync(group);
            var ownerRole = await _roleManager.FindByNameAsync("Owner");
            if (ownerRole == null)
                throw new Exception("Role 'Owner' not found");
            //add owner to the group
            var member = new UserGroupRole
            {
                UserId = userId,
                GroupId = group.Id,
                RoleId = ownerRole.Id,
                AcceptedBy = userId,
                InvitedBy = userId,
            };
            await _userGroupRoleRepository.AddAsync(member);
            var groupDto = _mapper.Map<GroupDto>(group);
            groupDto.Creator = _mapper.Map<UserDto>(await _userRepository.GetFirstOrDefaultAsync(x => x.Id == userId));
            return groupDto;
        }

        public async Task<GroupDto> GetGroupInfoByInvitationCodeAsync(string code)
        {
            var group = await this._groupRepository.GetGroupByInvitationCode(code);

            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Avatar = group.Avatar,
                Description = group.Description,
                MaxQuantity = group.MaxQuantity,
            };
        }

        public async Task<JoinGroupRequestDto> RequestJoinGroupAsync(JoinGroupRequestDto request)
        {
            var userId = _claimService.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            var group = await _groupRepository.GetFirstOrDefaultAsync(x => x.Id == request.GroupId);
            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            //find invitationId based on invitation code
            Guid? invitationId = null;
            var invitation = await _groupInvitationRepository.GetFirstOrDefaultAsync(x => x.InvitationCode == request.InvitationCode);
            invitationId = invitation.Id;
            var newRequest = new JoinGroupRequest
            {
                Id = Guid.NewGuid(),
                GroupId = request.GroupId,
                InvitationId = invitationId,
                RequestedUserId = userId,
                CreatedOn = DateTime.UtcNow,
                Status = JoinRequestStatus.Pending.ToString()
            };
            await _joinGroupRequestRepository.AddAsync(newRequest);
            var requestDto = new JoinGroupRequestDto
            {
                GroupId = newRequest.GroupId,
                InvitationCode = invitation.InvitationCode,
            };
            _logger.LogInformation("Join group request created for UserId: {UserId}, GroupId: {GroupId}", userId,
                request.GroupId);
            return requestDto;
        }

        public async Task ApproveJoinGroupRequestAsync(RequestActionDto dto)
        {
            var ownerId = _claimService.GetUserId();
            var request = await _joinGroupRequestRepository
                .GetFirstOrDefaultAsync(
                    x => x.Id == dto.JoinGroupRequestId,
                    x => x.Include(x => x.Group)
                        .Include(x => x.Invitation));
            if (request == null)
                throw new NotFoundException("Join request not found");

            if (request.Group.CreatorId != ownerId)
                throw new UnauthorizedAccessException("Only group owner can approve requests");

            // Cập nhật trạng thái yêu cầu
            request.Status = JoinRequestStatus.Approved.ToString();
            request.UpdatedOn = DateTime.UtcNow;
            request.UpdatedBy = ownerId;
            await _joinGroupRequestRepository.UpdateAsync(request);

            // Tạo user group role
            var memberRole = await _roleManager.FindByNameAsync("Member");
            var newUserGroupRole = new UserGroupRole
            {
                Id = Guid.NewGuid(),
                UserId = request.RequestedUserId,
                GroupId = request.GroupId,
                RoleId = memberRole.Id,
                AcceptedBy = ownerId,
                InvitedBy = request.Invitation?.CreatedBy ?? ownerId // nếu có
            };
            await _userGroupRoleRepository.AddAsync(newUserGroupRole);
            _logger.LogInformation("User {UserId} approved join request {RequestId} to group {GroupId}", ownerId, dto.JoinGroupRequestId, request.GroupId);
        }
    }
}
