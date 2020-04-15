# PortableResolver
Simple, Fast, Reliable, Always-Available ASP.NET Core Service Resolver

[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://github.com/azhdari/Mohmd.AspNetCore.PortableResolver/blob/master/License.txt)
[![NuGet](https://img.shields.io/badge/nuget-3.0.0-blue.svg?style=flat-square)](https://www.nuget.org/packages/Mohmd.AspNetCore.PortableResolver/3.0.0)

## Getting Started
Use these instructions to get the package and use it.

### Install
From the command prompt
```bash
dotnet add package Mohmd.AspNetCore.PortableResolver --version 3.0.0
```
or
```bash
Install-Package Mohmd.AspNetCore.PortableResolver -Version 3.0.0
```
or
```bash
paket add Mohmd.AspNetCore.PortableResolver --version 3.0.0
```
<br/>
<br/>

### Configure
Add service
```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddScoped<Service1>();
  services.AddScoped<Service2>();

  // 1. First add portable-resolver required services
  services.AddPortableResolver();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
  // 2. Then prepare the app to use it.
  app.UsePortableResolver();
}
```
<br/>
<br/>

### Usage
Use one of methods below to resolve your desired service
```csharp
// Resolve a single registered service
ResolverContext.Current.Resolve(typeof(FooService));
ResolverContext.Current.Resolve<FooService>();

// Resolve a service with multiple registrations
ResolverContext.Current.ResolveAll<BarService>();
```
<br/>

#### Resolving Unregistered Services
Resolve an unregistered `class` with at list one constructor defined with registered parameters
```csharp
ResolverContext.Current.ResolveUnregistered(typeof(FooUnregisteredService));
ResolverContext.Current.ResolveUnregistered<FooUnregisteredService>();
```
Example for resolving Unregistered `class`
```csharp
// Declaration
public class FooUnregisteredService
{
  public FooUnregisteredService(FooService fooService, BarService barService)
  {
  }
}

// Startup > ConfigureServices
services.AddScoped<FooService>();
services.AddScoped<BarService>();

// Usage
var service = ResolverContext.Current.ResolveUnregistered<FooUnregisteredService>();
```