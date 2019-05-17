using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HamrahBina.Common.Enums;
using HamrahBina.Common.Tools;
using HamrahBina.Data;
using HamrahBina.Models.Dto;
using HamrahBina.Models.Entities;
using HamrahBina.Models.ViewModels.Account;
using HamrahBina.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HamrahBina.Controllers
{
    /// <summary>
    /// This controller is the base transform mechanism of the application.
    /// Use of different providers, needs this controller to be updated.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OcrController : ControllerBase
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region Ctor
        public OcrController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor accessor,
            IConfiguration config,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _accessor = accessor;
            _config = config;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        #region Actions
        /// <summary>
        /// The main action for transforming image to text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiExceptionFilter]
        public async Task<OkObjectResult> Transform(IFormFile file)
        {
            if (file == null)
                return new OkObjectResult(new ApiResponseDto<TransformResponseDto>
                {
                    Status = false,
                    StatusCode = (int)ApiStatusCodeEnum.InputsAreInvalid,
                    Message = "فایل ورودی صحیح نیست"
                });

            var userId = _userManager.GetUserId(User);
            var now = DateTime.Now.ToString("yyyy_mm_dd_hh_mm_ss");
            var fileName = $"{userId}_{now}_{file.FileName}";
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "UserUploads");
            if (file.Length > 0)
            {
                var filePath = Path.Combine(uploads, $"{fileName}");
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            var apiCallLog = new ApiCallLog
            {
                CreateDate = DateTime.Now,
                Input = $"{fileName}#" + Newtonsoft.Json.JsonConvert.SerializeObject(file),
                UserId = userId,
            };

            _context.ApiCallLogs.Add(apiCallLog);
            _context.SaveChanges(true);

            var ocrApi = new HamrahBina.Providers.OcrProviders.CloudmersiveProvider();
            var ocrCallLog = new OcrCallLog
            {
                CreateDate = DateTime.Now,
                OcrName = ocrApi.Name,
                UserId = userId,
                ApiCallLogId = apiCallLog.Id,
            };

            var readStream = file.OpenReadStream();
            var result = ocrApi.Transform(readStream);
            ocrCallLog.ResponseDate = DateTime.Now;
            ocrCallLog.Output = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            ocrCallLog.IsSuccessful = result.IsSuccessful;
            _context.OcrCallLogs.Add(ocrCallLog);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new ApiResponseDto<TransformResponseDto>
            {
                Status = result.IsSuccessful,
                StatusCode = result.IsSuccessful ? (int)ApiStatusCodeEnum.Successful : (int)ApiStatusCodeEnum.ExceptionOccured,
                Response = new TransformResponseDto
                {
                    TransformedText = result.IsSuccessful ? result.TransformedText : result.ErrorMessage
                }
            });
        }
        #endregion
    }
}