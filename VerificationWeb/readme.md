Example config - appsettings.json

{
  "DiscordToken": "",                           <-- Token of the discord bot
  "DiscordId": "",                              <-- Client id for the discord authentication
  "DiscordSecret": "",                          <-- Client secret for the discord authentication
  "GuildId" : 123123123,                        <-- Id of the guild

  "FasId":"",                                   <-- Client id for fedora account authentication
  "FasSecret": "",                              <-- Client secret for fedora account authentication
  "FedoraOidcDiscoveryUri" : "",                <-- URL to redhat OIDC configuration

  "RedditAuthId" :  "",                         <-- Client id of reddit authentication
  "RedditAuthSecret" : "",                      <-- Client secret of reddit authentication
  "RedditBotId" : "",                           <-- Client id of reddit bot app (the one that gives flairs)
  "RedditBotSecret" : "",                       <-- Client secret of reddit bot app (the one that gives flairs)
  "RedditBotRefreshToken" : "",                 <-- Refresh token of reddit bot app to retrieve access tokens

  "Subreddit" : "/r/subreddit,                  <-- Subreddit where the flairs are given at
  "RedirectUri" : "https://127.0.0.1:5001/",    <-- Redirect URI for the reddit bot app, base URI of the web.

  "RedhatClientId" : "",                        <-- Client id for redhat authentication
  "RedhatClientSecret" : "",                    <-- Client secret for redhat authentication
  "RedhatOidcDiscoveryUri" : "",                <-- URL to redhat OIDC configuration

  "RoleConditions" : {                          <-- Each key is the requirement for the value Role or Flair
    "cla/done" : "Contributor",                 <-- to get flair or role "Contributor", user must have signed cla ("cla/done" claim)
    "dotnet-team": "Dotnet",                    <-- to get flair or role "Dotnet", user must be member of dotnet-team group
  },

  "DiscordRoles": {                             <-- Discord id of each role defined above
    "Contributor" : 605756010153639956,
    "Dotnet": 606110843276361729,
    "Redhat": 649672841087942676                <-- "Redhat" key must be set, condition is automatically login through Redhat SSO
  },
  "RedditFlairs" : {                            <-- Discord id of each role defined above
    "Contributor" : "Contributor",
    "Dotnet": "Dotnet",
    "Redhat": "Employee",                       <-- "Redhat" key must be set, condition is automatically login through Redhat SSO
  }
}