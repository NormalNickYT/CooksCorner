# CooksCorner
At CooksCorner, users are greeted with a warm and welcoming environment where they can log in and unlock a world of endless recipe possibilities. With just a few clicks, they can effortlessly add their own recipes, ensuring that their unique creations receive the recognition they deserve.

But CooksCorner is not just about showcasing individual recipes; it's a vibrant community of cooking aficionados who relish in the joy of discovering and exploring new flavors. Users have the privilege of immersing themselves in a treasure trove of diverse recipes shared by fellow enthusiasts. Whether it's a delectable dessert, a comforting family recipe, or a daring fusion of flavors, there's something to captivate every palate.

What sets CooksCorner apart is its ingenious feature that allows users to curate their personal collection of culinary masterpieces. By saving their favorite recipes directly to their account, users can easily revisit and recreate their most beloved dishes, creating a personal cookbook tailored to their tastes.

Furthermore, CooksCorner fosters a sense of connection and camaraderie among its users. As a delightful surprise, users are promptly notified whenever another passionate cook adds their recipes to their favorites. This thoughtful gesture not only honors the hard work and creativity of the recipe creators but also creates a sense of appreciation and encouragement within the community.

CooksCorner is more than just a website; it's a virtual haven for cooking enthusiasts to connect, inspire, and elevate their culinary skills. It beckons all food lovers to embark on a gastronomic adventure where they can share their love for cooking, discover new recipes, and forge meaningful connections with fellow epicureans. Join us at CooksCorner and let your culinary journey unfold in a world brimming with flavors, innovation, and shared passion.

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
