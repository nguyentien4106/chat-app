using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Interceptors;
using ChatApp.Infrastructure.Services.Auth;
using ChatApp.Application.Interfaces.Auth;
using ChatApp.Application.Settings;
using System.Text;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Repositories;
using ChatApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Infrastructure.Services;

namespace ChatApp.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        ArgumentNullException.ThrowIfNull(jwtSettings, "JwtSettings was missed !");

        services.AddScoped<ChatAppDbInitializer>();

        // Add services to the container.
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddDbContext<ChatAppDbContext>((sp, options) =>
        {
            var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ChatAppDbContext).Assembly.GetName().Name))
                .AddInterceptors(auditableInterceptor);
        });
        services.AddAppIdentity();
        services.AddJwtConfiguration(jwtSettings);
        services.AddApplicationServices();
        services.AddCorsPolicy(jwtSettings);

        return services;
    }

    private static IServiceCollection AddJwtConfiguration(this IServiceCollection services, JwtSettings jwtSettings)
    {

        services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.UseSecurityTokenValidators = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                    
                    // For SignalR authentication
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
            
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                    
                });

        services.AddAuthorization();
        services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build());
        services.AddSignalR();

        return services;
    }

    private static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    // Configuration for authentication fields
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                })
                .AddEntityFrameworkStores<ChatAppDbContext>()
                .AddDefaultTokenProviders()
                .AddRoles<IdentityRole<Guid>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddEntityFrameworkStores<ChatAppDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default");

        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services.AddScoped<IChatAppDbContext>(provider => provider.GetRequiredService<ChatAppDbContext>());
        services.AddScoped<IAuthenticateService, AuthenticateService>();
        services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenValidatorService, RefreshTokenService>();

        services.AddScoped<IChatAppDbContext, ChatAppDbContext>();
        services.AddSingleton<IStorageService, R2StorageService>();

        // Repository registration
        services.AddScoped(typeof(IRepositoryPagedQuery<>), typeof(PagedQueryRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(Repository<>), typeof(EfRepository<>));

        services.AddScoped<IRepository<Message>, EfRepository<Message>>();
        services.AddScoped<IRepository<Group>, EfRepository<Group>>();
        services.AddScoped<IRepository<GroupMember>, EfRepository<GroupMember>>();
        services.AddScoped<IRepository<Conversation>, EfRepository<Conversation>>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("corsPolicy", builder =>
            {
                builder.WithOrigins([
                            jwtSettings.Audience
                        ])
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }
}
