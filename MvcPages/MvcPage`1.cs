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

// This class is based on WebViewPage`1 and Controller from ASP.NET MVC

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcPages {

   public abstract class MvcPage<TModel> : MvcPage {

      ViewDataDictionary<TModel> _viewData;

      public new AjaxHelper<TModel> Ajax { get; set; }
      public new HtmlHelper<TModel> Html { get; set; }

      public new TModel Model { 
         get { return this.ViewData.Model; } 
         set { this.ViewData.Model = value; } 
      }

      public new ViewDataDictionary<TModel> ViewData {
         get {
            if (_viewData == null)
               SetViewData(new ViewDataDictionary<TModel>());
            return _viewData;
         }
         set {
            SetViewData(value);
         }
      }

      protected override void SetViewData(ViewDataDictionary viewData) {

         _viewData = new ViewDataDictionary<TModel>(viewData);

         base.SetViewData(_viewData);
      }

      public override void InitHelpers() {

         base.InitHelpers();

         this.Ajax = new AjaxHelper<TModel>(this.ViewContext, this);
         this.Html = new HtmlHelper<TModel>(this.ViewContext, this);
      }

      /// <summary>
      /// This member supports the MvcPages infrastructure and is not intended to be used directly from your code.
      /// </summary>
      /// <returns></returns>
      protected override Type EnsureModel() {

         TModel model = this.Model;

         if (model == null) {
            this.Model = Activator.CreateInstance<TModel>();
            return typeof(TModel);
         }

         return base.EnsureModel();
      }
   }
}
