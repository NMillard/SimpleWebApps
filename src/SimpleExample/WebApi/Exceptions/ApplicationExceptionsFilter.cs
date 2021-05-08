using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Exceptions {
    public class ApplicationExceptionsFilter : IAsyncExceptionFilter {
        
        public Task OnExceptionAsync(ExceptionContext context) {
            
            if (context.Exception is ApplicationExceptionBase ex) {
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(new { ex.Message });
            }
            
            return Task.CompletedTask;
        }
    }
}