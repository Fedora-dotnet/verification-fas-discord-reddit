using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebApplication1.EXtensions;

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
                })
                .AddOpenIdConnect(o =>
                {
                    o.ClientId = Configuration["FasId"];
                    o.ClientSecret = Configuration["FasSecret"];
                    o.Authority = "https://iddev.fedorainfracloud.org";
                    o.Scope.Add("https://id.fedoraproject.org/scope/groups");
                    o.Scope.Add("https://id.fedoraproject.org/scope/cla");
                    o.ClaimActions.MapJsonKey("cla", "cla");
                    o.ClaimActions.MapJsonKey("nickname", "nickname");
                    o.ClaimActions.MapJsonKey("groups", "groups");
//                    o.SaveTokens = true;
//                    o.RequireHttpsMetadata = false;
                    o.CallbackPath = "/signin-oidc";
                    o.ResponseType = OpenIdConnectResponseType.Code;
//                    o.CorrelationCookie.IsEssential = true;
                    o.GetClaimsFromUserInfoEndpoint = true;
//                    o.CorrelationCookie.Expiration = TimeSpan.Zero;
//                    o.ClaimActions.MapAll();
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddDiscord(x =>
                {
                    x.AppId = Configuration["DiscordId"];
                    x.AppSecret = Configuration["DiscordSecret"];
                    x.SaveTokens = true;
                    x.CorrelationCookie.IsEssential = true;
                    x.ClaimActions.MapAll();
                });

            services.AddDistributedMemoryCache();
            services.AddSession(options => { options.Cookie.IsEssential = true; });

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

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
                app.UseHsts();
            }

            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    // add claims here 
                    context.User.AddIdentity(context.User.Identities.FirstOrDefault());

//                    context.User.Identities.FirstOrDefault(x => x.AuthenticationType == "myAuth").Claims.Append(new Claim("type-x","value-x"));
                }

                await next();
            });
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();
        }
    }
}