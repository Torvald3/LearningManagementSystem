using LMS.App.Extensions;
using LMS.Users.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModulesServices(builder.Configuration);

builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseModules();

app.Run();