using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HamrahBina.Common.Tools
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model, string absolutePath = null);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly Microsoft.AspNetCore.Http.HttpContext _httpContext;


        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor accessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContext = accessor.HttpContext;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model, string absolutePath = null)
        {
            var actionContext = new ActionContext(_httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult;
                if (!string.IsNullOrWhiteSpace(absolutePath))
                {
                    viewResult = _razorViewEngine.GetView(absolutePath, viewName, false);

                }
                else
                {
                    viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
                }

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                )
                {
                    RouteData = _httpContext.GetRouteData()
                };
                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}