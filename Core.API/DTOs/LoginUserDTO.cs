using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.DTOs
{
    public class LoginUserDTO
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailLogin { get; set; } = false;
        public bool RememberMe { get; set; } = false;
    }
}
