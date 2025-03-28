using SI_Horarios_CTPCB.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class Session
    {
        public string? CedulaEstudio { get; set; } = "";
        public string? Jwt { get; set; } = "";


    }
}
