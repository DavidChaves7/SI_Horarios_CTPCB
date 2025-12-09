using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DTOs
{
    public class EnviarEmailDto
    {
        public string correo{ get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;

        public string? base64file { get; set; }

        public string? FileName { get; set; }
    }
}
