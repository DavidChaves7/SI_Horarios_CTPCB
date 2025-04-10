using SI_Horarios_CTPCB.Infrastructure.Services;

namespace SI_Horarios_CTPCB.Infrastructure.Middleware
{
    public class UserServiceMiddleware
    {
        private readonly RequestDelegate next;

        public UserServiceMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, UserService service)
        {
            service.SetUser(context.User);
            await next(context);
        }
    }
}
