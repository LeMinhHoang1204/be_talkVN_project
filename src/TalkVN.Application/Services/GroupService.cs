using TalkVN.Application.Services.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Application.Models;
using TalkVN.Application.Models.Dtos.Conversation;
using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Models.Dtos.User;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.ChatEntities;
using TalkVN.Domain.Entities.SystemEntities.Group;
using TalkVN.Domain.Entities.SystemEntities.Relationships;
using TalkVN.Domain.Enums;
using TalkVN.Domain.Identity;

namespace TalkVN.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IBaseRepository<UserGroup> _userGroupRepository;
        private readonly IBaseRepository<JoinGroupRequest> _joinGroupRequestRepository;
        private readonly IBaseRepository<UserGroupRole> _userGroupRoleRepository;
        private readonly IBaseRepository<TextChat> _textChatRepository;
        private readonly IBaseRepository<GroupInvitation> _groupInvitationRepository;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupService> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IBaseRepository<TextChatParticipant> _textChatParticipantRepository;

        public GroupService(IGroupRepository groupRepository,
            IClaimService claimService,
            IRepositoryFactory repositoryFactory,
            IConversationRepository conversationRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GroupService> logger,
            RoleManager<ApplicationRole> roleManager)
        {
            _groupRepository = groupRepository;
            _userGroupRepository = repositoryFactory.GetRepository<UserGroup>();
            _userGroupRoleRepository = repositoryFactory.GetRepository<UserGroupRole>();
            _textChatRepository = repositoryFactory.GetRepository<TextChat>();
            _joinGroupRequestRepository = repositoryFactory.GetRepository<JoinGroupRequest>();
            _groupInvitationRepository = repositoryFactory.GetRepository<GroupInvitation>();
            _textChatParticipantRepository = repositoryFactory.GetRepository<TextChatParticipant>();
            _claimService = claimService;
            _userRepository = userRepository;
            _conversationRepository = conversationRepository;
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


        public async Task<List<UserGroupDto>> GetMembersByGroupIdAsync(Guid groupId)
        {
            var userId = _claimService.GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            var members = await _userGroupRepository.GetAllAsync(
                x => x.GroupId == groupId,
                query => query.Include(x => x.User)
            );
            List<UserGroupDto> response = new();
            foreach (var member in members)
            {
                var userGroupDto = _mapper.Map<UserGroupDto>(member);
                userGroupDto.User = _mapper.Map<UserDto>(member.User);
                response.Add(userGroupDto);
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
            var ownerRole = await _roleManager.FindByNameAsync("GroupOwner");
            if (ownerRole == null)
                throw new Exception("Role 'Owner' not found");
            //add owner to the group
            var member = new UserGroup
            {
                UserId = userId,
                GroupId = group.Id,
                AcceptedBy = userId,
                InvitedBy = userId,
            };
            await _userGroupRepository.AddAsync(member);
            var groupDto = _mapper.Map<GroupDto>(group);
            groupDto.Creator = _mapper.Map<UserDto>(await _userRepository.GetFirstOrDefaultAsync(x => x.Id == userId));

            // add role for the owner
            var userGroupRole = new UserGroupRole
            {
                UserGroupId = member.Id,
                RoleId = ownerRole.Id,
                Id = Guid.NewGuid()
            };
            await _userGroupRoleRepository.AddAsync(userGroupRole);

            //create default textChats
            var textChat = new TextChat
            {
                Name = "General Chat",
                GroupId = group.Id,
                TextChatType = TextChatType.GroupChat.ToString(),
                IsSeen = false,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            await this._conversationRepository.AddAsync(textChat);
            var groupChat = new TextChat
            {
                Name = "General Call",
                GroupId = group.Id,
                TextChatType = TextChatType.GroupCall.ToString(),
                IsSeen = false,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            await this._conversationRepository.AddAsync(groupChat);
            //add owner to groupChats
            var textchatParticipant = new TextChatParticipant
            {
                UserId = userId,
                TextChatId = textChat.Id,
                Status = GroupStatus.Active
            };
            await this._textChatParticipantRepository.AddAsync(textchatParticipant);
            var groupchatParticipant = new TextChatParticipant
            {
                UserId = userId,
                TextChatId = textChat.Id,
                Status = GroupStatus.Active
            };
            await this._textChatParticipantRepository.AddAsync(groupchatParticipant);
            //TODO: add role for the owner in those chats
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

        public async Task<List<UserDto>> GetUsersByUsernamesAsync(List<string> usernames, PaginationFilter pagination)
        {
            var paginationResult = await _userRepository.GetAllAsync(
                u => usernames.Contains(u.UserName),
                u => u.OrderByDescending(x => x.DisplayName), // hoặc OrderByDescending nếu muốn
                pagination.PageIndex,
                pagination.PageSize
            );

            return _mapper.Map<List<UserDto>>(paginationResult.Items);
        }


        public async Task<List<TextChatDto>> GetAllTextChatsByGroupIdAsync(Guid groupId, PaginationFilter query)
        {
            var textChats = await _textChatRepository.GetAllAsync(
                tc => tc.GroupId == groupId,
                tc => tc.OrderByDescending(tc => tc.UpdatedOn),
                query.PageIndex,
                query.PageSize
            );

            if (textChats == null || !textChats.Items.Any())
            {
                throw new NotFoundException("No text chats found for this group");
            }

            return _mapper.Map<List<TextChatDto>>(textChats.Items);
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
            var newUserGroup = new UserGroup
            {
                Id = Guid.NewGuid(),
                UserId = request.RequestedUserId,
                GroupId = request.GroupId,
                AcceptedBy = ownerId,
                InvitedBy = request.Invitation?.CreatedBy ?? ownerId // nếu có
            };
            await _userGroupRepository.AddAsync(newUserGroup);

            //add role
            var userGroupRole = new UserGroupRole
            {
                Id = Guid.NewGuid(),
                UserGroupId = newUserGroup.Id,
                RoleId = memberRole.Id
            };
            await _userGroupRoleRepository.AddAsync(userGroupRole);

            //add user to group's chats
            await this.AddUserToChatsAsync(request.GroupId, request.RequestedUserId);

            this._logger.LogInformation("User {UserId} approved join request {RequestId} to group {GroupId}", ownerId, dto.JoinGroupRequestId, request.GroupId);
        }

        public async Task AddUserToChatsAsync(Guid groupId, string userId)
        {
            List<TextChat> textChats = await _groupRepository.GetAllTextChatsByGroupIdAsync(groupId);
            for(int i = 0; i < textChats.Count; i++)
            {
                _logger.LogWarning("No text chats found for group {GroupId}", groupId);
                return;
            }

            var participants = textChats.Select(chat => new TextChatParticipant
            {
                UserId = userId,
                TextChatId = chat.Id,
                Status = GroupStatus.Active
            }).ToList();

            await _textChatParticipantRepository.AddRangeAsync(participants); //bulk insert
            _logger.LogInformation("User {UserId} added to chats of group {GroupId}", userId, groupId);
        }

        public async Task UpdateUserRoleInGroupAsync(UpdateUserRoleInGroupDto dto)
        {
            var group = await _groupRepository.GetFirstOrDefaultAsync(x => x.Id == dto.GroupId);
            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            var UserGroup = await _userGroupRepository.GetFirstOrDefaultAsync(
                x => x.GroupId == dto.GroupId && x.UserId == dto.UserId
            );

            if (UserGroup == null)
            {
                throw new NotFoundException("User group not found");
            }

            // Find the user group role
            var userGroupRole =
                await _userGroupRoleRepository.GetFirstOrDefaultAsync(x =>
                    x.UserGroupId == UserGroup.Id
                );

            if (userGroupRole == null)
            {
                // create a new user group role if it does not exist

                // get the role by name
                var role = await _roleManager.FindByNameAsync("Member");

                if (role == null)
                {
                    throw new NotFoundException("Role not found");
                }

                var newUserGroupRole = new UserGroupRole
                {
                    Id = Guid.NewGuid(),
                    UserGroupId = UserGroup.Id,
                    RoleId = role.Id
                };
                await _userGroupRoleRepository.AddAsync(newUserGroupRole);
                return;
            }
            // else update the role
            // If the role is the same, no need to update
            var updatedRole = await _roleManager.FindByIdAsync(dto.RoleId.ToString());

            if (updatedRole == null)
            {
                throw new NotFoundException("Role not found");
            }

            if (userGroupRole.RoleId == updatedRole.Id)
            {
                // No update needed
                _logger.LogInformation("User role in group {GroupId} for UserGroupId {UserGroupId} is already {RoleName}");
                return;
            }

            // Update the role
            userGroupRole.RoleId = updatedRole.Id;
            await _userGroupRoleRepository.UpdateAsync(userGroupRole);
            _logger.LogInformation("User Group Role updated");
        }

        public async Task RejectJoinGroupRequestAsync(RequestActionDto dto)
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
                throw new UnauthorizedAccessException("Only group owner can reject requests");

            // Cập nhật trạng thái yêu cầu
            request.Status = JoinRequestStatus.Rejected.ToString();
            request.UpdatedOn = DateTime.UtcNow;
            request.UpdatedBy = ownerId;
            _joinGroupRequestRepository.UpdateAsync(request);
            _logger.LogInformation("User {UserId} rejected join request {RequestId} to group {GroupId}", ownerId, dto.JoinGroupRequestId, request.GroupId);
        }

        public async Task<List<GroupDto>> GetUserJoinedGroupsAsync(PaginationFilter query)
        {
            var userId = _claimService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not found");
            }
            _logger.LogInformation("GetUserJoinedGroupsAsync started for UserId: {UserId}", userId);

            var paginationResponse = await _groupRepository.GetAllAsync(
                g => g.UserGroups.Any(ug => ug.UserId == userId && ug.Status == GroupStatus.Active),
                g => g.OrderByDescending(x => x.UpdatedOn),
                query.PageIndex,
                query.PageSize,
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
    }
}
