using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RookieShop.ProductCatalog.Infrastructure.Configurations;
using RookieShop.ProductReview.Infrastructure.Configurations;
using RookieShop.WebApi.Customers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

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

builder.Services.AddMassTransit(bus =>
{
    bus.AddProductCatalogConsumers();

    bus.AddMediator(mediator =>
    {
        mediator.AddProductCatalogConsumers();
        mediator.AddProductCatalogConsumers();
    });
    
    bus.UsingRabbitMq((context, rabbitMq) =>
    {
        rabbitMq.Host(builder.Configuration["Messaging:RabbitMq:Host"]!, host =>
        {
            host.Username(builder.Configuration["Messaging:RabbitMq:Username"]!);
            host.Password(builder.Configuration["Messaging:RabbitMq:Password"]!);
        });
        
        rabbitMq.ConfigureEndpoints(context);
    });
});

builder.Services.AddProductCatalog(productCatalog =>
{
    productCatalog.SetDatabaseConnectionString(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return configuration.GetConnectionString("Postgresql")!;
    });
    
    productCatalog.SetMigrationAssembly("RookieShop.WebApi");
});

builder.Services.AddProductReview(productReview =>
{
    productReview.SetDatabaseConnectionString(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return configuration.GetConnectionString("Postgresql")!;
    });

    productReview.SetMigrationAssembly("RookieShop.WebApi");
});

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