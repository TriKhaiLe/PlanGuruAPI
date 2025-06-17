using Application.Common.Interface.Persistence;
using Application.Wikies.Commands;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Domain.Entities.WikiEntities;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs.WikiDTOs;
using Application.Wikies;

namespace PlanGuruAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContributionsController : ControllerBase
    {
        private readonly IWikiRepository _wikiRepository;
        private readonly WikiCommandManager _invoker;

        public ContributionsController(IWikiRepository wikiRepository, WikiCommandManager invoker)
        {
            _wikiRepository = wikiRepository;
            _invoker = invoker;
        }

        [HttpPost("{wikiId}/contributions/{contributionId}/approve")]
        public async Task<IActionResult> ApproveContribution(Guid wikiId, Guid contributionId)
        {
            var approveCommand = new ApproveContributionCommand(_wikiRepository, wikiId, contributionId);
            _invoker.SetCommand(approveCommand);
            await _invoker.ExecuteCommandAsync();

            var updatedWiki = await _wikiRepository.GetByIdAsync(wikiId);
            if (updatedWiki == null)
            {
                return NotFound("Wiki not found");
            }

            var wikiDto = new
            {
                updatedWiki.Content,
                ContributorsCount = updatedWiki.Contributors.Count,
            };

            return Ok(wikiDto);
        }

        [HttpPost("{wikiId}/contributions/undo-last-operation")]
        public async Task<IActionResult> UndoLastOperation(Guid wikiId)
        {
            // Use shared invoker instance for undo
            var undoResult = await _invoker.UndoLastCommandAsync();

            if (!undoResult.Success)
            {
                return Ok(new UndoResponse
                {
                    Msg = undoResult.Message
                });
            }

            // Lấy thông tin phiên bản hiện tại sau khi undo
            var updatedWiki = await _wikiRepository.GetByIdAsync(wikiId);

            var result = new UndoResponse(
                undoResult.Message,
                _invoker.CanUndo,
                updatedWiki?.Content,
                updatedWiki?.Contributors?.Count ?? 0
            );

            return Ok(result);
        }

        [HttpGet("{wikiId}/pending-contributions")]
        public async Task<IActionResult> GetPendingContributions(Guid wikiId)
        {
            var pendingContributions = await _wikiRepository.GetPendingContributionsAsync(wikiId);
            return Ok(pendingContributions);
        }

        [HttpGet("{wikiId}/contributions/{contributionId}")]
        public async Task<IActionResult> GetOriginalAndContributionContent(Guid wikiId, Guid contributionId)
        {
            var originalContent = await _wikiRepository.GetOriginalContentAsync(wikiId);
            var contributionContent = await _wikiRepository.GetContributionContentAsync(contributionId);

            if (originalContent == null || contributionContent == null)
            {
                return NotFound();
            }

            var diffBuilder = new InlineDiffBuilder(new DiffPlex.Differ());
            var diff = diffBuilder.BuildDiffModel(originalContent, contributionContent);

            var diffLines = new List<DiffLine>();
            foreach (var line in diff.Lines)
            {
                var diffType = line.Type switch
                {
                    ChangeType.Inserted => DiffType.Added,
                    ChangeType.Deleted => DiffType.Deleted,
                    _ => DiffType.Unchanged
                };

                diffLines.Add(new DiffLine
                {
                    Content = line.Text,
                    Type = diffType
                });
            }

            var result = new ContentDiffResult
            {
                OriginalContent = originalContent,
                ContributionContent = contributionContent,
                DiffLines = diffLines
            };

            return Ok(result);
        }

        [HttpPost("{wikiId}/contributions/{contributionId}/reject")]
        public async Task<IActionResult> RejectContribution(Guid wikiId, Guid contributionId, [FromBody] string reason)
        {
            var rejectCommand = new RejectContributionCommand(_wikiRepository, wikiId, contributionId, reason);
            _invoker.SetCommand(rejectCommand);
            await _invoker.ExecuteCommandAsync();

            var updatedWiki = await _wikiRepository.GetByIdAsync(wikiId);
            if (updatedWiki == null)
            {
                return NotFound("Wiki not found");
            }

            var wikiDto = new
            {
                updatedWiki.Content,
                ContributorsCount = updatedWiki.Contributors.Count,
            };

            return Ok(wikiDto);
        }

        [HttpGet("{wikiId}/contribution-history")]
        public async Task<IActionResult> GetContributionHistory(Guid wikiId)
        {
            var contributionHistory = await _wikiRepository.GetContributionHistoryAsync(wikiId);
            return Ok(contributionHistory);
        }

        [HttpPost("{wikiId}/contributions")]
        public async Task<IActionResult> CreateContribution(Guid wikiId, [FromBody] CreateContributionRequest request)
        {
            var wiki = await _wikiRepository.GetByIdAsync(wikiId);
            if (wiki == null)
            {
                return NotFound("Wiki not found");
            }

            var contribution = new Contribution
            {
                WikiId = wikiId,
                Content = request.Content,
                ContributorId = request.ContributorId,
                Status = ContributionStatus.Pending
            };

            await _wikiRepository.AddContributionAsync(contribution);

            // Map to DTO
            var contributionDto = new
            {
                Id = contribution.Id,
                WikiId = contribution.WikiId,
                Content = contribution.Content,
                Status = contribution.Status,
                ContributorId = contribution.ContributorId
            };

            return Ok(contributionDto);
        }
    }
}
