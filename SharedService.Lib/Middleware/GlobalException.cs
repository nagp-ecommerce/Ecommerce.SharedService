using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SharedService.Lib.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string title = "Error";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string msg = "Internal Server Error";

            try
            {
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    msg = "Not Authorized to View";
                    statusCode = context.Response.StatusCode;
                    await ModifyHeader(context, title, msg, statusCode);
                }
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    msg = "Not Allowed to Access";
                    statusCode = context.Response.StatusCode;
                    await ModifyHeader(context, title, msg, statusCode);
                }
            }
            catch (Exception ex) {
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of Time";
                    msg = "Reqeust Timeout. Try Again";
                    statusCode = context.Response.StatusCode;
                }
                await ModifyHeader(context, title, msg, statusCode);
            }
        }

        private async Task ModifyHeader(HttpContext context, string title, string msg, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(
                new ProblemDetails() 
                { 
                    Detail=msg,
                    Title=title,
                    Status=statusCode
                }
            ), CancellationToken.None);
        }
    }
}
