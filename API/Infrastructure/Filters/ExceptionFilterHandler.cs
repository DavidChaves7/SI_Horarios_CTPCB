using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Infrastructure.Filters
{
    public class ExceptionFilterHandler : ExceptionFilterAttribute
    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            Console.WriteLine(context.Exception);
            await base.OnExceptionAsync(context);
        }
    }
}
