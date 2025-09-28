using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepo;
    private readonly IUserRepository _userRepo;
    private string _fileUploadBaseUrl;
    private readonly IPostFileRepository _postFileRepo;
    private readonly IFileService _fileService;
    private readonly ILikeRepository _likeRepo;
    private readonly INotificationService _notificationService;
    private readonly IFeedService _feedService;
    public PostService(IPostRepository postRepository, IConfiguration configuration,
        IUserRepository userRepository, IFileService fileService, IPostFileRepository postFileRepository,
        ILikeRepository likeRepository, INotificationService notificationService, IFeedService feedService)
    {
        _postRepo = postRepository;
        _fileUploadBaseUrl = configuration["FileUploadServer:BaseUrl"]!;
        _userRepo = userRepository;
        _fileService = fileService;
        _postFileRepo = postFileRepository;
        _likeRepo = likeRepository;
        _notificationService = notificationService;
        _feedService = feedService;
    }

    public async Task<Result<PostResponse>> CreatePostAsync(CreatePostRequest request, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure<PostResponse>(new Error("CreatePost.UserNotFount", "User not found"));

        Post newPost = await _postRepo.AddAsync(request.ToEntity(currentUserId));
        var postResponse = newPost.ToResponse(
            _fileUploadBaseUrl,
            0,
            false
        );

        // Send real-time feed notification
        await _feedService.NotifyNewPostAsync(currentUserId, postResponse);

        return Result.Success(postResponse);
    }

    public async Task<Result> DeletePostAsync(Guid postId)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));

        var authorId = post.UserId;
        await _postRepo.DeleteAsync(post);

        // Send real-time feed notification
        await _feedService.NotifyPostDeletedAsync(authorId, postId);

        return Result.Success();
    }

    public async Task<Result<CursorList<PostResponse>>> GetPostsAsync(PostQueryParameters query)
    {
        CursorList<PostResponse> posts = await _postRepo.GetPostsResponseAsync(query, _fileUploadBaseUrl);
        return Result.Success(posts);
    }

    public async Task<Result<CursorList<PostResponse>>> GetPostsByUserIdAsync(Guid userId, PostQueryParameters query, Guid? currentUserId = null)
    {
        // Verify user exists
        User? user = await _userRepo.GetByIdAsync(userId);
        if (user is null)
            return Result.Failure<CursorList<PostResponse>>(new Error("User.NotFound", "User not found"));

        CursorList<PostResponse> posts = await _postRepo.GetPostsByUserIdResponseAsync(userId, query, _fileUploadBaseUrl, currentUserId);
        return Result.Success(posts);
    }

    public async Task<Result<PostResponse>> GetPostByIdAsync(Guid postId, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure<PostResponse>(new Error("CreatePost.UserNotFount", "User not found"));

        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));

        return Result.Success(
            post.ToResponse(
                _fileUploadBaseUrl,
                await _likeRepo.GetTargetLikeNumber(postId, LikeType.Post),
                await _likeRepo.IsUserLiked(currentUserId, postId, LikeType.Post)
            )
        );
    }

    public async Task<Result> UpdatePostPrivacyAsync(Guid postId, ChangePostPrivacyRequest request)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure(new Error("Post.NotFound", "Post not found"));

        await _postRepo.UpdatePrivacy(postId, request.Privacy);
        return Result.Success();
    }

    public async Task<Result<PostResponse>> UpdatePostAsync(Guid postId, UpdatePostRequest request, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure<PostResponse>(new Error("CreatePost.UserNotFount", "User not found"));

        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));
        
        post.Content = request.Content;
        post.Privacy = request.Privacy;
        await _postRepo.UpdateAsync(post);
        
        var updatedPostResponse = post.ToResponse(
            _fileUploadBaseUrl,
            await _likeRepo.GetTargetLikeNumber(postId, LikeType.Post),
            await _likeRepo.IsUserLiked(currentUserId, postId, LikeType.Post)
        );

        // Send real-time feed notification
        await _feedService.NotifyPostUpdatedAsync(currentUserId, updatedPostResponse);

        return Result.Success(updatedPostResponse);
    }

    public async Task<Result<IEnumerable<PostFileResponse>>> AddPostFileAsync(Guid postId, IFormFileCollection files)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<IEnumerable<PostFileResponse>>(new Error("Post.NotFound", "Post not found"));

        List<string> ValidExtentions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        List<PostFile> postFiles = [];
        foreach (var file in files)
        {
            FileAsset fileAsset = await _fileService.UploadFileAsync(file, ValidExtentions, "PostAsset");
            PostFile newPostFile = new()
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                FileAsset = fileAsset
            };
            postFiles.Add(newPostFile);
        }
        await _postFileRepo.AddRangeAsync(postFiles);
        return Result.Success(postFiles.Select(pf => pf.ToResponse(_fileUploadBaseUrl)));
    }

    public async Task<Result> DeletePostFileAsync(Guid postId, Guid postFileId)
    {
        PostFile? postFile = await _postFileRepo.GetByIdAsync(postFileId);
        if (postFile is null)
            return Result.Failure(new Error("PostFile.NotFound", "Post file not found"));

        await _postFileRepo.DeleteAsync(postFile);
        _fileService.DeleteFile(postFile.FileAsset.StorageKey);
        return Result.Success();
    }

    public async Task<Result> LikePostAsync(Guid postId, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure(new Error("CreatePost.UserNotFount", "User not found"));

        bool isUserLiked = await _likeRepo.IsUserLiked(currentUserId, postId, LikeType.Post);
        if (isUserLiked)
            return Result.Failure(new Error("UserLikePost", "User has liked this post"));

        await _likeRepo.CreateLikeAsync(currentUserId, postId, LikeType.Post);

        // create notification
        var post = await _postRepo.GetByIdAsync(postId);
        await _notificationService.CreateNotificationAsync(
            recipientId: post!.UserId,
            triggerId: currentUserId,
            type: NotificationType.NewLikeOnPost,
            targetId: postId
        );

        return Result.Success();
    }

    public async Task<Result> UnlikePostAsync(Guid postId, Guid currentUserId)
    {
        User? user = await _userRepo.GetByIdAsync(currentUserId);
        if (user is null)
            return Result.Failure(new Error("CreatePost.UserNotFount", "User not found"));

        bool isUserLiked = await _likeRepo.IsUserLiked(currentUserId, postId, LikeType.Post);
        if (!isUserLiked)
            return Result.Failure(new Error("UserUnLikePost", "User still haven't like this post"));

        await _likeRepo.DeleteLikeAsync(currentUserId, postId, LikeType.Post);
        return Result.Success();
    }
}
