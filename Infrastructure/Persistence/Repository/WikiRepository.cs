using Application.Common.Interface.Persistence;
using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repository
{
    public class WikiRepository : IWikiRepository
    {
        private readonly PlanGuruDBContext _context;

        public WikiRepository(PlanGuruDBContext context)
        {
            _context = context;
        }

        public async Task AddWikiAsync(Wiki wiki)
        {
            await _context.Wikis.AddAsync(wiki);
            await _context.SaveChangesAsync();
        }

        public async Task<Wiki?> GetByIdAsync(Guid id)
        {
            return await _context.Wikis
                .Include(w => w.Contributors)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task UpdateWikiAsync(Wiki wiki, List<Guid> productIds)
        {
            var existingWiki = await _context.Wikis
                .FirstOrDefaultAsync(w => w.Id == wiki.Id);

            if (existingWiki != null)
            {
                existingWiki.Title = wiki.Title;
                existingWiki.Description = wiki.Description;
                existingWiki.ThumbnailImageUrl = wiki.ThumbnailImageUrl;
                existingWiki.Content = wiki.Content;
                existingWiki.AttachedProducts = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Wiki>> GetAllAsync()
        {
            return await _context.Wikis
                .Include(w => w.Contributors)
                .ToListAsync();
        }

        public async Task<List<Contribution>> GetPendingContributionsAsync(Guid wikiId)
        {
            return await _context.Contributions
                .Where(c => c.WikiId == wikiId && c.Status == ContributionStatus.Pending)
                .ToListAsync();
        }

        public async Task<string?> GetOriginalContentAsync(Guid wikiId)
        {
            var wiki = await _context.Wikis.FindAsync(wikiId);
            return wiki?.Content;
        }

        public async Task<string?> GetContributionContentAsync(Guid contributionId)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);
            return contribution?.Content;
        }

        public async Task<bool> ApproveContributionAsync(Guid wikiId, Guid contributionId)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);
            if (contribution == null) return false;

            var wiki = await _context.Wikis
                .Include(w => w.Contributors)
                .FirstOrDefaultAsync(w => w.Id == wikiId);
            if (wiki == null) return false;

            wiki.Content = contribution.Content;
            contribution.Status = ContributionStatus.Approved;

            if (!wiki.Contributors.Any(c => c.Id == contribution.ContributorId))
            {
                var contributor = await _context.Users.FindAsync(contribution.ContributorId);
                if (contributor != null)
                {
                    wiki.Contributors.Add(contributor);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectContributionAsync(Guid wikiId, Guid contributionId, string reason)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);
            if (contribution == null) return false;

            contribution.Status = ContributionStatus.Rejected;
            contribution.RejectionReason = reason;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Contribution>> GetContributionHistoryAsync(Guid wikiId)
        {
            return await _context.Contributions
                .Where(c => c.WikiId == wikiId && (c.Status == ContributionStatus.Approved || c.Status == ContributionStatus.Rejected))
                .ToListAsync();
        }

        public async Task AddContributionAsync(Contribution contribution)
        {
            await _context.Contributions.AddAsync(contribution);
            await _context.SaveChangesAsync();
        }

        // Undo approval of a contribution and revert contributor if they were just added
        public async Task<bool> RevertContributorAsync(Guid wikiId, Guid contributionId)
        {
            var wiki = await GetByIdAsync(wikiId);
            if (wiki == null)
                return false;

            var contribution = wiki.Contributions.FirstOrDefault(c => c.Id == contributionId);
            if (contribution == null)
                return false;

            var contributorId = contribution.ContributorId;

            // Only revert if the contributor was just added by the last approval:
            // check exist contributor in this wiki & count number of contribution = 1
            var wasContributorJustAdded = wiki.Contributors.Any(u => u.Id == contributorId) &&
                wiki.Contributions.Count(c => c.ContributorId == contributorId && c.Status == ContributionStatus.Approved) == 1;

            // Set contribution status back to Pending
            contribution.Status = ContributionStatus.Pending;

            // Remove contributor if they were just added by this approval
            if (wasContributorJustAdded)
            {
                var userToRemove = wiki.Contributors.FirstOrDefault(u => u.Id == contributorId);
                if (userToRemove != null)
                {
                    wiki.Contributors.Remove(userToRemove);
                }
            }

            await UpdateWikiAsync(wiki, wiki.AttachedProducts?.Select(p => p.Id).ToList() ?? new List<Guid>());
            return true;
        }
    }
}
