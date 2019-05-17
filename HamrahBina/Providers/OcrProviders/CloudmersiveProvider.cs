﻿using System;
using Cloudmersive.APIClient.NET.OCR.Api;
using Cloudmersive.APIClient.NET.OCR.Client;
using Cloudmersive.APIClient.NET.OCR.Model;
using System.Diagnostics;

namespace HamrahBina.Providers.OcrProviders
{
    /// <summary>
    /// Cloudmersive provider
    /// </summary>
    public class CloudmersiveProvider : IOCRProvider
    {
        public string Name { get => "CloudmersiveProvider"; }

        public OCRTransformationResult Transform(System.IO.Stream imageStream)
        {
            try
            {
                // Configure API key authorization: Apikey
                Configuration.Default.AddApiKey("Apikey", "11b47b08-4328-4620-90ab-a1499f4f67dc");

                // Instantiate a cloudmersive ocr api
                var apiInstance = new ImageOcrApi();

                // System.IO.Stream | Image file to perform OCR on. Common file formats such as PNG, JPEG are supported.
                //var imageFile = new System.IO.FileStream("D:\\Downloads\\Untitled.png", System.IO.FileMode.Open);

                // Language of the document - FAS = Farsi(Persian)
                var language = "FAS";

                // Preprocessing the image. Auto = default value
                var preprocessing = "AUTO";

                // Convert a scanned image into text
                var result = apiInstance.ImageOcrPost(imageStream, language, preprocessing);
                return new OCRTransformationResult
                {
                    IsSuccessful = true,
                    TransformedText = result.TextResult
                };
            }
            catch (Exception ex)
            {
                return new OCRTransformationResult
                {
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}