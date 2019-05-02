using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Common.Enums
{
    /// <summary>
    /// Status coded of Api responsed
    /// </summary>
    public enum ApiStatusCodeEnum : int
    {
        /// <summary>
        /// Request processed successfuy
        /// </summary>
        Successful = 0,

        /// <summary>
        /// Unhandled exception occured
        /// </summary>
        ExceptionOccured = 1,

        /// <summary>
        /// An error occured during process of request
        /// </summary>
        ErrorOccured = 2,

        /// <summary>
        /// User is disabled
        /// </summary>
        UserIsDisabled = 100,

        /// <summary>
        /// User is locked out
        /// </summary>
        UserIsLockedOut = 101,

        /// <summary>
        /// Username and/or password is incorrect
        /// </summary>
        UsernameOrPasswordIsIncorrect = 102,

        /// <summary>
        /// User not found
        /// </summary>
        UserNotFound = 103
    }
}