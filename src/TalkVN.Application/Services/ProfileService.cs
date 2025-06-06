using AutoMapper;

using TalkVN.Application.Exceptions;
using TalkVN.Application.Helpers;
using TalkVN.Application.Localization;
using TalkVN.Application.Models.Dtos.User.Profile;
using TalkVN.Application.Services.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.DataAccess.Repositories.Interface;
using TalkVN.Domain.Entities.UserEntities;

using Microsoft.EntityFrameworkCore;

namespace TalkVN.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Domain.Entities.UserEntities.Profile> _profileRepository;
        private readonly IClaimService _claimService;
        private readonly IUserRepository _userRepository;
        private readonly IBaseRepository<UserFollower> _userFollowerRepository;
        public ProfileService(IMapper mapper
            , IBaseRepository<Domain.Entities.UserEntities.Profile> profileRepository
            , IBaseRepository<UserFollower> userFollowerRepository
            , IClaimService claimService
            , IUserRepository userRepository)
        {
            this._mapper = mapper;
            this._profileRepository = profileRepository;
            this._claimService = claimService;
            this._userRepository = userRepository;
            this._userFollowerRepository = userFollowerRepository;
        }
        public async Task<List<ProfileDto>> GetAllProfilesAsync(ProfileSearchQueryDto query)
        {
            var userId = _claimService.GetUserId();
            var paginationResponse = await _profileRepository.GetAllAsync(
                            p => p.IsDeleted == false && p.SearchData.Contains(query.SearchText), p => p.OrderByDescending(p => p.Id)
                            , query.PageIndex, query.PageSize, p => p.Include(p => p.UserApplication));
            var response = _mapper.Map<List<ProfileDto>>(paginationResponse.Items);
            foreach (var profile in response)
            {
                profile.IsFollowed = await _userFollowerRepository.AnyAsync(p => p.FollowerId == userId);
            }
            return response;
        }
        public async Task<ProfileDto> GetProfileByIdAsync(Guid userId)
        {
            var userProfile = await _profileRepository.GetFirstOrDefaultAsync(p => p.Id == userId, p => p.Include(c => c.UserApplication));
            if (userProfile == null)
            {
                throw new NotFoundException(ValidationTexts.NotFound.Format("User", userId));
            }
            var response = _mapper.Map<ProfileDto>(userProfile);
            response.IsFollowed = await _userFollowerRepository.AnyAsync(p => p.FollowerId == userId.ToString());
            return response;
        }
        public async Task<ProfileDto> CreateProfileAsync(ProfileRequestDto request)
        {
            var userId = _claimService.GetUserId();
            Domain.Entities.UserEntities.Profile userProfile = this._mapper.Map<Domain.Entities.UserEntities.Profile>(request);
            userProfile.Id = Guid.Parse(userId);
            userProfile.UserApplicationId = userId;
            if (await _profileRepository.GetFirstOrDefaultAsync(p => p.Id == userProfile.Id) != null)
                throw new ConflictException(ValidationTexts.Conflict.Format(userProfile.GetType(), userId));
            var userApplication = await _userRepository.GetFirstOrDefaultAsync(p => p.Id == userId.ToString());
            userProfile.SearchData = this.GenerateSearchData(request.DisplayName, userApplication.Email ?? "", userId);
            await _profileRepository.AddAsync(userProfile);
            _mapper.Map(request, userApplication);
            await _userRepository.UpdateAsync(userApplication);
            var response = _mapper.Map<ProfileDto>(userProfile);
            _mapper.Map(userApplication, response);
            return response;
        }
        public async Task<ProfileDto> UpdateProfileAsync(Guid userId, ProfileRequestDto request)
        {
            if (userId.ToString() != _claimService.GetUserId())
            {
                throw new ForbiddenException(ValidationTexts.Forbidden.Format(typeof(Domain.Entities.UserEntities.Profile), userId));
            }
            var userProfile = await _profileRepository.GetFirstOrDefaultAsync(p => p.Id == userId);
            var userApplication = await _userRepository.GetFirstOrDefaultAsync(p => p.Id == userId.ToString());
            if (userProfile == null || userApplication == null)
            {
                throw new NotFoundException(ValidationTexts.NotFound.Format(userProfile.GetType(), userId));
            }
            _mapper.Map(request, userProfile);
            _mapper.Map(request, userApplication);
            userProfile.SearchData = this.GenerateSearchData(request.DisplayName, userApplication.Email ?? "", userId.ToString());
            await _profileRepository.UpdateAsync(userProfile);
            await _userRepository.UpdateAsync(userApplication);
            var response = _mapper.Map<ProfileDto>(userProfile);
            _mapper.Map(userApplication, response);
            return response;
        }
        private string GenerateSearchData(string displayName, string email, string userId)
        {
            return displayName + email + userId;

        }
    }
}
