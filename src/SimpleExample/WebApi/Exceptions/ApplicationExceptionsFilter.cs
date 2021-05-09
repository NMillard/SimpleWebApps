using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Exceptions {
    public class ApplicationExceptionsFilter : IAsyncExceptionFilter {
        
        public Task OnExceptionAsync(ExceptionContext context) {
            
            if (context.Exception is ApplicationExceptionBase ex) {
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = ex.GetType().GetGenericTypeDefinition() == typeof(EntityNotFoundException<>) ? 
                        StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;
                context.Result = new ObjectResult(new { ex.Message });
            }
            
            return Task.CompletedTask;
        }
    }
}