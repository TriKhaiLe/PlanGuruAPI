using Application.Common.Interface.Persistence;
using Application.Votes;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repository;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<PlanGuruDBContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var dbContext = serviceProvider.GetRequiredService<PlanGuruDBContext>();
                dbContext.Database.EnsureCreated(); 
            }

            services.AddPersistence();
            services.AddVoteStrategy();

            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPlantPostRepository, PlantPostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IWikiRepository, WikiRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IQuizManager, QuizServices>();

            return services;
        }

        // register for strategy pattern
        public static IServiceCollection AddVoteStrategy(this IServiceCollection services)
        {
            services.AddScoped<VoteStrategyFactory>();
            services.AddScoped<PostVoteStrategy>();
            services.AddScoped<CommentVoteStrategy>();
            return services;
        }

    }
}
