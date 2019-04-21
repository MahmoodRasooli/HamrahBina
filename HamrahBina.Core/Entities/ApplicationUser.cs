//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Threading.Tasks;

//namespace HamrahBina.Models.Entities
//{
//    // Add profile data for application users by adding properties to the ApplicationUser class
//    public class ApplicationUser : IdentityUser
//    {
//        [PersonalData]
//        [MaxLength(20)]
//        [Display(Name = "نام")]
//        public string FirstName { get; set; }

//        [PersonalData]
//        [MaxLength(20)]
//        [Display(Name = "نام خانوادگی")]
//        public string LastName { get; set; }

//        [PersonalData]
//        [MaxLength(20)]
//        [Display(Name = "نام پدر")]
//        public string FatherName { get; set; }

//        [PersonalData]
//        [MaxLength(10)]
//        [Display(Name = "کد ملی")]
//        public string NationalId { get; set; }

//        [PersonalData]
//        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "تاریخ تولد")]
//        public DateTime BrithDate { get; set; }

//        [PersonalData]
//        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "تاریخ ایجاد")]
//        public DateTime CreateDate { get; set; }

//        [PersonalData]
//        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = " آخرین ورود")]
//        public DateTime LastLoginDate { get; set; }

//        [Display(Name = "غیر فعال")]
//        public bool IsDisabled { get; set; }
        
//        // Identity Relation
//        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
//        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
//        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
//        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

//        //Not Map
//        [NotMapped]
//        [Display(Name = "نام و نام خانوادگی")]
//        public string FullName => FirstName + " " + LastName;

//        [NotMapped]
//        public string Search => FirstName + " " + LastName + " " + NationalId;
//    }
//}
