using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RookieShop.Application.Abstractions;
using RookieShop.Application.Services;
using RookieShop.WebApi.Customers;
using RookieShop.WebApi.ExceptionHandlers;
using RookieShop.WebApi.Infrastructure.CustomerService;
using RookieShop.WebApi.Infrastructure.Persistence;
using RookieShop.WebApi.Infrastructure.ProfanityChecker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<RookieShopExceptionHandler>();

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("rookie-shop-back-office-cors", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:5173");
    });
});

builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var address = builder.Configuration["Keycloak:AuthSettings:Address"];
        var realm = builder.Configuration["Keycloak:AuthSettings:Realm"];
        options.Authority = $"{address}/realms/{realm}";
        options.Audience = builder.Configuration["Keycloak:AuthSettings:Audience"];
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.ConfigureOptions<CustomerServiceOptionsSetup>();

builder.Services.AddSingleton<ICustomerService, CustomerService>();

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

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("rookie-shop-back-office-cors");

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();