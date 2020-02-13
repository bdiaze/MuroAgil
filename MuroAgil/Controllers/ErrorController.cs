using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MuroAgil.Others;

namespace MuroAgil.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
			var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewData["ClaseError"] = exception.Error.GetType().FullName;
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
			ViewData["Message"] = exception.Error.Message;
			ViewData["StackTrace"] = exception.Error.StackTrace;
			return View("Index");
        }

		public IActionResult AccessDenied() {
			return View("AccessDenied");
		}
	}
}