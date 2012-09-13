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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MvcPages {
   
   public abstract class MvcViewPage : WebViewPage {

      protected override void ConfigurePage(WebPageBase parentPage) {
         CopyParentState(this, parentPage);
      }

      internal static void CopyParentState(WebViewPage currentPage, WebPageBase parentPage) {

         WebViewPage viewPage = parentPage as WebViewPage;

         if (viewPage != null) {
            currentPage.ViewContext = viewPage.ViewContext;
            currentPage.ViewData = viewPage.ViewData;

         } else {

            MvcPage page = parentPage as MvcPage;

            if (page != null) {
               currentPage.ViewContext = page.ViewContext;
               currentPage.ViewData = page.ViewData;

            } else {
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "WrongViewBase", new object[] { parentPage.VirtualPath }));
            }
         }

         currentPage.InitHelpers();
      }
   }
}
