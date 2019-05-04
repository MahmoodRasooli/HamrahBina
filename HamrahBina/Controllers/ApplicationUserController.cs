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
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() => View());
        }

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
        #endregion
    }
}