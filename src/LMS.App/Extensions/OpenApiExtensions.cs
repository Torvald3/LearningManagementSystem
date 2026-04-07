using LMS.App.Configurations;
using Scalar.AspNetCore;

namespace LMS.App.Extensions;

public static class OpenApiExtensions
{
    public static WebApplication UseOpenApi(this WebApplication app, OpenApiConfiguration openApiConfiguration)
    {
        if (openApiConfiguration.EnableScalar)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    
        return app;
    }
}
