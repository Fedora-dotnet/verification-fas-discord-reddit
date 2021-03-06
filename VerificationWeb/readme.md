Example config - appsettings.json

```
{
  "DiscordToken": "",                           <-- Token of the discord bot
  "DiscordId": "",                              <-- Client id for the discord authentication
  "DiscordSecret": "",                          <-- Client secret for the discord authentication
  "GuildId" : 123123123,                        <-- Id of the guild

  "FasId":"",                                   <-- Client id for fedora account authentication
  "FasSecret": "",                              <-- Client secret for fedora account authentication

  "RedditAuthId" :  "",                         <-- Client id of reddit authentication
  "RedditAuthSecret" : "",                      <-- Client secret of reddit authentication
  "RedditBotId" : "",                           <-- Client id of reddit bot app (the one that gives flairs)
  "RedditBotSecret" : "",                       <-- Client secret of reddit bot app (the one that gives flairs)
  "RedditBotRefreshToken" : "",                 <-- Refresh token of reddit bot app to retrieve access tokens

  "Subreddit" : "/r/subreddit,                  <-- Subreddit where the flairs are given at
  "RedhatSubreddit" : "/r/subreddit,                  <-- Subreddit where the flairs are given at
  "RedirectUri" : "https://127.0.0.1:5001/",    <-- Redirect URI for the reddit bot app, base URI of the web.

  "RedhatClientId" : "",                        <-- Client id for redhat authentication
  "RedhatClientSecret" : "",                    <-- Client secret for redhat authentication
  "RedhatOidcDiscoveryUri" : "",                <-- URL to redhat OIDC configuration

    // Removed older RoleConditions,
    
  "FedoraFlairCss" : "FedoraFlairCss",
  "RedhatFlairCss" : "RedhatFlairCss",

  "ContributorRoles": [605756010153639956, 675171600278093834], <-- All roles that user gets as contributor at any server
  "DotnetRoles": [606110843276361729],
  "RedhatRoles": [649672841087942676],
    
    
  "RedditFlairs" : {                            <-- Flair key = necessary groups, key name of the flair
    "cla/done" : "Contributor",
    "dotnet-team": "Dotnet",
    "Redhat": "Employee",                       <-- "Redhat" key must be set, condition is automatically login through Redhat SSO
  }
}
```