using Aptiverse.Auth;
using Aptiverse.Auth.Middleware;
using Aptiverse.Auth.Utilities;
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

app.UseDefaultFiles();
app.UseStaticFiles();

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
    options.DocumentTitle = "Aptiverse API";
    options.SpecUrl = "/openapi/v1.json";
});

app.Run();