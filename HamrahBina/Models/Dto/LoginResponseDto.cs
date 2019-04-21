using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Dto
{
    /// <summary>
    /// Login response
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// First name of the logged in user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the logged in user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The authentication token which will be needed for further requests
        /// </summary>
        public string Token { get; set; }
    }
}