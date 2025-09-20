using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialWebsite.DTOs.comment;
using SocialWebsite.Entities;
using SocialWebsite.Interfaces.Repositories;
using SocialWebsite.Shared.Enums;

namespace SocialWebsite.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SocialWebsiteContext _context;

        public CommentRepository(SocialWebsiteContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Comment> AddAsync(Comment entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Comment entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            _context.Comments.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Comment>> GetRepliesByCommentId(Guid parentCommentId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.ParentCommentId != null && c.ParentCommentId == parentCommentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetRootCommentsByPostId(Guid postId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.ParentCommentId == null)
                .ToListAsync();
        }

        public async Task UpdateAsync(Comment entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            _context.Comments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentResponse>> GetRootCommentResponsesByPostId(Guid postId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponse(
                    c.Id,
                    c.PostId,
                    c.UserId,
                    c.ParentCommentId,
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt,
                    _context.Likes.Count(l => l.TargetId == c.Id && l.Type == LikeType.Comment),
                    _context.Comments.Count(r => r.ParentCommentId == c.Id)
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<CommentResponse>> GetReplyResponsesByCommentId(Guid parentCommentId)
        {
            return await _context.Comments
                .AsNoTracking()
                .Where(c => c.ParentCommentId == parentCommentId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponse(
                    c.Id,
                    c.PostId,
                    c.UserId,
                    c.ParentCommentId,
                    c.Content,
                    c.CreatedAt,
                    c.UpdatedAt,
                    _context.Likes.Count(l => l.TargetId == c.Id && l.Type == LikeType.Comment),
                    _context.Comments.Count(r => r.ParentCommentId == c.Id)
                ))
                .ToListAsync();
        }
    }
}
