using System;
using System.Collections.Generic;
using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;

namespace Application.Wikies.Memento
{
    // stores a snapshot of Wiki state
    public class WikiMemento
    {
        public Guid WikiId { get; }
        public string Content { get; }
        public List<Guid> ContributorIds { get; }
        public string Title { get; }
        public string Description { get; }
        public string ThumbnailImageUrl { get; }
        public List<string> QuizIds { get; }

        public ContributionSnapshot? ContributionSnapshot { get; }

        public WikiMemento(Wiki wiki, Contribution? contribution = null)
        {
            WikiId = wiki.Id;
            Content = wiki.Content;
            ContributorIds = new List<Guid>(wiki.Contributors?.ConvertAll(u => u.Id) ?? new List<Guid>());
            Title = wiki.Title;
            Description = wiki.Description;
            ThumbnailImageUrl = wiki.ThumbnailImageUrl;
            QuizIds = new List<string>(wiki.QuizIds ?? new List<string>());
            if (contribution != null)
            {
                ContributionSnapshot = new ContributionSnapshot(contribution);
            }
        }
    }

    // Helper class to store contribution state
    public class ContributionSnapshot
    {
        public Guid ContributionId { get; }
        public ContributionStatus Status { get; }
        public string? RejectionReason { get; }
        public string Content { get; }

        public ContributionSnapshot(Contribution contribution)
        {
            ContributionId = contribution.Id;
            Status = contribution.Status;
            RejectionReason = contribution.RejectionReason;
            Content = contribution.Content;
        }
    }
}