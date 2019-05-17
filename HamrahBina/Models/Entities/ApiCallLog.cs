using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MD.PersianDateTime;

namespace HamrahBina.Models.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiCallLog
    {
        /// <summary>
        /// PrimaryKey
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The input object which is serialized and keeped in this property
        /// </summary>
        [Required]
        public string Input { get; set; }

        /// <summary>
        /// The output of the Ocr service which is serialized and keeped in this property
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Which user called the ocr
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The date and time of when user called Ocr service
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The username of the api call log
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// User's uploaded file
        /// </summary>
        [NotMapped]
        public string InputFileName => !string.IsNullOrEmpty(Input) ? Input.Split('#').FirstOrDefault() : "";

        /// <summary>
        /// Returns the persian date and time of the equivalant gregorian CreateDate
        /// </summary>
        [NotMapped]
        public string CreateDateFa => $"{new MD.PersianDateTime.PersianDateTime(CreateDate).ToShortDateString()} {CreateDate.ToString("HH:mm:ss")}";
    }
}
