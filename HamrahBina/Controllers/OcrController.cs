using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HamrahBina.Common.Enums;
using HamrahBina.Data;
using HamrahBina.Models.Dto;
using HamrahBina.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HamrahBina.Controllers
{
    /// <summary>
    /// This controller is the base transform mechanism of the application.
    /// Use of different providers, needs this controller to be updated.
    /// </summary>
    [Route("api/[controller]/action")]
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
        #endregion

        #region Ctor
        public OcrController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor accessor,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _accessor = accessor;
            _config = config;
        }
        #endregion

        #region Actions
        /// <summary>
        /// The main action for transforming image to text
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<OkObjectResult> Transform([FromBody]TransformRequestDto input)
        {
            await Task.Run(() => { });
            return new OkObjectResult(new ApiResponseDto<TransformResponseDto>
            {
                Status = true,
                StatusCode = (int)ApiStatusCodeEnum.Successful,
                Response = new TransformResponseDto
                {
                    TransformedText = "This is a dummy text."
                }
            });
        } 
        #endregion
    }
}