This web-application aims to provide identity verification of Reddit and Discord users against Fedora Account Service, in order to verify that they are Fedora contributors.

This will be a simple ASP.NET Core[0] web application written in C#, which will be using 3rd party libraries described below.

The user will be met with a simple web interface which will have two options on the landing page, and that would be the choice between Reddit and Discord verification. Both of these sub-pages will have Fedora Account System (aka FAS) login using ipsilon[1] as the first step to complete, and then either Reddit or Discord login using their respective libraries.[2, 3, 4] After they're logged in with both FAS and Reddit, they will be able to choose one of their FAS groups as a flair on Reddit, or in case of Discord, they will be assigned a "Contributor" role on the Fedora Discord, if they have CLA[5] signed.

Potential milestones could be:

* Simple graphical interface as described above, without the actual functionality.*
* FAS login using ipsilon.
* Discord login.
* Discord role assignment.
* Reddit login.
* Reddit flair assignment.

_*Note: You can ask for some suggestions about the looks of it, and help from the Fedora Design team, and the final website will have to be approved by the Council for the use of the Fedora trademark (this will be handled by your Mentor.)_

**We should aim to deliver at least Discord verification in full and if the time won't permit, consider Reddit part as a secondary goal.**

[0] ASP.NET Basics (it's a whole series, not just one video, and although it's about aspnet-core-1.0 it's very similar to 2.0 as well. The project file is different.) https://mva.microsoft.com/en-US/training-courses/aspnet-core-10-crossplatform-17039?l=xVagIgJOD_7201937555

[1] Ipsilon authentication with the Fedora Account System: https://ipsilon-project.org/
* FAS: https://admin.fedoraproject.org/accounts/
* Wiki: https://fedoraproject.org/wiki/Account_System

[2] RedditSharp: https://github.com/CrustyJew/RedditSharp

[3] Discord.NET (we will be using my fork on dev branch, but you can use the nuget/myget library they provide) https://github.com/RogueException/Discord.Net

[4] Discord.NET guide: https://discord.foxbot.me/docs/guides/getting_started/installing.html

[5] CLA = Contributors License Agreement are two FAS groups that are assigned to everyone who signed the CLA and FPCA (Fedora project contributors agreement.) These groups are "cla_done" and "cla_fpca"
