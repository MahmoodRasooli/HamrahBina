using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HamrahBina.Data;
using HamrahBina.Models.Entities;
using HamrahBina.Models.ViewModels.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace HamrahBina.Controllers
{
    /// <summary>
    /// Application User
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ApplicationUserController : Controller
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Ctor
        public ApplicationUserController(ApplicationDbContext context,
            IHttpContextAccessor accessor,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _accessor = accessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Actions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetGridData(DataTableRequestViewModel<ApplicationUser> dataTableModel)
        {
            if (dataTableModel != null)
            {
                var query = _context.Users.AsQueryable();
                var totalCount = query.Count();

                if (dataTableModel.SearchObject != null)
                {
                    if (!string.IsNullOrEmpty(dataTableModel.SearchObject.Email))
                        query = query.Where(p => p.Email.ToLower().Contains(dataTableModel.SearchObject.Email.ToLower()));

                    if (!string.IsNullOrEmpty(dataTableModel.SearchObject.FirstName))
                        query = query.Where(p => p.FirstName.ToLower().Contains(dataTableModel.SearchObject.FirstName.ToLower()));

                    if (!string.IsNullOrEmpty(dataTableModel.SearchObject.LastName))
                        query = query.Where(p => p.LastName.ToLower().Contains(dataTableModel.SearchObject.LastName.ToLower()));

                    if (!string.IsNullOrEmpty(dataTableModel.SearchObject.PhoneNumber))
                        query = query.Where(p => p.PhoneNumber.ToLower().Contains(dataTableModel.SearchObject.PhoneNumber.ToLower()));
                }

                if (!string.IsNullOrEmpty(dataTableModel.SortPhrase))
                    query = query.OrderBy(dataTableModel.SortPhrase);
                else
                    query = query.OrderBy(p => p.Email);

                var result = query.Skip(dataTableModel.Start).Take(dataTableModel.Length).ToList();
                return new JsonResult(new GridDataResponseViewModel<ApplicationUser>
                {
                    Draw = dataTableModel.Draw,
                    RecordsTotal = totalCount,
                    RecordsFiltered = totalCount,
                    Data = result,
                });
            }
            else
                return new JsonResult(null);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> View(string id)
        {
            var entity = _context.Users.FirstOrDefault(p => p.Id == id);
            return View(entity);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var entity = _context.Users.FirstOrDefault(p => p.Id == id);
            var userRoles = await _userManager.GetRolesAsync(entity);

            entity.RolesList = _context.Roles.ToList().Select(x => new SelectItem
            {
                Selected = userRoles.Contains(x.Name),
                Title = x.Name,
                Value = x.Name
            }).ToList();

            return View(entity);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, ApplicationUser input)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.Users.FirstOrDefault(p => p.Id == input.Id);
                entity.FirstName = input.FirstName;
                entity.LastName = input.LastName;
                entity.IsDisabled = input.IsDisabled;
                entity.Email = input.Email;
                entity.UserName = input.Email;
                entity.PhoneNumber = input.PhoneNumber;
                entity.NationalId = input.NationalId;

                var currentUserRoles = await _userManager.GetRolesAsync(entity);
                var newUserRoles = input.RolesList.Where(p=>p.Selected)
                    .Select(p => p.Value)
                    .ToList();

                var result = await _userManager.AddToRolesAsync(entity, newUserRoles.Except(currentUserRoles).ToList());
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                result = await _userManager.RemoveFromRolesAsync(entity, currentUserRoles.Except(newUserRoles).ToList());
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                _context.Users.Update(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> Delete(string id)
        {
            var entity = _context.Users.FirstOrDefault(p => p.Id == id);
            var userRoles = _context.ApplicationUserRole
                .Where(p => p.UserId == id)
                .ToList();

            var currentUserRoles = await _userManager.GetRolesAsync(entity);
            var result = await _userManager.RemoveFromRolesAsync(entity, currentUserRoles);
            if (!result.Succeeded)
                return Json(new
                {
                    status = false,
                    message = result.Errors.Select(p => p.Description).FirstOrDefault()
                });

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return Json(new
            {
                status = true,
                message = "عملیات با موفقیت انجام شد."
            });
        }
        #endregion
    }
}