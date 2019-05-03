using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Dto
{
    /// <summary>
    /// Used for user's registration
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل وارد شده صحیح نمی باشد")]
        public string Email { get; set; }

        [Required(ErrorMessage = " رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Display(Name = "نام")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} الزامی میباشد")]
        [Display(Name = "نام خانوادگی")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }
        
        [RegularExpression(@"(98|0)9([01239])\d{8}", ErrorMessage = "{0} وارد شده صحیح نمی باشد")]
        [Display(Name = "شماره همراه")]
        [DataType(DataType.Text)]
        public string PhoneNumber { get; set; }
    }
}
