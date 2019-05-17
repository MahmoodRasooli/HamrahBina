using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
