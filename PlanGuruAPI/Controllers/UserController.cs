using Application.Users.Command.SetNameAndAvatar;
using Application.Users.Command.SignUp;
using Application.Users.Common;
using Application.Users.Querry.Login;
using AutoMapper;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.UserDTOs;
using System.Diagnostics.Eventing.Reader;

namespace PlanGuruAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public UserController(PlanGuruDBContext context, ISender mediator, IMapper mapper)
        {
            _context = context;
            _mediator = mediator;
            _mapper = mapper;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var querry = _mapper.Map<LoginQuerry>(loginRequest);
            var loginResult = await _mediator.Send(querry); 
            return Ok(loginResult);
        }
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequest signUpRequest)
        {
            var command = _mapper.Map<SignUpCommand>(signUpRequest);
            var signUpResult = await _mediator.Send(command);       
            return Ok(signUpResult);
        }
        [HttpPost("setNameAndAvatar")]
        public async Task<IActionResult> SetNameAndAvatar(SetNameAndAvatarRequest setNameAndAvatarRequest)
        {
            var command = _mapper.Map<SetNameAndAvatarCommand>(setNameAndAvatarRequest);
            var result = await _mediator.Send(command);
            return Ok(result);      
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _context.Users.Select(p => new {p.UserId, p.Name, p.Avatar, p.Email, p.IsHavePremium}).ToListAsync());
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user == null)
            {
                return NotFound("Can't find this user");
            }
            return Ok(new { user.UserId, user.Name, user.Avatar, user.Email, user.IsHavePremium });
        }
        [HttpPost("goPremium")]
        public async Task<IActionResult> GoPremium(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user == null)
            {
                return NotFound("Can't find this user");
            }
            if (user.IsHavePremium)
            {
                return BadRequest("This user is already have premium");
            }
            user.IsHavePremium = true;
            await _context.SaveChangesAsync();
            return Ok("Upgrade account successfully");
        }
        [HttpPost("removePremium")]
        public async Task<IActionResult> RemovePremium(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Can't find this user");
            }
            if (!user.IsHavePremium)
            {
                return BadRequest("This user don't have premium");
            }
            user.IsHavePremium = false;
            await _context.SaveChangesAsync();
            return Ok("Remove premium from this account successfully");
        }
    }
}
