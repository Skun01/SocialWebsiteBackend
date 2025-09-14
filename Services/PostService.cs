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
    private readonly IPostFileRepository _postFileRepo;
    private readonly IFileService _fileService;
    public PostService(IPostRepository postRepository, IConfiguration configuration,
        IUserRepository userRepository, IFileService fileService, IPostFileRepository postFileRepository)
    {
        _postRepo = postRepository;
        _fileUploadBaseUrl = configuration["FileUploadServer:BaseUrl"]!;
        _userRepo = userRepository;
        _fileService = fileService;
        _postFileRepo = postFileRepository;
    }

    public async Task<Result<PostResponse>> CreatePostAsync(CreatePostRequest request)
    {
        User? user = await _userRepo.GetByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<PostResponse>(new Error("CreatePost.UserNotFount", "User not found"));

        Post newPost = await _postRepo.AddAsync(request.ToEntity());
        return Result.Success(newPost.ToResponse(_fileUploadBaseUrl));
    }

    public async Task<Result> DeletePostAsync(Guid postId)
    {
        Post? post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            return Result.Failure<PostResponse>(new Error("Post.NotFound", "Post not found"));

        await _postRepo.DeleteAsync(post);
        return Result.Success();
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
        return Result.Success();
    }
}
