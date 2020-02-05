using System;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using RedditSharp;
using VerificationWeb.Configuration;
using VerificationWeb.Services;
using VerificationWeb.EXtensions;
using VerificationWeb.Models;

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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = new TimeSpan(0, 0, 10, 00);
                })
                .AddDiscordAuthentication(Config.DiscordId, Config.DiscordSecret)
                .AddRedditAuthentication(Config.RedditAuthId, Config.RedditAuthSecret)
                .AddFedoraAuthentication(SessionClaims.FedoraScheme, Config.FasId, Config.FasSecret)
                .AddRedhatAuthentication(SessionClaims.RedhatScheme, Config.RedhatClientId, Config.RedhatClientSecret, Config.RedhatOidcDiscoveryUri);
            
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
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseAuthentication();
            app.UseSession();
            app.Use((context, next) =>
            {
            // setting https scheme because of the oidc lib and the way they create redirect uris
                context.Request.Scheme = "https";
                return next();
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();
        }
    }
}