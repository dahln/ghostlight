[![Build Status](https://dev.azure.com/dahln/glubfish/_apis/build/status/glubfish%20CI%20build?branchName=master)](https://dev.azure.com/dahln/glubfish/_build/latest?definitionId=14&branchName=master)

# glubfish

This is a simple CRM/XRM application. A simple designer is used to create a form. This form is composed of standard HTML input fields (text, textarea, number, select, date and time). Using the designer, the user selects a field type, specifies a name for the field and selects a row, column, and column width for the field. As many fields as the user wants can be added to the form. This form represents a dynamic/user-defined data type. Each dynamic/user-defined data type must belong to a Folder. A "Folder" can contain zero-many dynamic/user-defined types.

Each folder has its own permissions. Users can be added to a folder as a regular user, or granted an "administrator" role. Administrators of a folder can manage the dynamic data types for the folder, and add/remove users from that folder.

When a new user signs into the system, they should first create a folder, then any dynamic/user-defined types they want in that folder.

All users can create/manage their own folders and types. A user can own/manage zero-many folders. A user can be added to zero-many folders by other users.  Any folder that a user has access to is accessible via the "Search" drop down on the main navigation.

# License

This application is open source. Please review the license for use and distribution. If you are interested in deploying this application within your organization, please feel free to contact me at admin@dahln.io.

# Configuration/Setup

This application uses Entity Framework, Code-First. Run 'Update-Database' to create the initial DB schema. Automatic migrations are not enabled. Be sure to first update your appsettings file with the appropriate connection string.

Sensative configuration data, such as the DB connection strings, and Send Grid config are added to the appsettings.json files. Do not check in these values to the repo. Use the following commands to ignore changes to the appsettings.json files:

* git update-index --assume-unchanged appsettings.json
* git update-index --assume-unchanged appsettings.Development.json
* git update-index --assume-unchanged appsettings.Productions.json

Alternatively, sensative configuration values can be set as variables in your server config.

# Issues/Contributions

If there are issues with the application, please create an "issue" here at github. The issue will be reviewed and responded to.
