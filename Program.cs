using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using VirtualStoreApp.Auth;
using VirtualStoreApp.Components;
using VirtualStoreApp.Services;

var builder = WebApplication.CreateBuilder(args);


var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookies as default scheme
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // USe OpenID Connect for challenges
    })
    .AddCookie() 
    .AddOpenIdConnect("Auth0", options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        options.ResponseType = "code";
        options.CallbackPath = new PathString("/callback");
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.ClaimsIssuer = "Auth0";
        options.SkipUnrecognizedRequests = true;


        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.SetParameter("audience", builder.Configuration["Auth0:Audience"]);
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.Requirements.Add(new HasScopeRequirement("admin", domain)));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth0"));


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorizationCore();

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
