using LMS.App.Configurations;
using LMS.App.Extensions;
using LMS.Identity.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddObservability();

builder.Services.AddModulesServices(builder.Configuration);

builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseRequestMetrics();
app.UseAuthentication();
app.UseAuthorization();

app.MapMetricsEndpoints();

app.UseModules();
app.UseOpenApi(builder.Configuration.GetRequiredSection("OpenApi").Get<OpenApiConfiguration>()!);

app.Run();

public partial class Program
{
}