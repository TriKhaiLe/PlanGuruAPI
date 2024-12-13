using Application.Common.Interface.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repository
{
    public class PlantPostRepository : IPlantPostRepository
    {
        private readonly PlanGuruDBContext _context;

        public PlantPostRepository(PlanGuruDBContext context)
        {
            _context = context;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public IQueryable<Post> QueryPosts()
        {
            return _context.Posts.AsQueryable();
        }

        public async Task AddPostUpvoteAsync(PostUpvote postUpvote)
        {
            _context.PostUpvotes.Add(postUpvote);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePostUpvoteAsync(PostUpvote postUpvote)
        {
            _context.PostUpvotes.Remove(postUpvote);
            await _context.SaveChangesAsync();
        }

        public async Task AddPostDevoteAsync(PostDevote postDevote)
        {
            _context.PostDevotes.Add(postDevote);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePostDevoteAsync(PostDevote postDevote)
        {
            _context.PostDevotes.Remove(postDevote);
            await _context.SaveChangesAsync();
        }

        public async Task<PostUpvote?> GetPostUpvoteAsync(Guid userId, Guid postId)
        {
            return await _context.PostUpvotes
                .FirstOrDefaultAsync(pu => pu.UserId == userId && pu.PostId == postId);
        }

        public async Task<PostDevote?> GetPostDevoteAsync(Guid userId, Guid postId)
        {
            return await _context.PostDevotes
                .FirstOrDefaultAsync(pd => pd.UserId == userId && pd.PostId == postId);
        }

        public async Task<int> GetPostUpvoteCountAsync(Guid postId)
        {
            return await _context.PostUpvotes.CountAsync(pu => pu.PostId == postId);
        }

        public async Task UpdatePostAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ApprovePostByAdmin(Post post)
        {
            post.IsApproved = true;
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if(post == null)
            {
                throw new Exception("Can't find this post");
            }
            return post;
        }

        public async Task<List<Post>> GetUnApprovedPost()
        {
            var listPost = await _context.Posts.Where(
                p => p.IsApproved == false)
                .Include(p => p.PostComments)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostComments)
                .Include(p => p.PostShares)
                .ToListAsync();
            return listPost;
        }

        public async Task<List<Post>> GetApprovedPost()
        {
            return await _context.Posts.Where(
                p => p.IsApproved == true)
                .Include(p => p.PostComments)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostComments)
                .Include(p => p.PostShares)
                .ToListAsync();
        }
    }
}
