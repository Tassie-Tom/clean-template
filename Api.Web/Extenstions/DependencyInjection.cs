using Api.Web.Infrastructure;
using Asp.Versioning;

namespace Api.Web.Extenstions;

internal static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpContextAccessor();

        services.AddControllers();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        //services.AddApiVersioning(options =>
        //{
        //    options.DefaultApiVersion = new ApiVersion(1);
        //    options.ApiVersionReader = new UrlSegmentApiVersionReader();
        //}).AddApiExplorer(options =>
        //{
        //    options.GroupNameFormat = "'v'V";
        //    options.SubstituteApiVersionInUrl = true;
        //});

        //services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        return services;
    }
}

