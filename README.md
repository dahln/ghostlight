When .NET 5 is official released (a non-RC release), this template will be updated to the new version. This template currently does not show how to upload a file; the updated .NET 5 version will demonstrate that.

[![Build Status](https://dev.azure.com/dahln/template/_apis/build/status/template%20CI%20build?branchName=master)](https://dev.azure.com/dahln/template/_build/latest?definitionId=18&branchName=master)

# template

'template' is my Blazor app template. Microsoft provides several versions of templates for Blazor apps - they work great. My personal preference for a template is to have more functionality. This template provides simple pages and components that show CRUD ops, API calls, Authentication/Authorization with Identity.

# How to create a new project
When I use this template to create a new project, I clone the project and then open the folder in VS Code. Then I do a global search & replace for 'template' and replace it with the name of the new project. This also replace the namespaces as well. Second, I go into the file explorer and I rename the all the folder with the name template in them. And last, I rename the sln and csproj files to match the new name of the project.

When cloning the repo, notice that the appsettings does not have any SMTP credentials. Those values are excluded for security reasons - fill them in with your own credentials. The demo site (template.dahln.io), when deployed, fills in those values. Email and Account confirmation is disabled for ease-of-use with new projects and during development, I recommend enabling them; they can be enabled in the Startup.cs.

Note: I have noticed that Identity doesn't seem to be sending recovery emails, unless an account is confirmed. To recieve a password reset (recovery email) first confirm your email, either by cliking the link in the original email or resending the confirmation email.

# Configuration/Setup

Out of the box this project uses Sqlite - this is for ease of startup. You will find in the startup.cs a snippet of commented code that will switch to MSSQL when uncommented. DB migrations for Sqlite are included and should work the first time the application starts up. If you switch to MSSQL, delete the migrations folder and create a new 'Initial Migration' by running 'Add-Migration InitialCreate', the 'Update-Database'. Also, besure to add the connecting string to the appropriate property in the appsettings.json file.

Sensative configuration data, such as the DB connection strings, and email config are added to the appsettings.json files. Do not check in these values to the repo. Use the following commands to ignore changes to the appsettings.json files:

* git update-index --assume-unchanged appsettings.json
* git update-index --assume-unchanged appsettings.Development.json	
* git update-index --assume-unchanged appsettings.Production.json

Change the environment (and appsettings loaded) with the following commands:
* $Env:ASPNETCORE_ENVIRONMENT = "Development"
* $Env:ASPNETCORE_ENVIRONMENT = "Production"

Alternatively, sensative configuration values can be set as variables in your server config.

# License

This application is open source. Please review the license before use. To summarize - this project can be used, closed, and distributed freely, without warrenty or liability. Questions or concerns can be send to admin@dahln.io.

# Issues/Contributions

If there are issues with the application, please create an "issue" here at github. The issue will be reviewed and responded to.
