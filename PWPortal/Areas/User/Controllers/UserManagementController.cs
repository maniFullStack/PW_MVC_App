using PWPortal.Areas.User.Models;
using PWPortal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SharedClass;
using Newtonsoft.Json;
using SharedClass.Resources;

namespace PWPortal.Areas.User.Controllers
{
    public class UserManagementController : Controller
    {
        //
        // GET: /User/UserManagement/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load Users List page
        /// </summary>
        /// <returns></returns>
        public ActionResult UsersList()
        {
            var model = new List<UserModel>();        

            return PartialView(model);
        }

        /// <summary>
        /// Get users list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUsersList()
        {
            var model = new List<UserModel>();
            AccountDbAccess ADA = new AccountDbAccess();
            DataTable dt = ADA.GetUsersList(1);

            foreach(DataRow row in dt.Rows)
            {
               //DateTime? LtLogin = Convert.ToDateTime(row["LastLogin"]);
 
               model.Add(new UserModel
                {
                    UserId = Convert.ToInt32(row["UserID"]),
                    FullName = row["FullName"].ToString(),
                    Email = row["Email"].ToString(),
                    Group = row["Group"].ToString(),
                    Status = Convert.ToBoolean(row["Status"]),
                    LastLogin = row["LastLogin"].ToString()
                    //LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null
                });              
            }

            return Json(new { data = model}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Load page to Add user
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
            var model = new AddUserModel();
            model.groupList = new List<ItemsList>();

            DataTable dtGroups = AccountDbAccess.LoadGroupOptions();         
                   
            foreach(DataRow row in dtGroups.Rows)
            {
                model.groupList.Add(new ItemsList { Value = row["GroupID"].ToString(), Text = row["GroupName"].ToString() });
            }

            model.selectedGroupId = dtGroups.Rows[0]["GroupID"].ToString();

            TempData["groupList"] = model.groupList;

            return PartialView(model);
        }

        /// <summary>
        /// Process created user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUser(AddUserModel model)
        {
            string emailError;
            model.groupList = (List<ItemsList>)TempData["groupList"];
            TempData.Keep();

            if (!AccountDbAccess.IsExistingEmail(model.Email, out emailError))
            {
                ModelState.AddModelError("Email", "This email address is already used for another user. Please use a unique email address.");
            }

            if(ModelState.IsValid)
            {
                int createdUserId = 1;
                int msg;              

                string temporaryPswd = CommonFunctions.CreatePassword();               

                if (temporaryPswd != null)
                {
                    AccountDbAccess.AddNewuser(model.Email, temporaryPswd, model.FirstName, model.LastName, model.Email, model.Phone, Convert.ToInt32(model.selectedGroupId), createdUserId, out msg);

                    if (msg == 0)
                    {
                        //Send mail to created user with username and password
                        if (!SubFunctions.SendNewAccountMail(model.Email, model.FirstName, model.LastName, model.Email, temporaryPswd))
                        {
                           //Show email error msg here
                        }
                        return JavaScript("window.top.location.href ='" + Url.Action("Index", "UserManagement", new { area = "User" }) + "';");
                    }
                    else
                    {
                        return PartialView(model);
                    }                   
                }
                else
                {                                           
                    return PartialView(model);
                }                             
            }
            else
            {                                             
                return PartialView(model);
            }           
        }

        /// <summary>
        ///  Load page to Edit user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>      
        public ActionResult EditUser(int userId)
        {
             AccountDbAccess ADA = new AccountDbAccess();
             DataTable dtUserDetails = ADA.LoadUserDetails(userId);

            if(dtUserDetails != null)
            {                
                var model = new EditUserModel
                {
                    FirstName = dtUserDetails.Rows[0]["FirstName"].ToString(),
                    LastName = dtUserDetails.Rows[0]["LastName"].ToString(),
                    Email = dtUserDetails.Rows[0]["Email"].ToString(),
                    Phone = dtUserDetails.Rows[0]["Phone"].ToString(),
                    selectedGroupId = dtUserDetails.Rows[0]["GroupID"].ToString(),
                    SeletedStatusId = dtUserDetails.Rows[0]["Active"].ToString(),
                };
                model.UserId = userId;
                model.groupList = new List<ItemsList>();
                model.statusList = new List<ItemsList>()
                {
                    new ItemsList { Value = "1", Text = "Active" },
                    new ItemsList { Value = "0", Text = "InActive" }
                }; 

                DataTable dtGroups = AccountDbAccess.LoadGroupOptions();

                foreach (DataRow row in dtGroups.Rows)
                {
                    model.groupList.Add(new ItemsList { Value = row["GroupID"].ToString(), Text = row["GroupName"].ToString() });
                }

                TempData["groupList"] = model.groupList;
                TempData["statusList"] = model.statusList;

                return PartialView(model);
            }
            else
            {
                // show error message here and redirect ToString error page
                return null;
            }         
        }

        /// <summary>
        /// process Updated user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUser(EditUserModel model)
        {
            model.groupList = (List<ItemsList>)TempData["groupList"];
            model.statusList = (List<ItemsList>)TempData["statusList"];
            int modifiedUserID = 1;

            if(ModelState.IsValid)
            {
                string msgEditUser = AccountDbAccess.UpdateUser(model.UserId, modifiedUserID, model.FirstName, model.LastName, model.Email, model.Phone, Convert.ToInt32(model.selectedGroupId), Convert.ToInt32(model.SeletedStatusId));
                    
                return JavaScript("window.top.location.href ='" + Url.Action("Index", "UserManagement", new { area = "User" }) + "';");
            }
            else
            {
                return PartialView(model);
            }
        }


        /// <summary>
        /// Check edited email is exist
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditedEmailExist(string email)
        {
            string emailError;

            if (!AccountDbAccess.IsExistingEmail(email, out emailError))
            {
                return Json(emailError , JsonRequestBehavior.AllowGet);
            }
            return Json(" " , JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Delete user from list
        /// </summary>
        [HttpPost]
        public JsonResult DeleteUser(int userId)
        {            
            string delMsg;

            int msg = AccountDbAccess.DeleteUser(userId);
            if (msg == 1)
            {
                delMsg = UserResource.Success_DeleteUser;
            }
            else
            {
                delMsg = UserResource.Error_DeleteUser;
            }

            return Json(delMsg, JsonRequestBehavior.AllowGet);
        }
	}
}