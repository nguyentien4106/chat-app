using ChatApp.Application.Behaviours;
using ChatApp.Application.Commands.Messages.SendMessage.Strategy;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application;

public static class DependencyInjectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register SendMessage strategies
        services.AddScoped<ISendMessageStrategy, GroupMessageStrategy>();
        services.AddScoped<ISendMessageStrategy, ConversationMessageStrategy>();
        services.AddScoped<SendMessageStrategyContext>();
    }

}
