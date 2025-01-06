using Core.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.Service.Interface
{
    public interface IEmailSender
    {
        Task SendEmailWithGoogleAsync(SendEmailDTO sendEmailDTO);
    }
}
