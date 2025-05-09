using Domain.Entities.ECommerce;
using Domain.Entities.WikiService;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWikiBuilder
    {
        IWikiBuilder WithTitle(string title);
        IWikiBuilder WithDescription(string description);
        IWikiBuilder WithThumbnailImageUrl(string thumbnailUrl);
        IWikiBuilder WithAuthor(User author);
        IWikiBuilder WithProducts(IEnumerable<Product> products);
        IWikiBuilder WithStatus(WikiStatus status);
        IWikiBuilder WithInitialVotes(int upvotes = 0, int downvotes = 0);
        Wiki Build();
    }
}
