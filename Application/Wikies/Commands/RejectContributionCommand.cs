using Application.Common.Interface.Persistence;
using Application.Wikies.Memento;
using Domain.Entities;
using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;
using System;
using System.Threading.Tasks;

namespace Application.Wikies.Commands
{
    public class RejectContributionCommand : IWikiCommand
    {
        private readonly IWikiRepository _wikiRepository;
        private readonly Guid _wikiId;
        private readonly Guid _contributionId;
        private readonly string _reason;
        public string? Description => $"Từ chối đóng góp (Reject Contribution) cho WikiId: {_wikiId}, ContributionId: {_contributionId}";

        public RejectContributionCommand(IWikiRepository wikiRepository, Guid wikiId, Guid contributionId, string reason)
        {
            _wikiRepository = wikiRepository;
            _wikiId = wikiId;
            _contributionId = contributionId;
            _reason = reason;
        }

        public async Task ExecuteAsync()
        {
            var wiki = await _wikiRepository.GetByIdAsync(_wikiId);
            Contribution? contribution = null;
            if (wiki != null)
            {
                contribution = wiki.Contributions?.Find(c => c.Id == _contributionId);
                WikiMementoStack.Instance.Push(_wikiId, new WikiMemento(wiki, contribution));
            }
            await _wikiRepository.RejectContributionAsync(_wikiId, _contributionId, _reason);
        }

        public async Task UndoAsync()
        {
            var memento = WikiMementoStack.Instance.Pop(_wikiId);
            if (memento == null)
                return;

            var wiki = await _wikiRepository.GetByIdAsync(_wikiId);
            if (wiki == null)
                return;

            RestoreFromMemento(wiki, memento);

            // Restore contribution state
            if (memento.ContributionSnapshot != null)
            {
                var contribution = wiki.Contributions?.Find(c => c.Id == memento.ContributionSnapshot.ContributionId);
                if (contribution != null)
                {
                    contribution.Status = memento.ContributionSnapshot.Status;
                    contribution.RejectionReason = memento.ContributionSnapshot.RejectionReason;
                    contribution.Content = memento.ContributionSnapshot.Content;
                }
            }

            await _wikiRepository.UpdateWikiAsync(wiki, wiki.AttachedProducts?.ConvertAll(p => p.Id) ?? new System.Collections.Generic.List<Guid>());
        }

        private void RestoreFromMemento(Wiki wiki, WikiMemento memento)
        {
            wiki.Content = memento.Content;
            wiki.Title = memento.Title;
            wiki.Description = memento.Description;
            wiki.ThumbnailImageUrl = memento.ThumbnailImageUrl;
            wiki.QuizIds = memento.QuizIds;
            wiki.Contributors = wiki.Contributors?.FindAll(u => memento.ContributorIds.Contains(u.Id)) ?? new System.Collections.Generic.List<User>();
        }
    }
}