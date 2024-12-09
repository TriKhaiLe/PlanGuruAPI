﻿using Application.Comments.Command;
using Application.Common.Interface.Persistence;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs.CommentDTOs;

namespace PlanGuruAPI.Controllers
{
    [ApiController]
    [Route("api/posts/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICommentRepository _commentRepository;

        public CommentsController(ISender mediator, ICommentRepository commentRepository)
        {
            _mediator = mediator;
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            var command = new CreateCommentCommand(createCommentDto.PostId, createCommentDto.UserId, createCommentDto.Message);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpGet("test/get-all")]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            return Ok(comments);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateCommentDto)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.Message = updateCommentDto.Message;
            // Update other properties if needed

            await _commentRepository.UpdateCommentAsync(comment);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _commentRepository.GetCommentByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepository.DeleteCommentAsync(id);
            return NoContent();
        }

        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId, [FromQuery] Guid? parentCommentId = null)
        {
            var comments = await _commentRepository.GetCommentsByPostIdAsync(postId, parentCommentId);
            var commentDtos = comments.Select(c => new CommentDto
            {
                CommentId = c.Id,
                UserId = c.UserId,
                Name = c.User.Name,
                Avatar = c.User.Avatar,
                Message = c.Message,
                NumberOfUpvote = c.CommentUpvotes.Count,
                NumberOfDevote = c.CommentDevotes.Count,
                ReplyComment = new List<CommentDto>()
            }).ToList();

            return Ok(commentDtos);
        }

        [HttpPost("upvote")]
        public async Task<IActionResult> UpvoteComment([FromBody] UpvoteDto upvoteDto)
        {
            var commentUpvote = new CommentUpvote
            {
                UserId = upvoteDto.UserId,
                CommentId = upvoteDto.TargetId
            };

            var existingDevote = await _commentRepository.GetCommentDevoteAsync(upvoteDto.UserId, upvoteDto.TargetId);

            if (existingDevote != null)
            {
                await _commentRepository.RemoveCommentDevoteAsync(existingDevote);
            }

            var existingUpvote = await _commentRepository.GetCommentUpvoteAsync(upvoteDto.UserId, upvoteDto.TargetId);

            if (existingUpvote != null)
            {
                await _commentRepository.RemoveCommentUpvoteAsync(existingUpvote);
                var response2 = new
                {
                    status = "success",
                    message = "Remove upvote comment successfully",
                    numberOfUpvotes = await _commentRepository.GetCommentUpvoteCountAsync(upvoteDto.TargetId)
                };
                return Ok(response2);
            }

            await _commentRepository.AddCommentUpvoteAsync(commentUpvote);
            var response = new
            {
                status = "success",
                message = "Upvote comment successfully",
                numberOfUpvotes = await _commentRepository.GetCommentUpvoteCountAsync(upvoteDto.TargetId)
            };

            return Ok(response);
        }

        [HttpPost("devote")]
        public async Task<IActionResult> DevoteComment([FromBody] DevoteDto devoteDto)
        {
            var commentDevote = new CommentDevote
            {
                UserId = devoteDto.UserId,
                CommentId = devoteDto.TargetId
            };

            var existingUpvote = await _commentRepository.GetCommentUpvoteAsync(devoteDto.UserId, devoteDto.TargetId);

            if (existingUpvote != null)
            {
                await _commentRepository.RemoveCommentUpvoteAsync(existingUpvote);
            }

            var existingDevote = await _commentRepository.GetCommentDevoteAsync(devoteDto.UserId, devoteDto.TargetId);

            if (existingDevote != null)
            {
                await _commentRepository.RemoveCommentDevoteAsync(existingDevote);
                var response2 = new
                {
                    status = "success",
                    message = "Remove devote comment successfully",
                    numberOfUpvotes = await _commentRepository.GetCommentUpvoteCountAsync(devoteDto.TargetId)
                };
                return Ok(response2);
            }

            await _commentRepository.AddCommentDevoteAsync(commentDevote);
            var response = new
            {
                status = "success",
                message = "Devote comment successfully",
                numberOfUpvotes = await _commentRepository.GetCommentUpvoteCountAsync(devoteDto.TargetId)
            };

            return Ok(response);
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyComment([FromBody] ReplyCommentDto replyCommentDto)
        {
            var parentComment = await _commentRepository.GetCommentByIdAsync(replyCommentDto.ParentCommentId);
            if (parentComment == null)
            {
                return NotFound(new { message = "Parent comment not found" });
            }

            // Check if the parent comment is already a reply to another comment
            if (parentComment.ParentCommentId != Guid.Empty)
            {
                var grandParentComment = await _commentRepository.GetCommentByIdAsync(parentComment.ParentCommentId);
                if (grandParentComment?.ParentCommentId != Guid.Empty)
                {
                    return BadRequest(new { message = "Cannot reply to a comment more than 2 levels deep" });
                }
            }

            var replyComment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = replyCommentDto.UserId,
                PostId = parentComment.PostId,
                ParentCommentId = replyCommentDto.ParentCommentId,
                Message = replyCommentDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddCommentAsync(replyComment);

            return Ok(new { message = "Reply comment created successfully", replyCommentId = replyComment.Id });
        }
    }
}
