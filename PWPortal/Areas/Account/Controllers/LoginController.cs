using PWPortal.Areas.Account.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWPortal.Common;
using SharedClass;
using SharedClass.Resources;
using SharedClass.Enums;
using PWPortal.Models;
using WebsiteUtilities;
using System.Web.Security;

namespace PWPortal.Areas.Account.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// Login Index main view
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string id = null)
        {
            ViewBag.Id = id;
            return View("Index");
        }


        /// <summary>
        /// Partial view Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            var model = new LoginModel();
            return PartialView(model);
        }


        /// <summary>
        /// Process Logout
        /// </summary>      
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();          
            return RedirectToAction("Index", "Login");
        }


        /// <summary>
        /// Process Login data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                int outputMessage;
                AccountDbAccess ADA = new AccountDbAccess();
                ADA.LogInUser(model.userName, model.password, true, Config.clientId, out outputMessage);
                LoginErrorCode errorCode = (LoginErrorCode)outputMessage;

                if(outputMessage == 0)
                {
                    //Check is this first login, then redirect to change password
                    if (UserInfoModel.PasswordExpireDate < DateTime.Now)
                    {                        
                        return RedirectToAction("ChangePassword", new { @userId = UserInfoModel.UserID });
                    }

                    ViewBag.message = "Successfully loggedIn";
                    ViewBag.messageType = "success";

                    return JavaScript("window.top.location.href ='" + Url.Action("Index", "Home", new { area = "" }) + "';");       
                }
                else
                {
                    ModelState.AddModelError("LoginModel", "Login failed");

                    ViewBag.message = "Login failed";
                    ViewBag.messageType = "error";    
                }                       
            }
            else
            {
                ModelState.AddModelError("Login", "Login failed");  
              
                ViewBag.message = "Login failed";
                ViewBag.messageType = "error";               
            }

            return PartialView(model);
        }

        /// <summary>
        /// Partial view ResetPassword
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            var model = new ResetPasswordModel();
            return PartialView(model);
        }


        /// <summary>
        /// Process ResetPassword
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //int outputMessage;
                SubFunctions.SendResetEmail(model.email);

                ViewBag.message = "Successfully reset password";
                ViewBag.messageType = "success";                
            }
            else
            {
                ModelState.AddModelError("Login", "Login failed");

                ViewBag.message = "Login failed";
                ViewBag.messageType = "error";
            }

            return PartialView(model);
        }


        /// <summary>
        /// Partial view ChangePassword
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword(string userId = null, string id = null)      
        {
            var model = new ChangePasswordModel();
            AccountDbAccess ADA = new AccountDbAccess();
            string userEmail = null;

            if(userId != null)
            {
                userEmail = ADA.GetUserByUserId(userId);
                

                //If no user was loaded, output error message
                if (userEmail == null)
                {                   
                    //Show error msg;
                    ViewBag.HideForm = true;
                    return PartialView(model);
                }
                else
                {
                    model.email = userEmail;
                    ViewBag.HideForm = false;
                    return PartialView(model);
                }              
            }
            else if (id != null)
            {
                userEmail = ADA.GetUser(id);
                //isGuidReset = true;
               
                //If no user was loaded, output error message
                if (userEmail == null)
                {
                    //Show error msg;
                    ViewBag.HideForm = true;
                    return PartialView(model);
                }
                else
                {
                    model.email = userEmail;
                    ViewBag.HideForm = false;
                    return PartialView(model);
                }
            }           

            ViewBag.HideForm = true;
            return PartialView(model);            
        }


        /// <summary>
        /// Process ChangePassword
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {              
                //Validate new password
                if (model.newPswd != model.confirmPswd)
                {
                    ViewBag.message = AccountResource.ResourceManager.GetString("passwordMismatch");
                    ViewBag.messageType = MessageTypes.Error;
                    return PartialView(model);
                }

                if (UserInfoModel.UserID <= 0)
                {
                    ViewBag.message = AccountResource.ResourceManager.GetString("error_PswdChange");
                    ViewBag.messageType = MessageTypes.Error;
                    return PartialView(model);
                }
                else
                {
                    AccountDbAccess ADA = new AccountDbAccess();
                    string resetError;

                    if (!ADA.UpdatePassword(UserInfoModel.UserID, model.newPswd, out resetError))
                    {
                        ViewBag.message = resetError;
                        ViewBag.messageType = MessageTypes.Error;
                        return PartialView(model);
                    }

                    //Clears login attempt History for specified userId
                    int rows = ADA.ClearLoginAttempts(UserInfoModel.UserID);

                    //Get user with new password to make sure that everything is OK
                    int outputValue;

                    ADA.LogInUser(model.email, model.newPswd, true, Config.clientId, out outputValue);

                    if (outputValue == 0)
                    {
                        ViewBag.message = "Successfully changed password";
                        ViewBag.messageType = "success";

                        return JavaScript("window.top.location.href ='" + Url.Action("Index", "Login", new { area = "Account" }) + "';");
                    }
                    else
                    {
                        //UserInfoModel.UserInfo = SessionWrapper.Get<UserInfoModel>("UserDetail", null);                        
                        ViewBag.message = AccountResource.ResourceManager.GetString("criticalError");
                        ViewBag.messageType = MessageTypes.Error;
                        return PartialView(model);
                    }                  
                }               
            }
            else
            {
                ModelState.AddModelError("Login", "Password couldn't change");

                ViewBag.message = "Password change failed";
                ViewBag.messageType = "error";
            }
            return PartialView(model);
        }

       
    }
}