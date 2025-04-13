using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RookieShop.Application.Abstractions;
using RookieShop.Application.Services;
using RookieShop.WebApi.Infrastructure.Persistence;
using RookieShop.WebApi.Infrastructure.ProfanityChecker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:AuthSettings:Authority"];
        options.Audience = builder.Configuration["Keycloak:AuthSettings:Audience"];
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "roles"
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = (context) =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
                {
                    return Task.CompletedTask;
                }
                
                var roles = context.Principal.FindAll("roles");

                foreach (var role in roles)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Value));
                }
                
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDbContext<RookieShopDbContextImpl>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    
    var connectionString = configuration.GetConnectionString("Postgresql");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("RookieShop.WebApi");
    });
});

builder.Services.AddScoped<RookieShopDbContext, RookieShopDbContextImpl>();
builder.Services.AddScoped<IPurchaseChecker, RookieShopDbContextImpl>();
builder.Services.AddSingleton<IProfanityChecker>(_ =>
{
    var filter = new ProfanityFilter.ProfanityFilter();
    return new ProfanityCheckerAdapter(filter);
});

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<RatingService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();