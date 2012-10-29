// Copyright 2012 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// This class is based on WebViewPage and Controller from ASP.NET MVC

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

namespace MvcPages {

   public abstract class MvcPage : WebPage, IViewDataContainer, IView {

      ViewDataDictionary _ViewData;
      IValueProvider _ValueProvider;
      DynamicViewDataDictionary _ViewBag;
      ITempDataProvider _TempDataProvider;

      public AjaxHelper<object> Ajax { get; set; }

      public new HtmlHelper<object> Html { get; set; }

      public UrlHelper Url { get; set; }

      public new object Model {
         get { return this.ViewData.Model; }
         set { this.ViewData.Model = value; }
      }

      public new ModelStateDictionary ModelState { 
         get { return ViewData.ModelState; } 
      }

      public IValueProvider ValueProvider {
         get {
            if (_ValueProvider == null) 
               _ValueProvider = ValueProviderFactories.Factories.GetValueProvider(this.ViewContext);
            return _ValueProvider;
         }
         set { _ValueProvider = value; }
      }
 
      public ViewContext ViewContext { get; set; }

      public ViewDataDictionary ViewData {
         get {
            if (_ViewData == null)
               SetViewData(new ViewDataDictionary());
            return _ViewData;
         }
         set {
            SetViewData(value);
         }
      }

      public dynamic ViewBag {
         get {
            if (_ViewBag == null) 
               _ViewBag = new DynamicViewDataDictionary(() => ViewData);
            return _ViewBag;
         }
      }

      public TempDataDictionary TempData {
         get { return ViewContext.TempData; }
      }

      public ITempDataProvider TempDataProvider {
         get {
            if (_TempDataProvider == null) 
               _TempDataProvider = CreateTempDataProvider();
            return _TempDataProvider;
         }
         set {
            _TempDataProvider = value;
         }
      }
 
      protected virtual void SetViewData(ViewDataDictionary viewData) {
         _ViewData = viewData;
      }

