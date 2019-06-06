using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RestSharp;

namespace HamrahBina.Providers.OcrProviders
{
    /// <summary>
    /// IranOCR provider
    /// </summary>
    public class IranOcrProvider : IOCRProvider
    {
        #region Properties
        /// <summary>
        /// Name of the Ocr provider
        /// </summary>
        public string Name => "IranOCR";

        /// <summary>
        /// Base address of the ocr
        /// </summary>
        public string BaseUrl => "https://www.iranocr.ir";
        #endregion

        #region Methods
        public OCRTransformationResult Transform(IFormFile file)
        {
            try
            {
                var sendImageClient = new RestClient("https://www.iranocr.ir");
                var sendImageRequest = new RestRequest("api/OCR_Picture", Method.POST);
                sendImageRequest.AddParameter("UserName", "pooyaalapoor@yahoo.com");
                sendImageRequest.AddParameter("WebServiceKey", "431808584");
                sendImageRequest.AddParameter("WebServicePassword", "98Hamrahbina");
                sendImageRequest.AddParameter("PictureFileName", "example.png");
                sendImageRequest.AddParameter("PictureContentType", "image/png");
                sendImageRequest.AddParameter("LanguageTag", "fa");
                sendImageRequest.AddFile(file.Name, ReadToEnd(file.OpenReadStream()), file.FileName);
                var sendImageResponse = sendImageClient.Execute(sendImageRequest);
                var sendImageContent = sendImageResponse.Content;

                if (ErrorMesssages.Select(p => p.Item1.ToLower()).Contains(sendImageContent.ToLower()))
                {
                    var errorMessage = ErrorMesssages.Where(p => p.Item1.ToLower() == sendImageContent.ToLower())
                        .Select(p => p.Item2)
                        .FirstOrDefault();

                    return new OCRTransformationResult
                    {
                        ErrorMessage = errorMessage,
                        IsSuccessful = false
                    };
                }

                var getTextClient = new RestClient("https://www.iranocr.ir");
                var getTextRequest = new RestRequest("api/Get_TXT", Method.POST);
                getTextRequest.AddParameter("FileID", sendImageContent);
                var getTextResponse = getTextClient.Execute(getTextRequest);
                var getTextContent = getTextResponse.Content;

                return new ExtendedOCRTransformationResult<IranOCRResult>
                {
                    ErrorMessage = "",
                    IsSuccessful = true,
                    TransformedText = getTextContent,
                    ExtraInfo = new IranOCRResult
                    {
                        FileId = sendImageContent
                    }
                };
            }
            catch(Exception ex)
            {
                return new OCRTransformationResult
                {
                    ErrorMessage = ex.Message,
                    IsSuccessful = false,
                };
            }
        }
        #endregion

        #region Tools
        public List<Tuple<string, string>> ErrorMesssages
        {
            get
            {
                var result = new List<Tuple<string, string>>();
                result.Add(new Tuple<string, string>("ERR_USER_CREDIT_NOT_VALID", "عدم اعتبار کاربر"));
                result.Add(new Tuple<string, string>("ERR_OCR_FAIL", "خطا در مراحل OCR"));
                result.Add(new Tuple<string, string>("ERR_CREDIT_LOW", "میزان اعتبار کافی نیست"));
                result.Add(new Tuple<string, string>("ERR_Output_NA", "خطا در مراحل تولید فایل خروجی"));
                result.Add(new Tuple<string, string>("ERR_PIC_NOT_VALID", "عدم اعتبار فایل عکس"));
                result.Add(new Tuple<string, string>("ERR_WSPASS_NOT_VALID", "عدم اعتبار کلمه عبور وب سرویس"));
                result.Add(new Tuple<string, string>("ERR_WS_NOT_VALID", "عدم اعتبار کلید وب سرویس"));
                result.Add(new Tuple<string, string>("ERR_File_NA", "عدم وجود فایل در آدرس"));
                result.Add(new Tuple<string, string>("ERR_Exception", "خطای ناشناخته"));
                return result;
            }
        }

        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// IranOcr's extra info which is needed to be saved
    /// </summary>
    public class IranOCRResult
    {
        /// <summary>
        /// File Id which is returned from ocr first call
        /// </summary>
        public string FileId { get; set; }
    }
}
