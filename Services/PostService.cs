using System;
using SocialWebsite.DTOs.Post;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Interfaces.Services;
using SocialWebsite.Mapping;
using SocialWebsite.Shared;

namespace SocialWebsite.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepo;
    private readonly IUserRepository _userRepo;
    private string _fileUploadBaseUrl;
    private readonly IFileService _fileService;
    public PostService(IPostRepository postRepository, IConfiguration configuration,
        IUserRepository userRepository, IFileService fileService)
    {
        _postRepo = postRepository;
        _fileUploadBaseUrl = configuration["FileUploadServer:BaseUrl"]!;
        _userRepo = userRepository;
        _fileService = fileService;
    }

    public async Task<Result<PostResponse>> CreatePostAsync(CreatePostRequest request)
    {
        User? user = await _userRepo.GetByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<PostResponse>(new Error("CreatePost.UserNotFount", "User not found"));

        Post newPost = await _postRepo.AddAsync(request.ToEntity());
        return Result.Success(newPost.ToResponse(_fileUploadBaseUrl));
    }

    public Task<Result> DeletePostAsync(Guid postId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<PostResponse>>> GetAllPostAsync()
    {
        IEnumerable<PostResponse> posts = (await _postRepo.GetAllAsync())
            .Select(p => p.ToResponse(_fileUploadBaseUrl));
        return Result.Success(posts);
    }

    public async Task<Result<PostResponse>> GetPostByIdAsync(Guid postId)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));

        return Result.Success(post.ToResponse(_fileUploadBaseUrl));
    }

    public Task<Result<PostResponse>> EditPostAsync(EditPostRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdatePostPrivacyAsync(Guid postId, ChangePostPrivacyRequest request)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure(new Error("Post.NotFound", "Post not found"));

        await _postRepo.UpdatePrivacy(postId, request.Privacy);
        return Result.Success();
    }

    public async Task<Result<PostResponse>> UpdatePostAsync(Guid postId, UpdatePostRequest request)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));
        
        post.Content = request.Content;
        post.Privacy = request.Privacy;
        await _postRepo.UpdateAsync(post);
        return Result.Success(post.ToResponse(_fileUploadBaseUrl));
    }

}
