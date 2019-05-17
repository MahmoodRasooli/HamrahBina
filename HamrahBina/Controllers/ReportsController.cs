using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HamrahBina.Data;
using HamrahBina.Models.Entities;
using HamrahBina.Models.ViewModels.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using HamrahBina.Providers;

namespace HamrahBina.Controllers
{
    /// <summary>
    /// Reports controller
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        #endregion

        #region Ctor
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Actions
        /// <summary>
        /// Get action of the api logs page
        /// </summary>
        /// <returns></returns>
        public IActionResult ApiLogs()
        {
            return View();
        }

        /// <summary>
        /// Post action of the api logs page, which fills the grid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ApiLogs(DataTableRequestViewModel<ApiCallLog> dataTableModel)
        {
            if (dataTableModel != null)
            {
                var query = from a in _context.ApiCallLogs.AsQueryable()
                            join u in _context.Users on a.UserId equals u.Id
                            select new ApiCallLog
                            {
                                Id = a.Id,
                                CreateDate = a.CreateDate,
                                UserName = u.UserName,
                                Input = a.Input
                            };

                var totalCount = query.Count();

                if (dataTableModel.SearchObject != null)
                {
                    var searchObj = dataTableModel.SearchObject;

                    if (searchObj.Id != Guid.Empty)
                        query = query.Where(p => p.Id == searchObj.Id);

                    if (!string.IsNullOrEmpty(searchObj.UserName))
                        query = query.Where(p => p.UserName.Contains(searchObj.UserName));

                    if (searchObj.CreateDate > DateTime.MinValue)
                        query = query.Where(p => p.CreateDate == searchObj.CreateDate);

                    if (!string.IsNullOrEmpty(searchObj.Input))
                        query = query.Where(p => p.Input.Contains(searchObj.Input));
                }

                if (!string.IsNullOrEmpty(dataTableModel.SortPhrase))
                    query = query.OrderBy(dataTableModel.SortPhrase);
                else
                    query = query.OrderBy(p => p.Id);

                var result = query.Skip(dataTableModel.Start).Take(dataTableModel.Length).ToList();
                return new JsonResult(new GridDataResponseViewModel<ApiCallLog>
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


        /// <summary>
        /// Get action of the ocr logs page
        /// </summary>
        /// <returns></returns>
        public IActionResult OcrLogs()
        {
            return View();
        }

        /// <summary>
        /// Post action of the api logs page, which fills the grid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OcrLogs(DataTableRequestViewModel<OcrCallLog> dataTableModel)
        {
            if (dataTableModel != null)
            {
                var query = from o in _context.OcrCallLogs
                            join u in _context.Users on o.UserId equals u.Id
                            join a in _context.ApiCallLogs on o.ApiCallLogId equals a.Id
                            select new OcrCallLog
                            {
                                Id = o.Id,
                                CreateDate = o.CreateDate,
                                UserName = u.UserName,
                                Input = a.Input,
                                ApiCallLogId = a.Id,
                                IsSuccessful = o.IsSuccessful,
                                OcrName = o.OcrName,
                                Output = o.Output,
                                ResponseDate = o.ResponseDate,
                                UserId = o.UserId,
                            };

                var totalCount = query.Count();

                if (dataTableModel.SearchObject != null)
                {
                    var searchObj = dataTableModel.SearchObject;

                    if (!string.IsNullOrEmpty(searchObj.UserName))
                        query = query.Where(p => p.UserName.Contains(searchObj.UserName));

                    if (searchObj.CreateDate > DateTime.MinValue)
                        query = query.Where(p => p.CreateDate == searchObj.CreateDate);

                    if (!string.IsNullOrEmpty(searchObj.Input))
                        query = query.Where(p => p.Input.Contains(searchObj.Input));

                    if (!string.IsNullOrEmpty(searchObj.OcrName))
                        query = query.Where(p => p.OcrName.Contains(searchObj.OcrName));
                }

                if (!string.IsNullOrEmpty(dataTableModel.SortPhrase))
                    query = query.OrderBy(dataTableModel.SortPhrase);
                else
                    query = query.OrderBy(p => p.Id);

                var result = query.Skip(dataTableModel.Start).Take(dataTableModel.Length).ToList();
                return new JsonResult(new GridDataResponseViewModel<OcrCallLog>
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

        /// <summary>
        /// Get the ocr call response text
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetOcrResponse(Guid id)
        {
            try
            {
                var ocrCallLog = _context.OcrCallLogs
                    .Where(p => p.Id == id)
                    .Select(p => p.Output)
                    .FirstOrDefault();

                if (ocrCallLog == null)
                    return new JsonResult(new
                    {
                        status = false,
                        isSuccessful = false,
                        message = "رکورد یافت نشد.",
                        exception = ""
                    });


                var ocrResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<OCRTransformationResult>(ocrCallLog);

                return new JsonResult(new
                {
                    status = true,
                    isSuccessful = ocrResponse.IsSuccessful,
                    message = ocrResponse.IsSuccessful ? ocrResponse.TransformedText : ocrResponse.ErrorMessage,
                    exception = ""
                });
            }
            catch(Exception ex)
            {
                return new JsonResult(new
                {
                    status = false,
                    isSuccessful = false,
                    message = "خطا در عملیات",
                    exception = ex.Message
                });
            }
        }
        #endregion
    }
}