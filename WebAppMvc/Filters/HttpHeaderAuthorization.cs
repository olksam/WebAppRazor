using System.Data.Common;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppMvc.Filters {

    public class GlobalExceptionFilter : IExceptionFilter {
        public void OnException(ExceptionContext context) {
            Console.WriteLine(context.Exception.ToString());

            if (context.Exception is KeyNotFoundException || context.Exception is NullReferenceException) {
                context.Result = new RedirectResult("/home/error");
            } else if (context.Exception is DbException) {
                context.Result = new StatusCodeResult(500);
            }
        }
    }

    public class ApiKeyQueryAuthorization : IAuthorizationFilter {

        public void OnAuthorization(AuthorizationFilterContext context) {
            var isAuthorized = context.HttpContext.Request.Query.Any(e => e.Key == "apiKey" && e.Value == "pass123");

            if (!isAuthorized) {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
