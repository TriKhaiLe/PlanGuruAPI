using Application.Common.Interface.Persistence;
using Domain.Entities;
using Domain.Entities.ECommerce;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class SeedData
    {
        public static void seedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PlanGuruDBContext>();
                var voteRepository = serviceScope.ServiceProvider.GetService<IVoteRepository>();
                Console.WriteLine("Seeding Data");

                // Seed Users
                for (int i = 0; i < 5; i++)
                {
                    User user = new User()
                    {
                        UserId = Guid.NewGuid(),
                        Email = $"gmail{i}@gmail.com",
                        Avatar = $"avatar{i}.png",
                        Password = $"password{i}",
                        Name = $"name{i}"
                    };
                    context.Users.Add(user);
                }
                User user2 = new User()
                {
                    UserId = Guid.NewGuid(),
                    Email = "ndam8175@gmail.com",
                    Password = "123123",
                    Avatar = "assda.png",
                    Name = "namdam"
                };
                context.Users.Add(user2);
                context.SaveChanges();

                // Get the first two users
                var firstTwoUsers = context.Users.Take(2).ToList();

                if (firstTwoUsers.Count >= 2)
                {
                    var firstUser = firstTwoUsers[0];
                    var secondUser = firstTwoUsers[1];

                    // Seed Posts
                    for (int i = 0; i < 3; i++)
                    {
                        Post post = new Post()
                        {
                            UserId = firstUser.UserId,
                            Title = $"Post Title {i + 1}",
                            Description = $"This is the description for post {i + 1}.",
                            ImageUrl = "https://i.pinimg.com/736x/d9/95/e3/d995e3f52c60ff8bc39f0ae2303bec6f.jpg",
                            Tag = "Plants",
                            Background = "https://i.pinimg.com/736x/6b/8d/55/6b8d557af9e7122dbd7eec1c2593232b.jpg",
                        };
                        context.Posts.Add(post);
                    }
                    context.SaveChanges();

                    // Get the first post
                    var firstPost = context.Posts.FirstOrDefault();

                    if (firstPost != null)
                    {
                        // Seed Comments
                        for (int i = 0; i < 2; i++)
                        {
                            Comment comment = new Comment()
                            {
                                Id = Guid.NewGuid(),
                                PostId = firstPost.Id,
                                UserId = firstUser.UserId,
                                Message = $"This is comment {i + 1} on the first post.",
                                CreatedAt = DateTime.Now
                            };
                            context.Comments.Add(comment);
                        }
                        context.SaveChanges();

                        // Get the first comment
                        var firstComment = context.Comments.FirstOrDefault();

                        if (firstComment != null)
                        {
                            // Seed Upvotes for the first comment by the first two users
                            foreach (var user in firstTwoUsers)
                            {
                                var commentUpvote = new Vote
                                {
                                    UserId = user.UserId,
                                    TargetId = firstComment.Id,
                                    TargetType = TargetType.Comment,
                                    IsUpvote = true
                                };
                                voteRepository.AddVoteAsync(commentUpvote).Wait();
                            }

                            // Seed Upvotes for the first post by the first two users
                            foreach (var user in firstTwoUsers)
                            {
                                var postUpvote = new Vote
                                {
                                    UserId = user.UserId,
                                    TargetId = firstPost.Id,
                                    TargetType = TargetType.Post,
                                    IsUpvote = true
                                };
                                voteRepository.AddVoteAsync(postUpvote).Wait();
                            }
                        }
                    }

                    // Seed Products for the first user
                    for (int i = 0; i < 2; i++)
                    {
                        Product product = new Product()
                        {
                            Id = Guid.NewGuid(),
                            SellerId = firstUser.UserId,
                            ProductName = $"Product {i + 1}",
                            Description = $"This is the description for product {i + 1}.",
                            Price = (double)(10.0m * (i + 1))
                        };
                        context.Products.Add(product);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
