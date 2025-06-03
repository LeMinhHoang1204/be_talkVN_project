using AutoMapper;

using Microsoft.Extensions.Logging;

using TalkVN.Application.Models.Dtos.Group;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.SystemEntities.Group;

namespace TalkVN.Application.Services
{
    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IGroupInviteRepository _groupInviteRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupInvitationService> _logger;
        private readonly string _baseInvitationUrl = "https://talkvn.vercel.app/invitation/"; // hoặc config

        public GroupInvitationService(IGroupInviteRepository groupInviteRepo
            , IMapper mapper
            ,ILogger<GroupInvitationService> logger)
        {
            _groupInviteRepo = groupInviteRepo;
            _mapper = mapper;
            _logger = logger;
        }

        //find existing invitation by groupId and userId
        // if not found, create new invitation
        public async Task<GroupInvitationDto> CreateGroupInvitationAsync(Guid groupId, string userId)
        {
            _logger.LogDebug("CreateGroupInvitationAsync started - GroupId: {GroupId}, UserId: {UserId}", groupId, userId);

            var existingCode = await _groupInviteRepo.GetUserInvitaionsByGroupId(groupId, userId);
            if (existingCode != null)
            {
                //create invite link with existing code
                return new GroupInvitationDto
                {
                    InvitationCode = existingCode.InvitationCode,
                    InvitationUrl = $"{_baseInvitationUrl}{existingCode.InvitationCode}",
                    ExpirationDate = existingCode.ExpirationDate,
                    CreatedDate = existingCode.CreatedOn,
                    GroupId = existingCode.GroupId,
                    InviterId = existingCode.InviterId
                };
            }
            else
            {
                // Nếu chưa có thì tạo mới
                var code = Guid.NewGuid().ToString("N").Substring(0,6); // hoặc gen ngắn hơn
                _logger.LogDebug("No existing invitation found. Generating new code: {NewCode}", code);
                var newInvite = new GroupInvitation
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    InviterId = userId,
                    InvitationCode = code,
                    CreatedOn = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddDays(7)
                };

                await _groupInviteRepo.AddAsync(newInvite);
                _logger.LogInformation("New invitation created - Code: {Code}, GroupId: {GroupId}, UserId: {UserId}",
                    code, groupId, userId);

                //tra ve cho fe
                return new GroupInvitationDto
                {
                    Id = newInvite.Id,
                    InvitationCode = code,
                    InvitationUrl = $"{_baseInvitationUrl}{code}",
                    CreatedDate = newInvite.CreatedOn,
                    ExpirationDate = newInvite.ExpirationDate,
                    GroupId = groupId,
                    InviterId = userId
                };
            }
        }
    }
}
