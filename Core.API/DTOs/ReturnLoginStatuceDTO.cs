using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.API.DTOs
{
    public class ReturnLoginStatuceDTO
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
