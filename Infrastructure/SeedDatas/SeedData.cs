using Application.Common.Interface.Persistence;
using Domain.Entities;
using Domain.Entities.ECommerce;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure
{
    public static partial class SeedData
    {
        public static async void seedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PlanGuruDBContext>();

                if (context == null)
                {
                    Console.WriteLine("Database context is null.");
                    return;
                }

                Console.WriteLine("Seeding Data...");

                // Gọi các phần seed khác
                SeedUsers(context);
                SeedPosts(context, serviceScope);
                SeedProducts(context);
                SeedWikis(context);
                SeedMemberships(context);

                #region Seed group


                var users = context.Users.ToList();
                var firstUser = users[0];
                var secondUser = users[1];
                var thirdUser = users[4];

                List<Group> groups = new List<Group>()
                {
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Passionate Plant Lovers",
                        GroupImage = "https://cdn.pixabay.com/photo/2018/01/29/07/11/flower-3115353_640.jpg",
                        Description = "A community for those who love growing and sharing rare plants.",
                        MasterUser = firstUser,
                        MasterUserId = firstUser.UserId
                    },
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Orchid Enthusiasts Club",
                        GroupImage = "https://cdn.pixabay.com/photo/2021/12/04/04/44/flowers-6844359_640.jpg",
                        Description = "Dedicated to orchid care, swapping tips, and sharing photos of blooms.",
                        MasterUser = firstUser,
                        MasterUserId = firstUser.UserId
                    },
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Succulent Addicts",
                        GroupImage = "https://cdn.pixabay.com/photo/2024/11/25/10/40/margaritas-9223058_640.jpg",
                        Description = "A place for succulent lovers to exchange ideas and rare varieties.",
                        MasterUser = secondUser,
                        MasterUserId = secondUser.UserId
                    },
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Cactus & Desert Plants Community",
                        GroupImage = "https://cdn.pixabay.com/photo/2022/03/26/11/43/flower-7092794_640.jpg",
                        Description = "For fans of hardy cacti and desert flora, showcasing collections and tips.",
                        MasterUser = secondUser,
                        MasterUserId = secondUser.UserId
                    },
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Indoor Jungle Tribe",
                        GroupImage = "https://cdn.pixabay.com/photo/2022/03/26/11/43/flower-7092794_640.jpg",
                        Description = "Turning homes into lush jungles with indoor plant inspiration and advice.",
                        MasterUser = thirdUser,
                        MasterUserId = thirdUser.UserId
                    },
                    new Group()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        GroupName = "Bonsai Artists Network",
                        GroupImage = "https://cdn.pixabay.com/photo/2022/03/26/11/43/flower-7092794_640.jpg",
                        Description = "A group for bonsai lovers to learn and share techniques in miniature tree art.",
                        MasterUser = thirdUser,
                        MasterUserId = thirdUser.UserId
                    },
                };
                var listUserForGroup = await context.Users.ToListAsync();
                for(int j = 0; j < groups.Count; j++)
                {
                    var group = groups[j];
                    for (int i = 0; i < listUserForGroup.Count; i++)
                    {
                        if (listUserForGroup[i].UserId == group.MasterUserId) continue;
                        var groupUser = new GroupUser()
                        {
                            Group = group,
                            GroupId = group.Id,
                            User = listUserForGroup[i],
                            UserId = listUserForGroup[i].UserId,
                        };
                        if ((i + j) % 4 == 0) continue;
                        if ((i + j) % 4 == 1) groupUser.Status = "Pending";
                        if ((i + j) % 4 == 2) groupUser.Status = "Joined";
                        if ((i + j) % 4 == 3) groupUser.Status = "Forbidden";
                        context.GroupUsers.Add(groupUser);
                    }
                }

                context.Groups.AddRange(groups);
                context.SaveChanges();

                var plantNames = new List<string>
                        {
                            "Rose", "Chrysanthemum", "Lotus", "Maple Tree", "Cactus",
                            "Orchid", "Sunflower", "Tulip", "Jasmine", "Daffodil"
                        };

                var plantDescriptions = new List<string>
                        {
                            "A timeless symbol of love and beauty 🌹. Can't get enough of these stunning blooms!",
                            "Simple yet elegant, chrysanthemums always brighten up my day! 🌼",
                            "The serenity of lotus flowers floating on a calm lake is unmatched. 💧🌸",
                            "Autumn vibes are incomplete without the vibrant hues of maple leaves 🍁.",
                            "Resilient and unique, cacti remind me that beauty can thrive anywhere 🌵.",
                            "Orchids are like the royalty of the flower world 👑. Just look at that elegance!",
                            "Sunflowers: a little piece of sunshine to carry with you 🌻☀️.",
                            "Tulips in full bloom are such a mesmerizing sight. Spring is here! 🌷✨",
                            "The delicate scent of jasmine is pure magic on a summer evening 🌿🌼.",
                            "Daffodils are the perfect way to welcome the new season 🌸🌞."
                        };

                var plantImages1 = new List<string>
                        {
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS0w6lYV-pe9pHQVre7Qbs9uHwIHMifgrPsfBTTMwiAwUTwvtIrrTYtRb2sui1MHg4_hzA&usqp=CAU",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBhFD9HPMZb3sBIJw8b7kpd5oO2vWygl8WZA&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRnwgXUcfxKO0_4TIXcLnrjmz2uZKZ2uMvNCA&s",
                            "https://images.squarespace-cdn.com/content/v1/5cb3ca007a1fbd45aeff89ea/f0a9c6c6-52bf-4bac-b865-bb8dc1fc8e55/nashville-tree-conservation-corps-tree-of-month-red-maple-tn.jpg?format=500w",
                            "https://cdn.shopify.com/s/files/1/0968/5384/files/Cactus-spines-are-modified-leaves-jpj_480x480.jpg?v=1724131342",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQdxGXq1WUtUaKipN9WXeMpM0b19GHHLXH-6Q&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ4FguqujkIGPgeMQ57aBikf-W7qQdBpU4ZWA&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT9Mbp7lfD5rlyZLUmuGNNg8uDTZIN0dnnKZw&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIDPMaeZ2wkIzdTS_2q61GYYTqyHV0hSK6wg&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTRrD7BfNazbV803hbUctSS90XgGnU7QEJcOg&s"
                        };
                var plantImages2 = new List<string>
                        {
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRxh0A_Y4S3dAh-v_8fXdnuAzS7-xy80jHvdbLuhR80JjAH0itOgvR0zh8yu7goU0fT3PQ&usqp=CAU",
                            "https://ruthgoudy.com/wp-content/uploads/2023/11/Chrysanthemum-scaled-1-1024x768.jpg",
                            "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f5/Lotus_flower_%28978659%29.jpg/1280px-Lotus_flower_%28978659%29.jpg",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcThB-Nc9n5nLZPK76DnzrU82eKeStuirOKaCQ&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSlzP9L6gSHOxVcfjO-SUhcdMRFYuRteyyXIg&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS6UJbQfJK0g7fAxwclWqdKp_W89M_hoQKelg&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQpLlB1l_6ZKbvxLlvmGzVtw4erc9SqhGpYWA&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTIUkhtYnSNeLM5OxaaM4F-RnIGlUzRBlCvlA&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRjgM4mJSb9yfVyucnmyflXy0z7AN3RlkPEEw&s",
                            "https://www.dutchgrown.co.uk/cdn/shop/files/Daffodil_Fortissimo-1.jpg?v=1683635838"
                        };
                var plantImages3 = new List<string>
                        {
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQjYt0I93QC33rnUrCDDpYXaPjRy8_O3uR5HiKaycBPzc0UVW6B3dqGayOe52DARBJ1S7M&usqp=CAU",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ4-ad0q_yJAaIWGWINAexg4ke8hgaFIKSRGQ&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSqqSbG9CfMmh5UEQnTzy1plbzhwYJbzQK8dQ&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR4Mzh1ArOsWrjtSBTonSGaON72RZfoxKMQeA&s",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR4i_kO_DJKegrJ6KTLy9M3vc5G0c_jNZQ0_A&s",
                            "https://static.wixstatic.com/media/nsplsh_79414a736f687562667134~mv2_d_6720_4480_s_4_2.jpg/v1/crop/x_1344,y_0,w_3035,h_4480/fill/w_340,h_506,al_c,q_80,usm_0.66_1.00_0.01,enc_avif,quality_auto/Image%20by%20Heidi%20Erickson.jpg",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRNJxZvRq8QVqH3f1y3ZLDbiwv-LlmG7RS0Pg&s",
                            "https://media.istockphoto.com/id/1478889850/vi/vec-to/b%E1%BB%A9c-tranh-hoa-tulip-m%C3%A0u-h%E1%BB%93ng-tuy%E1%BB%87t-%C4%91%E1%BA%B9p.jpg?s=612x612&w=0&k=20&c=CP_02UXYxWOaYsD8H77AIW2VtJG_WS4pDhEhan86pOs=",
                            "https://cdn.mos.cms.futurecdn.net/iNamfrqmhBNHZnuQJZWKPH-1200-80.jpg",
                            "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPiedz8P1wIMk5r_KZaD97XC-fUuy2VWo4_g&s"
                        };


                foreach (var group in groups)
                {
                    var randomUser = context.GroupUsers.Where(p => p.GroupId == group.Id && p.Status == "Joined").FirstOrDefault();
                    var tag = "plants";

                    for (int i = 0; i < 20; i++)
                    {
                        var randomTag = tag;

                        int plantIndex = i % plantNames.Count;

                        var post = new Post
                        {
                            UserId = randomUser.UserId,
                            Title = $"Look at this {plantNames[plantIndex]}!",
                            Description = plantDescriptions[plantIndex],
                            Tag = randomTag,
                            Background = plantImages1[plantIndex],
                            Group = group,
                            GroupId = group.Id,
                        };

                        var postImage1 = new PostImage()
                        {
                            Image = plantImages1[plantIndex],
                            Post = post,
                            PostId = post.Id,
                        };

                        var postImage2 = new PostImage()
                        {
                            Image = plantImages2[plantIndex],
                            Post = post,
                            PostId = post.Id,
                        };

                        var postImage3 = new PostImage()
                        {
                            Image = plantImages3[plantIndex],
                            Post = post,
                            PostId = post.Id,
                        };

                        context.PostImages.Add(postImage1);
                        context.PostImages.Add(postImage2);
                        context.PostImages.Add(postImage3);

                        if (i < 10) post.IsApproved = true;
                        context.Posts.Add(post);
                    }

                }

                #endregion

                #region Seed order

                var listUser = context.Users.Skip(2).ToList();
                var listProduct = context.Products.ToList().Take(3);
                foreach (var user in listUser)
                {
                    foreach (var item in listProduct)
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            var order = new Order()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                LastModifiedAt = DateTime.Now,
                                Product = item,
                                ProductId = item.Id,
                                User = user,
                                UserId = user.Id,
                                TotalPrice = item.Price,
                                Quantity = 3,
                                ShippingAddress = "99 Nguyễn Văn Trỗi Dĩ An Bình Dương",
                                Status = OrderStatus.NotPaid
                            };
                            context.Orders.Add(order);
                        }
                    }
                    foreach (var item in listProduct)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var order = new Order()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                LastModifiedAt = DateTime.Now,
                                Product = item,
                                ProductId = item.Id,
                                User = user,
                                UserId = user.Id,
                                TotalPrice = item.Price,
                                Quantity = 3,
                                ShippingAddress = "99 Nguyễn Văn Trỗi Dĩ An Bình Dương",
                                Status = OrderStatus.Paid
                            };
                            context.Orders.Add(order);
                        }
                    }
                    foreach (var item in listProduct)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var order = new Order()
                            {
                                Id = Guid.NewGuid(),
                                CreatedAt = DateTime.Now,
                                LastModifiedAt = DateTime.Now,
                                Product = item,
                                ProductId = item.Id,
                                User = user,
                                UserId = user.Id,
                                Quantity = 3,
                                TotalPrice = item.Price,
                                ShippingAddress = "99 Nguyễn Văn Trỗi Dĩ An Bình Dương",
                                Status = OrderStatus.Success
                            };
                            context.Orders.Add(order);
                        }
                    }
                }
                context.SaveChanges();

                #endregion
            }

        }
    }
}
