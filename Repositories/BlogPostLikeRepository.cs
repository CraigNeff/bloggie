﻿
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogPostLikeRepository(BloggieDbContext bloggieDbContext)
        {
            this._bloggieDbContext = bloggieDbContext;
        }

        public async Task<BlogPostLike?> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            var existingLike = await _bloggieDbContext.BlogPostLike.FirstOrDefaultAsync(x => x.BlogPostId == blogPostLike.BlogPostId && x.UserId == blogPostLike.UserId);

            if (existingLike !=null)
            {
                //like already exists, return null
                return null;
            }
            await _bloggieDbContext.BlogPostLike.AddAsync(blogPostLike);
            await _bloggieDbContext.SaveChangesAsync();

            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostLike.Where(x => x.BlogPostId == blogPostId)
                .ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await _bloggieDbContext.BlogPostLike
                .CountAsync(x => x.BlogPostId == blogPostId);
        }
    }
}
