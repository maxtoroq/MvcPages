using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Samples.Account {

   public class AccountService {

      public static readonly string UserName = "demo";
      public static string Password = "demo";

      readonly ModelStateDictionary modelState;

      public AccountService(ModelStateDictionary modelState) {
         this.modelState = modelState;
      }

      public bool Login(LoginModel model) {

         if (UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)
            && Password.Equals(model.Password)) {

            return true;
         }

         modelState.AddModelError("", "The user name or password provided is incorrect.");
         
         return false;
      }

      public bool ChangePassword(ChangePasswordModel model) {

         if (model.OldPassword == Password) {

            Password = model.NewPassword;
            return true;
         }

         modelState.AddModelError("OldPassword", "Current Password is incorrect.");

         return false;
      }
   }
}