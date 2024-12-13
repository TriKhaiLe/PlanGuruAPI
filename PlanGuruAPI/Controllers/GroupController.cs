using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.AdminDTOs;
using PlanGuruAPI.DTOs.GroupDTOs;
using PlanGuruAPI.DTOs.PlantPostDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly IMapper _mapper;

        public GroupController(PlanGuruDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGroup()
        {
            var listGroup = await _context.Groups.ToListAsync();
            return Ok(listGroup.Select(p => new { p.Id,  p.GroupName, p.MasterUserId }).ToList());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound("Can't find this group");
            }
            return Ok(new { group.Id, group.GroupName, group.MasterUserId });
        }
        [HttpGet("search/{searchName}")]
        public async Task<IActionResult> SearchGroup(string searchName)
        {
            var listGroup = await _context.Groups.Where(p => p.GroupName.ToLower().Contains(searchName.ToLower())).ToListAsync();
            return Ok(listGroup.Select(p => new {p.Id, p.GroupName, p.MasterUserId}));
        }
        [HttpGet("posts/pending/{groupId}")]
        public async Task<IActionResult> GetPendingPost(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if(group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listPost = await _context.Posts
                .Where(p => p.GroupId == groupId && p.IsApproved == false)
                .Include(p => p.PostComments)
                .Include(p => p.PostUpvotes)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostShares)
                .Include(p => p.User).ToListAsync();
            return Ok(_mapper.Map<List<PostInGroupDTO>>(listPost));
        }
        [HttpGet("posts/approved/{groupId}")]
        public async Task<IActionResult> GetPostsInGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listPost = await _context.Posts
                .Where(p => p.GroupId == groupId && p.IsApproved == true)
                .Include(p => p.PostComments)
                .Include(p => p.PostUpvotes)
                .Include(p => p.PostDevotes)
                .Include(p => p.PostShares)
                .Include(p => p.User).ToListAsync();
            return Ok(_mapper.Map<List<PostInGroupDTO>>(listPost));
        }
        [HttpGet("users/{groupId}")]
        public async Task<IActionResult> GetUserInGroup(Guid groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var listUser = await _context.GroupUsers.Where(p => p.GroupId == groupId).Include(p => p.User).Select(p => new { p.User.UserId, p.User.Name, p.User.Avatar}).ToListAsync();
            return Ok(listUser);    
        }
        [HttpPost("posts/approvePost")]
        public async Task<IActionResult> ApprovePost(ApprovePostRequest request)
        {
            var post = await _context.Posts.FindAsync(request.PostId);
            if (post == null)
            {
                return BadRequest("Can't find this post");
            }
            post.IsApproved = true;
            await _context.SaveChangesAsync();
            return Ok("Post is approved");
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.MasterUserId);
            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            var group = _mapper.Map<Group>(request);
            group.Id = Guid.NewGuid();
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
            return Ok(new { group.Id, group.GroupName, group.MasterUserId });
        }
        [HttpPost("join")]
        public async Task<IActionResult> JoinGroup(JoinGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);  
            var group = await _context.Groups.FindAsync(request.GroupId);   
            if(user == null)
            {
                return BadRequest("Can't find this user");
            }
            if(group == null)
            {
                return BadRequest("Can't find this group");
            }
            var groupUser = _mapper.Map<GroupUser>(request);
            await _context.GroupUsers.AddAsync(groupUser);
            await _context.SaveChangesAsync();
            return Ok("Join group successfully");
        }
        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost(CreatePostInGroupRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (user == null)
            {
                return BadRequest("Can't find this user");
            }
            if (group == null)
            {
                return BadRequest("Can't find this group");
            }
            var post = _mapper.Map<Post>(request);
            post.Id = Guid.NewGuid();           
            post.IsApproved = false;
            await _context.Posts.AddAsync(post);    
            await _context.SaveChangesAsync();  
            return Ok("Post created successfully, wating for admin approve");
        }
    }
}
