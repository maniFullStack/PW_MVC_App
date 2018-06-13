using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebsiteUtilities;
using SharedClass.Enums;
using SharedClass;
using PWPortal.Models;
using SharedClass.Resources;

namespace PWPortal.Common
{
    public class AccountDbAccess: UserInformation
    {
        /// <summary>
        /// The account's group.
        /// </summary>
        public AcccountGroups Group { get; set; }

        public void LogInUser(string usernameOrEmail, string password, bool useEmailForLogin, int clientID, out int outputValue)
        {
            outputValue = -1;
            //Set up the sql request
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();
            sqlParams.Add(useEmailForLogin ? "@Email" : "@Username", usernameOrEmail);

            SqlParameter outParam;

            sqlParams.Add("@Password", password)
                     .Add("@ClientID", clientID)
                     .Add("@IP", RequestVars.GetRequestIPv4Address())
                     .AddOutputParam("@OutputValue", 4, out outParam);

            //Try and get the user's info
            DataTable dt = sql.ExecStoredProcedureDataTable("spCOM_WebReportingLogon", sqlParams);

            if (!sql.HasError)
            {
                outputValue = Conversion.StringToInt(outParam.Value.ToString(), -1);
                if (outputValue == 0 && dt.Rows.Count > 0)
                {
                    //Success!
                    SetUserDataFromDr(dt.Rows[0]);
                    //SessionWrapper.Add<UserInfoModel>("AccountDetail", this);
                    return;
                }
            }
            UserID = -1;
        }


        /// <summary>
        ///     Sets the UserInformation's properties based on a DataRow.
        /// </summary>
        /// <param name="dr">The data row to pull user information from.</param>
        private void SetUserDataFromDr(DataRow dr)
        {
            if (dr == null)
            {
                UserID = -1;
                return;
            }


            UserInfoModel.UserID = Conversion.StringToInt(dr["UserID"].ToString());
            UserInfoModel.Username = dr["Username"].ToString();
            UserInfoModel.FirstName = dr["FirstName"].ToString();
            UserInfoModel.LastName = dr["LastName"].ToString();
            UserInfoModel.Email = dr["Email"].ToString();
            UserInfoModel.GUID = dr["GUID"].ToString();
            UserInfoModel.LoginCount = Conversion.StringToInt(dr["LoginCount"].ToString(), 0);
            UserInfoModel.LastLoginDate = (dr["LastLoginDate"] != DBNull.Value)
                                ? Conversion.XMLDateToDateTime(dr["LastLogindate"].ToString())
                                : DateTime.MinValue;
            UserInfoModel.PasswordResetDate = (dr["PasswordResetDate"] != DBNull.Value)
                                    ? Conversion.XMLDateToDateTime(dr["PasswordResetDate"].ToString())
                                    : DateTime.MinValue;
            UserInfoModel.PasswordExpireDate = (dr["PasswordExpireDate"] != DBNull.Value)
                                     ? Conversion.XMLDateToDateTime(dr["PasswordExpireDate"].ToString())
                                     : DateTime.MinValue;
            UserInfoModel.CreationDate = (dr["TSCreated"] != DBNull.Value)
                               ? Conversion.XMLDateToDateTime(dr["TSCreated"].ToString())
                               : DateTime.MinValue;
            UserInfoModel.ModificationDate = (dr["TSModified"] != DBNull.Value)
                                   ? Conversion.XMLDateToDateTime(dr["TSModified"].ToString())
                                   : DateTime.MinValue;
            UserInfoModel.ModifiedUserID = Conversion.StringToInt(dr["ModifiedUserID"].ToString(), -1);

            UserInfoModel.IsActive = Conversion.StringToBool(dr["Active"].ToString());

            UserInfoModel.GroupID = Conversion.StringToInt(dr["GroupID"].ToString(), -1);           
        }

        /// <summary>
        ///   Gets a user's information from the database using UserID.
        /// </summary>
        public string GetUserByUserId(string userId)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            //Try and get the user's info
            DataTable dt = sql.ExecStoredProcedureDataTable("spCOM_LoadUserData", new SqlParameter("@UserID", userId));
            if (!sql.HasError)
            {
                if (dt.Rows.Count > 0)
                {
                    
                    return dt.Rows[0]["Email"].ToString();
                }
                else
                {
                    return null;
                }
            }
            return null;
        }


