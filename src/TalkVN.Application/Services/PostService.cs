// using System.Linq.Expressions;
//
// using AutoMapper;
//
// using TalkVN.Application.Exceptions;
// using TalkVN.Application.Helpers;
// using TalkVN.Application.Localization;
// using TalkVN.Application.MachineLearning.Services.Interface;
// using TalkVN.Application.Models.Dtos.Notification;
// using TalkVN.Application.Models.Dtos.Post;
// using TalkVN.Application.Models.Dtos.Post.Comments;
// using TalkVN.Application.Models.Dtos.Post.CreatePost;
// using TalkVN.Application.Services.Interface;
// using TalkVN.DataAccess.Repositories.Interface;
// using TalkVN.DataAccess.Repositories.Interface;
// using TalkVN.Domain.Common;
// using TalkVN.Domain.Entities.PostEntities;
// using TalkVN.Domain.Entities.PostEntities.Reaction;
// using TalkVN.Domain.Entities.SystemEntities;
// using TalkVN.Domain.Enums;
// using TalkVN.Domain.Extensions;
//
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.EntityFrameworkCore;
//
// namespace TalkVN.Application.Services
// {
//     public class PostService : IPostService
//     {
//         private readonly IClaimService _claimService;
//         private readonly IMapper _mapper;
//         private readonly IBaseRepository<Comment> _commentRepository;
//         private readonly IBaseRepository<Post> _postRepository;
//         private readonly IUserInteractionRepository _userInteractionRepository;
//         private readonly IBaseRepository<PostMedia> _postMediaRepository;
//         private readonly IBaseRepository<ReactionPost> _reactionPostRepository;
//         private readonly IBaseRepository<ReactionComment> _reactionCommentRepository;
//         private readonly ICloudinaryService _cloudinaryService;
//         private readonly IUserRepository _userRepository;
//         private readonly INotificationService _notificationService;
//         private readonly IWebHostEnvironment _env;
//         private readonly ITrainingModelService _trainingModelService;
//         public PostService(
//             IClaimService claimService
//             , IMapper mapper
//             , IRepositoryFactory repositoryFactory
//             , IUserRepository userRepository
//             , ICloudinaryService cloudinaryService
//             , INotificationService notificationService
//             , IWebHostEnvironment env
//             , ITrainingModelService trainingModelService
//             , IUserInteractionRepository userInteractionRepository)
//         {
//             _claimService = claimService;
//             _mapper = mapper;
//             _userRepository = userRepository;
//             _postRepository = repositoryFactory.GetRepository<Post>();
//             _commentRepository = repositoryFactory.GetRepository<Comment>();
//             _userInteractionRepository = userInteractionRepository;
//             _postMediaRepository = repositoryFactory.GetRepository<PostMedia>();
//             _reactionPostRepository = repositoryFactory.GetRepository<ReactionPost>();
//             _reactionCommentRepository = repositoryFactory.GetRepository<ReactionComment>();
//             _cloudinaryService = cloudinaryService;
//             _notificationService = notificationService;
//             _trainingModelService = trainingModelService;
//             _env = env;
//         }
//         #region GET
//         public async Task<List<PostDto>> GetAllPostsAsync(PostUserSearchQueryDto query)
//         {
//             Expression<Func<Post, bool>> filter = x => !x.IsDeleted;
//             var userId = query.UserId;
//             if (string.IsNullOrEmpty(userId))
//                 userId = _claimService.GetUserId();
//             var user = await _userRepository.GetFirstAsync(p => p.Id == userId);
//             if (user == null)
//             {
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("User", userId));
//             }
//             filter = x => !x.IsDeleted && x.UserId == userId;
//
//             var paginationResponse = await _postRepository.GetAllAsync(
//                 filter,
//                 p => p.OrderByDescending(p => p.CreatedOn),
//                 query.PageIndex,
//                 query.PageSize,
//                 p => p.IgnoreAutoIncludes().Include(c => c.PostMedias).Include(c => c.User)
//             );
//
//             var postDtos = _mapper.Map<List<PostDto>>(paginationResponse.Items);
//             foreach (var postDto in postDtos)
//             {
//                 if (await _reactionPostRepository.AnyAsync(c => c.PostId == postDto.Id))
//                     postDto.IsReacted = true;
//                 if (postDto.PostMedias != null)
//                     postDto.PostMedias = postDto.PostMedias.OrderBy(m => m.MediaOrder).ToList();
//             }
//             return postDtos;
//         }
//         public async Task<PostDto> GetPostByIdAsync(Guid postId)
//         {
//             var post = await _postRepository.GetFirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted
//                                                                     , p => p.Include(c => c.PostMedias)
//                                                                     .Include(c => c.Comments).ThenInclude(c=>c.UserPosted)
//                                                                     .Include(c => c.User));
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             post.Comments = post.Comments.Where(p => p.CommentType == CommentType.Parent.ToString()).ToList();
//
//             post.PostMedias = post.PostMedias.OrderBy(m => m.MediaOrder).ToList();
//             PostDto postDto = _mapper.Map<PostDto>(post);
//             if (await _reactionPostRepository.AnyAsync(c => c.PostId == postDto.Id))
//                 postDto.IsReacted = true;
//             foreach (var comment in postDto.Comments)
//             {
//                 if (await _reactionCommentRepository.AnyAsync(c => c.CommentId == comment.Id))
//                     comment.IsReacted = true;
//             }
//             return postDto;
//         }
//         public async Task<List<PostDto>> GetReccomendationPostsAsync(PostSearchQueryDto query)
//         {
//             var userId = _claimService.GetUserId();
//             var userInteraction = await _userInteractionRepository.GetUserInteractionModelForTraining(10000);
//             var postReccomendation = await _postRepository.GetAllAsync(p => !p.IsDeleted && p.UserId != userId
//             && p.Description.Contains(query.SearchText), p => p.Include(c => c.PostMedias)
//                                                     .Include(c => c.Comments)
//                                                     .Include(c => c.User));
//             var response = _trainingModelService.GetRecommendationPostModel(userId, userInteraction, postReccomendation);
//             return _mapper.Map<List<PostDto>>(response.OrderByDescending(p => p.Score).Skip(query.PageIndex * query.PageSize).Take(query.PageSize).ToList());
//         }
//         public async Task<List<CommentDto>> GetAllReplyCommentsAsync(Guid postId, Guid commentId)
//         {
//             var commentParent = _commentRepository.GetFirstOrDefaultAsync(p => p.Id == commentId && p.CommentType == CommentType.Parent.ToString() && !p.IsDeleted);
//             if (commentParent == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Comment", commentId));
//             var comments = await _commentRepository.GetAllAsync(p => p.PostId == postId && p.ParentCommentId == commentId);
//             var commentDtos = _mapper.Map<List<CommentDto>>(comments);
//             foreach (var commentDto in commentDtos)
//             {
//                 if (await _reactionCommentRepository.AnyAsync(c => c.CommentId == commentDto.Id))
//                     commentDto.IsReacted = true;
//             }
//             return commentDtos;
//         }
//         #endregion
//         #region POST
//         public async Task<PostDto> CreateNewPostAsync(CreatePostRequestDto requestDto)
//         {
//             string userId = _claimService.GetUserId();
//             Post post = _mapper.Map<Post>(requestDto);
//             post.UserId = userId;
//             post.Id = Guid.NewGuid();
//             var requestPostMedia = await _cloudinaryService.PostMediaToCloudAsync(requestDto.Files, post.Id);
//             var postMedias = _mapper.Map<List<PostMedia>>(requestPostMedia);
//             await _postRepository.AddAsync(post);
//             int i = 0;
//             foreach (var postMedia in postMedias)
//             {
//                 postMedia.PostId = post.Id;
//                 postMedia.MediaOrder = i++;
//             }
//
//             await _postMediaRepository.AddRangeAsync(postMedias);
//             return _mapper.Map<PostDto>(post);
//         }
//         public async Task<CommentDto> CreateNewCommentAsync(Guid postId, CommentRequestDto requestDto)
//         {
//             var userId = _claimService.GetUserId();
//             var comment = _mapper.Map<Comment>(requestDto);
//             var post = await _postRepository.GetFirstAsync(p => p.Id == postId);
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             UserInteraction userInteraction = new UserInteraction
//             {
//                 UserId = userId,
//                 PostId = postId,
//                 InteractionType = InteractionType.Comment
//             };
//             comment.PostId = postId;
//             comment.UserPostedId = userId;
//             comment.CommentType = CommentType.Parent.ToString();
//             ++post.CommentCount;
//             await _commentRepository.AddAsync(comment);
//             await _postRepository.UpdateAsync(post);
//             await _userInteractionRepository.AddAsync(userInteraction);
//             CreateCommentNotificationDto createCommentNotification = new CreateCommentNotificationDto
//             {
//                 CommentId = comment.Id,
//                 ReceiverUserId = post.UserId,
//                 LastInteractorUserId = comment.UserPostedId,
//                 Action = NotificationActionEnum.COMMENT_POST.ToString(),
//                 Type = NotificationType.Comment.ToString(),
//                 Reference = NotificationRefText.CommentRef(postId, comment.Id)
//             };
//             await _notificationService.CreateOrUpdateCommentNotificationAsync(createCommentNotification);
//             return _mapper.Map<CommentDto>(comment);
//         }
//         public async Task<CommentDto> CreateReplyCommentAsync(Guid postId, Guid parentCommentId, CommentRequestDto requestDto)
//         {
//             var post = await _postRepository.GetFirstAsync(p => p.Id == postId);
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             var parentComment = await _commentRepository.GetFirstOrDefaultAsync(p => p.Id == parentCommentId && p.CommentType == CommentType.Parent.ToString());
//             if ((parentComment == null))
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Comment", parentCommentId));
//             var userId = _claimService.GetUserId();
//             var comment = _mapper.Map<Comment>(requestDto);
//             comment.CommentType = CommentType.Reply.ToString();
//
//             UserInteraction userInteraction = new UserInteraction
//             {
//                 UserId = userId,
//                 PostId = postId,
//                 InteractionType = InteractionType.Comment
//             };
//             comment.PostId = postId;
//             comment.UserPostedId = userId;
//             comment.ParentCommentId = parentCommentId;
//             ++post.CommentCount;
//             await _commentRepository.AddAsync(comment);
//             await _userInteractionRepository.AddAsync(userInteraction);
//             await _postRepository.UpdateAsync(post);
//             CreateCommentNotificationDto createCommentNotification = new CreateCommentNotificationDto
//             {
//                 CommentId = comment.Id,
//                 ReceiverUserId = parentComment.UserPostedId,
//                 LastInteractorUserId = userId,
//                 Action = NotificationActionEnum.REPLY_COMMENT.ToString(),
//                 Type = NotificationType.Comment.ToString(),
//                 Reference = NotificationRefText.CommentRef(postId, comment.Id),
//                 PostId = postId
//             };// cho người được reply
//             CreateCommentNotificationDto createCommentNotificationUserPost = new CreateCommentNotificationDto
//             {
//                 CommentId = comment.Id,
//                 ReceiverUserId = post.UserId,
//                 LastInteractorUserId = userId,
//                 Action = NotificationActionEnum.COMMENT_POST.ToString(),
//                 Type = NotificationType.Comment.ToString(),
//                 Reference = NotificationRefText.CommentRef(postId, comment.Id)
//                 ,
//                 PostId = postId
//             }; // cho người đăng bài
//             await _notificationService.CreateOrUpdateCommentNotificationAsync(createCommentNotification);
//             await _notificationService.CreateOrUpdateCommentNotificationAsync(createCommentNotificationUserPost);
//             return _mapper.Map<CommentDto>(comment);
//         }
//         #endregion
//         #region PUT
//         public async Task<PostDto> UpdatePostByIdAsync(UpdatePostRequestDto postDto, Guid postId)
//         {
//             Post post = await _postRepository.GetFirstAsync(p => p.Id == postId);
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             if (post.UserId != _claimService.GetUserId())
//                 throw new ForbiddenException(ValidationTexts.Forbidden.Format("Post", postId));
//             post = _mapper.Map(postDto, post);
//             await _postRepository.UpdateAsync(post);
//             return _mapper.Map<PostDto>(post);
//         }
//         public async Task<bool> ToggleReactPostAsync(Guid postId)
//         {
//             var userId = _claimService.GetUserId();
//             var post = await _postRepository.GetFirstOrDefaultAsync(p => p.Id == postId);
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             var reactionPost = await _reactionPostRepository.GetFirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);
//             UserInteraction userInteraction = await _userInteractionRepository.GetFirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);
//             if (reactionPost == null)
//             {
//                 reactionPost = new ReactionPost
//                 {
//                     PostId = postId,
//                     UserId = userId,
//                 };
//                 if (userInteraction == null)
//                 {
//                     userInteraction = new UserInteraction
//                     {
//                         UserId = userId,
//                         PostId = postId,
//                         InteractionType = InteractionType.Like
//                     };
//                     await _userInteractionRepository.AddAsync(userInteraction);
//                 }
//                 await _reactionPostRepository.AddAsync(reactionPost);
//                 ++post.ReactionCount;
//                 await _postRepository.UpdateAsync(post);
//
//                 CreatePostNotificationDto createPostNotification = new CreatePostNotificationDto
//                 {
//                     PostId = postId,
//                     ReceiverUserId = post.UserId,
//                     LastInteractorUserId = userId,
//                     Action = NotificationActionEnum.LIKE_POST.ToString(),
//                     Type = NotificationType.Post.ToString(),
//                     Reference = NotificationRefText.PostRef(postId)
//                 };
//                 await _notificationService.CreateOrUpdatePostNotificationAsync(createPostNotification);
//                 return true;
//             }
//             else
//             {
//                 if (userInteraction != null)
//                 {
//                     await _userInteractionRepository.DeleteAsync(userInteraction);
//                 }
//                 await _reactionPostRepository.DeleteAsync(reactionPost);
//                 --post.ReactionCount;
//                 await _postRepository.UpdateAsync(post);
//                 return false;
//             }
//         }
//         public async Task<CommentDto> UpdateCommentAsync(CommentRequestDto commentDto, Guid commentId)
//         {
//             var comment = await _commentRepository.GetFirstOrDefaultAsync(p => p.Id == commentId);
//             if (comment == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Comment", commentId));
//             if (comment.UserPostedId != _claimService.GetUserId())
//                 throw new ForbiddenException(ValidationTexts.Forbidden.Format("Comment", commentId));
//             comment = _mapper.Map(commentDto, comment);
//             await _commentRepository.UpdateAsync(comment);
//             return _mapper.Map<CommentDto>(comment);
//         }
//         public async Task<bool> ToggleReactCommentAsync(Guid commentId)
//         {
//             var comment = await _commentRepository.GetFirstOrDefaultAsync(p => p.Id == commentId);
//             if (comment == null)
//             {
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Comment", commentId));
//             }
//
//             var reactionComment = await _reactionCommentRepository.GetFirstOrDefaultAsync(p => p.CommentId == commentId);
//             if (reactionComment == null)
//             {
//                 reactionComment = new ReactionComment
//                 {
//                     CommentId = commentId,
//                     UserId = _claimService.GetUserId()
//                 };
//                 comment.ReactionCount++;
//                 await _commentRepository.UpdateAsync(comment);
//                 await _reactionCommentRepository.AddAsync(reactionComment);
//                 if (_claimService.GetUserId() != comment.UserPostedId)
//                 {
//                     CreateCommentNotificationDto createCommentNotificationDto = new CreateCommentNotificationDto
//                     {
//                         CommentId = commentId,
//                         ReceiverUserId = comment.UserPostedId,
//                         LastInteractorUserId = _claimService.GetUserId(),
//                         Action = NotificationActionEnum.LIKE_COMMENT.ToString(),
//                         Type = NotificationType.Comment.ToString(),
//                         Reference = NotificationRefText.CommentRef(comment.PostId, commentId),
//                     };
//                     await _notificationService.CreateOrUpdateCommentNotificationAsync(createCommentNotificationDto);
//                 }
//                 return true;
//             }
//             else
//             {
//                 comment.ReactionCount--;
//                 await _commentRepository.UpdateAsync(comment);
//                 await _reactionCommentRepository.DeleteAsync(reactionComment);
//                 return false;
//             }
//         }
//
//         #endregion
//         #region DELETE
//         public async Task<PostDto> DeletePostByIdAsync(Guid postId)
//         {
//             Post post = await _postRepository.GetFirstAsync(p => p.Id == postId);
//             if (post == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Post", postId));
//             if (post.UserId != _claimService.GetUserId())
//                 throw new ForbiddenException(ValidationTexts.Forbidden.Format("Post", postId));
//             var postMedia = await _postMediaRepository.GetAllAsync(p => p.PostId == postId);
//             if (postMedia != null)
//             {
//                 foreach (var media in postMedia)
//                 {
//                     await _postMediaRepository.DeleteAsync(media);
//                     await _cloudinaryService.DeleteMediaFromCloudAsync(postId, media.Id);
//                 }
//             }
//             await _postRepository.DeleteAsync(post);
//             return _mapper.Map<PostDto>(post);
//         }
//         public async Task<CommentDto> DeleteCommentByIdAsync(Guid commentId)
//         {
//             var comment = await _commentRepository.GetFirstOrDefaultAsync(p => p.Id == commentId);
//             if (comment == null)
//                 throw new NotFoundException(ValidationTexts.NotFound.Format("Comment", commentId));
//             if (comment.UserPostedId != _claimService.GetUserId())
//                 throw new ForbiddenException(ValidationTexts.Forbidden.Format("Comment", commentId));
//             await _commentRepository.DeleteAsync(comment);
//             return _mapper.Map<CommentDto>(comment);
//         }
//
//
//         #endregion
//     }
// }
//
