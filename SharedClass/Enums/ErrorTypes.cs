using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClass.Enums
{
    public enum ErrorTypes
    {
        /// <summary>
        /// Default. Don't use this one if it can be avoided.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// General errors.
        /// </summary>
        General = 1,
        /// <summary>
        /// Error in SQL queries.
        /// </summary>
        SQLError = 4,
        /// <summary>
        /// Error in LogHandler. Do not use this.
        /// </summary>
        LogError = 5,
        /// <summary>
        /// Error in LogHandler. Do not use this.
        /// </summary>
        LogErrorSQL = 6,
        /// <summary>
        /// Error in LogHandler. Do not use this.
        /// </summary>
        LogErrorEvent = 7,
        /// <summary>
        /// Error in LogHandler. Do not use this.
        /// </summary>
        LogErrorFile = 8,
        /// <summary>
        /// Connection error.
        /// </summary>
        ConnectionError = 9
    }
}
