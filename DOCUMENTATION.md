# Documentation

## Introduction

Given my personal time constraint and the objectives of this project, I've decided to limit the number of features available to the user and to focus on the actual code implementation to showcase my understanding of some of ASP.NET Core MVC fundamentals & design methods such as:

* Separation of concerns (MVC)
* Routing
* Dependency injection (services)
* Middleware
* Data Validation (Client/Server)
* Responsive Design

This focus I believe can demonstrate some of my ability in C#, ASP.NET Core and the MVC design pattern but more importantly provide a vision of my design thinking approach regarding software development.

Taking advantage of all these built-in features of ASP.NET Core provides the following benefits:

* Modularity
* Reusability
* Maintainability
* Testability

# Features

## General

The following features have been implemented:

1. Create a short URL
2. Add an Alias to customize the short URL
3. Process request to redirect use when a short URL is called
4. Client/Server Validation using Model Data Validation
5. Client Validation using Remote Attribute Data Validation to enhance UX experience
6. Client/Server Validation using Custom Remote Attribute Data Validation if javascript is disabled
7. Create a responsive design 
8. ShortURLs Page -> For Developement Enviroment only to view in-memory objects

## Limitations

1. URL format as an input
2. Alias format as an input (max 20 alphanumeric characters) 

The format definition for all inputs limits the use of the service.

> The purpose of this service is not to check if a URL redirects to an existing and valid address but to shorten a URL.

Therefore as long as the input does not break the system and is likely to be a URL, it should be accepted as an input Url.

# Code Implementation

These are the areas of focus that I decided to document for the purpose of explaining my design thinking process.

Because of the simplicity of the application, I did not deem necessary to use a database but instead I implemented a singleton service to store the data in-memory.

## Separation of concerns (MVC)

The solution contains the following structure:

1. **Common:** provides extension methods classes
2. **Controllers:** components that handle user interaction
3. **Interfaces:** service interfaces
4. **Models:** contains object entities 
5. **Services:** service class to handle CRUD operations
6. **Views:** user interface using Razor view engine

As it is such a simple project, the separation of the Services and Interfaces into two separate folders is not necessary.

## Routing

The short URL is created using the following pattern:
```
http(s)://{host}:{port}/{alias}

Example: https://urlshortener.com/alias
```
Where the host is the URLShortener website host address and the Alias is auto-generated or provided by the user.
The Alias is recorded with the corresponding "Original" URL where a user gets redirected to when calling the short URL.

The route matching a short URL is defined in the Startup.Configure as follow:

```
[File->Startup.cs]

endpoints.MapControllerRoute(
	name: "alias",
	pattern: "{alias}",
	defaults: new { controller = "Home", action = "AliasRedirect" });
```

The implementation of the action method is in the Home Controller as follow:

```
[File->\Controllers\HomeController.cs]

public IActionResult AliasRedirect(string alias)
{
	var shortUrl = _shortUrlService.GetByAlias(alias);

	if (shortUrl == null)
		return NotFound();
	else
		return Redirect(shortUrl.Url);
}
```

## Dependency injection (services)

Creation of a short URL service and interface to expose it from the service container.

```
[File->\Services\ShortUrlService.cs]

public interface IShortUrlService
{
	List<ShortUrl> Get();
	ShortUrl GetByAlias(string alias);
	/// <summary>
	/// Add a url without alias
	/// </summary>
	/// <returns>auto-generated alias</returns>
	string Add(string url);
	void Add(string alias, string url);
	bool AliasExists(string alias);
	string CreateUniqueAlias();
}
```

Adding the service to the service container:

```
[File->Startup.cs]

public void ConfigureServices(IServiceCollection services)
{
	services.AddControllersWithViews();
	services.AddSingleton<IShortUrlService, ShortUrlService>();
	services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}
```

## Middleware

In order to create the short URL, the base path must be retrieved from the HttpContext. This could be implemented in several ways:

* Hard coded *(not recommended!)*
* In the view at run time
* As abstract helper class
* As a [middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1) using Dependency injection

For the purpose of using middleware, I've defined a static class ***MyHttpContext.cs*** and an extension method of HttpContext.

```
[File->\Common\MyHttpContext.cs]

    public class MyHttpContext
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Current => _httpContextAccessor.HttpContext;
        public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";
        internal static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
    public static class HttpContextExtensions
    {
        public static IApplicationBuilder UseMyHttpContext(this IApplicationBuilder app)
        {
            MyHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
            return app;
        }
    }
```

The last step is to add the middleware to the pipeline in the Configure method located in Startup.cs

```
[File->Startup.cs]

// Add our HttpContext Extension to the pipeline
app.UseMyHttpContext();
```

From this  moment, we can access the base path of our application from anywhere in the code by calling our middleware:

```
var appBaseUrl = MyHttpContext.AppBaseUrl;
```

As implemented in the view :

```
[File->\Views\Home\Success.cshtml]

@($"{MyHttpContext.AppBaseUrl}/{Model.Alias}")

```

## Data Validation


