using Application.Common.Interface.Persistence;
using Application.PlantPosts.Command.CreatePost;
using Application.PlantPosts.Query.GetPlantPosts;
using Application.PlantPosts.Query.GetTags;
using Application.Votes;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs;
using PlanGuruAPI.DTOs.CommentDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PlantPostController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IPlantPostRepository _postRepository;
        private readonly VoteStrategyFactory _voteStrategyFactory;

        public PlantPostController(PlanGuruDBContext context,
            ISender mediator,
            IMapper mapper,
            IPlantPostRepository postRepository,
            VoteStrategyFactory voteStrategyFactory)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
            _postRepository = postRepository;
            _voteStrategyFactory = voteStrategyFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePlantPostRequest request)
        {
            var command = _mapper.Map<CreatePlantPostCommand>(request);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("test/get-all")]
        public async Task<IActionResult> GetAllPlantPosts()
        {
            return Ok(await _postRepository.GetApprovedPost());
        }
        [HttpGet("plantPostUserCount")]
        public async Task<IActionResult> GetPlantPostUserCount()
        {
            var users = await _context.Users.ToListAsync();
            var posts = await _context.Posts.ToListAsync();
            return Ok(new { numberOfUser = users.Count, numberOfPost = posts.Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] Guid userId, [FromQuery] int limit = 9, [FromQuery] int page = 1, [FromQuery] string? tag = null, [FromQuery] string? filter = "time")
        {
            var query = new GetPlantPostsQuery(limit, page, userId, tag, filter);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("test/get-all-tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _mediator.Send(new GetTagsQuery());
            return Ok(tags);
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> UpvotePost([FromBody] UpvoteDto upvoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = upvoteDto.UserId,
                TargetId = upvoteDto.TargetId,
                TargetType = TargetType.Post,
                IsUpvote = true
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Upvote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }

        [HttpPost("devote")]
        public async Task<IActionResult> DevotePost([FromBody] DevoteDto devoteDto)
        {
            var voteDto = new VoteDto
            {
                UserId = devoteDto.UserId,
                TargetId = devoteDto.TargetId,
                TargetType = TargetType.Post,
                IsUpvote = false
            };

            var strategy = _voteStrategyFactory.GetStrategy(voteDto.TargetType.ToString());
            await strategy.HandleVoteAsync(voteDto.UserId, voteDto.TargetId, voteDto.IsUpvote);

            var upvoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, true);
            var devoteCount = await strategy.GetVoteCountAsync(voteDto.TargetId, voteDto.TargetType, false);

            return Ok(new { status = "success", message = "Devote processed successfully", numberOfUpvotes = upvoteCount, numberOfDevotes = devoteCount });
        }
    }
}
