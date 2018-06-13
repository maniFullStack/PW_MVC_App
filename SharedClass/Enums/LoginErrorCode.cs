using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClass.Enums
{
    public enum LoginErrorCode
    {
        UnknownError = -1,
        Success = 0,
        UserLockedOut = 1,
        LoginFailed = 2,
        NoUserOrEmailSpecified = 3,
        UserOrEmailNotExists = 4,
        PasswordExpired = 5
    }
}
