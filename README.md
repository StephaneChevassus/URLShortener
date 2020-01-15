# URL Shortener sample ASP.NET Core MVC web application

This is a sample website written in ASP.NET Core 3.1 that allows any user to shorten a URL.

## Architecture

This sample web app is built with [ASP.NET Core MVC](https://dotnet.microsoft.com/apps/aspnet/mvc), a design pattern for achieving a clean separation of concerns.

The web app contains the following:

* Models: object entities
* Views: user interface using Razor view engine
* Controllers: route requests to controller actions, implemented as normal C# methods
* Services: service class to handle CRUD operations

For simplicity, the data isn't stored into a database but in the application memory using a singleton service.

## Application setup

Clone the following repository:

URLShortener:  https://github.com/StephaneChevassus/URLShortener

## Running in an IDE

To run the sample from [Visual Studio 2019](https://www.visualstudio.com/vs/):

1. Open the solution in your IDE (using the _URLShortener.sln_ file).
2. Run the app.

## Screenshots

### Landing Page & Sample URL Data Entry

![URLShortener Sample Data Entry](/static/screenshots/screenshot-home-sample-data-entry.png "URL Shortener Sample Data Entry")

### Short URL Created

![Short URL Created](/static/screenshots/screenshot-shortUrl-created.png "Short URL Created")

## Mobile Support

The web app is developed using the responsive web design approach so it can adapt to different screen sizes.

### Landing Page & Sample URL Data Entry

![URLShortener Sample Data Entry](/static/screenshots/screenshot-home-sample-data-entry-mobile.png "URL Shortener Sample Data Entry")

### Short URL Created

![Short URL Created](/static/screenshots/screenshot-shortUrl-created-mobile.png "Short URL Created")
