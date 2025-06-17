using Application.Common.Interface.Persistence;
using Application.Wikies.Memento;
using Domain.Entities;
using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;

namespace Application.Wikies.Commands
{
    public class ApproveContributionCommand : IWikiCommand
    {
        private readonly IWikiRepository _wikiRepository;
        private readonly Guid _wikiId;
        private readonly Guid _contributionId;
        public string? Description => $"Duyệt đóng góp (Approve Contribution) cho WikiId: {_wikiId}, ContributionId: {_contributionId}";

        public ApproveContributionCommand(IWikiRepository wikiRepository, Guid wikiId, Guid contributionId)
        {
            _wikiRepository = wikiRepository;
            _wikiId = wikiId;
            _contributionId = contributionId;
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
            await _wikiRepository.ApproveContributionAsync(_wikiId, _contributionId);
        }

        public async Task UndoAsync()
        {
            var memento = WikiMementoStack.Instance.Pop(_wikiId);
            if (memento == null)
                return;

            var wiki = await _wikiRepository.GetByIdAsync(_wikiId);
            if (wiki == null)
                return;

            RestoreWikiFromMemento(wiki, memento);

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
            await _wikiRepository.RevertContributorAsync(_wikiId, _contributionId);
        }

        private void RestoreWikiFromMemento(Wiki wiki, WikiMemento memento)
        {
            wiki.Content = memento.Content;
            wiki.Title = memento.Title;
            wiki.Description = memento.Description;
            wiki.ThumbnailImageUrl = memento.ThumbnailImageUrl;
            wiki.QuizIds = new System.Collections.Generic.List<string>(memento.QuizIds);
            wiki.Contributors = wiki.Contributors?.FindAll(u => memento.ContributorIds.Contains(u.Id)) ?? new System.Collections.Generic.List<User>();
        }
    }
}