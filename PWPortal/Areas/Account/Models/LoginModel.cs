using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PWPortal.Areas.Account.Models
{
    public class LoginModel
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string email { get; set; }       
    }

    public class ChangePasswordModel
    {       
        public string email { get; set; }
        [Required]
        public string newPswd { get; set; }
        [Required]
        public string confirmPswd { get; set; }
    }


}