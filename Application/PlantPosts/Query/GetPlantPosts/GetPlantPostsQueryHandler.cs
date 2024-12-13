using Application.Common.Interface.Persistence;
using Application.PlantPosts.Common.GetPlantPosts;
using Application.Votes;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlantPosts.Query.GetPlantPosts
{
    public class GetPlantPostsQueryHandler : IRequestHandler<GetPlantPostsQuery, GetPlantPostResult>
    {
        private readonly IPlantPostRepository _postRepo;
        private readonly VoteStrategyFactory _voteStrategyFactory;

        public GetPlantPostsQueryHandler(IPlantPostRepository postRepo, VoteStrategyFactory voteStrategyFactory)
        {
            _postRepo = postRepo;
            _voteStrategyFactory = voteStrategyFactory;
        }

        public async Task<GetPlantPostResult> Handle(GetPlantPostsQuery request, CancellationToken cancellationToken)
        {
            var skip = (request.Page - 1) * request.Limit;

            var baseQuery = _postRepo.QueryPosts()
                                     .Include(p => p.User)
                                     .Include(p => p.PostComments)
                                     .Include(p => p.PostShares);

            IQueryable<Post> query = baseQuery;
            query = query.Where(p => p.IsApproved == true);

            if (!string.IsNullOrEmpty(request.Tag))
            {
                query = query.Where(p => p.Tag == request.Tag);
            }

            var posts = await query.Skip(skip).Take(request.Limit).ToListAsync(cancellationToken);

            var postDtos = new List<PlantPostDto>();

            foreach (var post in posts)
            {
                var postVoteStrategy = _voteStrategyFactory.GetStrategy(TargetType.Post.ToString());
                var upvoteCount = await postVoteStrategy.GetVoteCountAsync(post.Id, TargetType.Post, true);
                var devoteCount = await postVoteStrategy.GetVoteCountAsync(post.Id, TargetType.Post, false);

                var hasUpvoted = await postVoteStrategy.HasUpvotedAsync(request.UserId, post.Id);
                var hasDevoted = await postVoteStrategy.HasDevotedAsync(request.UserId, post.Id);

                var postDto = new PlantPostDto(
                    post.Id,
                    post.UserId,
                    post.User.Name,
                    post.User.Avatar,
                    post.Title,
                    post.Description,
                    post.ImageUrl,
                    post.Tag,
                    post.Background,
                    upvoteCount,
                    devoteCount,
                    post.PostComments.Count,
                    post.PostShares.Count,
                    FormatCreatedAt(post.CreatedAt),
                    hasUpvoted, // Thêm thông tin này
                    hasDevoted  // Thêm thông tin này
                );

                postDtos.Add(postDto);
            }

            switch (request.Filter?.ToLower())
            {
                case "trending":
                    postDtos = postDtos.OrderByDescending(p => p.NumberOfUpvote + p.NumberOfDevote + p.NumberOfComment).ToList();
                    break;
                case "upvote":
                    postDtos = postDtos.OrderByDescending(p => p.NumberOfUpvote).ToList();
                    break;
                case "time":
                default:
                    postDtos = postDtos.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
            }

            var totalPosts = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalPosts / (double)request.Limit);

            return new GetPlantPostResult(postDtos, totalPages);
        }

        public static string FormatCreatedAt(DateTime createdAt)
        {
            var timeSpan = DateTime.UtcNow - createdAt;
            if (timeSpan.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (timeSpan.TotalHours < 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }
            else if (timeSpan.TotalHours < 24)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }
            else if (timeSpan.TotalDays < 7)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 day ago" : $"{days} days ago";
            }
            else
            {
                return createdAt.ToString("dd-MM-yyyy HH:mm");
            }
        }
    }
}
