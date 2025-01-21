using Core.API.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.Service.Interface
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(IdentityUser user, bool RememberME);
    }
}
