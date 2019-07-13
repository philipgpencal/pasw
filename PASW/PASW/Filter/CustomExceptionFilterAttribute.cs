using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace PASW.Filter
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            context.Result = new JsonResult(new ErrorModel
            {
                Message = exception.Message,
                StatusCode = HttpStatusCode.BadRequest
            });
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        }
    }

    public class ErrorModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
