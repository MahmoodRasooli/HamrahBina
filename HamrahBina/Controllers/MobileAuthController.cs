using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HamrahBina.Common.Enums;
using HamrahBina.Data;
using HamrahBina.Models.Dto;
using HamrahBina.Models.Entities;
using HamrahBina.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HamrahBina.Controllers
{
    /// <summary>
    /// Api to interact with clients for user's registeration and login.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MobileAuthController : ControllerBase
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _accessor;
        private readonly IConfiguration _config;
        #endregion

        #region Ctor
        public MobileAuthController(ApplicationDbContext context,
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
        /// The login method of api.
        /// </summary>
        /// <param name="input">Username and password of user</param>
        /// <returns>Returns user's info and an authentication token if login is successful and respective error if not</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<OkObjectResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _context.Users.FirstOrDefault(a => a.Email == model.Email);

                if (currentUser != null)
                {
                    if (currentUser.IsDisabled == true)
                        return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
                        {
                            Status = false,
                            StatusCode = (int)ApiStatusCodeEnum.UserIsDisabled,
                            Message = "حساب کاربری شما غیر فعال شده است.",
                        });

                    await _userManager.UpdateSecurityStampAsync(currentUser);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var changePasswordCheck = currentUser;
                    if (currentUser != null)
                    {
                        currentUser.LastLoginDate = DateTime.Now;
                        _context.Update(currentUser);
                        await _context.SaveChangesAsync();
                    }

                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(_config["Tokens:Issuer"], _config["Tokens:Issuer"], claims, expires: DateTime.Now.AddDays(30), signingCredentials: credential);
                    return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
                    {
                        Status = true,
                        StatusCode = (int)ApiStatusCodeEnum.Successful,
                        Message = "",
                        Response = new LoginResponseDto
                        {
                            Id = currentUser.Id,
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            UserName = currentUser.UserName
                        }
                    });
                }

                if (result.IsLockedOut)
                {
                    return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
                    {
                        Status = false,
                        StatusCode = (int)ApiStatusCodeEnum.UserIsLockedOut,
                        Message = "حساب کاربری محدود شده است. لطفا دقایقی دیگر مجددا تلاش کنید.",
                    });
                }

                return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
                {
                    Status = false,
                    StatusCode = (int)ApiStatusCodeEnum.UsernameOrPasswordIsIncorrect,
                    Message = "نام کاربری و/یا رمز عبور اشتباه است.",
                });
            }

            return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
            {
                Status = false,
                StatusCode = (int)ApiStatusCodeEnum.ErrorOccured,
                Message = ModelState.Values.SelectMany(p => p.Errors?.Select(q => q?.ErrorMessage)).FirstOrDefault()
            });
        }

        /// <summary>
        /// The users registeration mechanism of api.
        /// </summary>
        /// <param name="model">The info which is necessary to register a user</param>
        /// <returns>Success message or the errors that prevents from registeration</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<OkObjectResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email.ToLower(),
                    UserName = model.Email.ToLower(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return new OkObjectResult(new ApiResponseDto<bool>
                    {
                        Status = true,
                        StatusCode = (int)ApiStatusCodeEnum.Successful,
                        Message = "ثبت کاربر با موفقیت انجام شد.",
                        Response = true
                    });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return new OkObjectResult(new ApiResponseDto<bool>
            {
                Status = false,
                StatusCode = (int)ApiStatusCodeEnum.ErrorOccured,
                Message = ModelState.Values.SelectMany(p => p.Errors?.Select(q => q?.ErrorMessage)).FirstOrDefault(),
                Response = false
            });
        }

        /// <summary>
        /// This method is used to get user's information.
        /// </summary>
        /// <param name="model">Contains the email address of the user</param>
        /// <returns>User's basic information</returns>
        [HttpPost]
        public async Task<OkObjectResult> GetUserInfo([FromBody]GetUserInfoViewModel model)
        {
            var user = await Task.Run(() => _context.Users.FirstOrDefault(p => p.UserName.ToLower() == model.Email.ToLower()));
            if (user != null)
            {
                return new OkObjectResult(new ApiResponseDto<UserInfoResponseViewModel>
                {
                    Status = true,
                    StatusCode = (int)ApiStatusCodeEnum.Successful,
                    Message = "",
                    Response = new UserInfoResponseViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastLoginDate = user.LastLoginDate,
                        LastName = user.LastName
                    }
                });
            }

            return new OkObjectResult(new ApiResponseDto<LoginResponseDto>
            {
                Status = false,
                StatusCode = (int)ApiStatusCodeEnum.UserNotFound,
                Message = "کاربری با این مشخصات در سیستم ثبت نشده است"
            });
        }
        #endregion
    }
}