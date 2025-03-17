using Api.Application.Abstractions.Authenication;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Api.Infrastructure.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddFirebaseAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // Initialize Firebase if not already initialized
        if (FirebaseApp.DefaultInstance == null)
        {
            var credential = GoogleCredential.FromFile(
                configuration["Firebase:CredentialPath"] ??
                @"C:\Users\hello\Downloads\thegoodlotto-firebase-adminsdk-vef7k-f3a8b806c7.json");

            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = configuration["Firebase:ProjectId"],
            });
        }

        // Add authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{configuration["Firebase:ProjectId"]}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{configuration["Firebase:ProjectId"]}",
                    ValidateAudience = true,
                    ValidAudience = configuration["Firebase:ProjectId"],
                    ValidateLifetime = true
                };

                // Add token validation event to populate user claims
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Additional claim processing can be added here if needed
                        return Task.CompletedTask;
                    }
                };
            });

        // Register Firebase services
        services.AddSingleton(FirebaseAuth.DefaultInstance);
        services.AddScoped<IAuthService, FirebaseAuthService>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}