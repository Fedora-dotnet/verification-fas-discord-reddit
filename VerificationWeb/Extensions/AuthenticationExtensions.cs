using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using VerificationWeb.Models;

namespace VerificationWeb.EXtensions
{
    internal static class AuthenticationExtensions
    {

        public static AuthenticationBuilder AddFedoraAuthentication(this AuthenticationBuilder builder,string authScheme,string clientId, string clientSecret, string url)
        {
            return builder.AddOpenIdConnect(authScheme, o =>
            {
                o.ClientId = clientId;
                o.ClientSecret = clientSecret;
                o.Authority = url;
                o.Scope.Add($"{url}/scope/groups");
                o.Scope.Add($"{url}/scope/cla");
                o.ClaimActions.MapJsonKey(SessionClaims.Cla, SessionClaims.Cla);
                o.ClaimActions.MapJsonKey(SessionClaims.Username, "nickname");
                o.ClaimActions.MapJsonKey(SessionClaims.Groups, SessionClaims.Groups);
                o.CallbackPath = "/signin-fedora";
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.CorrelationCookie.IsEssential = true;
                o.GetClaimsFromUserInfoEndpoint = true;
            });
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
                o.Authority = "https://auth.redhat.com/auth/realms/EmployeeIDP";
                o.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(configUri,new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());
                o.Configuration = o.ConfigurationManager.GetConfigurationAsync(CancellationToken.None).Result;
                o.CallbackPath = "/signin-redhat";
                o.ResponseType = OpenIdConnectResponseType.Code;
                o.CorrelationCookie.IsEssential = true;
                o.GetClaimsFromUserInfoEndpoint = true;
                o.ClaimActions.MapJsonKey(SessionClaims.Username ,"preferred_username");
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
