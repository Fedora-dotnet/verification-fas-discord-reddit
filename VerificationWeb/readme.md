## Config

Config is saved to the appsettings.json file in following way:

{
  "DiscordToken": "",
  "DiscordId": "12312321321321",
  "DiscordSecret": "",
  "FasId":"",
  "FasSecret": "",
  "RedditVerificationId" :  "",     
  "RedditVerificationSecret" : "",
  "RedditBotId" : "",
  "RedditBotSecret" : "",
  "RedditBotRefreshToken" : "",
  "RedditBotAccessToken" : "",
  "GuildId" : 12312321321321,
  "Subreddit" : "/r/some_subreddit", 
  "RedirectUri" : "https://127.0.0.1:5001/",
  
  "DiscordRoles": {
      "Contributor" : 12312321321321, // Key is name of the role, value is its discord id
      "Dotnet": 12312321321321        // Key is name of the role, value is its discord id
  },
  "RolesConditions" : {
      "cla/done" : "Contributor", // the key is the attribute that the user must have to get the role
      "dotnet-team": "Dotnet"
  },
  "RedditFlairs" : {
    "cla/done" : "Contributor",  // the key is the attribute that the user must have to get the flair
    "dotnet-team": "Dotnet"
  }
}
