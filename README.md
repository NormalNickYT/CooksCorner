# CooksCorner
CooksCorner is a project i created for school. In CooksCorner users can add their own recipes and favorite other ones recipes. 
# Setup
The app is in .NET 6 and can be started by building the `CooksCornerAPP` in Visual Studio.

If you wan't to use the properties in the database you should run the following command:

```
# Add-Migration NameOfTheCommit

# Update-Database

```

This should add the database tables if you use SQLEXPRESS as a database the database can be changed in the `appsettings.json`
Also the Mail system and Captcha aren't setup in the `appsettings.json` so this functionality won't work if you don't set it up yourself. 

:) 
