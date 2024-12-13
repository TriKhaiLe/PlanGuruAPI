using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface IPlantPostRepository
    {
        Task<Post> CreatePostAsync(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
        IQueryable<Post> QueryPosts();
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(Guid postId);
        Task ApprovePostByAdmin(Post post);
        Task<List<Post>> GetUnApprovedPost();
        Task<List<Post>> GetApprovedPost();

        Task AddPostUpvoteAsync(PostUpvote postUpvote);
        Task RemovePostUpvoteAsync(PostUpvote postUpvote);
        Task AddPostDevoteAsync(PostDevote postDevote);
        Task RemovePostDevoteAsync(PostDevote postDevote);
        Task<PostUpvote> GetPostUpvoteAsync(Guid userId, Guid postId);
        Task<PostDevote> GetPostDevoteAsync(Guid userId, Guid postId);
        Task<int> GetPostUpvoteCountAsync(Guid postId);

    }
}
