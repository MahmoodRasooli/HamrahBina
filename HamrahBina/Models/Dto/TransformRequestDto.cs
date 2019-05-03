using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Models.Dto
{
    /// <summary>
    /// Base class for sending the image to the api.
    /// </summary>
    public class TransformRequestDto
    {
        /// <summary>
        /// The files which clients sends to api
        /// </summary>
        public IFormFile Files { get; set; }
    }

    /// <summary>
    /// The response of transformation of image to text
    /// </summary>
    public class TransformResponseDto
    {
        /// <summary>
        /// Extracted text from image
        /// </summary>
        public string TransformedText { get; set; }
    }
}
