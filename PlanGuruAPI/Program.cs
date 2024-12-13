using Application;
using Application.Common.Interface.Persistence;
using AutoMapper;
using Domain.Entities;
using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Utilities;
using Infrastructure;
using Infrastructure.Persistence.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using PlanGuruAPI.GraphQL.Mutations;
using PlanGuruAPI.GraphQL.Queries;
using PlanGuruAPI.GraphQL.Schemas;
using PlanGuruAPI.GraphQL.Types;
using PlanGuruAPI.Mapping;
using Serilog;

namespace PlanGuruAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddGraphQL().AddSystemTextJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.MapType<TargetType>(() => new OpenApiSchema
                {
                    Type = "integer",
                    Enum = Enum.GetValues(typeof(TargetType)).Cast<int>().Select(e => (IOpenApiAny)new OpenApiInteger(e)).ToList(),
                    Description = "Type of target. Values: 0 = Post, 1 = Comment, 2 = Wiki"
                });
            });
            builder.Services.AddApplication().AddInfrastructure();
            builder.Services.AddControllers();

            builder.Services.AddAutoMapper(typeof(Program));

            // Add GraphQL services
            builder.Services.AddScoped<WikiType>();
            builder.Services.AddScoped<ProductType>();
            builder.Services.AddScoped<ContentSectionType>();
            builder.Services.AddScoped<WikiQuery>();
            builder.Services.AddScoped<WikiMutation>();
            builder.Services.AddScoped<WikiSchema>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseExceptionHandler("/error");

            app.seedData();

            // graphql
            app.UseGraphQL<WikiSchema>();
            app.UseGraphQLGraphiQL("/ui/graphql");

            app.MapControllers();

            app.Run();
        }
    }
}
