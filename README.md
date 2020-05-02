This is simple CRM/XRM application. Dynamic data types can be defined, managed, and searched by the users. 

A user creates a group. Each groups has its own data types and user permissions. A user can belong to multiple groups (or no groups) and switch between them. All the data types created and managed fall under the management of an organization profile.

This application is open source. Please review the license for use and distribution. If you are interested in deploying this application within your organization, please feel free to contact me at admin@dahln.io.

This application uses SQL for all persistant storage - except for the storage of "instances" of the dynamic data types. The storage of those instances is stored in MongoDB.

Sensative configuration data, such as the DB connection strings and the signing key should be added to the appsettings.json files. Do not check in these values to the repo. Use the following commands to ignore changes to the appsettings.json files.

git update-index --assume-unchanged appsettings.json
git update-index --assume-unchanged appsettings.Development.json
Update the AppSettings.json to include this section, and add the appropriate values. If you don't want the sensative information in your AppSettings, You could include them as variables in your server configuration.

The SQL Database use Entity Framework, code first.  Automatic migrations are not enabled. When starting up the application for the first time, run "Update-Database" from the nuget console in Visual Studio.

The MongoBD database requires a $text index to support text searching across the dynmaic data types. Connect to the database, and run this monogo command to create the appropiate index:

db.instances.createIndex( { GroupId: 1, TypeId: 1, "$**": "text" } )
Tip: if you are using the MongoDB Atlas service, you can create the index by visiting the portal.

As of 4.30.20 - here is the of items needed to be completed before I consider the application production ready:
* Review/Adjust new user UI flow
* Reivew/Adjust initial deployment process
* (FIXED) Fix required/optional fields enforcement on instance view
* (FIXED) Switch Group Search Instance bug
* Modify styles/content of default Identity pages
* Security/Authorization
* Loading Spinner on API calls
* metadata
* favicon
* deploy to depot.dahln.io with ssl cert
* -------- backlog --------
* Add Confirmation Prompts (not necessary for v1)
* Linked fields (not necessary for v1)
* File Attachments fields (not necessary for v1)
