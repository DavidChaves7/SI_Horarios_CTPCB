
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using SI_Horarios_CTPCB.Components;
using SI_Horarios_CTPCB.Infrastructure.ApiClient;
using SI_Horarios_CTPCB.Infrastructure.Authentication;
using SI_Horarios_CTPCB.Infrastructure.Handlers;
using SI_Horarios_CTPCB.Infrastructure.Interfaces;
using SI_Horarios_CTPCB.Infrastructure.Middleware;
using SI_Horarios_CTPCB.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication;





var builder = WebApplication.CreateBuilder(args);

var conf = builder.Configuration;
var baseApiURL = builder.Environment.IsProduction() ? conf.GetSection("BaseApiURL").Value : conf.GetSection("LocalApiURL").Value; //obtener la URL del API, en dev es local y en prod(azure) es el appservice

// Add services to the container.
builder.Services.AddRazorComponents(opt =>
{
})
    .AddInteractiveServerComponents();
/*builder.Services.AddAuthentication().AddCookie(opt =>
{
    opt.LoginPath = "/Login";
    opt.LogoutPath = "/Login";
    //opt.AccessDeniedPath = "/MenuPrincipal";
    opt.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    opt.SlidingExpiration = true;
    opt.Cookie.IsEssential = true;
    opt.Cookie.HttpOnly = true;
    opt.Cookie.Name = "coopeBanCK";
    opt.Cookie.SameSite = SameSiteMode.Strict;
    //opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});*/

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
    



builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        options.Prompt = "select_account";

        // Donde el IdP (Microsoft) te devuelve tras el sign-out
        options.SignedOutCallbackPath = "/signout-callback-oidc";

        options.Events = new OpenIdConnectEvents
        {
            OnSignedOutCallbackRedirect = context =>
            {
                // Al volver del IdP, re-dirige a nuestro endpoint de login
                context.Response.Redirect("/Login");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });


builder.Services
    .AddControllersWithViews()
    .AddMicrosoftIdentityUI();


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<ICustomSessionStorage, CustomSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<CircuitServicesAccessor>();
builder.Services.AddScoped<CircuitHandler, ServicesAccessorCircuitHandler>();
builder.Services.AddScoped<AuthenticationHandler>();
builder.Services.AddScoped<ExcelService>();
builder.Services.AddScoped<TxtService>();
builder.Services.AddScoped<DataGridUtil>();
builder.Services.AddHttpClient("baseHttpClient", opt =>
{
    opt.BaseAddress = new Uri(baseApiURL);
}).AddHttpMessageHandler<AuthenticationHandler>();

builder.Services.AddScoped(
    provider =>
    {
        var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var httpClient = clientFactory.CreateClient("baseHttpClient");
        return new SIHAPI(baseApiURL, httpClient);
    }
);

builder.Services.AddScoped<UserService>();
builder.Services.TryAddEnumerable(
    ServiceDescriptor.Scoped<CircuitHandler, UserCircuitHandler>());

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

builder.Services.AddSingleton<SI_Horarios_CTPCB.Infrastructure.Services.TabSessionRegistry>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.Use(async (context, next) =>
//{
    
    //if(context.Request.Path == "/" || context.Request.Path == "")
    //{
    //    if (app.Environment.IsProduction())
    //        context.Response.Redirect("/Login");
    //   else
    //        context.Response.Redirect("/Login");
   //}
   // if (context.Response.StatusCode == 404)
   // {
   //     if (app.Environment.IsProduction())
  //      {
   //         context.Response.Redirect("/NotFound");
   //     }
    //    else
    //    {
    //        context.Response.Redirect("/NotFound");
    //    }
   // }
   // await next(context);
//});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseMiddleware<UserServiceMiddleware>();
app.MapControllers(); 
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();
//if (app.Environment.IsProduction())
//    app.UsePathBase("/");
app.MapGet("/Login", async (HttpContext ctx) =>
{
    var props = new AuthenticationProperties { RedirectUri = "/MenuPrincipal" }; // o "/"
    await ctx.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
    return Results.Empty;
});

app.MapGet("/Logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    return Results.Empty;
});

app.Run();

