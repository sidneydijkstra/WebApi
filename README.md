
  
# Web Api  
A Web Api using [ASP.NET 6](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio) with the focus on authentication, making use of a [JwtToken](https://jwt.io/) in combination with a RefreshToken. Using the default swagger page for testing, this Api can be used as a secure option for authentication.  
  
### Dependencies  
For this Api to work correctly there are a couple dependencies needed. Like a stable connection to a MySQL server for storing user details and the RefreshToken. Microsoft has great packages that handle this, by using [EntityFrameworkCore](https://docs.microsoft.com/en-us/ef/core/) we can easily add on to this project and not bother with any SQL code.  
  
Generating the JwtToken is also done with a package by Microsoft called [JwtBearer](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer?view=aspnetcore-6.0). This package is great for generating these tokens and has lots of options to get it up to standards.  
```
> Microsoft.AspNetCore.Authentication.JwtBearer 6.0.1 6.0.1  
> Microsoft.AspNetCore.Identity.EntityFrameworkCore 6.0.1 6.0.1  
> Microsoft.AspNetCore.Identity.UI 6.0.1 6.0.1  
> Microsoft.EntityFrameworkCore.Design 6.0.1 6.0.1  
> Microsoft.EntityFrameworkCore.SqlServer 6.0.1 6.0.1  
> Pomelo.EntityFrameworkCore.MySql 6.0.1 6.0.1  
> Swashbuckle.AspNetCore 6.2.3 6.2.3  
```  
Visual Studio and Visual Code have options to automatically install these packages.  
  
### Settings  
Before using the project some settings have to be defined. This is done in the file 'appsettings.json' and contains something like this:  
```
"ConnectionStrings": {  
	"DefaultConnection": "server=127.0.0.1;uid=root;pwd=password123;database=nameofdatabase;"  
},  
  
"JwtConfig": {  
	"Secret": "Yp6OeKj8rkrYLpB36i6bCdEkeQuB2iLM",  
	"JwtTokenLifetime": 60,  
	"RefreshTokenLifetime": 86400  
},  
  
"RolesConfig": {  
	"Default": "ROLE_USER",  
	"Admin": "ROLE_ADMIN",  
	"User": "ROLE_USER"  
},  
```
The most important value to change is the DefaultConnection used as connection string for the database. The JwtConfig can be used to change you're secret, this is used for generating the JwtTokens. You can also change the lifetime of the JwtToken and RefreshToken. And last the RolesConfig is used to created new Roles, these roles can be used to specify witch Api calls the user can and can't do.
