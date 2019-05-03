using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Dto
{
    /// <summary>
    /// To get user information, the email of the username is required
    /// </summary>
    public class GetUserInfoDto
    {
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل وارد شده صحیح نمی باشد")]
        public string Email { get; set; }
    }

    /// <summary>
    /// The response of api, which contains user's information
    /// </summary>
    public class UserInfoResponseDto
    {
        /// <summary>
        /// User's Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's last login date
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
    }
}
