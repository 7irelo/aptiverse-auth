using Aptiverse.Auth;
using Aptiverse.Auth.Middleware;
using Aptiverse.Auth.Utilities;
using Aptiverse.Infrastructure.Data;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<NullResultFilter>();
    opt.Filters.Add<ValidationFilter>();
    opt.Filters.Add<ExceptionHandlingFilter>();
    opt.Filters.Add<LoggingFilter>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddAntiforgery();

var app = builder.Build();

app.UseCors("AllowNextJS");

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
        Console.WriteLine("Roles seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding roles: {ex.Message}");
    }
}

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath)),
    RequestPath = ""
});

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapOpenApi();

app.MapScalarApiReference("dev", options =>
{
    options
        .WithTitle("Aptiverse API")
        .WithTheme(ScalarTheme.Purple)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseReDoc(options =>
{
    options.RoutePrefix = "docs";
    options.DocumentTitle = "Aptiverse API - ReDoc";
    options.SpecUrl = "/openapi/v1.json";
});

app.Run();