MvcPages
=============================================================================== 

MvcPages combines the simplicity of ASP.NET Web Pages with the
power of ASP.NET MVC. Use model binding, model validation,
strongly-typed HTML helpers, editor and display templates, etc.
directly from your Razor pages, no need for routes or controllers.

Licensing
---------

This software is licensed under the terms you may find in the file
named "LICENSE.txt" in the licenses directory of this distribution.

Getting Started
---------------

MvcPages can be used on both ASP.NET MVC and ASP.NET Web Pages projects.

To start using MvcPages just make your Razor page inherit `MvcPages.MvcPage`

```csharp
@inherits MvcPages.MvcPage
```

You can also inherit `MvcPages.MvcPage<>` to take advantage of strongly-typed
HTML helpers

```csharp  
@inherits MvcPages.MvcPage<MyModel>
```

Import the following namespaces to use the various ASP.NET MVC features, 
such as HTML helpers

```csharp  
@using System.Web.Mvc
@using System.Web.Mvc.Ajax
@using System.Web.Mvc.Html
@using System.Web.Routing
```

To avoid specifying these directives on every page you can add the
following to Web.config

```xml
<configSections>
   <sectionGroup name="system.web.webPages.razor" type="System.Web.WebPages.Razor.Configuration.RazorWebSectionGroup, System.Web.WebPages.Razor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35">
      <section name="host" type="System.Web.WebPages.Razor.Configuration.HostSection, System.Web.WebPages.Razor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" requirePermission="false"/>
      <section name="pages" type="System.Web.WebPages.Razor.Configuration.RazorPagesSection, System.Web.WebPages.Razor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" requirePermission="false"/>
   </sectionGroup>
</configSections>
<system.web.webPages.razor>
   <host factoryType="System.Web.Mvc.MvcWebRazorHostFactory, System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
   <pages pageBaseType="MvcPages.MvcPage">
      <namespaces>
         <add namespace="System.Web.Mvc"/>
         <add namespace="System.Web.Mvc.Ajax"/>
         <add namespace="System.Web.Mvc.Html"/>
         <add namespace="System.Web.Routing"/>
      </namespaces>
   </pages>
</system.web.webPages.razor>
```

With the above configuration in place you can omit @inherits and use
@model to inherit from `MvcPages.MvcPage<>`

```csharp 
@model MyModel
```

MvcPages was compiled against Web Pages 1 / MVC 3. To use with Web Pages 2 / 
MVC 4 add the following to Web.config

```xml
<runtime>
   <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
         <assemblyIdentity name="System.Web.Helpers" publicKeyToken="31bf3856ad364e35" />
         <bindingRedirect oldVersion="1.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
         <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
         <bindingRedirect oldVersion="1.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
         <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" />
         <bindingRedirect oldVersion="1.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
         <assemblyIdentity name="System.Web.WebPages.Razor" publicKeyToken="31bf3856ad364e35" />
         <bindingRedirect oldVersion="1.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
   </assemblyBinding>
</runtime>
```

In order for intellisense to work properly with Web Pages 2 / MVC 4
also make sure you update the version numbers in the configuration

```xml
<configSections>
   <sectionGroup name="system.web.webPages.razor" type="System.Web.WebPages.Razor.Configuration.RazorWebSectionGroup, System.Web.WebPages.Razor, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35">
      <section name="host" type="System.Web.WebPages.Razor.Configuration.HostSection, System.Web.WebPages.Razor, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" requirePermission="false"/>
      <section name="pages" type="System.Web.WebPages.Razor.Configuration.RazorPagesSection, System.Web.WebPages.Razor, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" requirePermission="false"/>
   </sectionGroup>
</configSections>
<system.web.webPages.razor>
   <host factoryType="System.Web.Mvc.MvcWebRazorHostFactory, System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" />
   ...
</system.web.webPages.razor>
```

To use MVC layouts from Web Pages make your views inherit `MvcPages.MvcViewPage`,
you can do this for all views by setting pageBaseType in ~/Views/Web.config

```xml
<system.web.webPages.razor>
   ...
   <pages pageBaseType="MvcPages.MvcViewPage">
      ...
   </pages>
</system.web.webPages.razor>
```

Finally, if you have Web Pages 2 / MVC 4 installed but want to use Web Pages 1 / 
MVC 3 you have to set the version like this

```xml
<appSettings>
   <add key="webpages:Version" value="1.0"/>
</appSettings>
```

Source code and releases
------------------------
Code hosted on [GitHub](https://github.com/maxtoroq/MvcPages). Releases available via [NuGet](http://www.nuget.org/packages/MvcPages).
