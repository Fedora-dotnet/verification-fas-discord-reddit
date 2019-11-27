using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace VerificationWeb.EXtensions
{
    internal static class AuthenticationExtensions
    {
        
        public static AuthenticationBuilder AddFedoraAuthentication(this AuthenticationBuilder builder,string authScheme,string clientId, string clientSecret)
        {
            builder.AddOpenIdConnect(authScheme, o =>
            {
                o.ClientId = clientId;
                o.ClientSecret = clientSecret;
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
            });
            return builder;
        }

        public static AuthenticationBuilder AddDiscordAuthentication(this AuthenticationBuilder builder, string clientId, string clientSecret)
        {
            return builder.AddDiscord(o =>
            {
                o.ClientId = clientId;
                o.ClientSecret = clientSecret;
                o.CorrelationCookie.IsEssential = true;
                o.ClaimActions.MapAll();
                o.CallbackPath = "/signin-discord";
            });
        }

        public static AuthenticationBuilder AddRedditAuthentication(this AuthenticationBuilder builder, string clientId, string clientSecret)
        {
            return builder.AddReddit(x =>
            {
                x.CorrelationCookie.IsEssential = true;
                x.ClientId = clientId;
                x.ClientSecret = clientSecret;
                x.ClaimActions.MapJsonKey("id", "id");
                x.ClaimActions.MapJsonKey("name", "name");
                x.CallbackPath = "/signin-reddit";
                x.Scope.Add("identity");
            });
        }

        public static AuthenticationBuilder AddRedhatAuthentication(this AuthenticationBuilder builder,string authScheme,string clientId, string clientSecret, string configUri)
        {
                      
            return builder.AddOpenIdConnect(authScheme, o =>
            {
                o.ClientId = clientId;
                o.ClientSecret = clientSecret;
                o.Authority = "https://auth.stage.redhat.com/auth/realms/EmployeeIDP";
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(configUri,new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());
                o.Configuration = o.ConfigurationManager.GetConfigurationAsync(CancellationToken.None).Result;
                o.CallbackPath = "/signin-redhat";
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.CorrelationCookie.IsEssential = true;
                o.GetClaimsFromUserInfoEndpoint = true;
                o.ClaimActions.MapJsonKey(ClaimTypes.Name, "fullname");
                o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                o.ClaimActions.MapJsonKey("preferred_username", "username");
                o.TokenValidationParameters = new TokenValidationParameters {
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidIssuer = o.Configuration.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = o.Configuration.SigningKeys,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };
            });
        }
    }
}