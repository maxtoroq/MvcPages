Request Validation
==================

To disable request validation put the following at the top of your page:

```csharp
ViewContext.Controller.ValidateRequest = false
```

This is the equivalent of using `[ValidateInput(false)]` on a controller action.

You can also put it in a start page, so it affects all pages:

```csharp
@functions {
   
   static void DisableRequestValidation(StartPage startPage) {

      if (startPage != null) {
         
         var mvcPage = startPage.ChildPage as MvcPages.MvcPage;

         if (mvcPage != null) {
            mvcPage.ViewContext.Controller.ValidateRequest = false;
         } else {
            DisableRequestValidation(startPage.ChildPage as StartPage);
         }
      }
   }
}
@{
   DisableRequestValidation(this);
}
```

Note that this only affects model binding. If you are accesing values in a more traditional way (e.g. `Request.Form`) please refer to the Web Pages documentation for more information.
