using SI_Horarios_CTPCB.Infrastructure.Services;
using System.Net.Http.Headers;
using System.Text;

namespace SI_Horarios_CTPCB.Infrastructure.ApiClient
{
    public abstract class BaseSIHAPI
    {

        private string? _jwt { get; set; }
        
        public void SetJwt(string jwt)
        {
            _jwt = jwt;
        }
        
        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var msg = new HttpRequestMessage();
            msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
            return Task.FromResult(msg);
        }
    }
}