### Client/Server Validation using Model Data Validation

Data Validation is the most important step before executing any business logic. Using Model Validation is a great way to validate data on both the client and server however depending on which Validation Attributes you are using, the validation can behave in unexpected ways depending if the validation is performed on the client or server and more importantly if ***javascript*** is disabled.

Making assumptions on how the validation behaves is not an option so proper understanding of data validation is crucial, especially if it is available to us as a built-in feature of ASP.NET Core.

While performing data validation tests, I noticed that some Validation Attributes, such as Url, did not get validated on the server if ***javascript*** was disabled.

> ModelState.IsValid would always return true for the following format: "https://stacko verflow.com"

To avoid writing more validation code "in case" of this or that, I chose to validate the URL format using Regular Expressions as it is proven to be executed on both the client and server. There is no perfect Regular Expression matching a URL format, so the one implemented isn't bullet proof but it is not necessarily our concern in this scenario. 


#### Client Validation using Remote Attribute Data Validation

Providing client-side validation creates a very responsive experience so I've implemented a Remote Attribute Validation to verify if the "Custom Alias" already exists.

```
[File->\Models\ShortUrl.cs]

[Remote(action: "VerifyAlias", controller: "Home")]
```

```
[File->\Controllers\HomeController.cs]

[AcceptVerbs("GET", "POST")]
public IActionResult VerifyAlias(string alias)
{
	[code removed for brevity]
}
```

This method only works on the client-side, therefore we would be required to check again on the server in case ***javascript*** is disabled.
Creating another validation step manually when the controller receives a request is a bit redundant so instead I have used another method to validate this specific input by creating a Custom Remote Attribute Data Validation.

#### Client/Server Validation using Custom Remote Attribute Data Validation

Custom Remote Attribute Validation is performed on both the client and server, it therefore satisfies our requirement to verify if an alias already exists. There is no need to worry if the client-side validation occured due to javascript being enabled or not.

The first step is to create a class that inherits from RemoteAttribute, and override the IsValid method.

```
[File->\Common\CustomRemoteAttribute.cs]

public class CustomRemoteAttribute : RemoteAttribute
{
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		// Get the controller using reflection
		Type controller = Assembly.GetExecutingAssembly().GetTypes()
			.FirstOrDefault(type => type.Name.ToLower() == string.Format("{0}Controller",
				this.RouteData["controller"].ToString()).ToLower());

		if (controller != null)
		{
			// Get the action method that has validation logic
			MethodInfo action = controller.GetMethods()
				.FirstOrDefault(method => method.Name.ToLower() ==
					this.RouteData["action"].ToString().ToLower());
			if (action != null)
			{
				// Create an instance of the controller class
				object instance = TryCreateController(validationContext, controller);

				// Invoke the action method that has validation logic
				object response = action.Invoke(instance, new object[] { value });
				if (response is JsonResult)
				{
					object jsonData = ((JsonResult)response).Value;

					if (jsonData is bool)
					{
						return (bool)jsonData ? ValidationResult.Success : new ValidationResult(this.ErrorMessage);
					}
					else
					{
						if (jsonData != null)
							return new ValidationResult(jsonData.ToString());
						else
							return new ValidationResult(this.ErrorMessage);
					}
				}
			}
		}
	}
	
	[code removed for brevity]
}
	
```

Then we need replace the Custom Attribute previously defined by our new CustomRemote Attribute in our Short Url data object.

```
[File->\Models\ShortUrl.cs]

[CustomRemote("VerifyAlias", "Home", ErrorMessage = "Alias already exists.")]
```

This CustomRemote Attribute can be reused to validate other attributes no matter which controller / action is being requested.
All it takes is to implement a new IActionResult method to execute the validation call in the corresponding controller as follow:

```
[File->\Controllers\HomeController.cs]

[AcceptVerbs("GET", "POST")]
public IActionResult VerifyAlias(string alias)
{
	[code removed for brevity]
}
```

The overriden method IsValid will automatically get an instance of the corresponding controller and invoke the action method dynamically.

If I had more time, I would explore other ways to achieve this, such as using:

> Custom attributes / Implement IValidatableObject

### Responsive Design (Mobile Support)

Although I did not focus on the design aspect of this project and used the default template provided by Visual Studio, I made sure to implement a very basic but functional responsive design for all views so the design can adapt to different screen sizes.

![URLShortener Sample Data Entry](https://mygithubstatic.s3-ap-southeast-2.amazonaws.com/URLShortener/screenshot-home-sample-data-entry-mobile.png "URL Shortener Sample Data Entry")

Sample URL Data Entry on a mobile phone

# Pending Features

Had I more time working on this project and if the application was built for a live production enviroment, here is a list of improvements or features I would have added:

## Business Requirements

- Implement a policy strategy for URL & Alias (format, whitelist/blacklist of domain name / alias etc...)

## Storage & Data Access

- SQL Server + Entity Framework Core

## Features
- UI design 
- Error handling
- Area (user auth)
- Metrics
- Bulk URL upload
- API access

## Testing

- Create unit testing
