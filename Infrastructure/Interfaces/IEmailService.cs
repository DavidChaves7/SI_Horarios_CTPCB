using Domain.Entities;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Horarios;
using Infrastructure.DTOs.Mantenimientos;
using Infrastructure.Response.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task<EnviarEmailResponse> SendEmail(EnviarEmailDto data);


    }
}