      protected override void ConfigurePage(WebPageBase parentPage) {

         // This is called for layout pages

         MvcPage page = parentPage as MvcPage;

         if (page != null) {
            this.ViewContext = page.ViewContext;
            this.ViewData = page.ViewData;
         
         } else {

            WebViewPage viewPage = parentPage as WebViewPage;

            if (viewPage != null) {
               this.ViewContext = viewPage.ViewContext;
               this.ViewData = viewPage.ViewData;

            } else {
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "WrongViewBase", new object[] { parentPage.VirtualPath }));
            }
         }
      }

      protected override void InitializePage() {

         base.InitializePage();

         var controller = new MvcPageController { 
            TempData = new TempDataDictionary(),
            ViewData = this.ViewData
         };

         // routeData must contain an item named 'controller' with a non-empty string value
         // required by view engine

         var routeData = new RouteData { 
            Values = { 
               { "controller", "MvcPage" } 
            } 
         };

         var controllerContext = new ControllerContext(this.Context, routeData, controller);

         controller.ControllerContext = controllerContext;

         this.ViewContext = new ViewContext(
            controllerContext,
            this,
            this.ViewData,
            controller.TempData,
            TextWriter.Null
         );

         InitHelpers();

         controller.Url = this.Url;
      }

      public virtual void InitHelpers() {

         this.Ajax = new AjaxHelper<object>(this.ViewContext, this);
         this.Html = new HtmlHelper<object>(this.ViewContext, this);
         this.Url = new UrlHelper(this.ViewContext.RequestContext);
      }

      public override void ExecutePageHierarchy() {

         this.ViewContext.Writer = this.Output;

         PossiblyLoadTempData();

         try {
            base.ExecutePageHierarchy();
         } finally {
            PossiblySaveTempData();
         }
      }

      protected virtual ITempDataProvider CreateTempDataProvider() {
         return new SessionStateTempDataProvider();
      }

      void PossiblyLoadTempData() {
         
         if (!this.ViewContext.IsChildAction) 
            this.TempData.Load(this.ViewContext, this.TempDataProvider);
      }

      void PossiblySaveTempData() {
         
         if (!this.ViewContext.IsChildAction) 
            this.TempData.Save(this.ViewContext, this.TempDataProvider);
      }

      #region [Try]UpdateModel / ValidateModel

      protected bool TryUpdateModel() {

         Type modelType = EnsureModel();

         ModelBinders.Binders
            .GetBinder(modelType)
            .BindModel(this.ViewContext, new ModelBindingContext {
               ModelMetadata = this.ViewData.ModelMetadata,
               ModelState = this.ModelState,
               ValueProvider = this.ValueProvider
            });

         return this.ModelState.IsValid;
      }

      protected bool TryUpdateModel(object model) {
         return TryUpdateModel(model, null, null, null, this.ValueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix) {
         return TryUpdateModel(model, prefix, null, null, this.ValueProvider);
      }

      protected bool TryUpdateModel(object model, string[] includeProperties) {
         return TryUpdateModel(model, null, includeProperties, null, this.ValueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix, string[] includeProperties) {
         return TryUpdateModel(model, prefix, includeProperties, null, this.ValueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix, string[] includeProperties, string[] excludeProperties) {
         return TryUpdateModel(model, prefix, includeProperties, excludeProperties, this.ValueProvider);
      }

      protected bool TryUpdateModel(object model, IValueProvider valueProvider) {
         return TryUpdateModel(model, null, null, null, valueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix, IValueProvider valueProvider) {
         return TryUpdateModel(model, prefix, null, null, valueProvider);
      }

      protected bool TryUpdateModel(object model, string[] includeProperties, IValueProvider valueProvider) {
         return TryUpdateModel(model, null, includeProperties, null, valueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix, string[] includeProperties, IValueProvider valueProvider) {
         return TryUpdateModel(model, prefix, includeProperties, null, valueProvider);
      }

      protected bool TryUpdateModel(object model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) {

         if (model == null) throw new ArgumentNullException("model");
         if (valueProvider == null) throw new ArgumentNullException("valueProvider");

         Type modelType = model.GetType();

         Predicate<string> propertyFilter = propertyName => IsPropertyAllowed(propertyName, includeProperties, excludeProperties);
         IModelBinder binder = ModelBinders.Binders.GetBinder(modelType);

         var bindingContext = new ModelBindingContext() {
            ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
            ModelName = prefix,
            ModelState = this.ModelState,
            PropertyFilter = propertyFilter,
            ValueProvider = valueProvider
         };
         
         binder.BindModel(this.ViewContext, bindingContext);
         
         return this.ModelState.IsValid;
      }

      static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties) {
         // We allow a property to be bound if its both in the include list AND not in the exclude list.
         // An empty include list implies all properties are allowed.
         // An empty exclude list implies no properties are disallowed.
         bool includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
         bool excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
         return includeProperty && !excludeProperty;
      }

      protected bool TryValidateModel() {

         EnsureModel();

         return TryValidateModel(this.Model);
      }

      protected bool TryValidateModel(object model) {
         return TryValidateModel(model, null /* prefix */);
      }

      protected bool TryValidateModel(object model, string prefix) {

         if (model == null) throw new ArgumentNullException("model");

         ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());

         foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(metadata, this.ViewContext).Validate(null)) 
            this.ModelState.AddModelError(CreateSubPropertyName(prefix, validationResult.MemberName), validationResult.Message);

         return this.ModelState.IsValid;
      }

      static string CreateSubPropertyName(string prefix, string propertyName) {
         
         if (String.IsNullOrEmpty(prefix)) 
            return propertyName;
         
         if (String.IsNullOrEmpty(propertyName)) 
            return prefix;
         
         return (prefix + "." + propertyName);
      }

      protected void UpdateModel() {

         bool success = TryUpdateModel();

         if (!success) {
            throw new InvalidOperationException(
               String.Format(CultureInfo.CurrentCulture, "The model of type '{0}' could not be updated.", this.Model.GetType().FullName)
            );
         }
      }

      protected void UpdateModel(object model) {
         UpdateModel(model, null, null, null, ValueProvider);
      }

      protected void UpdateModel(object model, string prefix) {
         UpdateModel(model, prefix, null, null, ValueProvider);
      }

      protected void UpdateModel(object model, string[] includeProperties) {
         UpdateModel(model, null, includeProperties, null, ValueProvider);
      }

      protected void UpdateModel(object model, string prefix, string[] includeProperties) {
         UpdateModel(model, prefix, includeProperties, null, ValueProvider);
      }

      protected void UpdateModel(object model, string prefix, string[] includeProperties, string[] excludeProperties) {
         UpdateModel(model, prefix, includeProperties, excludeProperties, ValueProvider);
      }

      protected void UpdateModel(object model, IValueProvider valueProvider) {
         UpdateModel(model, null, null, null, valueProvider);
      }

      protected void UpdateModel(object model, string prefix, IValueProvider valueProvider) {
         UpdateModel(model, prefix, null, null, valueProvider);
      }

      protected void UpdateModel(object model, string[] includeProperties, IValueProvider valueProvider) {
         UpdateModel(model, null, includeProperties, null, valueProvider);
      }

      protected void UpdateModel(object model, string prefix, string[] includeProperties, IValueProvider valueProvider) {
         UpdateModel(model, prefix, includeProperties, null, valueProvider);
      }

      protected void UpdateModel(object model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) {

         bool success = TryUpdateModel(model, prefix, includeProperties, excludeProperties, valueProvider);

         if (!success) {
            throw new InvalidOperationException(
               String.Format(CultureInfo.CurrentCulture, "The model of type '{0}' could not be updated.", model.GetType().FullName)
            );
         }
      }

      protected void ValidateModel() {

         EnsureModel();

         ValidateModel(this.Model);
      }

      protected void ValidateModel(object model) {
         ValidateModel(model, null /* prefix */);
      }

      protected void ValidateModel(object model, string prefix) {

         if (!TryValidateModel(model, prefix)) {
            throw new InvalidOperationException(
                String.Format(CultureInfo.CurrentCulture, "The model of type '{0}' is not valid.", model.GetType().FullName)
            );
         }
      }

      /// <summary>
      /// This member supports the MvcPages infrastructure and is not intended to be used directly from your code.
      /// </summary>
      /// <returns></returns>
      protected virtual Type EnsureModel() {

         object model = this.Model;

         if (model == null)
            throw new InvalidOperationException("Model cannot be null.");

         return model.GetType();
      }

      #endregion

      void IView.Render(ViewContext viewContext, TextWriter writer) {
         throw new NotImplementedException();
      }

      class MvcPageController : Controller { }
   }
}
