using System;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RedditSharp;
using VerificationWeb.Configuration;
using VerificationWeb.Services;
using VerificationWeb.EXtensions;

namespace VerificationWeb
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public readonly Config Config;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Config = Configuration.Get<Config>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            var redhatIssuer = Config.RedhatOidcDiscoveryUri;
            var redhatConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                redhatIssuer,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
            var redhatDiscoveryDocument =  redhatConfigurationManager.GetConfigurationAsync(CancellationToken.None).Result;
            
            var fedoraIssuer = Config.FedoraOidcDiscoveryUri;
            var fedoraConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                fedoraIssuer,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());
            var fedoraDiscoveryDocument =  fedoraConfigurationManager.GetConfigurationAsync(CancellationToken.None).Result;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/unauthorized";
                    //options.LogoutPath = "/logout";
                    options.ExpireTimeSpan = new TimeSpan(0, 0, 10, 00);
                })
                .AddOpenIdConnect("Fedora" ,o =>
                {
                    o.ClientId = Config.FasId;
                    o.ClientSecret = Config.FasSecret;
                    o.Authority = "https://iddev.fedorainfracloud.org";
                    o.Scope.Add("https://id.fedoraproject.org/scope/groups");
                    o.Scope.Add("https://id.fedoraproject.org/scope/cla");
                    o.ClaimActions.MapJsonKey("cla", "cla");
                    o.ClaimActions.MapJsonKey("nickname", "nickname");
                    o.ClaimActions.MapJsonKey("groups", "groups");
                    o.CallbackPath = "/signin-oidc";
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.CorrelationCookie.IsEssential = true;
                    o.GetClaimsFromUserInfoEndpoint = true;
                })
                .AddOpenIdConnect("Redhat",o =>
                {
                    o.ClientId = Config.RedhatClientId;
                    o.ClientSecret = Config.RedhatClientId;
                    o.Authority = "https://auth.stage.redhat.com/auth/realms/EmployeeIDP/protocol/openid-connect";
                    o.ClaimActions.MapJsonKey("given_name", "nickname");
                    o.CallbackPath = "/signin-redhat";
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.CorrelationCookie.IsEssential = true;
                    o.GetClaimsFromUserInfoEndpoint = true;
                })
                .AddDiscord(o =>
                {
                    o.ClientId = Config.DiscordId;
                    o.ClientSecret = Config.DiscordSecret;
                    o.CorrelationCookie.IsEssential = true;
                    o.ClaimActions.MapAll();
                    o.CallbackPath = "/signin-discord";
                })
                .AddReddit(x =>
                {
                    x.CorrelationCookie.IsEssential = true;
                    x.ClientId = Config.RedditVerificationId;
                    x.ClientSecret = Config.RedditVerificationSecret;
                    x.ClaimActions.MapJsonKey("id", "id");
                    x.ClaimActions.MapJsonKey("name", "name");
                    x.CallbackPath = "/signin-reddit";
                    x.Scope.Add("identity");
                });
//                .AddFedoraAuthentication("Fedora", Config.FasId, Config.FasSecret, fedoraDiscoveryDocument)
//                .AddRedhatAuthentication("Redhat", Config.RedhatClientId, Config.RedhatClientSecret, redhatDiscoveryDocument);
            
            var webAgent = new RefreshTokenWebAgent(Config.RedditBotRefreshToken, Config.RedditBotId, Config.RedditBotSecret, Config.RedirectUri);

            services.AddSingleton(webAgent);
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "Session";
            });
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddSingleton(Config);
            services.AddSingleton<RoleService>();
            services.AddDiscordBot();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseSession();
            // setting https scheme because of the oidc lib and the way they create redirect uris
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });
//            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();
        }
    }
}