using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Request.Authentication
{
    public class LoginRequest
    {
        public string? codUsuario { get; set; }
        public string? password { get; set; }
    }
}
