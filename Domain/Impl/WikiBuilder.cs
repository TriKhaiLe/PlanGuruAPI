using Domain.Entities.ECommerce;
using Domain.Entities.WikiService;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Impl
{
    public class WikiBuilder : IWikiBuilder
    {
        private readonly Wiki _wiki;

        public WikiBuilder()
        {
            _wiki = new Wiki
            {
                Contributors = new List<User>(),
                QuizIds = new List<string>(),
                AttachedProducts = new List<Product>(),
                Status = WikiStatus.Pending,
                Upvotes = 0,
                Downvotes = 0
            };
        }

        public IWikiBuilder WithTitle(string title)
        {
            _wiki.Title = title;
            return this;
        }

        public IWikiBuilder WithDescription(string description)
        {
            _wiki.Description = description;
            return this;
        }

        public IWikiBuilder WithThumbnailImageUrl(string thumbnailUrl)
        {
            _wiki.ThumbnailImageUrl = thumbnailUrl;
            return this;
        }

        public IWikiBuilder WithAuthor(User author)
        {
            _wiki.AuthorId = author.UserId;
            _wiki.Contributors.Add(author);
            return this;
        }

        public IWikiBuilder WithProducts(IEnumerable<Product> products)
        {
            _wiki.AttachedProducts = products.ToList();
            return this;
        }

        public IWikiBuilder WithStatus(WikiStatus status)
        {
            _wiki.Status = status;
            return this;
        }

        public IWikiBuilder WithInitialVotes(int upvotes = 0, int downvotes = 0)
        {
            _wiki.Upvotes = upvotes;
            _wiki.Downvotes = downvotes;
            return this;
        }

        public Wiki Build()
        {
            return _wiki;
        }
    }

}
