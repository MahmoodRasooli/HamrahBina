﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.ViewModels.Account
{
    /// <summary>
    /// To get user information, the email of the username is required
    /// </summary>
    public class GetUserInfoViewModel
    {
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل وارد شده صحیح نمی باشد")]
        public string Email { get; set; }
    }

    /// <summary>
    /// The response of api, which contains user's information
    /// </summary>
    public class UserInfoResponseViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
