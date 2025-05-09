using Application.Common.Behavior;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Files.Command.SaveFile;
using Application.Files.Command.UploadSupabaseFile;
using Application.Files.Common.SaveFile;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);

            services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            // // Manual DI registrations for the Adapter pattern
            services.AddScoped<SupabaseFileUploader>();
            services.AddScoped<UploadSupabaseFileCommandHandler>();
            services.AddScoped<IRequestHandler<SaveFileCommand, SaveFileResponse>, UploadSupabaseFileCommandHandlerAdapter>();
            
            return services;
        }
    }
}
