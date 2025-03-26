using Api.Application.Abstractions.Authenication;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            GoogleCredential credential;

            string firebaseCredentialJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");

            if (!string.IsNullOrEmpty(firebaseCredentialJson))
            {
                // Use credentials from environment variable
                credential = GoogleCredential.FromJson(firebaseCredentialJson);
            }
            else
            {
                // Fall back to file if environment variable is not set
                string credentialPath = configuration["Firebase:CredentialPath"];
                if (string.IsNullOrEmpty(credentialPath))
                {
                    throw new InvalidOperationException(
                        "Firebase credentials not found. Set FIREBASE_CREDENTIALS environment variable or Firebase:CredentialPath in configuration.");
                }

                credential = GoogleCredential.FromFile(credentialPath);
            }

            var projectId = configuration["Firebase:ProjectId"] ??
                Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");

            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException(
                    "Firebase Project ID not found. Set FIREBASE_PROJECT_ID environment variable or Firebase:ProjectId in configuration.");
            }

            FirebaseApp.Create(new AppOptions
            {
                Credential = credential,
                ProjectId = projectId,
            });
        }

        // Add authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var projectId = configuration["Firebase:ProjectId"] ??
                    Environment.GetEnvironmentVariable("FIREBASE_PROJECT_ID");

                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
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