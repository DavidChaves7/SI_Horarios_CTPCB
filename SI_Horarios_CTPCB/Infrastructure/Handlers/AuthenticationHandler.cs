
using SI_Horarios_CTPCB.Infrastructure.Authentication;
using SI_Horarios_CTPCB.Infrastructure.Interfaces;
using SI_Horarios_CTPCB.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace SI_Horarios_CTPCB.Infrastructure.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        readonly CircuitServicesAccessor circuitServicesAccessor;

        public AuthenticationHandler(CircuitServicesAccessor circuitServicesAccessor)
        {
            this.circuitServicesAccessor = circuitServicesAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authStateProvider = circuitServicesAccessor.Services?
            .GetRequiredService<AuthenticationStateProvider>();
            if (authStateProvider != null)
            {
                var authState = await authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var authClaim = user.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication");
                var _jwt = authClaim?.Value ?? "";
                if (!string.IsNullOrEmpty(_jwt))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
