using Application.Common.Interface.Persistence;
using Application.PlantPosts.Query.GetPlantPosts;
using Domain.Entities;
using Domain.Entities.WikiService;
using Domain.Impl;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs.WikiDTOs;

namespace PlanGuruAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WikiController : ControllerBase
    {
        private readonly IWikiRepository _wikiRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public WikiController(IWikiRepository wikiRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _wikiRepository = wikiRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpPost("CreateWikiArticle")]
        public async Task<IActionResult> CreateWikiArticle([FromBody] CreateWikiArticleRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new CreateWikiArticleResponse
                {
                    Success = false,
                    Message = "Title is required."
                });
            }

            var attachedProducts = await _productRepository.GetProductsByIdsAsync(request.ProductIds);
            var author = await _userRepository.GetByIdAsync(Guid.Parse(request.AuthorId));

            if (author == null)
            {
                return BadRequest(new CreateWikiArticleResponse
                {
                    Success = false,
                    Message = "Author not found."
                });
            }

            var wiki = new WikiBuilder()
                    .WithTitle(request.Title)
                    .WithDescription(request.Description)
                    .WithThumbnailImageUrl(request.ThumbnailImageUrl)
                    .WithAuthor(author)
                    .WithProducts(attachedProducts)
                    .WithStatus(WikiStatus.Pending)
                    .WithInitialVotes()
                    .Build();


            bool isSaved = await SaveWikiArticleToDatabaseAsync(wiki);

            if (!isSaved)
            {
                return BadRequest(new CreateWikiArticleResponse
                {
                    Success = false,
                    Message = "Failed to insert the wiki article into database"
                });
            }

            return Ok(new CreateWikiArticleResponse
            {
                Success = isSaved,
                Message = "Wiki article created successfully.",
                ArticleId = wiki.Id.ToString()
            });
        }

        private async Task<bool> SaveWikiArticleToDatabaseAsync(Wiki wiki)
        {
            try
            {
                await _wikiRepository.AddWikiAsync(wiki);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get sample request for CreateWikiArticle
        [HttpGet("GetSampleCreateWikiArticleRequest")]
        public async Task<IActionResult> GetSampleCreateWikiArticleRequest()
        {
            var author = await _userRepository.GetFirstUserAsync();
            if (author == null)
            {
                return BadRequest("No users found in the database.");
            }
            var products = await _productRepository.GetFirstNProductsAsync(2);
            if (products.Count < 2)
            {
                return BadRequest("Not enough products found in the database.");
            }
            var sampleRequest = new CreateWikiArticleRequest
            {
                Title = "Sample Wiki Article",
                Description = "This is a sample description for the wiki article.",
                ThumbnailImageUrl = "sample-thumbnail.png",
                AuthorId = author.UserId.ToString(),
                ProductIds = products.Select(p => p.Id.ToString()).ToList()
            };
            return Ok(sampleRequest);
        }

        // New API to get list of wiki cards
        [HttpGet("GetWikiCards")]
        public async Task<IActionResult> GetWikiCards()
        {
            var wikis = await _wikiRepository.GetAllAsync();
            var wikiCards = wikis.Select(wiki => new
            {
                wiki.Id,
                ThumbnailImageUrl = wiki.ThumbnailImageUrl,
                Title = wiki.Title,
                Upvotes = wiki.Upvotes,
                ContributorCount = wiki.Contributors.Count
            }).ToList();

            return Ok(wikiCards);
        }

        // New API to get wiki by id
        [HttpGet("GetWikiById/{id}")]
        public async Task<IActionResult> GetWikiById(Guid id)
        {
            var wiki = await _wikiRepository.GetByIdAsync(id);
            if (wiki == null)
            {
                return NotFound(new { Message = "Wiki not found." });
            }

            var response = new
            {
                wiki.Id,
                wiki.Title,
                wiki.Description,
                wiki.ThumbnailImageUrl,
                ContributorIds = wiki.Contributors.Select(c => c.UserId).ToList(),
                wiki.AuthorId,
                wiki.Upvotes,
                wiki.Downvotes,
                wiki.Content,
                CreatedAt = GetPlantPostsQueryHandler.FormatCreatedAt(wiki.CreatedAt)
            };

            return Ok(response);
        }

        [HttpPost("AddQuizToWiki")]
        public async Task<IActionResult> AddQuizToWiki([FromBody] AddQuizToWikiRequest request)
        {
            try
            {
                var wiki = await _wikiRepository.GetByIdAsync(request.WikiId);
                if (wiki == null)
                {
                    return NotFound(new { Success = false, Message = "Wiki not found." });
                }

                if (wiki.QuizIds == null)
                {
                    wiki.QuizIds = new List<string>();
                }

                if (!wiki.QuizIds.Contains(request.QuizId))
                {
                    wiki.QuizIds.Add(request.QuizId);
                    await _wikiRepository.UpdateWikiAsync(wiki, wiki.AttachedProducts?.Select(p => p.Id).ToList() ?? new List<Guid>());
                    return Ok(new { Success = true, Message = "Quiz added to wiki successfully." });
                }

                return BadRequest(new { Success = false, Message = "Quiz already exists in wiki." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while adding quiz to wiki.", Error = ex.Message });
            }
        }
    }
}
