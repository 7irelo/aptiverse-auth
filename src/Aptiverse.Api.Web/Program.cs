using Aptiverse.Api.Web;
using Aptiverse.Api.Web.Middleware;
using Aptiverse.Infrastructure.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Reflection;

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
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aptiverse API",
        Version = "v1",
        Description = "API documentation for Aptiverse services"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath)),
    RequestPath = ""
});

app.MapGet("/index.html", async (HttpContext context) =>
{
    var filePath = Path.Combine(builder.Environment.ContentRootPath, "index.html");

    if (File.Exists(filePath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(filePath);
    }
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("File not found");
    }
});

app.UseSwagger();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // <-- ADDED Production
{
    app.MapScalarApiReference("/dev", options =>
    {
        options.Title = "Aptiverse API – Scalar";
        options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
    });

    app.UseReDoc(options =>
    {
        options.RoutePrefix = "docs";
        options.DocumentTitle = "Aptiverse API - ReDoc";
        options.SpecUrl = "/swagger/v1/swagger.json";
    });
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aptiverse API V1");
        options.RoutePrefix = "swagger"; // Available at /swagger
        options.DocumentTitle = "Aptiverse API - Swagger UI";
    });
}

app.UseRateLimiter();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode} for {Method} {Path}",
        context.Response.StatusCode, context.Request.Method, context.Request.Path);
});

app.MapControllers();

app.Run();