        /// <summary>
        ///   Gets a user's information from the database using GUID.
        /// </summary>
        public string GetUser(string guid)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            //Try and get the user's info
            DataTable dt = sql.ExecStoredProcedureDataTable("spCOM_LoadUserData", new SqlParameter("@GUID", guid));
            if (!sql.HasError)
            {
                if (dt.Rows.Count > 0)
                {
                    //Success!
                    //SetUserDataFromDr(dt.Rows[0]);
                    return dt.Rows[0]["Email"].ToString();
                }
            }

           // UserID = -1;
            return null;
        }


        /// <summary>
        /// Check the email is valid user's before changing password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="outputValue"></param>
        public static string ResetPassword(string email, out int outputValue)
        {
            SqlParameter rowsUpdated;
            SqlParameter output;
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList()
                .Add("@Email", email)
                .Add("@ClientID", Config.clientId)
                .AddOutputParam("@OutputCode", 4, out output)
                .AddOutputParam("@RowCount", 4, out rowsUpdated);

            DataTable dt = sql.ExecStoredProcedureDataTable("spCOM_PasswordReset", sqlParams);
            if (!sql.HasError || Int32.Parse(rowsUpdated.Value.ToString()) != 0)
            {
                outputValue = Conversion.StringToInt(output.Value.ToString());
                return dt.Rows[0]["GUID"].ToString();
            }
            else
            {
                outputValue = -1;
                return null;
            }
        }


        /// <summary>
        /// Updating existing user with new password
        /// </summary>
        /// <param name="newPswd"></param>
        /// <param name="outputValue"></param>
        public bool UpdatePassword(int userId, string newPswd, out string outputMsg)
        {
            if (!CommonFunctions.IsPasswordValid(newPswd, out outputMsg))
            {
                return false;
            }
            if (!SubFunctions.ValidateUser(out outputMsg))
            {
                return false;
            }

            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SqlParameter affectedRows;
            SQLParamList sqlParams = new SQLParamList()
                .Add("@UserID", userId)
                .Add("@NewPassword", newPswd)
                .AddOutputParam("@AffectedRows", 4, out affectedRows);

            sql.ExecStoredProcedureDataTable("spCOM_UpdatePassword", sqlParams);
            if (sql.HasError)
            {
                outputMsg = AccountResource.Error_DatabaseLevel;
                return false;
            }
            else if (Conversion.StringToInt(affectedRows.Value.ToString(), 0) == 0)
            {
                outputMsg = AccountResource.Error_InvalidUserCantChangePassword;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears Password History for specified userId
        /// </summary>
        /// <returns></returns>
        public int ClearLoginAttempts(int userId)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            int rows = sql.NonQuery("DELETE FROM dbo.tblCOM_UserLoginAttempts WHERE UserID = @UserID", new SqlParameter("@UserID", userId));
            return rows;
        }


        /// <summary>
        /// Get all users list
        /// </summary>
        /// <returns></returns>
        public DataTable GetUsersList(int userId)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();

            sqlParams.Add("@UserID", userId);
            sqlParams.Add("@TextSearch", null);

            DataTable dt = sql.ExecStoredProcedureDataTable("spAdmin_User_List", sqlParams);
            if (!sql.HasError)
            {
                return dt;
            }

            return null;
        }


        /// <summary>
        /// Load user details
        /// </summary>
        /// <returns></returns>
        public DataTable LoadUserDetails(int userId)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();

            sqlParams.Add("@UserID", userId);
            DataTable dt = sql.ExecStoredProcedureDataTable("spAdmin_User_Get", sqlParams);

            if (!sql.HasError)
            {
                return dt;
            }

            return null;
        }


        /// <summary>
        /// Get list of Groups
        /// </summary>
        /// <returns></returns>
        public static DataTable LoadGroupOptions()
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            DataTable dt = sql.QueryDataTable("SELECT * FROM [tblCOM_Groups] WHERE ClientID = @ClientId", new SqlParameter("@ClientId", Config.clientId));

            if (!sql.HasError)
            {
                return dt;
            }
            return null;
        }


        /// <summary>
        /// Add new user
        /// </summary>
        /// <returns></returns>
        public static string AddNewuser(string userName, string password, string firstName, string lastName, string email, string phone, int addToGroup, int createdUserID, out int msg)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();
            SqlParameter userId = new SqlParameter("@UserID", SqlDbType.Int);
            userId.Direction = ParameterDirection.Output;

            sqlParams.Add("@Username", userName);
            sqlParams.Add("@Password", password);
            sqlParams.Add("@ClientID", Config.clientId);
            sqlParams.Add("@CreateUserID", createdUserID);
            sqlParams.Add("@Email", email);
            sqlParams.Add("@Phone", phone);
            sqlParams.Add("@FirstName", firstName);
            sqlParams.Add("@LastName", lastName);
            sqlParams.Add("@ExpirePassword", true);
            sqlParams.Add("@AddToGroup", addToGroup);
            sqlParams.Add(userId);

            sql.ExecStoredProcedureDataTable("spCOM_CreateNewUser", sqlParams);
            if (sql.HasError)
            {
                msg = 1;
                return UserResource.Error_UnableToSaveUser;
            }
            else
            {
                msg = 0;
                return UserResource.Message_AddSuccess;
            }
        }

        /// <summary>
        /// Checks the email is already in use
        /// </summary>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool IsExistingEmail(string email, out string message)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            DataTable dtEmail = sql.QueryDataTable("SELECT UserID FROM [tblCOM_Users] WHERE Email = @Email",
                                                    new SQLParamList().Add("@Email", email));
            if (sql.HasError)
            {
                message = UserResource.Error_DBEmailValidationError;
                return false;
            }
            else if (dtEmail.Rows.Count > 0)
            {
                message = UserResource.Error_EmailInUse;

                return false;
            }
            else
            {
                message = UserResource.Success_Email;
                return true;
            }

        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <returns></returns>
        public static string UpdateUser(int userId, int modifiedUserID, string firstName, string lastName, string email, string phone, int groupId, int status)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();

            sqlParams.Add("@UserID", userId);
            sqlParams.Add("@FirstName", firstName);
            sqlParams.Add("@LastName", lastName);
            sqlParams.Add("@Email", email);
            sqlParams.Add("@Phone", phone);
            sqlParams.Add("@ModifiedUserID", modifiedUserID);
            sqlParams.Add("@GroupID", groupId);
            sqlParams.Add("@Active", status);

            sql.ExecStoredProcedureDataTable("spAdmin_User_Update", sqlParams);
            if (sql.HasError)
            {
                return UserResource.Error_UnableToUpdateUser;
            }
            else
            {
                return UserResource.Message_UpdateSuccess;
            }
        }


        /// <summary>
        /// Delete user from list
        /// </summary>
        /// <returns></returns>
        public static int DeleteUser(int userID)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();
            SqlParameter output;
            SqlParameter rowsUpdated;

            sqlParams.Add("@UserID", userID);
            sqlParams.Add("@ClientID", Config.clientId);
            sqlParams.AddOutputParam("@OutputCode", 4, out output);
            sqlParams.AddOutputParam("@RowCount", 4, out rowsUpdated);

            DataTable dt = sql.ExecStoredProcedureDataTable("spCOM_DeleteUser", sqlParams);

            if (!sql.HasError)
            {
                return Convert.ToInt32(dt.Rows[0]["AffectedRows"]);
            }

            return 0;
        }


        /// <summary>
        /// Get report data
        /// </summary>
        /// <returns></returns>
        public static DataTable GetReport(string rptType, string van, int fromMnth, int toMnth)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();
            sqlParams.Add("@ReportSelection", rptType);
            sqlParams.Add("@Question", "");
            sqlParams.Add("@FromMnth", fromMnth);
            sqlParams.Add("@ToMnth", toMnth);
            sqlParams.Add("@Van", van);

            DataTable dt = sql.ExecStoredProcedureDataTable("sp_GetRecordsOfQuestions", sqlParams);

            if (!sql.HasError)
            {
                return dt;
            }

            return null;
        }


        /// <summary>
        /// Get list of Call Center follow-up questions
        /// </summary>
        /// <returns></returns>
        public static DataTable GetQuestions(string rptType)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();

            sqlParams.Add("@ReportType", rptType);

            DataTable dt = sql.ExecStoredProcedureDataTable("spAdmin_Question_List", sqlParams);

            if (!sql.HasError)
            {
                return dt;
            }

            return null;
        }


        /// <summary>
        /// Get Report per question based on month
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMonthlyRecordOfQuestion(string rptType, string QsnId, int fromMnth, int toMnth)
        {
            SQLDatabaseReporting sql = new SQLDatabaseReporting();
            SQLParamList sqlParams = new SQLParamList();

            sqlParams.Add("@ReportSelection", rptType);
            sqlParams.Add("@Question", QsnId);
            sqlParams.Add("@FromMnth", fromMnth);
            sqlParams.Add("@ToMnth", toMnth);

            DataTable dt = sql.ExecStoredProcedureDataTable("sp_GetMonthlyRecordsOfQuestion", sqlParams);

            if (!sql.HasError)
            {
                return dt;
            }
            return null;
        }

    }
}