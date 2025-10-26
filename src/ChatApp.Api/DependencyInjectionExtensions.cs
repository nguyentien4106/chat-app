using ChatApp.Application.Hubs;
using ChatApp.Application.Settings;
using ChatApp.Infrastructure;
using ChatApp.Api.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace ChatApp.Api;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSettings(configuration);
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.MaxDepth = 64; // Increase max depth if needed
            });
            
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatApp API", Version = "v1" });
            
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            config.AddSecurityRequirement(
                new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });
        // Register the global exception handler
        services.AddExceptionHandler<GlobalExceptionMiddleware>();
        services.AddProblemDetails();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

        }
        
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ChatAppDbInitializer>();
        initialiser.InitialiseAsync().Wait();
        initialiser.SeedAsync().Wait();
        
        app.UseExceptionHandler(options => { });
        app.UseCors("corsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<ChatHub>("/hubs/chat");

        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? null;
        ArgumentNullException.ThrowIfNull(jwtSettings, "JwtSettings was missed !");
        services.AddSingleton(jwtSettings);
        
        var r2Settings = configuration.GetSection("R2Settings").Get<R2Settings>() ?? new R2Settings();
        ArgumentNullException.ThrowIfNull(r2Settings, "R2Settings was missed !");
        services.AddSingleton(r2Settings);
        
        return services;
    }
}