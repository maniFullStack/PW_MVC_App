using SharedClass.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWPortal.Models
{
    public class UserInfoModel
    {
        #region User properties

        /// <summary>
        ///     Gets or sets the user's username.
        /// </summary>
        public static string Username { get; set; }

        /// <summary>
        ///     Gets or sets the user's GroupID.
        /// </summary>
        public static int GroupID { get; set; }

        /// <summary>
        ///     Gets or sets the user ID value from the database.
        /// </summary>
        public static int UserID { get; set; }

        /// <summary>
        ///     Gets or sets the user's first name.
        /// </summary>
        public static string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets the user's last name.
        /// </summary>
        public static string LastName { get; set; }

        /// <summary>
        ///     Gets or sets the user's email.
        /// </summary>
        public static string Email { get; set; }

        /// <summary>
        ///     The last time the user was logged in.
        /// </summary>
        public static DateTime LastLoginDate { get; set; }

        /// <summary>
        ///     The date the user's password expires.
        /// </summary>
        public static DateTime PasswordExpireDate { get; set; }

        /// <summary>
        ///     The date the user reset their password.
        /// </summary>
        public static DateTime PasswordResetDate { get; set; }

        /// <summary>
        ///     The amount of times the user has logged in.
        /// </summary>
        public static int LoginCount { get; set; }

        /// <summary>
        ///     The date the user was created.
        /// </summary>
        public static DateTime CreationDate { get; set; }

        /// <summary>
        ///     The last time this user was modified.
        /// </summary>
        public static DateTime ModificationDate { get; set; }

        /// <summary>
        ///     The user who last modified this user.
        /// </summary>
        public static int ModifiedUserID { get; set; }

        /// <summary>
        ///     The GUID of the user from the database.
        /// </summary>
        public static string GUID { get; set; }
        /// <summary>
        /// Is The User In a Deleted / Inactive 
        /// </summary>
        public static bool IsActive { get; set; }

        /// <summary>
        /// The user's group.
        /// </summary>
        public static UserGroups Group { get; set; }


        public static List<UserInfoModel> UserInfo { get; set; }

        #endregion
    }
}