using TalkVN.Application.Models.Dtos.User.Profile;

namespace TalkVN.Application.Services.Interface
{
    public interface IProfileService
    {
        Task<List<ProfileDto>> GetAllProfilesAsync(ProfileSearchQueryDto query);
        Task<ProfileDto> GetProfileByIdAsync(Guid userId);
        Task<ProfileDto> CreateProfileAsync(ProfileRequestDto request);
        Task<ProfileDto> UpdateProfileAsync(Guid userId, ProfileRequestDto request);

    }
}
