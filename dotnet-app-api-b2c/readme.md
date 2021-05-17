# .NET Web App calling .NET API App with Azure AD B2C AuthN/AuthZ

## Overview

The application consists of a WebPortal for interactive user login, and 2 APIs: WeatherAPI, and ProfileAPI (WIP). The architecture is very simple and basic.

![App Arch](docs/app-arch.png)

### Todo

- [ ] Add more documentation
- [ ] Add IaC using Bicep to deploy App Service and Apps
- [ ] Containerize APIs and WebApp

## Registering the applications

The WebPortal is a client type application that will be performing authentication/authorization, and obtaining access tokens in order to call protected API apps. It is of best practice to create a 1:1 relationship between app registrations and actual applications.

Weather API simple performs two operations `GetEcho()` which send back a simple string, and `GetWeather()` which requires authentication and sends back a JSON with weather information.

### Register the API apps (Weather API only)

Search for and launch **Azure AD B2C**

1. Under **Manage** select **App registrations > New registration**, enter a name for your API. No need to specify any **Platform configurations**
2. Under **Manage** select **Expose and API**
   1. Set the Application ID URI by providing a friendly name
   2. Select **Add a scope**, set Scope name to `Read.Weather`, display name and consent description to `Read Weather`, ensure State is enabled and select **Add scope** at the bottom of the pane

### Registering the WebPortal (Client app)

The steps described here is a quick-start type of guide, for full details and documentation refer to [Azure AD B2C App registrations](https://docs.microsoft.com/azure/active-directory-b2c/app-registrations-training-guide) and [Register app or web API](https://docs.microsoft.com/azure/active-directory/develop/quickstart-register-app).

Switch to the Azure AD B2C directory, search for and launch **Azure AD B2C**

1. Under **Manage** select **App registrations > New registration** to create a new app
   1. Enter a name for your application, ensure **Supported account types** is set to Accounts in any identity provider or organizational directory, and **Register**. Once created the Azure Portal displays the app registration's **Overview** pane
   1. Select **Authentication > Add a platform**
   1. Select **Web**, set the **Redirect URI** to `https://localhost:5001/signin-oidc`. For **Front-channel logout URL** set it to `https://localhost:5001/signout-oidc`. For **Implicit grant and hybrid flows** enable **ID Tokens**
1. Under **Manage** select **Certificates & secrets**
   1. Select **New client secret** provide a description and an expiration time, select **Add**
   2. Copy the value as it will not be visible after navigating away from the pane
1. Under **Manage** select **API permissions > Add a permission**
   1. Select **My APIs > WeatherAPI**
   2. Select `Read.Weather` and **Add permissions**
   3. Ensure to **Grant admin consent for Directory**

## Application code configuration

Before diving into code it is necessary to briefly help differentiate the available libraries we have to help us do authentication and authorization. In a nutshell, there are two main libraries: [Microsoft Authentication Library (MSAL) for .NET](https://github.com/azuread/microsoft-authentication-library-for-dotnet) and the [Microsoft Identity Web](https://github.com/azuread/microsoft-authentication-library-for-dotnet) library. 

**MSAL** enables developers to acquire tokens from the Microsoft identity platform in order to authenticate users and access secured web APIs. **Microsoft Identity Web** is a set of ASP.NET Core libraries that simplifies adding authentication and authorization support to web apps and web APIs integrating with the Microsoft identity platform. It provides a single-surface API convenience layer that ties together ASP.NET Core, its authentication middleware, and the Microsoft Authentication Library (MSAL) for .NET

### Weather API configuration

1. Provide the app's configuration: `appsettings.json`

   ```json
    "Azure": {
        "AzureAdB2C": {
        "Instance": "",                 // https://<AAD B2C tenant name>.b2clogin.com
        "ClientId": "",                 // <APP ID> xxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx
        "Domain": "",                   // <DOMAIN NAME> <AAD B2C domain name>.onmicrosoft.com
        "SignedOutCallbackPath": "",    // /signout/<AAD B2C Policy name>
        "SignUpSignInPolicyId": "",     // <AAD B2C POLICY NAME> B2C_1_<NAME>
        "CallbackPath": ""              // /signin-oidc
        }
    }
   ```

2. Add the Microsoft Identity Web service: `Startup.cs`

   ```csharp
    // ConfigureServices()
    services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "Azure:AzureAdB2C");

    // Configure()
    app.UseAuthentication();
    app.UseAuthorization();
   ```

3. Securing the API operations: `Controllers\WeatherForecastController.cs`

   ```csharp
    [Authorize]
    [HttpGet]
    public IEnumerable<WeatherForecast> GetWeather()
   ```

### WebPortal code configuration

1. Provide the app's configuration: `appsettings.json`

   ```json
    "Azure": {
        "AzureAdB2C": {
            "Instance": "",
            "ClientId": "",
            "Domain": "",
            "SignedOutCallbackPath": "",
            "SignUpSignInPolicyId": "",
            "ResetPasswordPolicyId": "",
            "EditProfilePolicyId": "",
            "CallbackPath": "",
            "ClientSecret": ""
        }
    },
    "ServiceApis": {
        "weatherApi": {
            "weatherApiScope": "",
            "weatherApiUrl": ""
        }
    }
   ```

2. Add the Microsoft Identity Web service: `Startup.cs`

   ```csharp
   // ConfigureServices()
    services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "Azure:AzureAdB2C")
        .EnableTokenAcquisitionToCallDownstreamApi(new string[] { Configuration["ServiceApis:weatherApi:weatherApiScope"] })
        .AddInMemoryTokenCaches();

    services.AddControllersWithViews();

    services.AddRazorPages().AddMicrosoftIdentityUI();

    services.AddOptions();
    services.Configure<OpenIdConnectOptions>(Configuration.GetSection("Azure:AzureAdB2C"));

    // Configure()
    app.UseAuthentication();
    app.UseAuthorization();
   ```

3. Obtaining access token and calling protected Weather API: `Controllers\AccountController.cs`

   ```csharp
    private async Task GetToken()
    {
        /* 
            Acquires access token from cache if present, else gets it from AAD B2C.
            */
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { _WeatherApiScope });
        _logger.LogInformation($"access token: {accessToken.ToString()}");


        /* 
            Sets the Bearer header with the access token on the HTTP Client used to 
            call the remote APIs
            */
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
   ```