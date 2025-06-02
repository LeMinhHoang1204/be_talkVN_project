using System.Reflection;

// using TalkVN.Application.MachineLearning.Services;
// using TalkVN.Application.MachineLearning.Services.Interface;
using TalkVN.Application.Mapping;
using TalkVN.Application.Services;
using TalkVN.Application.Services.Interface;
using TalkVN.Application.Validators;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
namespace TalkVN.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add Validators

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssemblyContaining<IValidatorMarker>();

            // Add MediaR

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // Add AutoMapper

            services.AddAutoMapper(typeof(IMappingProfileMarker));

            // Add Services
            services.AddServices();


            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IGroupInvitationService, GroupInvitationService>();
         //   services.AddScoped<IPostService, PostService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IEmailService, EmailService>();
            // services.AddScoped<ITrainingModelService, TrainingModelService>();
            services.AddScoped<IPermissionService, PermissionService>();
            //TODO: register service here
            return services;
        }
    }
}
