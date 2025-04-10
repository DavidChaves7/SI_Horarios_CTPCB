using Microsoft.AspNetCore.Components.Server.Circuits;

namespace SI_Horarios_CTPCB.Infrastructure.Authentication
{
    public class CircuitServicesAccessor
    {
        static readonly AsyncLocal<IServiceProvider> blazorServices = new();

        public IServiceProvider? Services
        {
            get => blazorServices.Value;
            set => blazorServices.Value = value;
        }
    }

    public class ServicesAccessorCircuitHandler : CircuitHandler
    {
        readonly IServiceProvider services;
        readonly CircuitServicesAccessor circuitServicesAccessor;

        public ServicesAccessorCircuitHandler(IServiceProvider services,
            CircuitServicesAccessor servicesAccessor)
        {
            this.services = services;
            this.circuitServicesAccessor = servicesAccessor;
        }

        public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
            Func<CircuitInboundActivityContext, Task> next)
        {
            return async context =>
            {
                circuitServicesAccessor.Services = services;
                await next(context);
                circuitServicesAccessor.Services = null;
            };
        }
    }
}
