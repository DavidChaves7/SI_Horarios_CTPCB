using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IMantenimientosRepository _mantenimientosRepository => HttpContext.RequestServices.GetRequiredService<IMantenimientosRepository>();

    }
}
