using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interface.Persistence
{
    public interface IWikiRepository
    {
        Task AddWikiAsync(Wiki wiki);
        Task<Wiki?> GetByIdAsync(Guid id);
        Task UpdateWikiAsync(Wiki wiki, List<Guid> productIds);
        Task<List<Wiki>> GetAllAsync();
        Task<List<Contribution>> GetPendingContributionsAsync(Guid wikiId);
        Task<string?> GetOriginalContentAsync(Guid wikiId);
        Task<string?> GetContributionContentAsync(Guid contributionId);
        Task<bool> ApproveContributionAsync(Guid wikiId, Guid contributionId);
        Task<bool> RejectContributionAsync(Guid wikiId, Guid contributionId, string reason);
        Task<List<Contribution>> GetContributionHistoryAsync(Guid wikiId);
        Task AddContributionAsync(Contribution contribution);
        Task<bool> RevertContributorAsync(Guid wikiId, Guid contributionId);
    }
}
