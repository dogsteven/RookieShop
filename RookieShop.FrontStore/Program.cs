using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RookieShop.FrontStore.ExceptionFilters;
using RookieShop.FrontStore.Modules.ImageGallery;
using RookieShop.FrontStore.Modules.ProductCatalog.Abstractions;
using RookieShop.FrontStore.Modules.ProductCatalog.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";

    options.Cookie.HttpOnly = true;
})
.AddOpenIdConnect(options =>
{
    var address = builder.Configuration["Keycloak:AuthSettings:Address"];
    var realm = builder.Configuration["Keycloak:AuthSettings:Realm"];
    var authority = $"{address}/realms/{realm}";
    var metadataAddress = $"{authority}/.well-known/openid-configuration";
    
    var clientId = builder.Configuration["Keycloak:ClientSettings:ClientId"];
    var clientSecret = builder.Configuration["Keycloak:ClientSettings:ClientSecret"];

    options.Authority = authority;
    options.MetadataAddress = metadataAddress;
    options.ClientId = clientId;
    options.ClientSecret = clientSecret;

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UseTokenLifetime = true;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;
    
    options.CallbackPath = "/callback-oidc";
    options.SignedOutCallbackPath = "/signed-out-callback-oidc";
    
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
});

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("RookieShop.WebApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RookieShop:WebApi:Address"]!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IProductService>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    var imageGalleryBasePath = builder.Configuration["RookieShop:WebApi:Address"]!;
    
    return new ProductService(httpClientFactory, imageGalleryBasePath);
});
builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IReviewService, ReviewService>();

builder.Services.AddSingleton<ImageGalleryUrlResolver>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();