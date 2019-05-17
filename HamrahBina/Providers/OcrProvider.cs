using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Providers
{
    /// <summary>
    /// Response of the The text
    /// </summary>
    public class OCRTransformationResult
    {
        /// <summary>
        /// Shows the result of calling api was successful or not
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Reponse of the ocr api
        /// </summary>
        public string TransformedText { get; set; }

        /// <summary>
        /// Error message in case of failure
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Base interface which should be implemented by every and each OCR api which is used.
    /// </summary>
    public interface IOCRProvider
    {
        /// <summary>
        /// Name of the ocr
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Base method for interacting with OCR apis
        /// </summary>
        /// <returns></returns>
        OCRTransformationResult Transform(System.IO.Stream imageStream);
    }

    /// <summary>
    /// A dummy class which returns a constant text.
    /// It is used for accelerating development process.
    /// </summary>    
    public class DummyProvider : IOCRProvider
    {
        public string Name => "DummyProvider";

        public OCRTransformationResult Transform(System.IO.Stream imageStream)
        {
            return new OCRTransformationResult
            {
                IsSuccessful = true,
                TransformedText = "This is a dummy text"
            };
        }
    }
}