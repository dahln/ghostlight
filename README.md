[![Build Status](https://dev.azure.com/dahln/depot/_apis/build/status/depot%20CI%20pipleline?branchName=release)](https://dev.azure.com/dahln/depot/_build/latest?definitionId=13&branchName=release)

This is simple CRM/XRM application. A simple designer is used to create a form. This form is composed of standard HTML input fields (text, textarea, number, select, date and time). Using the designer, the user selects a field type, specifies a name for the field and selects a row, column, and column width for the field. As many fields as the user wants can be added to the form. This form represents a dynamic/user-defined data type. Each dynamic/user-defined data type must belong to a Folder. A "Folder" can contain zero-many dynamic/user-defined types.

Each folder has its own permissions. Users can be added to a folder as a regular user, or granted a "administrator" role. Administrators of a folder can manage the dynamic data types for the folder, and add/remove users from that folder.

When a new user signs into the system, they should first create a folder, then any dynamic/user-defined types they want in that folder.

All users can create/manage their own folders and types. A user can own/manage zero-many folders. A user can be added to zero-many folders by other users.  Any folder that a user has access to is accessible via the "Search" drop down on the main navigation.

This application is open source. Please review the license for use and distribution. If you are interested in deploying this application within your organization, please feel free to contact me at admin@dahln.io.

This application uses SQL for all persistant storage - except for the storage of "instances" of the dynamic data types. The storage of those instances is stored in MongoDB.

Sensative configuration data, such as the DB connection strings, and Send Grid config can be added to the appsettings.json files. Do not check in these values to the repo. Use the following commands to ignore changes to the appsettings.json files:

* git update-index --assume-unchanged appsettings.json
* git update-index --assume-unchanged appsettings.Development.json
* git update-index --assume-unchanged appsettings.Productions.json

Alternatively, sensative configuration values can be set as variables in your server config.

The SQL Database use Entity Framework, code first.  Automatic migrations are not enabled. When starting up the application for the first time, run "Update-Database" from the nuget console in Visual Studio.

The MongoBD database requires a $text index to support text searching the dynmaic data types. On startup the system will check this index exists in the specified Mongo DB. If it does not exist, it will created it. The index can be manually created, but be sure to remove the automatic index check/creation code.

db.instances.createIndex( { FolderId: 1, TypeId: 1, "$**": "text" } )
Tip: if you are using the MongoDB Atlas service, you can create the index by visiting the portal.
