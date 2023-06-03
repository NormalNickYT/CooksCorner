# CooksCorner
Windesheim School Project

CooksCorner is a website that provides users with the ability to log in and add recipes. Additionally, users can view each other's recipes and save their favorites to their account. Users will receive notifications when another user adds their recipes to their favorites.

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
