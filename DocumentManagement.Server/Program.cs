using DocumentManagement.Server.Data;
using DocumentManagement.Server.Repository;
using DocumentManagement.Server.Repository.Interface;
using DocumentManagement.Server.Services;
using DocumentManagement.Server.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var frontEndOrigin = builder.Configuration["FrontEndOrigin"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder.WithOrigins(frontEndOrigin)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});


builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();


builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.None;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseHttpsRedirection();


app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers.Append("X-Content-Type-Options", "nosniff");
    headers.Append("X-Frame-Options", "DENY");
    headers.Append("X-XSS-Protection", "1; mode=block");

    await next();
});

app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;

    headers.Append("Content-Security-Policy", "default-src 'self';");
    headers.Append("Content-Security-Policy", "script-src 'self'");
    headers.Append("Content-Security-Policy", "style-src 'self'");
    headers.Append("Content-Security-Policy", "img-src 'self'");
    headers.Append("Content-Security-Policy", "font-src 'self'");
    headers.Append("Content-Security-Policy", "connect-src 'self'");
    headers.Append("Content-Security-Policy", "frame-ancestors 'self';");

    await next();
});


app.UseCors("AllowAngularApp");

app.UseAuthorization();


app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
