using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HamrahBina.Data;
using HamrahBina.Models.Entities;
using HamrahBina.Models.ViewModels.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HamrahBina.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            ApplicationDbContext context,
            IHttpContextAccessor accessor,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _accessor = accessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {                
                var currentUser = _context.Users.FirstOrDefault(a => a.Email == model.Email);
                if (currentUser != null)
                {
                    if (currentUser.IsDisabled == true)
                    {
                        ModelState.AddModelError("", "حساب کاربری شما غیر فعال شده است .");
                        return View();
                    }

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

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }

                ModelState.AddModelError(string.Empty, "نام کاربری یا رمز عبور اشتباه وارد شده است");
                return View();
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, "123456");
                }
                catch (Exception ex)
                {
                    return View();
                }

                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("RoleUserClaimAccess");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Lockout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return View();
            }
        }

        #region Temp
        [TempData]
        public string ErrorMessage { get; set; }
        #endregion
    }
}