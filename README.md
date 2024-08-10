# VerifyAPI .NET

[![NuGet](https://img.shields.io/nuget/v/verifyapi.svg)](https://www.nuget.org/packages/VerifyApi/)

A two-factor authentication API library for voice & SMS through the [VerifyAPI][verifyapi] service, supporting .NET Standard 2.0+, .NET Core 3.1, .NET 6.0+ and .NET Framework 4.6.1+ synchronously/asynchronously.

## Installation

Using the [.NET Core command-line interface (CLI) tools][dotnet-core-cli-tools]:

```sh
dotnet add package VerifyApi
```

Using the [NuGet Command Line Interface (CLI)][nuget-cli]:

```sh
nuget install VerifyApi
```

Using the [Package Manager Console][package-manager-console]:

```powershell
Install-Package VerifyApi
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "VerifyAPI".
5. Click on the VerifyAPI package, select the appropriate version in the right-tab and click *Install*.

## Documentation

The concepts behind VerifyAPI are available at our [guide][api-guide]. For comprehensive authentication and endpoint reference, check out the [API documentation][api-docs]. 

## Usage

### Authentication

VerifyAPI authenticates API requests using an API key issued by your account, which you can find in the [API keys][api-keys] section of the VerifyAPI portal. A default API key will have been issued when your account was first created.

The API key should be supplied as part of the constructor when creating a new client instance:


```csharp
VerifyApiClient client = new VerifyApiClient("YOUR_API_KEY");
```

### Create a verification

The `VerifyAsync` method of the client can be used to initiate a new verification

```csharp
var verification = await client.VerifyAsync("+12123464738", Channel.Sms);

// Check that the verification was successful
Console.WriteLine(verification.IsSuccess)
```

### Confirm a verification

The `ConfirmAsync` method of the client is used is used in conjunction with the `id` returned by `VerifyAsync` as well as the code that is supplied by the end-user.

```csharp
var confirmation = await client.ConfirmAsync("6603443f241aa2b2299003b6", "123456");

// Check that the confirmation code and id matched
Console.WriteLine(verification.IsSuccess)
```

### Using a custom `HttpClient`

You can configure the library with your own custom `HttpClient`:

```c#
VerifyApiClient client = new VerifyApiClient("YOUR_API_KEY", httpClient: new HttpClient());
```
This allows you to inject your own HTTP client with custom settings, such as a proxy server, custom message handler, etc.

## Support

New features and bug fixes are released on the latest major version of the VerifyAPI .NET client library. If you are on an older major version, we recommend that you upgrade to the latest in order to use the new features and bug fixes including those for security vulnerabilities. Older major versions of the package will continue to be available for use, but will not be receiving any updates.


[api-docs]: https://docs.verifyapi.dev/reference/
[api-guide]: https://docs.verifyapi.dev/guide/
[api-keys]: https://app.verifyapi.dev/tokens
[dotnet-core-cli-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/
[nuget-cli]: https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference
[package-manager-console]: https://docs.microsoft.com/en-us/nuget/tools/package-manager-console
[verifyapi]: https://www.verifyapi.dev