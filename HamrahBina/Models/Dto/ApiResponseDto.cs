using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Dto
{
    /// <summary>
    /// Base response class, all return types of Api actions should implement it.
    /// </summary>
    /// <typeparam name="T">The response type</typeparam>
    public class ApiResponseDto<T> where T: class
    {
        /// <summary>
        /// Determines if the request done successfuly or not
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Shows the final state of processing the request
        /// 0 = Done successfuly
        /// 1 = Exception occured
        /// otherwise = an error occured
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Contains the result of request
        /// </summary>
        public T Response { get; set; }

        /// <summary>
        /// If and error occures or an exception raises, ErrorMessage will contain the respective message
        /// </summary>
        public string Message { get; set; }
    }
}