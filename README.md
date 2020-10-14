[![Build Status](https://dev.azure.com/dahln/template/_apis/build/status/template%20CI%20build?branchName=master)](https://dev.azure.com/dahln/template/_build/latest?definitionId=18&branchName=master)

# template

'template' is my Blazor app template. Microsoft provides several versions of templates for Blazor apps - they work great. My personal preference for a template is to have more functionality. This template provides simple pages and components that show CRUD ops, API calls, Authentication/Authorization with Identity. The Microsoft template shows how to do this, but I don't feel like the solutions there are designed for larger applications - my experiences with several frontend frameworks have lead to the creatinon this template.

# License

This application is open source. Please review the license before use. To summarize - this project can be used, closed, and distributed freely, without warrenty or liability. Questions or concerns can be send to admin@dahln.io.

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

# Issues/Contributions

If there are issues with the application, please create an "issue" here at github. The issue will be reviewed and responded to.
