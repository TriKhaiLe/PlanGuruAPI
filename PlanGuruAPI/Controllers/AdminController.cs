using Application.Common.Interface.Persistence;
using Application.PlantPosts.Common.GetPlantPosts;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs.AdminDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPlantPostRepository _planPostRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public AdminController(IPlantPostRepository plantPostRepository, IMapper mapper, IUserRepository userRepository)
        {
            _planPostRepository = plantPostRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        [HttpPost("approvePost")]
        public async Task<IActionResult> ApprovePost([FromBody]ApprovePostRequest request)
        {
            var post = await _planPostRepository.GetPostByIdAsync(request.PostId);  
            await _planPostRepository.ApprovePostByAdmin(post);
            return Ok("Post approved");
        }
        [HttpGet("unApprovePosts")]
        public async Task<IActionResult> GetUnApprovePost()
        {
            var listUnApprovedPost = await _planPostRepository.GetUnApprovedPost();
            var listUnApprovedPostReadDTO = new List<PostReadDTO>();
            foreach (var post in listUnApprovedPost)
            {
                var user = await _userRepository.GetByIdAsync(post.UserId);
                var postReadDTO = new PostReadDTO()
                {
                    UserId = user.UserId,
                    UserAvatar = user.Avatar,
                    UserNickName = user.Name,
                    Background = post.Background,
                    CreatedDate = FormatCreatedAt(post.CreatedAt),
                    Description = post.Description,
                    ImageUrl = post.ImageUrl,
                    NumberOfComment = post.PostComments.Count,
                    NumberOfDevote = post.PostDevotes.Count,
                    NumberOfShare = post.PostShares.Count,
                    NumberOfUpvote = post.PostUpvotes.Count,
                    PostId = post.Id,
                    Tag = post.Tag,
                    Title = post.Title
                };
                listUnApprovedPostReadDTO.Add(postReadDTO);
            }
            return Ok(listUnApprovedPostReadDTO);
        }
        [HttpPost("unApprovePost")]
        public async Task<IActionResult> UnApprovePost([FromBody]ApprovePostRequest request)
        {
            var post = await _planPostRepository.GetPostByIdAsync(request.PostId);
            await _planPostRepository.DeletePostAsync(post.Id);
            return Ok("Post un approved");
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
