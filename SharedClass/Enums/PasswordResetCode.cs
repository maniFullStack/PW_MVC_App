using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClass.Enums
{
    public enum PasswordResetCode
    {

        UserNotFound = 2,
        Success = 1,
        CriticalError = 3,
        Other = -1,
        UserIdNotProvided = 4,
        InvalidEmail = 5
    }
}
