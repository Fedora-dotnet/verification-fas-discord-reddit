using System.Collections.Generic;

namespace VerificationWeb.Configuration
{
    public class Config
    {
        public string DiscordToken { get; set; }
        public string DiscordId { get; set; }
        public string DiscordSecret { get; set; }
        public string FasId { get; set; }
        public string FasSecret { get; set; }
        public string RedditVerificationId { get; set; }
        public string RedditVerificationSecret { get; set; }
        public string RedditBotId { get; set; }
        public string RedditBotSecret { get; set; }
        public string RedditBotRefreshToken { get; set; }
        public string RedhatClientId { get; set; }
        public string RedhatClientSecret { get; set; }
        public ulong GuildId { get; set; }
        public string Subreddit { get; set; }
        public string RedirectUri { get; set; }
        public Dictionary<string, ulong> DiscordRoles { get; set; }
        public Dictionary<string, string> RolesConditions { get; set; }
        public Dictionary<string, string> RedditFlairs { get; set; }
        
        public string RedhatOidcDiscoveryUri { get; set; }
        public string FedoraOidcDiscoveryUri { get; set; }
    }
}