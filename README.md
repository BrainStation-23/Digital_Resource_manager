Digital Resource Manager
========================
# ASP.NET MVC4 Webapi, Backbone Resource Management Application
 * This is  **First version of Resource Management Application** used for **ASP.NET MVC4 webapi and Backbone integration purposes**. 
 * This is a good code base of ASP.NET MVC4 webapi in restful architecture, since it covers many areas and development techniques.
 * We are using it as a personal and official resource management tool for our resource management activity, you may use it as well for this or any other purpose (it is very **easy to customize** to fit your needs) 
 * The solution has been created with **Visual Studio 2012**.
 
## Details Feature and documentation

[Feature and documentation](http://brainstation-23.github.com/Digital_Resource_manager/)

## Which tecnology used this application:

 * This application used **ASP.NET MVC4 webapi from scratch.** and uses these cool features  like:
   * **Code First**
   * **Entity Framework** and **LINQ**
   * **Facade pattern**
   * **Code First Membership Provider** pointing to your own database users table.
   * **Backbonejs** which gives good structure to the application in client side code.
   * **Requirejs** used it for AMD API for all JavaScript modules.
   * **Dust** templeting engine for better rendering experience.
   * **Twitter bootstrap** to see a awesome UI.
 * Every major development on this Resource Management app has been tagged (1.02)
 * You can download each tag (starting with 1.0), check progress and move to the next when you understood everything that has been done.
 * Follow the change log (tag history) and enjoy!
 
### Altering connectionStrings section 

Based on convention, EF will look for a connection string named as the DBContext (in this case "FileManagerDbContext") in webconfig and appconfig, and will use it, so feel free to set the data provider you want:

     <!-- 
         By default (convention over configuration, the connection string with the same name as your DBContext will be used0 
         You can select then wherever you will use SQL CE, SQL Server Express Edition, etc, here. 
     -->
     <add name="FileManagerDbContext" connectionString="Data Source=|DataDirectory|ResourceManager.sdf" providerName="System.Data.SqlServerCe.4.0" />
     <!--
     <add name="FileManagerDbContext" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=ResourceManager; Integrated Security=True; MultipleActiveResultSets=True" providerName="System.Data.SqlClient" />
     -->
	 

## Screenshots

### Create Resource

![Create Resource](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/addResource.png)

### Search

![Search](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/Search.png)

### List(Cloud Tag)

![List](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/list.png)	


### Favourite

![Favourite](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/Favourite.png)

### Basket

![Basket](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/Basket.png)

### Category

![Category](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/Category.png)

### User

![User](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/User.png)

### EditRole

![EditRole](https://github.com/BrainStation-23/Digital_Resource_manager/raw/master/ResourceScreenshot/EditRole.png)


## Developed By:

* [BrainStation-23](http://www.brainstation-23.com)

## Change Log:

### 1.02
	* Changed Home Url mapping.
	* Handle few unhandle exception.
	* Fix Login page css issue.
	* Fix requirejs loading timout issue.

### 1.01
	* Fix UI Issues.

### 1.00
	* Initial release of Digital Resource Manager.