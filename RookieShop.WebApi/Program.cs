using Azure.Storage.Blobs;
using MassTransit;
using MassTransit.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Quartz.AspNetCore;
using RookieShop.Customers.Infrastructure;
using RookieShop.ImageGallery.Infrastructure.Configurations;
using RookieShop.Ordering.Infrastructure.Configurations;
using RookieShop.ProductCatalog.Infrastructure.Configurations;
using RookieShop.Shopping.Infrastructure.Configurations;
using RookieShop.WebApi.HostedServices;
using RookieShop.WebApi.ImageGallery.ExceptionHandlers;
using RookieShop.WebApi.ProductCatalog.ExceptionHandlers;
using RookieShop.WebApi.Shopping.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource
            .AddService("RookieShop.WebApi");
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddSource("Shopping.MessageDispatcher")
            .AddSource("Shopping.MassTransitMessageDispatcher")
            .AddSource("Shopping.MassTransitMessageDispatcher")
            .SetSampler<AlwaysOnSampler>()
            .AddZipkinExporter();
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swaggerGen =>
{
    swaggerGen.AddSecurityDefinition("oidc", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OpenIdConnect,
        OpenIdConnectUrl = new Uri("http://localhost:8080/realms/rookie-shop/.well-known/openid-configuration"),
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8080/realms/rookie-shop/protocol/openid-connect/auth"),
                TokenUrl = new Uri("http://localhost:8080/realms/rookie-shop/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" }
                }
            }
        }
    });
    
    swaggerGen.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oidc"
                }
            },
            ["openid", "profile"]
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
    {
        resource.AddService("RookieShop", serviceVersion: "1.0.0", serviceInstanceId: Environment.MachineName);
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddZipkinExporter();
    });

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ProductCatalogExceptionHandler>();
builder.Services.AddExceptionHandler<ImageGalleryExceptionHandler>();
builder.Services.AddExceptionHandler<ShoppingExceptionHandler>();

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

builder.Services.AddHostedService<AzureBlobCreateContainerHostedService>();

builder.Services.AddSingleton<BlobServiceClient>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("Azurite")!;

    return new BlobServiceClient(connectionString);
});

builder.Services.AddMemoryCache();

builder.Services.AddQuartz(quartz =>
{
    quartz.UsePersistentStore(store =>
    {
        store.UsePostgres(postgresql =>
        {
            postgresql.ConnectionString = builder.Configuration.GetConnectionString("Quartz")!;
            postgresql.TablePrefix = "qrtz_";
        });
        
        store.UseSystemTextJsonSerializer();
    });
});

builder.Services.AddQuartzServer(quartzServer =>
{
    quartzServer.WaitForJobsToComplete = true;
});

builder.Services.AddMassTransit(bus =>
{
    bus.AddPublishMessageScheduler();
    
    bus.AddProductCatalogConsumers();
    bus.AddImageGalleryConsumers();
    bus.AddShoppingConsumers();
    bus.AddOrderingConsumers();

    bus.AddMediator(mediator =>
    {
        mediator.AddProductCatalogConsumers();
        mediator.AddImageGalleryConsumers();
        mediator.AddOrderingConsumers();
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

builder.Services.AddShopping(shopping =>
{
    shopping.SetDatabaseConnectionString(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return configuration.GetConnectionString("Postgresql")!;
    });
    
    shopping.SetMigrationAssembly("RookieShop.WebApi");
});

builder.Services.AddOrdering(ordering =>
{
    ordering.SetDatabaseConnectionString(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return configuration.GetConnectionString("Postgresql")!;
    });
    
    ordering.SetMigrationAssembly("RookieShop.WebApi");
});

builder.Services.AddImageGallery(imageGallery =>
{
    imageGallery.SetDatabaseConnectionString(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return configuration.GetConnectionString("Postgresql")!;
    });
    
    imageGallery.SetMigrationAssembly("RookieShop.WebApi");

    imageGallery.SetBlobContainerClient(provider =>
    {
        var blobServiceClient = provider.GetRequiredService<BlobServiceClient>();

        return blobServiceClient.GetBlobContainerClient("rookie-shop-images");
    });
});

builder.Services.AddCustomers(customers =>
{
    customers.SetKeycloakOptions(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();

        return new KeycloakCustomerServiceOptions
        {
            Address = configuration["Keycloak:AuthSettings:Address"]!,
            Realm = configuration["Keycloak:AuthSettings:Realm"]!,
            ClientId = configuration["Keycloak:ServiceAccount:ClientId"]!,
            ClientSecret = configuration["Keycloak:ServiceAccount:ClientSecret"]!,
            CustomersGroupId = configuration["Keycloak:Customers:GroupId"]!
        };
    });
    
    customers.SetHttpClient(provider =>
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        
        return httpClientFactory.CreateClient();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI(swaggerUi =>
    {
        swaggerUi.OAuthClientId("rookie-shop-web-api");
        swaggerUi.OAuthClientSecret("WdK1ICYLbzChpYuutps2X82yeFg6MtXx");
        swaggerUi.OAuthRealm("rookie-shop");
    });
}

app.UseCors("rookie-shop-back-office-cors");

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();