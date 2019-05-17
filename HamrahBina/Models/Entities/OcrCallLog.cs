using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Entities
{
    /// <summary>
    /// The Ocr service call log
    /// Stores data about api input, output, responsetime, ....
    /// </summary>
    public class OcrCallLog
    {
        /// <summary>
        /// PrimaryKey
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The input object which is serialized and keeped in this property
        /// </summary>
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
        /// The ocr name, in case where more than one ocr provider is available in system
        /// </summary>
        public string OcrName { get; set; }

        /// <summary>
        /// The date and time of when ocr responded
        /// </summary>
        public DateTime ResponseDate { get; set; }

        /// <summary>
        /// The date and time of when user called Ocr service
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The related ApiCallLog records Id
        /// </summary>
        public Guid ApiCallLogId { get; set; }

        /// <summary>
        /// Shows if the calling ocr was successful
        /// </summary>
        public bool IsSuccessful { get; set; }
    }
}
