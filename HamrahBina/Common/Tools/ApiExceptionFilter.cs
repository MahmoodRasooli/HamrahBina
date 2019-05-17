using HamrahBina.Common.Enums;
using HamrahBina.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HamrahBina.Common.Tools
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {        
        public override void OnException(ExceptionContext context)
        {
            context.Result = new OkObjectResult(new ApiResponseDto<string>
            {
                Message = "خطا در عملیات",
                Response = null,
                Status = false,
                StatusCode = (int)ApiStatusCodeEnum.ExceptionOccured
            });
        }
    }
}